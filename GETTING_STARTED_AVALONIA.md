# Getting Started with BG3ModManager (Avalonia)

Welcome! This guide will help you set up the BG3ModManager development environment and start contributing.

## Prerequisites

### System Requirements

**Windows:**
- Windows 10 or later
- .NET 8.0 SDK or later
- Git
- Visual Studio 2022 or Visual Studio Code

**Linux:**
- Ubuntu 20.04 LTS or later (or equivalent distribution)
- .NET 8.0 SDK or later
- Git
- Visual Studio Code or Rider

**macOS (optional, for future support):**
- macOS 10.15 or later
- .NET 8.0 SDK or later
- Xcode Command Line Tools
- Visual Studio Code or Rider

### Software Installation

#### Windows

1. **Install .NET 8.0 SDK**
   - Download from: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
   - Choose Windows x64 installer
   - Run installer and follow prompts
   - Verify: `dotnet --version` (should show 8.0.x or higher)

2. **Install Git**
   - Download from: https://git-scm.com/download/win
   - Use default installation settings

3. **Install IDE (choose one)**
   - **Visual Studio 2022:** https://visualstudio.microsoft.com/
     - Community edition is free
     - Install workload: ".NET desktop development"
   - **Visual Studio Code:** https://code.visualstudio.com/
     - Install extensions: C#, .NET Extension Pack

#### Linux (Ubuntu)

```bash
# Update package manager
sudo apt update

# Install .NET 8.0 SDK
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version 8.0

# Add to PATH
export PATH=$PATH:$HOME/.dotnet
echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc

# Verify installation
dotnet --version

# Install Git
sudo apt install git

# Install Visual Studio Code (optional)
sudo apt install code

# Install additional dependencies for GUI
sudo apt install libatk1.0-0 libpango-1.0-0 libgdk-pixbuf2.0-0
```

#### macOS

```bash
# Install using Homebrew
brew install dotnet@8
brew install git

# Or download from:
# https://dotnet.microsoft.com/en-us/download/dotnet/8.0 (macOS x64 or arm64)
```

## Setting Up the Repository

### 1. Clone the Repository

```bash
git clone https://github.com/LaughingLeader/BG3ModManager.git
cd BG3ModManager
```

### 2. Initialize Submodules

The project has external dependencies (LSLib, CrossSpeak) as Git submodules:

```bash
git submodule update --init --recursive
```

### 3. Verify Project Structure

```bash
# List main projects
ls -la src/

# Check for required files
ls BG3ModManager.sln
ls AVALONIA_MIGRATION_PLAN.md
ls AVALONIA_DEVELOPER_GUIDE.md
```

You should see:
```
src/
├── GUI/              (Legacy WPF - Windows-only)
├── GUI.Avalonia/     (New Avalonia - Cross-platform)
├── Core/             (Shared business logic)
└── Toolbox/          (Utility application)
```

## Building the Project

### Windows

**Using Visual Studio 2022:**
1. Open `BG3ModManager.sln` in Visual Studio
2. Wait for solution to load and restore NuGet packages
3. Select Configuration: **Debug**
4. Select Startup Project: **GUI.Avalonia**
5. Press **F5** to build and run

**Using Command Line:**
```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build --configuration Debug

# Run the application
dotnet run --project src/GUI.Avalonia/GUI.Avalonia.csproj --configuration Debug
```

### Linux

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build --configuration Debug

# Run the application
dotnet run --project src/GUI.Avalonia/GUI.Avalonia.csproj --configuration Debug

# Or build a release
dotnet publish --configuration Release --runtime linux-x64 --self-contained true \
  --project src/GUI.Avalonia/GUI.Avalonia.csproj
```

### macOS

```bash
# Same as Linux
dotnet restore
dotnet build --configuration Debug
dotnet run --project src/GUI.Avalonia/GUI.Avalonia.csproj --configuration Debug
```

## Verifying the Installation

1. **After a successful build**, the application should launch
2. **Check the console** for any error messages
3. **Main Window** should display:
   - Mod list (empty if this is first run)
   - Settings button
   - Export/Import buttons
   - Drag-and-drop area for mod reordering

### Troubleshooting Build Issues

**Issue:** `Project type not recognized` or `SDK not found`
```bash
# Verify .NET SDK is installed
dotnet --version

# If not version 8.0 or higher, install it:
# https://dotnet.microsoft.com/download/dotnet/8.0
```

**Issue:** `Submodules not initialized`
```bash
# Verify submodules are cloned
ls External/lslib/
ls External/CrossSpeak/

