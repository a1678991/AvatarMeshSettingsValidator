# Avatar Mesh Settings Validator

A Unity Editor extension that ensures all VRChat avatars have proper MAMeshSettings configuration from ModularAvatar.

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

1. Ensure VRChat SDK and ModularAvatar are installed in your project
2. Copy the `AvatarMeshSettingsValidator` folder to your `Assets/Tools/Editor/` directory
3. Unity will automatically compile and activate the extension

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

This tool is provided as-is for use in VRChat avatar development projects.