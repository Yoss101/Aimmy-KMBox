using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Visuality;

namespace MouseMovementLibraries.KMBoxWS
{
    internal class KMBoxWS
    {
        private static ClientWebSocket _webSocket = new ClientWebSocket();
        //private static Timer _keepAlive;

        public static async Task<bool> ConnectWS()
        {
            string localIP = GetLocalIP();
            string found = "";
            string gateway = localIP.Substring(0, localIP.LastIndexOf('.') + 1);
            Console.WriteLine("Searching for connection...");

            for (int i = 1; i < 255; i++)
            {
                string ip = gateway + i;
                bool isOpen = await IsPortOpen(ip, 8765, 10);
                if (isOpen)
                {
                    if (ip == localIP)
                    {
                        new NoticeBar("Do not run Aimmy and the KMBox client on the same machine!\nIf you do not have another computer to use, please select the 'KMBox' method instead.", 10000).Show();
                        continue;
                    }
                    else
                    {
                        found = ip;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(found))
            {
                new NoticeBar("KMBox not found! Make sure the server is running on the other machine!", 5000).Show();
                return false;
            }

            // Initialize the connection
            Uri uri = new Uri($"ws://{found}:8765");
            await _webSocket.ConnectAsync(uri, CancellationToken.None);
            new NoticeBar("KMBox Connected at " + found + ":8765!", 5000).Show();
            return true;
            //_keepAlive = new Timer(SendKeepAlive, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
        }

        public static async void Move(int x, int y)
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                string command = $"MouseMove {x} {y}";
                var buffer = Encoding.UTF8.GetBytes(command);
                await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
        //private static async void SendKeepAlive(object state)
        //{
        //    if (_webSocket.State == WebSocketState.Open)
        //    {
        //        string command = $"MouseMove {1} {1}";
        //        var buffer = Encoding.UTF8.GetBytes(command);
        //        await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        //    }
        //}

        public static async void Down(int vkey)
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                string command = $"Down {vkey}";
                var buffer = Encoding.UTF8.GetBytes(command);
                await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public static async void Up(int vkey)
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                string command = $"Up {vkey}";
                var buffer = Encoding.UTF8.GetBytes(command);
                await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public static string GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "";
        }

        public static async Task<bool> IsPortOpen(string host, int port, int timeout)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var cancellationTokenSource = new System.Threading.CancellationTokenSource(timeout);
                    var connectTask = client.ConnectAsync(host, port);
                    var completedTask = await Task.WhenAny(connectTask, Task.Delay(timeout, cancellationTokenSource.Token));

                    if (completedTask == connectTask)
                    {
                        return client.Connected;
                    }
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}
