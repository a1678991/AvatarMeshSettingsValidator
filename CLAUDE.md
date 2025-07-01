# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

AvatarMeshSettingsValidator is a Unity Editor extension for VRChat avatar development that validates and fixes configuration issues related to ModularAvatar's MAMeshSettings component. The tool ensures proper mesh rendering configuration for VRChat avatars.

## Key Commands

### Package Export
```bash
# Export UnityPackage using script
./Scripts/export-package.sh

# Or via Unity Editor menu:
# Tools > Avatar Tools > Export Package
```

### Release Process
```bash
# 1. Update version in package.json
# 2. Update CHANGELOG.md
# 3. Commit changes
git add -A
git commit -m "Prepare release vX.Y.Z"

# 4. Create and push tag
git tag vX.Y.Z
git push origin vX.Y.Z

# 5. Create GitHub release - UnityPackage builds automatically
```

## Architecture Overview

### Core Systems

1. **Validation System** (`Editor/AvatarMeshSettingsValidator.cs`)
   - Validates avatar GameObject hierarchy for missing MAMeshSettings
   - Checks anchor and bounds configuration
   - Provides fix operations
   - Entry point: `ValidateAvatar(GameObject avatar)`

2. **Visual Feedback** (`Editor/HierarchyIconDrawer.cs`)
   - Draws warning/error icons in Unity hierarchy
   - Updates in real-time as issues are fixed
   - Integrates with Unity's EditorApplication callbacks

3. **Batch Operations** (`Editor/AvatarMeshSettingsWindow.cs`)
   - Window for processing multiple avatars
   - Accessible via: Window > Avatar Tools > Mesh Settings Validator
   - Uses `FindAllAvatarsInScene()` for discovery

4. **Configuration** (`Editor/ValidatorSettings.cs`)
   - Project-wide settings stored in ProjectSettings
   - Per-user preferences in EditorPrefs
   - Settings accessible via Edit > Project Settings > Avatar Tools

5. **Performance Optimization** (`Editor/AvatarValidationCache.cs`)
   - Caches validation results to avoid redundant checks
   - Invalidates on hierarchy changes
   - Critical for large projects with many avatars

6. **Localization** (`Editor/Localization/`)
   - Multi-language support (EN, JA, ZH_CN)
   - Language detection based on Unity's system language
   - Extensible translation system

### Key Integration Points

- **VRChat SDK**: Conditional compilation with `#if VRC_SDK_VRCSDK3`
- **ModularAvatar**: Direct dependency on `nadena.dev.modular-avatar.core`
- **Unity Version**: Requires 2022.3+ for proper API support
- **Assembly**: Editor-only assembly defined in `AvatarMeshSettingsValidator.Editor.asmdef`

### Development Patterns

- All code is editor-only (in `/Editor/` directory)
- Uses Unity's immediate mode GUI for windows
- Follows Unity's menu item conventions
- Implements proper null checking for avatar components
- Handles both scene objects and prefabs