﻿using System;
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
using System.Windows.Shapes;
using System.IO.Ports;
using System.IO;

namespace VideoViewer
{
    /// <summary>
    /// Логика взаимодействия для OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        SerialPort selectedPort = new SerialPort();

        public OptionsWindow()
        {
            InitializeComponent();

            COMPortList.ItemsSource = SerialPort.GetPortNames();

            COMPortList.SelectedIndex = 0;

            using (StreamReader sw = new StreamReader("portOptions.txt", Encoding.Default))
            {
                sw.ReadLine();
                speedBaudTextBox.Text = sw.ReadLine();
                timerTextBox.Text = sw.ReadLine();
            }
        }

        private void OpenPortButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                selectedPort.PortName = (string)COMPortList.SelectedItem;
                selectedPort.BaudRate = int.Parse(speedBaudTextBox.Text);

                selectedPort.Open();
                selectedPort.Close();

                MessageBox.Show("Всё работает!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                using (StreamWriter sw = new StreamWriter("portOptions.txt", false, Encoding.Default))
                {
                    sw.WriteLine((string)COMPortList.SelectedItem);
                    sw.WriteLine(speedBaudTextBox.Text);
                    sw.Write(timerTextBox.Text);
                }

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshCOMPortsNamesButton_Click(object sender, RoutedEventArgs e)
        {
            COMPortList.ItemsSource = SerialPort.GetPortNames();

            COMPortList.SelectedIndex = 0;
        }
    }
}
