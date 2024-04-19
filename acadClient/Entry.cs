using Autodesk.AutoCAD.Runtime;
using System.Threading.Tasks;
using Grpc.Net.Client;
using System.Reflection;



[assembly: ExtensionApplication(typeof(acadClient.GrpcExtension))]
[assembly: CommandClass(typeof(acadClient.Entry))]
namespace acadClient
{

    public class GrpcExtension:IExtensionApplication
    {
        public void Initialize()
        {
        }

        public void Terminate()
        {
        }
    }
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
}
