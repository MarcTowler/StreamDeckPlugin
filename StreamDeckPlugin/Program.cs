using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace StreamDeckPlugin
{
    class Program
    {
        public enum Destinations : int
        {
            HARDWARE_AND_SOFTWARE = 0,
            HARDWARE_ONLY = 1,
            SOFTWARE_ONLY = 2
        }

        [Option(ShortName = "port")]
        public string port { get; set; }

        [Option(ShortName = "pluginUUID")]
        public string UUID { get; set; }

        [Option(ShortName = "registerEvent")]
        public string eventType { get; set; }

        [Option(ShortName = "info")]
        public string info { get; set; }

        private static ClientWebSocket socket = new ClientWebSocket();

        public static int Main(string[] args)
        => CommandLineApplication.Execute<Program>(args);

        private async void OnExecute()
        {
            Console.WriteLine($"Port is {port} and UUID is {UUID}");

            await socket.ConnectAsync(new Uri("ws://127.0.0.1:" + port), CancellationToken.None);

            var registration = new RegisterPlugin
            {
                @event = eventType,
                uuid = UUID
            };
            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(registration))),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public class RegisterPlugin
    {
        public string @event { get; set; }
        public string uuid { get; set; }
    }
}