# If empty, initialize them
git submodule update --init --recursive
```

**Issue:** `NuGet package restoration failed`
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore again
dotnet restore
```

**Issue:** Application crashes on startup (Linux)
```bash
# Install required system libraries
sudo apt install libgtk-3-0 libx11-6 libxkbcommon0

# Try running with verbose output
DOTNET_CLI_VERBOSITY=diagnostic dotnet run --project src/GUI.Avalonia/GUI.Avalonia.csproj
```

## Project Structure Quick Tour

```
BG3ModManager/
├── src/
│   ├── Core/                    # Shared business logic (cross-platform)
│   │   ├── Models/              # Data classes
│   │   ├── Services/            # Application services
│   │   ├── Platform/            # Cross-platform abstraction
│   │   └── ViewModels/          # Base MVVM classes
│   │
│   ├── GUI/                     # Legacy WPF (Windows-only)
│   │   ├── Views/               # XAML windows/dialogs
│   │   ├── ViewModels/          # UI logic
│   │   ├── Controls/            # Custom controls
│   │   ├── Themes/              # Styling
│   │   └── App.xaml             # Application resources
│   │
│   ├── GUI.Avalonia/            # New Avalonia (Cross-platform)
│   │   ├── Views/               # AXAML windows/dialogs
│   │   ├── ViewModels/          # UI logic
│   │   ├── Controls/            # Custom controls
│   │   ├── Themes/              # Styling
│   │   └── App.xaml             # Application resources
│   │
│   └── Toolbox/                 # Utility CLI application
│
├── External/                    # Git submodules (external projects)
│   ├── lslib/                   # Game file format library
│   └── CrossSpeak/              # Screen reader support
│
├── AVALONIA_MIGRATION_PLAN.md   # High-level migration strategy
├── AVALONIA_DEVELOPER_GUIDE.md  # Detailed development guide
├── GETTING_STARTED_AVALONIA.md  # This file
├── README.md                    # General project info
└── BG3ModManager.sln            # Visual Studio solution
```

## Common Development Tasks

### Running in Debug Mode

```bash
# With console output for debugging
dotnet run --project src/GUI.Avalonia/GUI.Avalonia.csproj \
  --configuration Debug \
  --verbose
```

### Building for Release

```bash
# Build optimized release for Windows
dotnet publish --configuration Release --runtime win-x64 \
  --project src/GUI.Avalonia/GUI.Avalonia.csproj \
  --self-contained true

# Build optimized release for Linux
dotnet publish --configuration Release --runtime linux-x64 \
  --project src/GUI.Avalonia/GUI.Avalonia.csproj \
  --self-contained true
```

### Running Tests

```bash
# If tests are implemented
dotnet test --configuration Debug
```

### Cleaning Build Output

```bash
# Remove build artifacts
dotnet clean

# Remove all bin/obj directories
find . -type d -name "bin" -o -type d -name "obj" | xargs rm -rf
```

## Using Git Effectively

### Clone with Submodules

```bash
# Initial clone with submodules
git clone --recurse-submodules https://github.com/LaughingLeader/BG3ModManager.git

# Or if already cloned without submodules
git submodule update --init --recursive
```

### Working on a Feature

```bash
# Create a feature branch
git checkout -b feature/my-feature-name

# Make changes, then stage and commit
git add .
git commit -m "Add my new feature"

# Push to GitHub
git push origin feature/my-feature-name

# Create a Pull Request via GitHub website
```

### Staying Up to Date

```bash
# Fetch latest changes from main branch
git fetch origin

# Update main branch locally
git checkout main
git pull origin main

# Update your feature branch with latest changes
git rebase origin/main
```

## IDE Setup

### Visual Studio Code Configuration

Create `.vscode/launch.json` for debugging:

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Launch (console)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/bin/Debug/net8.0/BG3ModManager.dll",
            "args": [],
            "cwd": "${workspaceFolder}",
            "stopAtEntry": false,
            "console": "internalConsole"
        }
    ]
}
```

### Rider Configuration

1. Open `BG3ModManager.sln` in Rider
2. Rider will automatically detect the project structure
3. Set **GUI.Avalonia** as the default run configuration
4. Press **Shift + F10** to run

## Understanding the Codebase

### Architecture Overview

The application follows **MVVM (Model-View-ViewModel)** pattern:

```
User Interface (View)
    ↓
User Input → ViewModel (Business Logic)
    ↓
Service (Core Business Logic)
    ↓
