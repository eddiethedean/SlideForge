# SlideForge

> An open-source, extensible e-learning authoring tool that replicates the core workflow of Articulate Storyline while remaining transparent and developer-friendly.

[![.NET Version](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Tests](https://img.shields.io/badge/tests-437%20passing-brightgreen)](tests/)
[![Coverage](https://img.shields.io/badge/coverage-97%25%2B-brightgreen)](tests/)

## ✨ Features

### Core Authoring Features
- 📐 **Slide-Based Editing** - Intuitive slide-based project structure with visual canvas
- 🎨 **Object Management** - Create and edit text, images, and buttons with drag-and-drop positioning
- 🎭 **Layers System** - Organize content with multiple layers, show/hide controls
- ⏱️ **Timeline Support** - Control object visibility with start time and duration
- 📊 **Variables System** - Create and manage Boolean, Number, and String variables
- 🎯 **Triggers & Actions** - Build interactivity with triggers (OnClick, OnTimelineStart) and actions (Navigate, SetVariable, Show/Hide Layer)
- ✅ **Real-Time Validation** - Automatic validation of references with helpful warnings

### Developer & User Experience
- 🔄 **Auto-Updates** - Built-in update checker connects to GitHub Releases
- 🎮 **Interactive Dragging** - Click and drag objects to reposition with bounds checking
- ⌨️ **Keyboard Shortcuts** - Full keyboard navigation support
- 🧪 **Comprehensive Testing** - 437+ tests with 97%+ code coverage
- 🚀 **CI/CD Pipeline** - Automated builds for Windows, macOS, and Linux (x64 and ARM64)

### Runtime Player
- 🌐 **Web-Based Player** - HTML/JavaScript runtime for browser compatibility
- 📱 **Responsive Design** - Works on desktop and mobile browsers
- 🎬 **Timeline Playback** - Play/pause controls with progress tracking
- 🔀 **Slide Navigation** - Previous/Next slide controls
- ⚡ **Full Feature Support** - Complete runtime implementation of all authoring features

## 🎯 Project Goals

- **Desktop-first** visual authoring experience
- **Trigger-based** interaction model
- **HTML/JavaScript** runtime output
- **SCORM-compatible** exports (planned)
- **Open data formats** and extensible architecture

## 🏗️ Architecture

The project is organized into four main components:

| Component | Status | Description |
|-----------|--------|-------------|
| **Authoring.Core** | ✅ Complete | Pure C# domain model (slides, layers, triggers, variables) |
| **Authoring.Desktop** | ✅ Complete | Visual editor application (Avalonia-based cross-platform UI) |
| **Authoring.Player** | ✅ Complete | HTML/JavaScript runtime player for executing projects |
| **Authoring.Export** | 📋 Planned | SCORM and HTML exporters for distribution |

**Legend:** ✅ Complete | 🚧 In Progress | 📋 Planned

## 🛠️ Technology Stack

- **Language**: C# (.NET 10.0)
- **Desktop UI**: Avalonia UI 11.3.11 (cross-platform)
- **Serialization**: System.Text.Json (polymorphic support)
- **Runtime**: HTML5, CSS3, JavaScript (ES6+)
- **Testing**: xUnit, Moq, FsCheck, BenchmarkDotNet, Avalonia.Headless
- **CI/CD**: GitHub Actions
- **Packaging**: SCORM 1.2 (planned), xAPI (future)

## 🚀 Quick Start

### Prerequisites

- **.NET SDK 10.0** or later ([Download](https://dotnet.microsoft.com/download))
- An IDE: **Visual Studio**, **Rider**, or **VS Code** (recommended: Rider or VS Code with C# extension)

### Installation

#### Option 1: Download Latest Release (Recommended)

Download pre-built binaries from [GitHub Releases](https://github.com/eddiethedean/SlideForge/releases):

- **Windows**: Extract `SlideForge-windows-x64-v*.zip` and run `Authoring.Desktop.exe`
- **macOS**: 
  1. Extract `SlideForge-macos-*.zip`
  2. **Drag `SlideForge.app` to Applications folder** (or run from anywhere)
  3. **Launch**: Double-click `SlideForge.app` - it will automatically remove quarantine on first run
  4. **If blocked**: Right-click `SlideForge.app` → Open → Click "Open" in security dialog
  5. **See also**: [INSTALL_MACOS.md](INSTALL_MACOS.md) for detailed instructions
- **Linux**: Extract `SlideForge-linux-*.tar.gz` and run `./Authoring.Desktop`

#### Option 2: Build from Source

```bash
# Clone the repository
git clone https://github.com/eddiethedean/SlideForge.git
cd SlideForge

# Build the solution
dotnet build

# Run the application
dotnet run --project src/Authoring.Desktop/Authoring.Desktop.csproj
```

### First Steps

1. **Create a New Project**: File → New Project
2. **Add Slides**: Click the "➕ Add Slide" button in the toolbar
3. **Add Objects**: Select a tool (Text, Image, or Button) and click on the canvas
4. **Create Variables**: Use the Variables panel to add project variables
5. **Add Triggers**: Select an object and add triggers in the Properties panel
6. **Save Your Project**: File → Save (projects are saved as JSON files)

## 📖 Usage

### Creating Interactive Content

1. **Add Objects**: Use the toolbar to place text, images, or buttons on your slide
2. **Position Objects**: Click and drag objects to reposition them
3. **Configure Properties**: Use the Properties panel to customize object appearance and behavior
4. **Create Variables**: Add variables in the Variables panel (Boolean, Number, or String)
5. **Add Triggers**: Select an object → Properties panel → Triggers section → Add Trigger
6. **Configure Actions**: Choose actions like Navigate to Slide, Set Variable, or Show/Hide Layer

### Previewing Your Project

Projects can be tested using the HTML/JavaScript runtime player. The player runtime is located in `src/Authoring.Player/www/` and can be used with any web server.

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test categories
dotnet test --filter "Category=Integration"
dotnet test --filter "Category=UI"
dotnet test --filter "Category=Performance"
```

### Test Coverage

- **Line Coverage**: 97.2%+
- **Branch Coverage**: 94%+
- **Total Tests**: 437+ tests
  - **Core Tests**: 125 tests (domain model, serialization, validation)
  - **Desktop Tests**: 312 tests (UI, ViewModels, Services, Integration)
  - **Player Tests**: 47 JavaScript tests (runtime functionality)

### Test Categories

- ✅ **Unit Tests** - Model, service, and ViewModel logic
- ✅ **Integration Tests** - Full workflows and lifecycle testing
- ✅ **UI Tests** - View instantiation and rendering (Avalonia.Headless)
- ✅ **Property-Based Tests** - Invariant verification (FsCheck)
- ✅ **Performance Tests** - Benchmarks for critical operations (BenchmarkDotNet)
- ✅ **JavaScript Runtime Tests** - Player functionality testing

See [tests/TESTING.md](tests/TESTING.md) for comprehensive testing guidelines and best practices.

## 📦 Project Structure

```
SlideForge/
├── src/
│   ├── Authoring.Core/          # Domain models and business logic
│   ├── Authoring.Desktop/       # Avalonia UI editor application
│   ├── Authoring.Player/        # HTML/JavaScript runtime player
│   └── Authoring.Export/        # Exporters (planned)
├── tests/
│   ├── Authoring.Core.Tests/    # Core model tests
│   ├── Authoring.Desktop.Tests/ # Desktop application tests
│   └── Authoring.Player.Tests/  # Player runtime tests
├── .github/
│   └── workflows/               # CI/CD pipelines
└── docs/                        # Documentation
```

## 🔄 Updates & Releases

SlideForge includes an automatic update checker. To check for updates:

1. Go to **Help** → **Check for Updates...**
2. The application will check GitHub Releases for newer versions
3. If an update is available, click **Download and Install**

### Release Schedule

Releases are published automatically when version tags are pushed to the repository. See [.github/workflows/README.md](.github/workflows/README.md) for details on the CI/CD pipeline.

## 📋 Current Status

### ✅ Completed Phases

- **Phase 1**: Core Data Model & JSON Schema ✅
- **Phase 2**: Minimal Desktop Editor (MVP) ✅
- **Phase 3**: Triggers & Variables System ✅
- **Phase 4**: HTML/JavaScript Runtime Player ✅

### 📋 Upcoming

- **Phase 5**: Export System (SCORM and HTML exporters)
- **Phase 6**: MVP Polish & Documentation

See [ROADMAP.md](ROADMAP.md) for detailed progress and upcoming milestones.

## 🤝 Contributing

Contributions are welcome! This project is in active development. Areas where help is needed:

- **Export System**: SCORM and HTML export functionality
- **Documentation**: User guides, tutorials, API documentation
- **Testing**: Additional test coverage and edge cases
- **Features**: See [ROADMAP.md](ROADMAP.md) for planned features

### Development Setup

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Run tests (`dotnet test`) to ensure everything passes
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

## 📄 License

This project will be licensed under **MIT** or **Apache 2.0** (to be determined).

## 🔗 Links

- **Repository**: [GitHub](https://github.com/eddiethedean/SlideForge)
- **Releases**: [GitHub Releases](https://github.com/eddiethedean/SlideForge/releases)
- **Roadmap**: [ROADMAP.md](ROADMAP.md)
- **Testing Guide**: [tests/TESTING.md](tests/TESTING.md)
- **Project Plan**: [Open_Source_Storyline_Alternative_Plan.md](Open_Source_Storyline_Alternative_Plan.md)

## 🙏 Acknowledgments

SlideForge is inspired by Articulate Storyline but built as an open-source alternative with transparency and extensibility in mind.

---

**Current Version**: v0.5.6  
**Status**: Phase 4 Complete - Runtime Player Implemented ✅  
**Next**: Phase 5 - Export System
