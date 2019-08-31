using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.IO;
using System.Threading;

namespace VideoViewer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random randomVideo = new Random();

        OptionsWindow optionsWindow;

        SerialPort myPort;
        int timer;

        public MainWindow()
        {
            InitializeComponent();

            myPort = new SerialPort();

            playerMediaElement.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Videos\0.mp4", UriKind.Absolute);

            playerMediaElement.LoadedBehavior = MediaState.Manual;
            playerMediaElement.UnloadedBehavior = MediaState.Manual;

            playerMediaElement.Play();

            string portName;
            string baudRate;

            using (StreamReader sr = new StreamReader("portOptions.txt", Encoding.Default))
            {
                portName = sr.ReadLine();
                baudRate = sr.ReadLine();
                timer = int.Parse(sr.ReadLine());
            }

            try
            {
                myPort.PortName = portName;
                myPort.BaudRate = int.Parse(baudRate);

                myPort.DataBits = 8;
                myPort.Handshake = Handshake.None;
                myPort.Parity = Parity.None;
                myPort.StopBits = StopBits.One;
                myPort.DtrEnable = true;

                myPort.Open();

                Thread threadFalseTimerState = new Thread(TestOnNewLogWhereTimer) { IsBackground = true };
                threadFalseTimerState.Start();
            }
            catch (Exception ex)
            {
                optionsWindow = new OptionsWindow();
                optionsWindow.Show();

                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                this.Close();
            }
        }

        void TestOnNewLogWhereTimer()
        {
            while (true)
            {
                try
                {
                    if (myPort.ReadLine() != null)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            playerMediaElement.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Videos\" + randomVideo.Next(1, 21) + ".mp4", UriKind.Absolute);
                            backroundTextBlock.Opacity = 100;
                            playerMediaElement.Play();
                            Thread.Sleep(500);
                            backroundTextBlock.Opacity = 0;
                        });
                        Thread.Sleep(timer * 1000);

                        myPort.DiscardInBuffer();
                        myPort.DiscardOutBuffer();
                    }
                }
                catch { break; }
            }
        }

        private void PlayerMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            playerMediaElement.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Videos\0.mp4", UriKind.Absolute);
            backroundTextBlock.Opacity = 100;
            playerMediaElement.Play();
            Thread.Sleep(500);
            backroundTextBlock.Opacity = 0;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.P)
            {
                myPort.Close();

                optionsWindow = new OptionsWindow();
                optionsWindow.Show();

                this.Close();
            }

            if (e.Key == Key.Space)
            {
                playerMediaElement.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"Videos\" + randomVideo.Next(1, 21) + ".mp4", UriKind.Absolute);
                backroundTextBlock.Opacity = 100;
                playerMediaElement.Play();
                Thread.Sleep(500);
                backroundTextBlock.Opacity = 0;
            }
        }
    }
}