Model (Data)
```

### Key Directories

| Directory | Purpose | Modifiable |
|-----------|---------|-----------|
| `src/Core` | Business logic, cross-platform | Yes |
| `src/GUI.Avalonia` | UI layer (Avalonia) | Yes |
| `src/GUI` | Legacy UI (WPF) | Deprecated |
| `External/` | External dependencies | No (submodules) |

### Important Files

| File | Purpose |
|------|---------|
| `src/Core/DivinityApp.cs` | Main application class |
| `src/Core/Platform/IPlatformServices.cs` | Platform abstraction interface |
| `src/GUI.Avalonia/App.xaml` | Application resources |
| `src/GUI.Avalonia/Views/MainWindow.axaml` | Main UI layout |
| `AVALONIA_MIGRATION_PLAN.md` | Migration strategy document |

## Making Your First Change

### Example: Adding a Message to the UI

1. **Find the UI file:**
   ```
   src/GUI.Avalonia/Views/MainWindow.axaml
   ```

2. **Open and locate the main StackPanel or Grid**

3. **Add a TextBlock:**
   ```xml
   <TextBlock Text="Hello from BG3ModManager!" Margin="10" />
   ```

4. **Save the file**

5. **Run the application:**
   ```bash
   dotnet run --project src/GUI.Avalonia/GUI.Avalonia.csproj --configuration Debug
   ```

6. **You should see your text in the window!**

### Example: Adding a New Feature

1. **Create a new file in the appropriate directory:**
   ```bash
   # For a new ViewModel
   src/GUI.Avalonia/ViewModels/MyFeatureViewModel.cs

   # For a new View
   src/GUI.Avalonia/Views/MyFeatureWindow.axaml
   src/GUI.Avalonia/Views/MyFeatureWindow.axaml.cs
   ```

2. **Implement the feature following patterns from existing files**

3. **Test locally:**
   ```bash
   dotnet run --project src/GUI.Avalonia/GUI.Avalonia.csproj
   ```

4. **Commit and push:**
   ```bash
   git add src/GUI.Avalonia/ViewModels/MyFeatureViewModel.cs
   git commit -m "Add my feature"
   git push origin feature/my-feature
   ```

## Tips for Developers

### Code Style

- Follow existing code patterns in the project
- Use meaningful variable names
- Add comments for complex logic
- Keep methods small and focused

### Testing

- Test your changes locally before committing
- Try on both Windows and Linux if possible
- Check the console for any warning messages

### Debugging

**Enable verbose output:**
```bash
DOTNET_CLI_VERBOSITY=diagnostic dotnet run --project src/GUI.Avalonia/GUI.Avalonia.csproj
```

**Inspect Visual Tree (if running with dev tools enabled):**
- Press `F12` in the application window to open developer tools

### Performance

- Use `ReactiveProperty` for data binding (reactive programming)
- Avoid blocking operations on UI thread
- Test loading large numbers of mods

## Updating Dependencies

### Checking for Updates

```bash
# List outdated NuGet packages
dotnet outdated
```

### Updating a Package

```bash
# Update a specific package
dotnet add src/Core/DivinityModManagerCore.csproj package Newtonsoft.Json --version 13.1.0

# Or edit the .csproj file manually and run
dotnet restore
```

## Getting Help

### Resources

- **Project Documentation:**
  - `AVALONIA_MIGRATION_PLAN.md` - Migration strategy
  - `AVALONIA_DEVELOPER_GUIDE.md` - Development patterns
  - `README.md` - General information

- **External Resources:**
  - [Avalonia Documentation](https://docs.avaloniaui.net/)
  - [ReactiveUI Documentation](https://reactiveui.net/)
  - [.NET 8.0 Documentation](https://docs.microsoft.com/en-us/dotnet/)

- **Community:**
  - GitHub Issues: https://github.com/LaughingLeader/BG3ModManager/issues
  - Discord: https://discord.gg/j5gp6MD (Leader's Lair)

### Asking for Help

When reporting issues or asking questions, include:
1. Your operating system and version
2. .NET SDK version (`dotnet --version`)
3. Steps to reproduce the issue
4. Error messages or console output
5. Screenshots if UI-related

## Next Steps

1. ✅ Set up your development environment
2. ✅ Build the project successfully
3. ✅ Understand the project structure
4. ✅ Read `AVALONIA_DEVELOPER_GUIDE.md` for detailed patterns
5. 📝 Make your first change
6. 🚀 Create a Pull Request

Welcome to the BG3ModManager project!

