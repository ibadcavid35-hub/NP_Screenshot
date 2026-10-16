using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var listener = new Socket(
                addressFamily: AddressFamily.InterNetwork,
                socketType: SocketType.Dgram,
                protocolType: ProtocolType.Udp);

            var ip = IPAddress.Parse("127.0.0.1");
            var port = 4567;

            var listenerEp = new IPEndPoint(ip, port);

            listener.Bind(listenerEp);

            var count = 0;
            var msj = string.Empty;
            var buffer = new byte[ushort.MaxValue - 1];

            var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0) as EndPoint;


            Console.WriteLine("The Server is ready.");

            while (true)
            {
                var result = await listener.ReceiveFromAsync(
                    new ArraySegment<byte>(buffer),
                    socketFlags: SocketFlags.None,
                    remoteEndPoint: remoteEndPoint);

                count = result.ReceivedBytes;
                msj = Encoding.Default.GetString(buffer, 0, count);
                Console.WriteLine($"RemoteEnd Point: {result.RemoteEndPoint} => {msj}");

                int width = 800;
                int height = 600;

                if (msj.Trim().ToLower() == "screenshot")
                {
                    using (var bitmap = new Bitmap(width, height))
                    {
                        using (var graphics = Graphics.FromImage(bitmap))
                        {
                            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                        }

                        using (var ms = new MemoryStream())
                        {
                            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            var imgBytes = ms.ToArray();

                            await listener.SendToAsync(
                                new ArraySegment<byte>(imgBytes),
                                socketFlags: SocketFlags.None,
                                result.RemoteEndPoint);
                        }

                    }
                }
            }
        }
    }
}