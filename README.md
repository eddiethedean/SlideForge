# SlideForge

An open-source, extensible e-learning authoring tool that replicates the core workflow of Articulate Storyline while remaining transparent and developer-friendly.

## Project Goals

- Desktop-first visual authoring experience
- Trigger-based interaction model
- HTML/JavaScript runtime output
- SCORM-compatible exports
- Open data formats and extensible architecture

## Architecture

The project is organized into four main components:

- **Authoring.Core** – Pure C# domain model (slides, layers, triggers, variables)
- **Authoring.Desktop** – Visual editor application (Avalonia-based)
- **Authoring.Player** – Generated HTML/JS runtime
- **Authoring.Export** – SCORM and HTML exporters

## Technology Stack

- **Language**: C# (.NET 10.0)
- **Desktop UI**: Avalonia (cross-platform)
- **Serialization**: JSON
- **Runtime**: HTML, CSS, JavaScript
- **Packaging**: SCORM 1.2 (initial), xAPI (future)

## Getting Started

### Prerequisites

- .NET SDK 10.0 or later
- An IDE (Visual Studio, Rider, or VS Code)

### Building the Solution

```bash
dotnet build
```

### Running the Desktop Application

```bash
dotnet run --project src/Authoring.Desktop/Authoring.Desktop.csproj
```

## MVP Feature Set

- Slide-based project structure
- Objects: text, images, buttons
- Simple timeline (start time + duration)
- Triggers: on click, on timeline start
- Variables: boolean, number, string
- Slide navigation actions

## Data Model

Projects are stored as JSON files describing slides, objects, timelines, triggers, and variables. The editor operates on this model, and exporters translate it into runnable output.

- Project → Slides → Layers → Objects
- Triggers evaluated at runtime
- Variables stored in a global state container

## License

This project will be licensed under MIT or Apache 2.0 (to be determined).

## Contributing

Contributions are welcome! This project is in early development. See the [project plan](Open_Source_Storyline_Alternative_Plan.md) for more details on the roadmap and architecture.
