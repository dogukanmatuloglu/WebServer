using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebServer
{
    public class HttpServer
    {
        private Socket httpServer;
        private int serverPort;
        private Thread thread;
        private string rootPath;

        public HttpServer()
        {
            serverPort = Convert.ToInt32(ConfigurationManager.AppSettings["portNumber"]);
            rootPath = ConfigurationManager.AppSettings["rootPath"];
        }
        public void Start()
        {
            try
            {
                httpServer = new Socket(SocketType.Stream, ProtocolType.Tcp);

                if (serverPort > 65535 || serverPort <= 0)
                {
                    throw new Exception("Server Port not within the range");
                }

                thread = new Thread(new ThreadStart(StartListeningForConnections));
                thread.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

        }

        /// <summary>
        /// Starts listening for incoming connections on the specified server port.
        /// Binds the server socket to any available IP address and begins handling connection requests.
        /// </summary>
        private void StartListeningForConnections()
        {
            try
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, serverPort);
                httpServer.Bind(endpoint);
                httpServer.Listen(2);
                HandleClientRequest();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Handles incoming client requests by receiving data, parsing HTTP requests,
        /// and responding with the requested content or a 404 Not Found response.
        /// </summary>
        private void HandleClientRequest()
        {
            while (true)
            {
                string data = "";
                byte[] bytes = new byte[2048];

                Socket client = httpServer.Accept();
                while (true)
                {
                    int numBytes = client.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, numBytes);

                    if (data.IndexOf("\r\n") > -1)
                        break;
                }
                string requestedPath = ParseHttpRequest(data);

                ProcessHttpGetMethod(client, requestedPath);

            }
        }

        private void ProcessHttpGetMethod(Socket client, string requestedPath)
        {
            if (!string.IsNullOrEmpty(requestedPath))
            {
                string filePath = Path.Combine(rootPath, requestedPath.TrimStart('/'));

                if (File.Exists(filePath))
                {
                    byte[] fileData = File.ReadAllBytes(filePath);

                    string contentType = GetContentType(filePath);

                    string responseHeader = "HTTP/1.1 200 OK\nServer: my_csharp_server\nContent-Type: " + contentType + "\n\n";

                    byte[] responseData = Encoding.UTF8.GetBytes(responseHeader);
                    byte[] responseFileData = new byte[responseData.Length + fileData.Length];
                    responseData.CopyTo(responseFileData, 0);
                    fileData.CopyTo(responseFileData, responseData.Length);

                    client.Send(responseFileData);

                    client.Close();
                }
                else
                {
                    Send404Response(client);
                }
            }
        }

        private string ParseHttpRequest(string httpRequest)
        {
            string[] requestLines = httpRequest.Split(new[] { "\r\n" }, StringSplitOptions.None);
            if (requestLines.Length > 0)
            {
                string[] requestParts = requestLines[0].Split(' ');
                if (requestParts.Length > 1)
                {
                    return requestParts[1];
                }
            }
            return string.Empty;
        }

        private string GetContentType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".html":
                    return "text/html";
                case ".css":
                    return "text/css";
                case ".js":
                    return "application/javascript";
                default:
                    return "application/octet-stream";
            }
        }

        private void Send404Response(Socket client)
        {
            string responseHeader = "HTTP/1.1 404 Not Found\nServer: my_csharp_server\nContent-Type: text/html\n\n";
            string responseBody = "<html><head><title>404 Not Found</title></head><body><h1>404 Not Found</h1></body></html>";
            string response = responseHeader + responseBody;
            client.Send(Encoding.UTF8.GetBytes(response));
            client.Close();
        }
    }
}
