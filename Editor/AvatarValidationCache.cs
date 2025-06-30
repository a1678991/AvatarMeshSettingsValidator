using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AvatarTools.Editor
{
    internal class AvatarValidationCache
    {
        private static AvatarValidationCache _instance;
        public static AvatarValidationCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AvatarValidationCache();
                }
                return _instance;
            }
        }

        private class CacheEntry
        {
            public ValidationResult Result;
            public double Timestamp;
            public int HierarchyVersion;
        }

        private readonly Dictionary<int, CacheEntry> _cache = new Dictionary<int, CacheEntry>();
        private int _currentHierarchyVersion = 0;
        private const double CACHE_LIFETIME_SECONDS = 2.0;

        private AvatarValidationCache()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            AvatarMeshSettingsValidator.OnAvatarValidationChanged += InvalidateCache;
        }

        ~AvatarValidationCache()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            AvatarMeshSettingsValidator.OnAvatarValidationChanged -= InvalidateCache;
        }

        public ValidationResult GetOrValidate(GameObject gameObject)
        {
            if (gameObject == null)
                return new ValidationResult { IsValid = true };

            var instanceId = gameObject.GetInstanceID();

            if (TryGetCachedResult(instanceId, out var result))
            {
                return result;
            }

            result = AvatarMeshSettingsValidator.GetValidationResult(gameObject);
            CacheResult(instanceId, result);
            return result;
        }

        public bool TryGetCachedResult(int instanceId, out ValidationResult result)
        {
            if (_cache.TryGetValue(instanceId, out var entry))
            {
                var currentTime = EditorApplication.timeSinceStartup;
                if (entry.HierarchyVersion == _currentHierarchyVersion &&
                    (currentTime - entry.Timestamp) < CACHE_LIFETIME_SECONDS)
                {
                    result = entry.Result;
                    return true;
                }
                else
                {
                    _cache.Remove(instanceId);
                }
            }

            result = null;
            return false;
        }

        public void CacheResult(int instanceId, ValidationResult result)
        {
            _cache[instanceId] = new CacheEntry
            {
                Result = result,
                Timestamp = EditorApplication.timeSinceStartup,
                HierarchyVersion = _currentHierarchyVersion
            };
        }

        public void InvalidateCache(GameObject gameObject)
        {
            if (gameObject != null)
            {
                var instanceId = gameObject.GetInstanceID();
                _cache.Remove(instanceId);

                // Also invalidate parent objects if recursive validation is enabled
                if (AvatarMeshSettingsValidator.Settings.RecursiveValidation)
                {
                    var parent = gameObject.transform.parent;
                    while (parent != null)
                    {
                        _cache.Remove(parent.gameObject.GetInstanceID());
                        parent = parent.parent;
                    }
                }
            }
        }

        public void InvalidateAll()
        {
            _cache.Clear();
            _currentHierarchyVersion++;
        }

        private void OnHierarchyChanged()
        {
            // Delay invalidation to avoid multiple invalidations in the same frame
            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                InvalidateAll();
            };
        }

        public void InvalidatePrefab(GameObject prefab)
        {
            if (prefab == null) return;

            var prefabPath = AssetDatabase.GetAssetPath(prefab);
            if (string.IsNullOrEmpty(prefabPath)) return;

            // Invalidate all instances of this prefab
            var toRemove = new List<int>();
            foreach (var kvp in _cache)
            {
                if (kvp.Value.Result.RootObject != null)
                {
                    var instancePath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(kvp.Value.Result.RootObject);
                    if (instancePath == prefabPath)
                    {
                        toRemove.Add(kvp.Key);
                    }
                }
            }

            foreach (var id in toRemove)
            {
                _cache.Remove(id);
            }
        }
    }
}