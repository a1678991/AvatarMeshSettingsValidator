# Avatar Mesh Settings Validator - Technical Specification

## API Reference

### Core Classes

#### AvatarMeshSettingsValidator
Main validation logic controller.

```csharp
namespace AvatarTools.Editor
{
    [InitializeOnLoad]
    public static class AvatarMeshSettingsValidator
    {
        // Events
        public static event Action<GameObject> OnAvatarValidationChanged;
        
        // Public Methods
        public static bool ValidateAvatar(GameObject avatar);
        public static ValidationResult GetValidationResult(GameObject avatar);
        public static void AddMeshSettings(GameObject avatar);
        public static void AddMeshSettingsToMultiple(GameObject[] avatars);
        public static GameObject[] FindAllAvatarsInScene();
        public static GameObject[] FindAllAvatarsInProject();
        
        // Configuration
        public static ValidatorSettings Settings { get; }
    }
}
```

#### HierarchyIconDrawer
Handles visual representation in the Unity Hierarchy window.

```csharp
namespace AvatarTools.Editor
{
    [InitializeOnLoad]
    internal static class HierarchyIconDrawer
    {
        // Icon types
        private enum IconType
        {
            None,
            Error,          // Missing MAMeshSettings
            Warning,        // Child has missing MAMeshSettings
            Valid           // Properly configured
        }
        
        // Main drawing callback
        private static void OnHierarchyGUI(int instanceID, Rect selectionRect);
        
        // Helper methods
        private static IconType GetIconType(GameObject obj);
        private static void DrawIcon(Rect rect, IconType type);
        private static string GetTooltip(GameObject obj);
    }
}
```

#### AvatarValidationCache
Performance optimization through caching.

```csharp
namespace AvatarTools.Editor
{
    internal class AvatarValidationCache
    {
        // Cache entry
        private class CacheEntry
        {
            public ValidationResult Result;
            public double Timestamp;
            public int HierarchyVersion;
        }
        
        // Public methods
        public bool TryGetCachedResult(GameObject avatar, out ValidationResult result);
        public void CacheResult(GameObject avatar, ValidationResult result);
        public void InvalidateCache(GameObject avatar);
        public void InvalidateAll();
        
        // Automatic invalidation
        private void OnHierarchyChanged();
        private void OnPrefabChanged(GameObject prefab);
    }
}
```

### Data Structures

#### ValidationResult
```csharp
public class ValidationResult
{
    public bool IsValid { get; set; }
    public ValidationIssue[] Issues { get; set; }
    public GameObject RootAvatar { get; set; }
    public GameObject[] AffectedChildren { get; set; }
    
    public bool HasChildIssues => AffectedChildren?.Length > 0;
}

public enum ValidationIssue
{
    None = 0,
    MissingMAMeshSettings = 1,
    InvalidConfiguration = 2,
    MissingInChild = 4
}
```

#### ValidatorSettings
```csharp
[CreateAssetMenu(fileName = "AvatarValidatorSettings", menuName = "Avatar Tools/Validator Settings")]
public class ValidatorSettings : ScriptableObject
{
    [Header("Display Options")]
    public bool ShowHierarchyIcons = true;
    public bool ShowValidIcons = false;
    public bool RecursiveValidation = true;
    
    [Header("Icon Configuration")]
    public Texture2D ErrorIcon;
    public Texture2D WarningIcon;
    public Texture2D ValidIcon;
    
    [Header("Validation Rules")]
    public bool RequireNonDefaultBounds = false;
    public bool ValidatePrefabs = true;
    
    [Header("Default Values")]
    public MAMeshSettings.InheritMode DefaultProbeAnchorMode = MAMeshSettings.InheritMode.SetOrInherit;
    public MAMeshSettings.InheritMode DefaultBoundsMode = MAMeshSettings.InheritMode.SetOrInherit;
}
```

## Implementation Details

