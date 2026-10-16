using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnCapture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var client = new Socket(
                    addressFamily: AddressFamily.InterNetwork,
                    socketType: SocketType.Dgram,
                    protocolType: ProtocolType.Udp);

                var ip = IPAddress.Parse("127.0.0.1");
                var port = 4567;

                var remoteEndPoint = new IPEndPoint(ip, port);

                var msj = Encoding.Default.GetBytes("screenshot");
                await client.SendToAsync(
                    new ArraySegment<byte>(msj),
                    socketFlags: SocketFlags.None,
                    remoteEP: remoteEndPoint);

                var buffer = new byte[ushort.MaxValue - 1];

                var result = await client.ReceiveFromAsync(
                    new ArraySegment<byte>(buffer),
                    socketFlags: SocketFlags.None,
                    remoteEndPoint: remoteEndPoint
                );

                using (var ms = new MemoryStream(buffer, 0, result.ReceivedBytes))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();

                    screenshotImage.Source = bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Occured: {ex.Message}");   
            }
        }
    }
}