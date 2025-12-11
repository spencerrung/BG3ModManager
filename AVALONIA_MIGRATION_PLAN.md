# BG3ModManager: WPF to Avalonia Migration Plan

## Executive Summary

This document outlines the strategy and implementation plan for migrating the BG3ModManager from WPF (Windows-only) to Avalonia (cross-platform). This will enable both Windows and Linux releases while maintaining code quality and team productivity.

**Timeline:** Phased approach with minimal disruption to the main branch
**Scope:** GUI layer rewrite with core logic remaining unchanged
**Risk Level:** Medium - requires UI rewrites but leverages existing MVVM/ReactiveUI patterns

---

## 1. Project Overview

### Current State
- **Framework:** .NET 8.0 with WPF
- **Architecture:** MVVM + ReactiveUI
- **Platform:** Windows-only (x64)
- **Codebase:** 104 C# Core files, 76 C# GUI files, 23 XAML files (~29k lines total)

### Target State
- **Framework:** .NET 8.0 with Avalonia
- **Architecture:** MVVM + ReactiveUI (unchanged)
- **Platforms:** Windows + Linux
- **Release Model:**
  - Windows: WPF or Avalonia (we'll use Avalonia for both to maintain single codebase)
  - Linux: Avalonia
  - macOS: Avalonia (future-proof)

### Migration Strategy
**Hybrid Approach:**
- Create new `GUI.Avalonia` project alongside existing `GUI` (WPF)
- Share all code from `DivinityModManagerCore`
- Gradually port UI code from WPF to Avalonia
- Update build/release scripts for multi-platform packaging
- Document process for team knowledge transfer

---

## 2. Phase 1: Foundation & Setup (Week 1)

### 2.1 Create Avalonia Project Structure

**Deliverables:**
1. New Avalonia project: `src/GUI.Avalonia/GUI.Avalonia.csproj`
2. Project should reference `DivinityModManagerCore`
3. Establish base project organization

**Tasks:**
- [ ] Create new .csproj file with Avalonia configuration
- [ ] Copy basic App.xaml and App.xaml.cs structure
- [ ] Copy Program.cs with Avalonia host setup
- [ ] Set up directory structure:
  ```
  src/GUI.Avalonia/
  ├── Views/
  ├── ViewModels/
  ├── Controls/
  ├── Converters/
  ├── Themes/
  ├── Resources/
  ├── Util/
  ├── App.xaml / App.xaml.cs
  ├── Program.cs
  └── GUI.Avalonia.csproj
  ```

**Dependencies to Add:**
```xml
<PackageReference Include="Avalonia" Version="11.1.x" />
<PackageReference Include="Avalonia.Controls.DataGrid" Version="11.1.x" />
<PackageReference Include="Avalonia.Themes.Fluent" Version="11.1.x" />
<PackageReference Include="ReactiveUI.Avalonia" Version="20.1.x" />
<PackageReference Include="DynamicData" Version="9.1.2" />
<PackageReference Include="System.Reactive" Version="6.0.1" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
<PackageReference Include="SharpCompress" Version="0.39.0" />
<PackageReference Include="ZstdSharp.Port" Version="0.8.5" />
<PackageReference Include="Gameloop.Vdf" Version="0.6.2" />
<PackageReference Include="NexusModsNET" Version="1.0.5" />
```

**Why Not Autoupdater.NET?**
- WPF-specific; for Avalonia we'll implement update checking in core logic with platform-specific installers

---

### 2.2 Create Platform Abstraction Layer

**Purpose:** Extract Windows-specific code into interfaces so both WPF and Avalonia can work with platform-specific features

**Location:** `src/Core/Platform/`

**Interfaces to Create:**

```csharp
// src/Core/Platform/IFileDialogService.cs
public interface IFileDialogService
{
    Task<string?> OpenFileAsync(string title, string filter);
    Task<IReadOnlyList<string>> OpenFilesAsync(string title, string filter);
    Task<string?> OpenFolderAsync(string title);
    Task<string?> SaveFileAsync(string title, string filter, string suggestedFileName);
}

// src/Core/Platform/IRegistryService.cs
public interface IRegistryService
{
    string? GetValue(string path, string key);
    void SetValue(string path, string key, string value);
}

// src/Core/Platform/IFileSystemService.cs
public interface IFileSystemService
{
    void DeleteToRecycleBin(string path);
    void CreateJunctionPoint(string linkPath, string targetPath);
    void DeleteJunctionPoint(string linkPath);
    bool IsJunctionPoint(string path);
}

// src/Core/Platform/IProcessService.cs
public interface IProcessService
{
    void LaunchGame(string processPath, string arguments);
    void OpenUrl(string url);
    void OpenFolder(string folderPath);
}

// src/Core/Platform/IAccessibilityService.cs
public interface IAccessibilityService
{
    void Announce(string message);
    void AnnounceAsync(string message);
}

// src/Core/Platform/IPlatformServices.cs
public interface IPlatformServices
{
    IFileDialogService FileDialogs { get; }
    IRegistryService Registry { get; }
    IFileSystemService FileSystem { get; }
    IProcessService Process { get; }
    IAccessibilityService Accessibility { get; }
}
```

**Implementation Plan:**
- Windows implementations wrap existing P/Invoke code (RecycleBinHelper, JunctionPoint, etc.)
- Linux implementations use native alternatives
- Create `Services.cs` registry to initialize correct implementations based on runtime platform

---

### 2.3 Update Core Services.cs

**Current Issue:** Core project has hardcoded Windows dependencies

**Changes Required:**
- [ ] Inject `IPlatformServices` into existing services
- [ ] Replace direct P/Invoke calls with interface calls
- [ ] Create Windows implementations of platform interfaces
- [ ] Create Linux implementations of platform interfaces
- [ ] Update DivinityApp.cs to initialize platform services

**Files to Modify:**
- `src/Core/Util/JunctionPoint.cs` → Implement IFileSystemService
- `src/Core/Util/RecycleBinHelper.cs` → Implement IFileSystemService
- `src/Core/Util/DivinityRegistryHelper.cs` → Implement IRegistryService
- `src/Core/Util/ProcessHelper.cs` → Implement IProcessService
- `src/Core/AppServices/ScreenReaderService.cs` → Implement IAccessibilityService
- `src/Core/Services.cs` → Register platform services

---

## 3. Phase 2: Core Refactoring (Week 1-2)

### 3.1 Refactor Windows-Specific Dependencies

**Action Items:**

1. **Create Windows Implementations**
   ```csharp
   // src/Core/Platform/Windows/WindowsFileSystemService.cs
   // src/Core/Platform/Windows/WindowsRegistryService.cs
   // src/Core/Platform/Windows/WindowsProcessService.cs
   // src/Core/Platform/Windows/WindowsAccessibilityService.cs
   ```

2. **Create Linux Implementations**
   ```csharp
   // src/Core/Platform/Linux/LinuxFileSystemService.cs
   // src/Core/Platform/Linux/LinuxRegistryService.cs
   // src/Core/Platform/Linux/LinuxProcessService.cs
   // src/Core/Platform/Linux/LinuxAccessibilityService.cs
   ```

3. **Update Target Frameworks**
   - Change `DivinityModManagerCore.csproj`:
     ```xml
     <TargetFramework>net8.0</TargetFramework>
     ```
     (Remove `-windows` to allow cross-platform compilation)

4. **Conditional P/Invoke Compilation**
   ```csharp
   #if WINDOWS
       // Windows-only P/Invoke code
   #else
       // Linux alternative implementation
   #endif
   ```

---

### 3.2 Remove WPF from Core Project

**Current:** `Core/Core.csproj` has `<UseWPF>true</UseWPF>`

**Action:**
- [ ] Remove `UseWPF` property
- [ ] Remove `System.Windows.Automation` dependencies
- [ ] Create `IAccessibilityService` wrapper instead
- [ ] Update custom automation peers to be GUI-specific

**Files to Move/Update:**
- Accessibility peers (5 files) → Move to `GUI/Util/ScreenReader/` (WPF-specific)
- Keep business logic in Core, accessibility implementation in GUI

---

## 4. Phase 3: Avalonia UI Implementation (Week 2-4)

### 4.1 Port Main Window

**Priority:** HIGH - This is the app's primary UI

**Files to Create:**
1. `src/GUI.Avalonia/Views/MainWindow.axaml` (Avalonia XAML)
2. `src/GUI.Avalonia/Views/MainWindow.axaml.cs` (Code-behind)
3. `src/GUI.Avalonia/ViewModels/MainWindowViewModel.cs` (copy from WPF)

**Porting Strategy:**
- Copy `MainWindow.xaml` content
- Convert WPF XAML syntax to Avalonia AXAML syntax
- Remove AdonisUI-specific controls
- Replace drag-and-drop (gong-wpf-dragdrop) with Avalonia native drag-and-drop
- Test with MainWindowViewModel from Core

**Key Changes:**
- `xmlns` changes from `http://schemas.microsoft.com/winfx/2006/xaml/presentation` to Avalonia namespaces
- Remove AdonisUI theme references
- Update binding syntax for Avalonia compatibility
- Custom controls need Avalonia-compatible rewrites

---

### 4.2 Port Custom Controls (13 controls)

**Priority:** MEDIUM-HIGH

**Controls in Order of Importance:**
1. AlertBar - Status message display
2. BusyIndicator - Loading spinner
3. ModEntryGrid - Mod list display (complex)
4. ModListView - Drag-and-drop list (complex)
5. AutoGrayableImage - Disabled state images
6. HyperlinkText - Styled hyperlinks
7. SelectableTextBlock - Copyable text
8. Others (8 more...)

**Approach:**
- Each control gets `ControlName.axaml` + `ControlName.axaml.cs`
- Recreate styling using Avalonia resources instead of AdonisUI
- Test each control independently before integration

---

### 4.3 Port Converters (11 converters)

**Priority:** HIGH

**Files to Create:**
```
src/GUI.Avalonia/Converters/
├── BoolToVisibilityConverter.cs
├── EnumToStringConverter.cs
├── IntToVisibilityConverter.cs
├── (copy remaining 8...)
```

**Changes Needed:**
- Update `IValueConverter` interface implementation for Avalonia
- Test with UI binding

---

### 4.4 Port Views (13 Views)

**Priority:** HIGH

**Views by Importance:**
1. **MainViewControl** - Main mod list interface (COMPLEX)
2. **SettingsWindow** - Settings UI (MEDIUM)
3. **HorizontalModLayout** / **VerticalModLayout** - Layout options (MEDIUM)
4. **ExportOrderToArchiveView** - Export dialog (MEDIUM)
5. **AppUpdateWindow** - Update notification (LOW)
6. **DeleteFilesConfirmationView** - Confirmation (LOW)
7. **ModUpdatesLayout** - Update info display (LOW)
8. **AboutWindow**, **HelpWindow**, **VersionGeneratorWindow**, **LeaderLibSettingsWindow** (LOW)

**Porting Process for Each View:**
```
For each .xaml file:
1. Copy content to new .axaml file
2. Convert WPF XAML → Avalonia AXAML syntax
3. Update control namespaces
4. Remove AdonisUI references
5. Replace WPF controls with Avalonia equivalents
6. Update bindings for Avalonia
7. Test with existing ViewModel
```

---

### 4.5 Implement Theme System (Replace AdonisUI)

**Current:** AdonisUI material design theme

**Approach:** Create custom Avalonia theme with Light/Dark variants

**Location:** `src/GUI.Avalonia/Themes/`

**Files to Create:**
```
src/GUI.Avalonia/Themes/
├── Light.axaml - Light theme resources
├── Dark.axaml - Dark theme resources
├── Colors.axaml - Color definitions
├── Brushes.axaml - Brush definitions
├── Styles/
│   ├── Button.axaml
│   ├── ComboBox.axaml
│   ├── TextBox.axaml
│   ├── Window.axaml
│   └── (other control styles)
└── ThemeManager.cs - Handle theme switching
```

**Strategy:**
- Keep visual design similar to current (users expect consistency)
- Use Avalonia's built-in Fluent theme as base
- Extend/customize for BG3ModManager's needs
- Implement dynamic theme switching (Light ↔ Dark)

---

## 5. Phase 4: Platform-Specific Integrations (Week 3-4)

### 5.1 Update Auto-Update System

**Current:** Autoupdater.NET (WPF-specific)

**New Approach:**
1. Move update checking logic to Core project (platform-agnostic)
2. Create `IUpdateService` interface
3. Implement platform-specific installers:
   - Windows: .msi or self-extracting executable
   - Linux: .AppImage or native package manager

**Files to Create:**
- `src/Core/Services/IUpdateService.cs`
- `src/GUI.Avalonia/Services/AvaloniaUpdateService.cs`
- `BuildRelease.py` - Update to create platform-specific installers

---

### 5.2 Handle File Dialogs

**Current:** Ookii.Dialogs.Wpf (Windows-specific)

**New Approach:**
- Use Avalonia's built-in `OpenFileDialog`, `SaveFileDialog`, `OpenFolderDialog`
- Implement `IFileDialogService` interface
- Remove Ookii.Dialogs dependency for Avalonia build

---

### 5.3 Implement Drag-and-Drop

**Current:** gong-wpf-dragdrop (WPF-only)

**New Approach:**
1. Use Avalonia's native drag-and-drop API
2. Porting: `ModListDragHandler.cs` and `ModListDropHandler.cs`
3. Update ModListView to use Avalonia drag-and-drop

**Testing Required:** Ensure mod reordering works correctly

---

## 6. Phase 5: Build & Release System (Week 4)

### 6.1 Update Project Structure

**Goal:** Support building both Windows (WPF) and Linux (Avalonia)

**Current Solution File:**
```xml
<Project>
  <ProjectReference Include="src/GUI/GUI.csproj" />
  <ProjectReference Include="src/Core/DivinityModManagerCore.csproj" />
</Project>
```

**Updated Solution File:**
```xml
<Project>
  <ProjectReference Include="src/GUI/GUI.csproj" Condition="'$(OSPlatform)' == 'Windows'" />
  <ProjectReference Include="src/GUI.Avalonia/GUI.Avalonia.csproj" Condition="'$(OSPlatform)' != 'Windows' OR '$(AlwaysUseAvalonia)' == 'true'" />
  <ProjectReference Include="src/Core/DivinityModManagerCore.csproj" />
</Project>
```

### 6.2 Update BuildRelease.py

**Current:** Windows-only zip release

**Changes Required:**
```python
# Detect platform and build accordingly
platforms = ["windows", "linux"]  # "macos" future

for platform in platforms:
    # Build for target platform
    # dotnet publish -c Publish -r [platform]-x64

    # Create platform-specific installer/archive
    # Windows: Create .exe or .msi with installer
    # Linux: Create .AppImage or tar.gz with dependencies

    # Package releases
```

### 6.3 CI/CD Workflow

**Create GitHub Actions workflow:**
```yaml
name: Build Releases
on: [push, release]
jobs:
  build-windows:
    runs-on: windows-latest
    steps:
      - Build for Windows
      - Create Windows installer

  build-linux:
    runs-on: ubuntu-latest
    steps:
      - Build for Linux
      - Create Linux AppImage
```

---

## 7. Phase 6: Documentation (Week 4)

### 7.1 Developer Guide

**File:** `AVALONIA_DEVELOPER_GUIDE.md`

**Sections:**
1. **Architecture Overview**
   - How to navigate codebase
   - Core vs GUI separation
   - Platform abstraction layer

2. **Common Tasks**
   - Adding a new view
   - Adding a new converter
   - Adding a custom control
   - Accessing platform-specific services

3. **Building & Testing**
   - Building for Windows
   - Building for Linux
   - Running locally
   - Testing on different platforms

4. **Styling & Theming**
   - How to modify themes
   - Adding new control styles
   - Dark/Light mode implementation

5. **Platform-Specific Code**
   - When to use #if WINDOWS/#if LINUX
   - Testing platform-specific code
   - Cross-platform alternatives

---

### 7.2 Migration Notes

**File:** `MIGRATION_NOTES.md`

**Content:**
- What changed from WPF
- Breaking changes (if any)
- New patterns introduced
- Known limitations

---

### 7.3 Team Onboarding

**File:** `GETTING_STARTED_AVALONIA.md`

**Content:**
- Prerequisites: .NET 8.0, Git, VS Code or Visual Studio
- Cloning repository
- Building locally (Windows & Linux)
- Running in debug mode
- Common troubleshooting

---

## 8. Dependency Changes Summary

### Removed (WPF-specific)
- ❌ AdonisUI 1.17.1
- ❌ AdonisUI.ClassicTheme 1.17.1
- ❌ DotNetProjects.Extended.Wpf.Toolkit 5.0.124
- ❌ Ookii.Dialogs.Wpf 5.0.1
- ❌ ReactiveUI.WPF 20.1.63
- ❌ WpfScreenHelper 2.1.1
- ❌ gong-wpf-dragdrop 4.0.0
- ❌ Autoupdater.NET.Official 1.9.2

### Added (Avalonia-specific)
- ✅ Avalonia 11.1.x
- ✅ Avalonia.Controls.DataGrid 11.1.x
- ✅ Avalonia.Themes.Fluent 11.1.x
- ✅ ReactiveUI.Avalonia 20.1.x

### Kept (Cross-platform)
- ✅ ReactiveUI 20.1.63
- ✅ ReactiveUI.Fody 19.5.41
- ✅ ReactiveProperty 9.7.0
- ✅ ReactiveHistory 0.10.7
- ✅ System.Reactive 6.0.1
- ✅ DynamicData 9.1.2
- ✅ Splat 15.3.1 & Splat.Drawing 15.3.1
- ✅ SharpCompress 0.39.0
- ✅ ZstdSharp.Port 0.8.5
- ✅ Newtonsoft.Json 13.0.3
- ✅ Gameloop.Vdf 0.6.2
- ✅ NexusModsNET 1.0.5
- ✅ System.CSharp 4.7.0
- ✅ System.Text.RegularExpressions 4.3.1
- ✅ System.Net.Http 4.3.4

---

## 9. Risk Assessment & Mitigation

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|-----------|
| LSLibNative compatibility on Linux | High | High | Investigate alternative approaches; may need custom wrapper |
| Custom control porting complexity | Medium | High | Port one control at a time with testing |
| ReactiveUI Avalonia binding differences | Medium | Medium | Create binding wrapper helpers; reference Avalonia docs |
| Performance regression | Low | Medium | Profile and benchmark both platforms |
| Accessibility regression (screen readers) | Medium | Medium | Test with screen readers on both platforms |

---

## 10. Testing Strategy

### 10.1 Unit Testing
- Core logic tests (unchanged from WPF)
- Platform abstraction layer tests
- Converter tests

### 10.2 Integration Testing
- Full workflow: Load mod → Organize → Export
- Platform-specific features: File dialogs, registry access, process launching
- Drag-and-drop functionality

### 10.3 Platform Testing
- Windows 10/11 (multiple versions)
- Linux (Ubuntu 20.04, 22.04, latest)
- Different screen sizes and DPI settings

---

## 11. Timeline & Milestones

| Phase | Duration | Milestone | Branch |
|-------|----------|-----------|--------|
| 1 | Week 1 | Foundation setup, Project structure created | `feature/avalonia-setup` |
| 2 | Week 1-2 | Core refactoring, Platform abstraction working | `feature/platform-abstraction` |
| 3 | Week 2-4 | Avalonia UI functional (75%+ complete) | `feature/avalonia-ui` |
| 4 | Week 3-4 | Platform integrations complete | `feature/platform-integrations` |
| 5 | Week 4 | Build system updated | `feature/build-system` |
| 6 | Week 4 | Documentation complete | `main` |
| 7 | Ongoing | Testing & bug fixes | `main` |

---

## 12. Rollout Strategy

### Phase A: Internal Testing (Weeks 1-3)
- Develop on feature branches
- Internal testing across Windows/Linux
- Gather team feedback

### Phase B: Beta Release (Week 4+)
- Release beta build to community
- Collect issue reports
- Fix critical issues

### Phase C: Full Release
- Publish Linux release alongside Windows
- Archive old WPF release
- Announce cross-platform support

---

## 13. Post-Migration Future Considerations

1. **macOS Support** - Avalonia support is already there, minimal effort for Mac release
2. **Web Version** - Avalonia has experimental web support
3. **Linux Package Managers** - Distribute via Snap, Flatpak, AppImage
4. **Native Installers** - Create installers for different Linux distributions

---

## Key Success Factors

1. ✅ Maintain clean separation between Core and GUI
2. ✅ Test thoroughly on both Windows and Linux
3. ✅ Document changes clearly for team
4. ✅ Keep backwards compatibility where possible
5. ✅ Performance parity with WPF version
6. ✅ Comprehensive accessibility support

---

## Questions & Discussion Points

1. **WPF Maintenance:** After Avalonia migration, should we deprecate WPF version or maintain both?
2. **LSLibNative:** Need to investigate Linux compilation or alternatives
3. **Screen Reader Support:** Avalonia's accessibility APIs - what's available on Linux?
4. **Release Cadence:** Continue weekly/monthly releases, or version-lock until migration complete?

