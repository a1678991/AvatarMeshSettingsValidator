# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.0.1-beta] - 2024-06-30

### Added
- Initial release of Avatar Mesh Settings Validator
- Automatic detection of VRChat avatars in Unity project
- Visual hierarchy alerts for configuration issues
- Smart auto-fix that configures MAMeshSettings with Hips bone
- Configurable validation rules for Anchor and Bounds overrides
- Batch operations window for project-wide fixes
- Context menu integration for quick fixes
- Performance-optimized caching system
- Unity Project Settings integration
- Support for both PC and Quest avatars
- Comprehensive documentation

### Features
- Validates MAMeshSettings component presence
- Checks Anchor Override Mode configuration
- Checks Bounds Override Mode configuration
- Recursive validation for child avatars
- Optional validation for non-default bounds
- Respects ModularAvatar's default inheritance behavior