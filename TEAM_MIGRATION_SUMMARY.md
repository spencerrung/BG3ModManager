# BG3ModManager: Team Migration Summary

**Project:** BG3ModManager - Baldur's Gate 3 Mod Manager
**Current Status:** WPF (Windows-only) → Avalonia (Cross-platform)
**Timeline:** Phased approach, no major disruption to main branch
**Target Release:** Windows + Linux support

---

## Quick Overview: Why We're Migrating

### The Problem
- Current WPF-based application is **Windows-only**
- Large user base requesting **Linux support**
- WPF cannot run natively on Linux

### The Solution
- Migrate to **Avalonia** (cross-platform XAML framework)
- Runs on Windows, Linux, and macOS
- Maintains existing MVVM/ReactiveUI patterns
- Minimal changes to business logic

### The Benefit
- **Single codebase** for all platforms
- **Easier to maintain** than dual WPF/GTK# approach
- **Users get native experience** on their OS
- **Future-proof** for macOS support

---

## Key Documents for Your Team

### 1. **AVALONIA_MIGRATION_PLAN.md** (Strategic Overview)
   - **Read this first** to understand the overall strategy
   - 6-phase implementation plan
   - Timeline and milestones
   - Risk assessment and mitigation
   - **Audience:** Project leads, architects, decision-makers

### 2. **AVALONIA_DEVELOPER_GUIDE.md** (Practical Reference)
   - Day-to-day development guide
   - How to add views, controls, converters
   - Platform abstraction layer explained
   - Styling and theming system
   - Debugging tips and troubleshooting
   - **Audience:** All developers

### 3. **GETTING_STARTED_AVALONIA.md** (Onboarding)
   - Environment setup instructions
   - Building and running the project
   - Verifying installation
   - Making your first change
   - Getting help resources
   - **Audience:** New team members, contributors

---

## Architecture at a Glance

### Before (WPF-based)
```
Windows Only
     ↓
  GUI (WPF)
     ↓
DivinityModManagerCore
     ↓
Windows-specific P/Invoke code
```

### After (Avalonia-based)
```
┌─ Windows ─┐
│ GUI.Avalonia (Avalonia)
└───────────┘
       ↓
┌─ Linux ──┐
│ GUI.Avalonia (Avalonia)
└───────────┘
       ↓
┌─ Core ───┐
│ DivinityModManagerCore (Pure C#)
└───────────┘
       ↓
┌─ Platform Services (Abstraction)
│ - File System (symlinks vs junctions)
│ - Registry (file-based config vs Windows registry)
│ - Dialogs (platform-native file dialogs)
│ - Process (game launching)
│ - Accessibility (screen readers)
└───────────────┘
```

---

## What Each Team Member Needs to Know

### UI/Frontend Developers

**Your Role:**
- Porting WPF XAML → Avalonia AXAML
- Creating custom Avalonia controls
- Implementing theming system
- Testing UI on Windows and Linux

**Key Changes:**
- XAML namespaces change from `http://schemas.microsoft.com/winfx/...` to `https://github.com/avaloniaui`
- Remove AdonisUI (WPF material design theme)
- Use Avalonia's `{DynamicResource}` for theming
- Replace `gong-wpf-dragdrop` with Avalonia's native drag-and-drop

