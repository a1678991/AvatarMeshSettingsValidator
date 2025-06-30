# Avatar Mesh Settings Validator - Design Document

## Overview
This Unity Editor extension ensures that all VRChat avatars in the project have the ModularAvatar MAMeshSettings component properly configured. It provides visual alerts in the Unity Hierarchy window to help developers maintain consistent mesh settings across all avatars.

## Problem Statement
- VRChat avatars require proper mesh settings for optimal performance and visual quality
- MAMeshSettings from ModularAvatar provides essential configuration for probe anchors and bounds
- Currently, developers must manually check each avatar to ensure MAMeshSettings is present
- Missing MAMeshSettings can lead to rendering issues or inconsistent behavior

## Solution Architecture

### Core Components

#### 1. Avatar Detection System
- **Purpose**: Identify all GameObjects with VRCAvatarDescriptor component
- **Implementation**:
  - Use `FindObjectsOfType<VRCAvatarDescriptor>()` for scene objects
  - Recursively check prefabs in the project
  - Cache results for performance

#### 2. Validation System
- **Purpose**: Check if avatars have MAMeshSettings component
- **Validation Rules**:
  - Avatar root GameObject must have MAMeshSettings
  - MAMeshSettings should be properly configured (non-default values)
  - Optional: Check for specific configuration patterns

#### 3. Hierarchy Alert System
- **Purpose**: Display visual alerts in Unity Hierarchy
- **Features**:
  - Icon/color overlay on avatars missing MAMeshSettings
  - Recursive parent alerts (parent shows alert if any child avatar is invalid)
  - Tooltip information on hover
  - Click to select problematic avatar

#### 4. Auto-Fix System (Optional)
- **Purpose**: Provide one-click solution to add MAMeshSettings
- **Features**:
  - Add MAMeshSettings with sensible defaults
  - Batch operation for multiple avatars
  - Undo support

### Technical Design

#### Class Structure
```
AvatarMeshSettingsValidator/
├── Editor/
│   ├── AvatarMeshSettingsValidator.cs       // Main validator logic
│   ├── HierarchyIconDrawer.cs               // Hierarchy UI rendering
│   ├── AvatarValidationCache.cs             // Performance caching
│   └── AvatarMeshSettingsWindow.cs          // Optional editor window
└── Resources/
    └── Icons/                                // Alert icons
```

#### Key Interfaces
```csharp
public interface IAvatarValidator
{
    bool IsValid(GameObject avatar);
    string GetValidationMessage(GameObject avatar);
    void FixIssue(GameObject avatar);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; }
    public GameObject[] AffectedObjects { get; set; }
}
```

### User Interface Design

#### Hierarchy Indicators
- **Red icon**: Avatar missing MAMeshSettings
- **Yellow icon**: Parent contains invalid child avatars
- **Green checkmark**: Avatar properly configured (optional)
- **Tooltip format**: "Missing MAMeshSettings on [Avatar Name]"

#### Context Menu Integration
- Right-click avatar → "Add MAMeshSettings"
- Right-click multiple selection → "Add MAMeshSettings to All"
- Tools menu → "Avatar Tools/Validate All Mesh Settings"

#### Editor Window (Optional)
- List all avatars in project/scene
- Show validation status for each
- Batch operations
- Configuration templates

## Implementation Plan

### Phase 1: Core Validation
1. Create avatar detection system
2. Implement MAMeshSettings validation logic
3. Set up basic caching mechanism

### Phase 2: Hierarchy Visualization
1. Implement HierarchyWindowAdapter using EditorApplication.hierarchyWindowItemOnGUI
2. Create icon rendering system
3. Add recursive parent alerts

### Phase 3: User Interactions
1. Add context menu items
2. Implement auto-fix functionality
3. Add undo/redo support

### Phase 4: Polish & Optimization
1. Optimize performance for large projects
2. Add configuration options
3. Create documentation

## Performance Considerations
- Cache validation results per frame
- Use event system to invalidate cache on changes
- Lazy evaluation for prefab validation
- Minimal hierarchy redraw impact

## Dependencies
- VRChat SDK 3.x
- ModularAvatar 1.x
- Unity 2022.3+

## Configuration Options
- Enable/disable hierarchy indicators
- Custom icon styles
- Validation strictness levels
- Default MAMeshSettings values

## Future Enhancements
- Integration with NDMF build pipeline
- Validation presets for different avatar types
- Automated testing support
- Performance profiling tools