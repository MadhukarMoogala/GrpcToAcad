# Integrating gRPC.NET with AutoCAD

This project demonstrates how to talk with gRPC server from AutoCAD plugin client.

### Client

.NET 8.0 plugin for AutoCAD 2025.

```csharp
public class Entry
{
    [CommandMethod("TestGrpc")]
    public static async void TestGrpc()
    {
        var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument ?? throw new NullReferenceException("no active document");
        var ed = doc.Editor;
        using var channel = GrpcChannel.ForAddress("https://localhost:8080");
        var client = new Greeter.GreeterClient(channel);
        var reply = await client.SayHelloAsync(
                          new HelloRequest { Name = "GreeterClient" });
        ed.WriteMessage($"Greeting:{reply.Message}\n");
    }

}
```

- It establishes a gRPC channel using `GrpcChannel.ForAddress("https://localhost:8080")`. This creates a communication channel to a gRPC server running on localhost (host machine) at port 8080

- Creates a gRPC client object of type `Greeter.GreeterClient` using the established channel. This client allows interacting with the gRPC service defined by the `Greeter` interface

- An asynchronous call is made to the gRPC service's `SayHelloAsync` method. This method likely takes a `HelloRequest` message (containing a name field) as input and returns a `HelloReply` message.

- In this particular code, the `HelloRequest` has its `Name` property set to "GreeterClient".

- The code awaits the response (`reply`) from the `SayHelloAsync` call.

- It then extracts the `Message` property  from the `reply` object.

- Finally, it uses the document editor (`ed.WriteMessage`) to write a formatted message ("Greeting:{reply.Message}") into the active AutoCAD document.

### Protobuf

    

```protobuf
syntax = "proto3";
option csharp_namespace = "acadClient";
package greet;

// The greeting service definition.
service Greeter {
  // Sends a greeting
  rpc SayHello (HelloRequest) returns (HelloReply);
}

// The request message containing the user's name.
message HelloRequest {
  string name = 1;
}

// The response message containing the greetings.
message HelloReply {
  string message = 1;
}
```

- Defines a `Greeter` service with a `SayHello` RPC method. Clients can send a `HelloRequest` message containing their name, and the server will respond with a `HelloReply` message containing a greeting. The `.NET`-specific `csharp_namespace` option ensures the generated code will be compatible with C# projects.

### Server

```csharp
using GrpcGreeterService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Hello Autodesk!");
app.Run();
```

- Expose the `GreeterService` as a gRPC service, allowing clients to send `SayHello` requests and receive greetings.
- Provide a basic HTTP endpoint at the root path for potential testing or informational purposes.

When you run this application, it will act as a gRPC server and an HTTP server at the same time. Clients can interact with the `GreeterService` using gRPC clients, while you can also access the root path using a web browser or other HTTP clients to get the "Hello Autodesk!" message.

### Service

```csharp
public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}
```

- When a client sends a `SayHello` request to the gRPC server, an instance of the `GreeterService` class will be used to handle the request.
- The `SayHello` method extracts the client's name from the `request` object.
- It constructs a `HelloReply` message with a personalized greeting ("Hello" + client's name).
- The completed task with the `HelloReply` message is returned, which will be sent back to the client as the response.

### Build Instructions

```bash
git clone https://github.com/MadhukarMoogala/GrpcToAcad.git
cd GrpcToAcad
dotnet build acadClient -c Debug -a x64
dotnet build GrpcGreeterService -c Debug -a x64
```

### Usage Instructions

- Start gRPC server    
  
  ```bash
  cd GrpcGreeterService
  dotnet run
  ```

- Launch AutoCAD 2025
  
  - NETLOAD `acadClient.dll`
  - Run `TestGrpc`

![GrpcToAcad](https://github.com/MadhukarMoogala/GrpcToAcad/assets/6602398/e8bf0699-0c4d-4a3d-973a-506e420a594f)

### License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT).

### Written by

Madhukar Moogala, [APS](http://aps.autodesk.com/) @galakar


