# Avatar Mesh Settings Validator

A Unity Editor extension that ensures all VRChat avatars have proper MAMeshSettings configuration from ModularAvatar.

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-blue)
![VRChat SDK](https://img.shields.io/badge/VRChat%20SDK-3.x-green)
![ModularAvatar](https://img.shields.io/badge/ModularAvatar-1.x-orange)
![License](https://img.shields.io/badge/License-MIT-yellow)

## What It Validates

The tool checks that every VRChat avatar has:
1. **MAMeshSettings component** - Required for proper mesh rendering configuration
2. **Anchor Override Mode set to "Set"** - Ensures explicit anchor configuration (configurable)
3. **Valid Anchor Override target** - Should point to a valid GameObject (typically Hips bone)
4. **Bounds Override Mode set to "Set"** - Ensures explicit bounds configuration (configurable)
5. **Valid Bounds Root Bone** - Should point to a valid GameObject (typically Hips bone)

Note: Anchor and Bounds validation can be individually enabled/disabled in the settings.

## Features

- 🔍 **Automatic Detection**: Finds all VRChat avatars in your project
- 🚨 **Visual Alerts**: Shows warning icons in the Unity Hierarchy for avatars with configuration issues
- 🔄 **Recursive Validation**: Parent objects show alerts when child avatars have issues
- 🛠️ **Smart Auto-Fix**: Automatically configures MAMeshSettings with proper Anchor Override (Hips bone)
- ✅ **Comprehensive Validation**: Checks for missing components and proper Anchor Override configuration
- ⚡ **Performance Optimized**: Efficient caching system for large projects

## Requirements

- Unity 2022.3 or later
- VRChat SDK 3.x
- ModularAvatar 1.x

## Installation

### Method 1: Unity Package Manager (Recommended)
1. Open Unity Package Manager (Window → Package Manager)
2. Click the + button and select "Add package from git URL"
3. Enter: `https://github.com/a1678991/AvatarMeshSettingsValidator.git`
4. Click Add

### Method 2: Manual Installation
1. Download the latest release from the [Releases page](https://github.com/a1678991/AvatarMeshSettingsValidator/releases)
2. Extract to your Unity project's `Assets` folder
3. Unity will automatically compile and activate the extension

### Method 3: Git Submodule
```bash
git submodule add https://github.com/a1678991/AvatarMeshSettingsValidator.git Assets/AvatarMeshSettingsValidator
```

## Usage

### Hierarchy Indicators

The tool automatically displays icons in the Unity Hierarchy:

- 🔴 **Red Icon**: Avatar has configuration issues (missing MAMeshSettings or invalid Anchor Override)
- 🟡 **Yellow Icon**: This GameObject has child avatars with configuration issues
- ✅ **Green Icon**: Avatar is properly configured (optional, disabled by default)

### Adding MAMeshSettings

The tool automatically configures MAMeshSettings with:
- **Anchor Override Mode**: Set to "Set" (when validation enabled, otherwise uses ModularAvatar default: "Inherit")
- **Anchor Override Target**: Automatically finds and sets the Hips bone
- **Bounds Override Mode**: Set to "Set" (when validation enabled, otherwise uses ModularAvatar default: "Inherit")
- **Bounds Root Bone**: Automatically finds and sets the Hips bone

#### Single Avatar
1. Right-click on an avatar in the Hierarchy
2. Select `GameObject > Modular Avatar > Add Mesh Settings`

#### Multiple Avatars
1. Select multiple avatars (Ctrl/Cmd + Click)
2. Right-click and select `GameObject > Modular Avatar > Add Mesh Settings`

#### All Avatars in Scene
1. Go to `Tools > Avatar Tools > Validate All Mesh Settings`
2. Click "Fix All Issues" in the window that appears

### Configuration

Access settings through `Edit > Project Settings > Avatar Tools > Mesh Settings Validator`:

#### Display Options
- **Show Hierarchy Icons**: Enable/disable visual indicators
- **Show Valid Icons**: Show green checkmarks for properly configured avatars
- **Recursive Validation**: Check child GameObjects for avatar issues

#### Validation Rules
- **Validate Anchor Override**: Enable/disable validation of Anchor Override configuration
- **Validate Bounds Override**: Enable/disable validation of Bounds Override configuration
- **Require Non-Default Bounds**: Enforce custom bounds configuration
- **Validate Prefabs**: Include prefabs in project-wide validation

## Best Practices

1. **Regular Validation**: Run validation before uploading avatars to VRChat
2. **Consistent Settings**: Use the same MAMeshSettings configuration across similar avatars
3. **Version Control**: Commit MAMeshSettings components with your avatar prefabs

## Troubleshooting

### Icons not appearing
- Ensure the extension is in an Editor folder
- Check that VRChat SDK and ModularAvatar are properly installed
- Try reopening the Unity project

### Performance issues
- Disable "Show Valid Icons" for better performance with many avatars
- The tool automatically caches validation results

### MAMeshSettings not adding
- Ensure the GameObject has a VRCAvatarDescriptor component
- Check that ModularAvatar is properly installed
- Verify you have write permissions to the prefab

## API Usage

For custom tools and automation:

```csharp
using AvatarTools.Editor;

// Validate a single avatar
bool isValid = AvatarMeshSettingsValidator.ValidateAvatar(avatarGameObject);

// Add MAMeshSettings to an avatar
AvatarMeshSettingsValidator.AddMeshSettings(avatarGameObject);

// Find all avatars in the current scene
GameObject[] avatars = AvatarMeshSettingsValidator.FindAllAvatarsInScene();
```

## Contributing

Feel free to submit issues or pull requests to improve this tool.

## License

MIT License - Copyright (c) 2024 a1678991

See [LICENSE](LICENSE) file for details.