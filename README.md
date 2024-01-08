# HTTP Server with C#

This is a simple HTTP server implemented in C# using sockets. It can handle basic GET requests and serve static files from a specified root directory.

## Features

- Handles basic HTTP GET requests
- Serves static files from the specified root directory
- Supports configurable server port and root path

## Requirements

- .NET Framework (or .NET Core) installed
- Visual Studio or any other C# development environment

## Usage

1. Clone the repository:

    ```bash
    git clone https://github.com/yourusername/http-server-csharp.git
    cd http-server-csharp
    ```

2. Open the solution in your preferred C# development environment.

3. Set the server port and root path in the `app.config` file:

    ```xml
    <appSettings>
        <add key="portNumber" value="9001" />
        <add key="rootPath" value="wwwroot" />
    </appSettings>
    ```

    - `portNumber`: The port on which the server will listen.
    - `rootPath`: The root directory from which the server will serve files.

4. Build and run the project.

5. Open your web browser and navigate to `http://localhost:9001` (or the specified port).

## Contributing

Contributions are welcome! Please feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
