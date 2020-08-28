# Escher Terminal

Project dedicate for brushing up C# skills, checking new features since C# 4.0 and playing around with the latest .NET core.

## Getting Started
Use `dotnet` CLI to run the project or download [v/1.0.0](https://github.com/gavar/escher-terminal/releases/download/v%2F1.0.0/escher-terminal-win.zip) prebuilt binaries.
>dotnet run -p src/Escher.Terminal/Escher.Terminal.csproj

## Prerequisites
.NET Core 3.0+

## Summary
Demo console application simulating terminal for entering person details and store on the disk.
Project use flat file database structure to persist records in the `db` folder. Each storage file has own `.meta` containing latest seed for automatic identity incremental.  

## Project Structure

##### [Escher.Xunit](src/Escher.Xunit)
Utilities for file contents assertions via Xunit.

##### [Escher.Core](src/Escher.Core) 
Core library without any application specific business logic. Provides abstraction layer for entity persistence via Repository pattern. 

##### [Escher.Core.Test](test/Escher.Core.Test) 
Unit tests for the `Escher.Core` core foundation.

##### [Escher.App](src/Escher.App) 
Application business layer. Provides configuration bundle for easy installation into external service container.

##### [Escher.App.Test](src/Escher.Core) 
Unit tests for the `Escher.App` application business layer.

##### [Escher.Terminal](src/Escher.Terminal) 
Console application using terminal for interaction with the application.

## Contributing
Project has strictly education purposes and will no accept any pull requests.

## Authors
* **[Max Stankevich](https://github.com/gavar)**

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE.md) file for details
