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
using System.Windows.Shapes;
using System.IO;

namespace TesterHID
{
    /// <summary>
    /// Логика взаимодействия для WindowLog.xaml
    /// </summary>
    public partial class WindowLog : Window
    {       
        private bool isCapture; //Flag: 1 - capture in progress, 0 - capture stopped

        //History Log timer
        LogTimer _Timer;

        //Constructor
        public WindowLog(LogTimer Timer)
        {
            InitializeComponent();
            isCapture = false;
            this._Timer = Timer;
            this.Button_Exit.Visibility = Visibility.Hidden;
            this.Button_Minimize.Visibility = Visibility.Hidden;
            this.Button_Capture.Content = "Start Capture";
            this.Button_Capture.ToolTip = "Start data capture";
            this.Button_Capture.BorderBrush = this.BorderBrush = new SolidColorBrush(Color.FromRgb(0x22, 0xB1, 0x4C));
        }

        public void AddItem(HistoryItem item)
        {
            if (this.isCapture)
                this.Log_ListView.Items.Add(item);
        }

        public void AddLoadedItem(HistoryItem item)
        {
            this.Log_ListView.Items.Add(item);
        }

        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Log_ListView.Items.Clear();
            this._Timer.StopTimer();
            this._Timer.GetTime();
        }

        private void Capture_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!isCapture)
            {
                //not capture data now
                //start capture
                this.Button_Capture.Content = "Stop Capture";
                this.Button_Capture.ToolTip = "Stop data capture";
                this.Button_Capture.BorderBrush = this.BorderBrush = new SolidColorBrush(Color.FromRgb(0xED, 0x1C, 0x24));//RED
                this._Timer.GetTime();
                isCapture = true;
            }
            else
            {
                //capture in progress
                //stop capture
                this.Button_Capture.Content = "Start Capture";
                this.Button_Capture.ToolTip = "Start data capture";
                this.Button_Capture.BorderBrush = this.BorderBrush = new SolidColorBrush(Color.FromRgb(0x22, 0xB1, 0x4C));//GREEN
                this._Timer.StopTimer();
                isCapture = false;
            }

        }
        
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(@"LOG.bin", FileMode.Create));

            foreach (HistoryItem item in this.Log_ListView.Items)
            {

                byte[] keyboardReport = item._Report_arr;

                foreach (byte b in keyboardReport)
                {
                    writer.Write(b);
                }

                writer.Write(item._Time);
                writer.Write(item._keyCode);
                writer.Write(item._UsagePage);
                writer.Write((byte)item._keyState);
            }
            writer.Close();

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void LogWindow_OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LogWindow_OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
