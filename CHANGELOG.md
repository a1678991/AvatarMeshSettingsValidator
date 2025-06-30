# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0-beta.1] - 2025-07-01

### Added
- GitHub Actions workflows for automated UnityPackage builds
  - Standard workflow using Unity Hub installation
  - GameCI-based workflow for containerized builds
- PackageExporter utility for local package creation
  - Editor menu integration for easy export
  - Command-line support for automation
- Export script for local development and testing
- Comprehensive release documentation in README

### Changed
- Updated development workflow documentation
- Enhanced contribution guidelines

### Technical
- Automated release process now creates .unitypackage files
- Support for both manual and automated package exports

## [0.1.0] - 2025-07-01

### Added
- Full internationalization (i18n) support
- English language support
- Japanese language support (日本語)
- Simplified Chinese language support (简体中文)
- Language selector in Project Settings
- Automatic language detection based on Unity system language

### Changed
- All UI text now uses the localization system
- Settings window dynamically updates when language is changed

## [0.0.1-beta] - 2025-07-01

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