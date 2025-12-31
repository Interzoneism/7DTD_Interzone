# Changelog - Machete Animation Randomizer

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2024-12-31

### Added
- Initial release of Machete Animation Randomizer
- Dynamic animation speed randomization (0.8x to 1.5x multiplier)
- Weapon position offset randomization (±0.1 units in X/Y, ±0.075 in Z)
- Weapon rotation offset randomization (±15° pitch, ±10° yaw, ±20° roll)
- Machete weapon detection via name and tag matching
- Normal attack filtering (excludes power attacks)
- Per-entity randomization tracking system
- Automatic cleanup to prevent memory leaks
- Comprehensive error handling with try-catch blocks
- Debug logging for testing and verification
- Harmony patches for:
  - `AnimatorMeleeAttackState.OnStateEnter` - Generate randomization
  - `AnimatorMeleeAttackState.OnStateUpdate` - Apply transform changes
  - `AnimatorMeleeAttackState.OnStateExit` - Reset and cleanup
  - `AnimationGunjointOffsetData.InitStatic` - Initialize offset system

### Documentation
- Comprehensive README.md with feature overview
- BUILD_AND_USAGE_GUIDE.md with detailed instructions
- TECHNICAL_SUMMARY.md with architecture details
- QUICK_REFERENCE.md for quick customization
- CHANGELOG.md (this file)
- Inline code documentation with XML comments
- Example configurations and customization tips

### Project Structure
- API.cs - Mod initialization and Harmony setup
- MacheteAnimRandomizerPatches.cs - All Harmony patch implementations
- ModInfo.xml - Mod metadata for 7DTD
- MacheteAnimRandomizer.csproj - Visual Studio project file
- packages.config - NuGet package configuration
- .gitignore - Build artifact exclusions

### Dependencies
- Harmony 2.2.2+ (via TFP_Harmony mod)
- Assembly-CSharp.dll (from 7DTD)
- UnityEngine.CoreModule.dll (from Unity)
- .NET Framework 4.8

### Compatibility
- 7 Days to Die: Alpha 21+
- Tested on: v2.4
- Target Framework: .NET 4.8

## [Unreleased]

### Planned Features
- Configuration file support (XML-based)
- Per-weapon randomization profiles
- Visual effects synchronized with randomization
- Sound variation tied to speed changes
- Combo system with progressive randomization
- Network synchronization for multiplayer
- Performance monitoring and statistics
- Advanced randomization patterns (weighted, gaussian)
- Integration with other animation systems

### Under Consideration
- Support for additional weapon types
- Animation blending effects
- Custom particle effects
- Camera shake variations
- Stamina-based randomization intensity
- Time-of-day randomization variations
- Weather-influenced randomization

## Version History Summary

| Version | Date | Key Changes |
|---------|------|-------------|
| 1.0.0 | 2024-12-31 | Initial release with core randomization features |

## Migration Guide

### From Nothing to v1.0.0
This is the initial release, no migration needed.

### Future Migrations
Instructions for upgrading from v1.0.0 to future versions will be added here.

## Breaking Changes

None (initial release)

## Deprecations

None (initial release)

## Security

### v1.0.0
- No known security vulnerabilities
- All user input is sanitized
- No external network calls
- No file system modifications outside mod folder

### Reporting Security Issues
If you discover a security vulnerability, please open a GitHub issue or contact the author directly.

## Bug Fixes

### v1.0.0
- N/A (initial release)

### Known Issues
- Transform changes may persist briefly between attacks (visual only, no gameplay impact)
- Third-person view may show offsets differently than first-person
- Very rapid attack chains with extreme randomization may cause visual glitches
- Potential conflicts with other animation mods (load order dependent)

## Performance Improvements

### v1.0.0
- Optimized per-frame updates with early returns
- Static Random instance prevents GC allocations
- Dictionary-based caching for randomization data
- Lazy cleanup strategy for memory management
- Silent error handling in update loop

## Testing

### v1.0.0
- Manual testing with various machete weapons
- Single attack randomization verified
- Rapid attack chain testing completed
- Memory leak testing (30+ minutes continuous combat)
- Multiplayer compatibility not yet tested

## Contributors

### v1.0.0
- **Interzoneism** - Initial development and documentation

## Acknowledgments

- **Andreas Pardeike** - Harmony library
- **The Fun Pimps** - 7 Days to Die
- **7DTD Modding Community** - Examples and support

## License

Free to use and modify. Attribution appreciated but not required.

---

For detailed information about any version, see the corresponding documentation files.
