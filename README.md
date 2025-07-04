# Control Panel

Proof of Concept System for Admin Panels

## Tooling

- [Git](https://git-scm.com/) - Used for Source Version Control
- [Task](https://taskfile.dev/installation/) - Alternative to Make that supports Windows
- [.NET 9 Sdk](https://dotnet.microsoft.com/en-us/download) - The .NET shared code used for .Net services
- [Libman](https://learn.microsoft.com/en-us/aspnet/core/client-side/libman/libman-cli?view=aspnetcore-9.0#installation) - Javascript Library management CLI 
- [Protobuf](https://learn.microsoft.com/en-us/aspnet/core/grpc/protobuf?view=aspnetcore-9.0) - Serialization technology used for sending messages between services

## Development

The control panel can be developed on any system and using any IDE that supports .Net 9.0.

## Design

TODO

### Build From Source

1. First verify you have all the [listed tooling installed](#tooling).
2. Restore Javascript libraries by running `task libman-restore`
3. Build panel by running `task build-panel` or the service by running `task build-example`

### Supported IDES

The following IDEs support .NET development but also have different tradeoffs depending on your needs.

- [Jetbrains Rider](https://www.jetbrains.com/rider/)
- [Visual Studio 2022 with ASP.NET workload](https://visualstudio.microsoft.com/)
- [Visual Studio Code](https://code.visualstudio.com/)

`Visual Studio Code`, also referred to as `VSCode`, is crossplatform and the lightest IDE option but offers less intellisense help than the other two options.
`Visual Studio 2022` offers the most rich intellisense and feature support while only supporting Windows development and is commonly paired with tools such as [ReSharper](https://www.jetbrains.com/resharper/).
`JetBrains Rider` is a cross-platform IDE that supports a majority of the checks and features Visual Studio provides but is not an official Microsoft tool so it can sometimes miss the newest features. Rider does also provide the types of features Resharper is used for natively in the IDE.