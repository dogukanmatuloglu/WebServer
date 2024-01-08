using System.Net.Sockets;

namespace WebServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HttpServer server = new HttpServer();
            server.Start();
        }

        
    }
}