### Hierarchy Drawing Process
```csharp
// Simplified implementation
[InitializeOnLoad]
internal static class HierarchyIconDrawer
{
    static HierarchyIconDrawer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }
    
    private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;
        
        var result = AvatarValidationCache.Instance.GetOrValidate(obj);
        if (!result.IsValid || result.HasChildIssues)
        {
            var iconRect = new Rect(selectionRect.xMax - 16, selectionRect.y, 16, 16);
            DrawIcon(iconRect, result);
        }
    }
}
```

### Validation Logic
```csharp
public static bool ValidateAvatar(GameObject avatar)
{
    // Check for VRCAvatarDescriptor
    var descriptor = avatar.GetComponent<VRCAvatarDescriptor>();
    if (descriptor == null) return true; // Not an avatar
    
    // Check for MAMeshSettings
    var meshSettings = avatar.GetComponent<ModularAvatarMeshSettings>();
    if (meshSettings == null) return false;
    
    // Optional: Validate configuration
    if (Settings.RequireNonDefaultBounds)
    {
        if (meshSettings.Bounds == ModularAvatarMeshSettings.DEFAULT_BOUNDS)
            return false;
    }
    
    return true;
}
```

### Menu Integration
```csharp
public static class AvatarMeshSettingsMenu
{
    [MenuItem("GameObject/Modular Avatar/Add Mesh Settings", false, 10)]
    private static void AddMeshSettingsMenu()
    {
        var selected = Selection.gameObjects
            .Where(go => go.GetComponent<VRCAvatarDescriptor>() != null)
            .ToArray();
            
        if (selected.Length == 0)
        {
            EditorUtility.DisplayDialog("No Avatars Selected", 
                "Please select one or more avatars with VRCAvatarDescriptor.", "OK");
            return;
        }
        
        AvatarMeshSettingsValidator.AddMeshSettingsToMultiple(selected);
    }
    
    [MenuItem("GameObject/Modular Avatar/Add Mesh Settings", true)]
    private static bool AddMeshSettingsMenuValidate()
    {
        return Selection.gameObjects.Any(go => 
            go.GetComponent<VRCAvatarDescriptor>() != null &&
            go.GetComponent<ModularAvatarMeshSettings>() == null);
    }
}
```

## Event System

### Validation Events
```csharp
// Avatar validation changed
AvatarMeshSettingsValidator.OnAvatarValidationChanged += (GameObject avatar) =>
{
    Debug.Log($"Validation state changed for {avatar.name}");
};

// Batch validation completed
AvatarMeshSettingsValidator.OnBatchValidationComplete += (ValidationResult[] results) =>
{
    var invalidCount = results.Count(r => !r.IsValid);
    Debug.Log($"Found {invalidCount} avatars with issues");
};
```

## Performance Metrics

### Expected Performance
- Hierarchy icon rendering: < 0.1ms per visible item
- Avatar validation: < 1ms per avatar
- Cache hit rate: > 95% during normal editing
- Memory usage: ~1KB per cached avatar

### Optimization Strategies
1. **Lazy Evaluation**: Only validate visible hierarchy items
2. **Frame Debouncing**: Batch validation updates
3. **Smart Caching**: Invalidate only affected objects
4. **Prefab Optimization**: Cache prefab validation results separately

## Error Handling

### Common Scenarios
```csharp
try
{
    var meshSettings = avatar.AddComponent<ModularAvatarMeshSettings>();
    // Configure with defaults
}
catch (Exception e)
{
    Debug.LogError($"Failed to add MAMeshSettings: {e.Message}");
    // Fallback behavior
}
```

### User Notifications
- Missing ModularAvatar package: Show helpful dialog with installation instructions
- Permission errors: Suggest checking out files from version control
- Invalid avatar setup: Provide detailed error messages

## Testing Strategy

### Unit Tests
- Validation logic for various avatar configurations
- Cache invalidation scenarios
- Menu state validation

### Integration Tests
- Hierarchy rendering with large scenes (1000+ objects)
- Prefab modification detection
- Multi-scene validation

### Manual Testing Checklist
- [ ] Icons appear correctly in hierarchy
- [ ] Tooltips show accurate information
- [ ] Context menu items enable/disable properly
- [ ] Batch operations work with undo/redo
- [ ] Performance acceptable with 50+ avatars