**References:**
- `AVALONIA_DEVELOPER_GUIDE.md` - Task 1: Adding a New View
- `AVALONIA_DEVELOPER_GUIDE.md` - Styling & Theming section
- [Avalonia XAML Docs](https://docs.avaloniaui.net/)

### Backend/Core Developers

**Your Role:**
- Creating platform abstraction layer
- Implementing Windows/Linux-specific services
- Refactoring P/Invoke code
- Testing business logic on both platforms

**Key Changes:**
- Extract Windows-specific code into `IFileSystemService`, `IRegistryService`, etc.
- Create Windows implementations (wrap existing P/Invoke)
- Create Linux implementations (use Linux APIs)
- Remove `UseWPF=true` from Core project
- No WPF dependencies in Core

**References:**
- `AVALONIA_MIGRATION_PLAN.md` - Phase 2: Core Refactoring
- `AVALONIA_DEVELOPER_GUIDE.md` - Platform Abstraction Layer section
- Create files in: `src/Core/Platform/Windows/` and `src/Core/Platform/Linux/`

### DevOps/Build Engineers

**Your Role:**
- Updating build scripts for multi-platform
- Creating CI/CD workflows for Windows and Linux builds
- Packaging releases (installers, AppImage, etc.)

**Key Changes:**
- `BuildRelease.py` needs to handle both platforms
- Create GitHub Actions workflows
- Build for `win-x64` and `linux-x64` runtime identifiers
- Different release formats (NSIS installer for Windows, AppImage for Linux)

**References:**
- `AVALONIA_MIGRATION_PLAN.md` - Phase 5: Build & Release System
- `AVALONIA_MIGRATION_PLAN.md` - Phase 6: CI/CD Workflow

### QA/Test Engineers

**Your Role:**
- Testing on Windows and Linux
- Integration testing (mod loading, export, drag-and-drop)
- Platform-specific feature testing
- Regression testing vs WPF version

**Test Scenarios:**
1. Load mod → Verify displays correctly
2. Drag-and-drop reordering → Verify order changes
3. Export load order → Verify file is created
4. Settings window → Verify changes persist
5. File dialogs → Verify on Windows and Linux
6. Dark/Light theme → Verify colors switch

**References:**
- `AVALONIA_DEVELOPER_GUIDE.md` - Testing Guide section

---

## Development Workflow

### Phase 1: Foundation & Setup (Week 1)
- ✅ Create `GUI.Avalonia` project
- ✅ Set up directory structure
- Create platform abstraction layer (`src/Core/Platform/`)

### Phase 2: Core Refactoring (Week 1-2)
- Extract Windows-specific code
- Create Windows/Linux platform service implementations
- Remove WPF from Core project

### Phase 3: UI Implementation (Week 2-4)
- Port MainWindow
- Port 13 custom controls
- Port 13 views
- Port 11 converters

### Phase 4: Platform Integrations (Week 3-4)
- Update auto-update system
- Implement file dialogs
- Implement drag-and-drop

### Phase 5: Build System (Week 4)
- Update `BuildRelease.py`
- Create GitHub Actions workflows
- Test packaging for both platforms

### Phase 6: Documentation (Week 4)
- Developer guide ✅ (Created)
- Migration notes
- Team onboarding ✅ (Created)

---

## Key Files to Watch

### Files Being Created
```
src/GUI.Avalonia/              # NEW - Main Avalonia UI project
src/Core/Platform/             # NEW - Cross-platform abstraction
  ├── Windows/
  └── Linux/
AVALONIA_MIGRATION_PLAN.md     # NEW - Strategy document
AVALONIA_DEVELOPER_GUIDE.md    # NEW - Dev reference
GETTING_STARTED_AVALONIA.md    # NEW - Onboarding guide
```

### Files Being Modified
```
BG3ModManager.sln              # Add GUI.Avalonia project reference
src/Core/Services.cs           # Register platform services
src/Core/DivinityModManagerCore.csproj  # Remove UseWPF
BuildRelease.py                # Support multi-platform builds
```

### Files Being Deprecated (But Not Deleted)
```
src/GUI/                       # Legacy WPF - kept for reference
src/GUI/GUI.csproj             # Can be archived/removed later
```

---

## Common Questions

### Q: Will we have to maintain two codebases?
**A:** No. We create one Avalonia codebase that replaces WPF. All UI logic is in one place. Business logic (Core) is unchanged.

### Q: Will the application run on macOS?
**A:** Avalonia supports macOS, so technically yes. macOS support can be added as a future goal with minimal effort.

### Q: What about the LSLib native library?
**A:** This needs investigation. LSLib provides C++ bindings for game file format handling. We may need to:
- Investigate if it compiles on Linux
- Create a wrapper abstraction if not
- Document workarounds for Linux users

### Q: Can we release Windows + Linux at the same time?
**A:** Yes, once Avalonia UI is complete. We can build both platforms in parallel.

### Q: What happens to the current WPF version?
**A:** After Avalonia is stable, the WPF version can be deprecated and removed, or kept as a legacy archive.

### Q: How long will this take?
**A:** Based on the plan: 4-6 weeks of focused development, assuming a team of 2-3 developers.

---

## Dependencies Summary

### Removed
- ❌ `AdonisUI` (WPF theme framework)
- ❌ `Ookii.Dialogs.Wpf` (Windows file dialogs)
- ❌ `ReactiveUI.WPF` (WPF-specific bindings)
- ❌ `gong-wpf-dragdrop` (Drag-and-drop for WPF)
- ❌ `Autoupdater.NET` (WPF-specific updates)
- ❌ `WpfScreenHelper` (WPF screen info)

### Added
- ✅ `Avalonia` (Core framework)
- ✅ `Avalonia.Controls.DataGrid` (Data grid control)
- ✅ `Avalonia.Themes.Fluent` (Modern theme)
- ✅ `ReactiveUI.Avalonia` (Avalonia-specific bindings)

### Unchanged
- ✅ `ReactiveUI` (MVVM framework)
- ✅ `System.Reactive` (Reactive programming)
- ✅ `DynamicData` (Reactive collections)
- ✅ Core game handling libraries

---

## Resources

### For Developers

**Avalonia:**
- Official Docs: https://docs.avaloniaui.net/
- GitHub: https://github.com/AvaloniaUI/Avalonia
- Community Chat: https://gitter.im/AvaloniaUI/Avalonia

**ReactiveUI:**
- Official Docs: https://reactiveui.net/
- GitHub: https://github.com/reactiveui/ReactiveUI

**.NET:**
- Official Docs: https://docs.microsoft.com/en-us/dotnet/

### Internal Documentation

**Must Read (in order):**
1. This file (TEAM_MIGRATION_SUMMARY.md) - Overview
2. AVALONIA_MIGRATION_PLAN.md - Strategic plan
3. GETTING_STARTED_AVALONIA.md - Setup & onboarding
4. AVALONIA_DEVELOPER_GUIDE.md - Daily reference

---

## Success Metrics

How will we know the migration is successful?

✅ **Core functionality works on both platforms**
- Mod loading
- Drag-and-drop reordering
- Export to game
- Settings persistence

✅ **No performance regression**
- Application startup time ≈ same
- UI responsiveness ≈ same
- Memory usage ≈ same

✅ **Cross-platform consistency**
- Identical features on Windows and Linux
- Consistent look and feel
- Same keyboard shortcuts and hotkeys

✅ **Accessibility maintained**
- Screen reader support (CrossSpeak)
- Keyboard navigation
- High contrast mode

✅ **Smooth user experience**
- Easy installation on both platforms
- Auto-update works on both platforms
- Error messages are helpful

---

## Timeline & Milestones

| Week | Phase | Status | Milestone |
|------|-------|--------|-----------|
| Week 1 | 1-2 | 🎯 Next | Project setup + Core refactoring started |
| Week 2 | 2-3 | 📋 Planning | Platform abstraction complete, UI porting starts |
| Week 3 | 3-4 | 🔄 In Progress | 50% of UI ported |
| Week 4 | 3-6 | 🔄 In Progress | UI complete, integrations, build system |
| Week 5+ | 7 | ✨ Testing | Beta testing, bug fixes |
| Release | - | 🚀 Launch | Public Windows + Linux release |

---

## Questions or Need Clarification?

1. **Technical Questions:** Check `AVALONIA_DEVELOPER_GUIDE.md`
2. **Architecture/Strategy Questions:** Check `AVALONIA_MIGRATION_PLAN.md`
3. **Setup Issues:** Check `GETTING_STARTED_AVALONIA.md`
4. **Issues Not Covered:** Open a GitHub issue or contact the project lead

---

## Next Steps for Team

1. **Read this summary** (you just did! 👍)
2. **Review AVALONIA_MIGRATION_PLAN.md** (strategists & leads)
3. **Set up your development environment** using GETTING_STARTED_AVALONIA.md
4. **Bookmark AVALONIA_DEVELOPER_GUIDE.md** for daily reference
5. **Wait for Phase 1 setup to complete** before starting work
6. **Ask questions early** - better now than during implementation!

---

**Let's build cross-platform support for BG3ModManager! 🚀**

