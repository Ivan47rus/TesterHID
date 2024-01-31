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
using System.Runtime.InteropServices;

using System.Threading;
using System.Diagnostics;

namespace TesterHID
{

    public class IntPtrEventArgs : EventArgs
    {
        private readonly IntPtr _value;

        public IntPtr Value
        {
            get { return _value; }
        }

        public IntPtrEventArgs(IntPtr value)
        {
            _value = value;
        }
    }

    public sealed class ForegroundHook
    {
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        private WinEventDelegate _event;
        private IntPtr _hook;

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        public ForegroundHook()
        {
            _event = new WinEventDelegate(WinEventProc);
            _hook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _event, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            OnForegroundChanged();
        }

        public event EventHandler ForegroundChanged;

        private void OnForegroundChanged()
        {
            var handler = ForegroundChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }

    public static class WinApiImport
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern ushort GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        //Import Keybd_event function
        //send virtual key code to PC
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool keybd_event(ushort bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    }

    public class FocusWatcher
    {
        private readonly ForegroundHook _foregroungHook;

        private IntPtr _lastHandle = IntPtr.Zero;


        public IntPtr LastWindowHandleExceptCurrentProcess
        {
            get { return _lastHandle; }
        }

        public FocusWatcher()
        {
            _foregroungHook = new ForegroundHook();
            _foregroungHook.ForegroundChanged += OnForegroundChanged;
        }

        private void OnForegroundChanged(object sender, EventArgs eventArgs)
        {
            SyncLastActive();
        }

        private void SyncLastActive()
        {
            IntPtr currentHandle = WinApiImport.GetForegroundWindow();

            if (currentHandle == _lastHandle)
                return;

            uint pid;
            WinApiImport.GetWindowThreadProcessId(currentHandle, out pid);

            if (_lastHandle != IntPtr.Zero && Process.GetCurrentProcess().Id == pid)
                return;

            _lastHandle = currentHandle;       
            OnLastActiveWindowChanged(currentHandle);
        }

        public event EventHandler<IntPtrEventArgs> LastActiveWindowChanged;
     
        protected virtual void OnLastActiveWindowChanged(IntPtr e)
        {
            var handler = LastActiveWindowChanged;
            if (handler != null) handler(this, new IntPtrEventArgs(e));
        }
    }

    /// <summary>
    /// Логика взаимодействия для Panel.xaml
    /// </summary>
    public partial class keybdPanel : Window
    {

        private  FocusWatcher _watcher;
        private uint _layout;

        WindowLog       _HistoryLog;        //pointer to Log Window
        _UsbDeviceClass _UsbDevice;         //pointer to Device module
        LogTimer        _HistroyLogTimer;    //pointer to Log timer

        keyboardButton _MagicKey; //key to activate keyboard panel
        public bool EnablePanel; //Flag: 1 - panel is enable, 0 - else

        //next two List must finished with 0x00
        List<ushort> pressedKeyCode;        //keyCode of ALL button with the same ButtonState
        List<ushort> pressedVirtualCode;    //VirtualCode of ALL button with the same ButtonState
        List<ushort> pressedUsagePage;      //UsagePage of ALL button from pressedKeyCode

        //list of all button on this panel
        public List<keyboardButton> buttonArray = new List<keyboardButton>();

        //Constructor
        public keybdPanel(WindowLog HistoryLog, _UsbDeviceClass UsbDevice, LogTimer HistroyLogTimer, FocusWatcher watcher)
        {
            InitializeComponent();
            buttonArray = new List<keyboardButton>();
            this._HistoryLog = HistoryLog;
            this._UsbDevice = UsbDevice;
            this._HistroyLogTimer = HistroyLogTimer;
            this.pressedKeyCode = new List<ushort>();
            this.pressedUsagePage = new List<ushort>();
            this.pressedVirtualCode = new List<ushort>();
            this.EnablePanel = false;
            _MagicKey = new keyboardButton();
            _MagicKey.Click += keyBoardMagickButtonClick;
            _MagicKey.ToolTip = "Activate panel";
            this.wrapPanel.Children.Add(_MagicKey);

            this._watcher = watcher;      
        }
        
        public void OnLastActiveWindowChanged(object sender, IntPtrEventArgs intPtr)
        {
            UpdateLocalise(intPtr.Value);
        }

        private void UpdateLocalise(IntPtr intPtr)
        {
            uint pid;
            var layout = WinApiImport.GetKeyboardLayout(WinApiImport.GetWindowThreadProcessId(intPtr, out pid));
            if (_layout == layout)
                return;
            //LocalisedKeys = KeysLocaliser.Localise(layout);
            _layout = layout;
        }

        //Move window
        private void Border_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //User press one of the button from buttonArray 
        public void keyBoardButton_Down(ushort keyCode, ushort usagePage)
        {           
            byte[] reportArr;           //report array of byte 
            string reportStr = "";      //report array converting to string

            //Search button with state Down
            foreach (keyboardButton _button in buttonArray)
            {
                if (_button._keyState == ButtonState.Down)
                {
                    pressedKeyCode.Add(_button._keyCode);
                    pressedUsagePage.Add(_button._usagePage);
                    pressedVirtualCode.Add(_button._virtualCode);
                }
            }

            //List must finished with 0x00
            pressedKeyCode.Add(0x00);
            pressedUsagePage.Add(0x00);
            pressedVirtualCode.Add(0x00);

            //Get report
            reportArr = _UsbDevice.GenerateReport(pressedKeyCode, pressedUsagePage);

            //if this is report from zero interface, we don't show report ID
            //else, should show report ID            
            if (reportArr[1] == 0)
            {
                //zero interface
                for (int i = 2; i < reportArr.Count(); i++)
                {
                    reportStr += (reportArr[i].ToString("X2") + " ");
                }
            }
            else
            {
                //other interface
                for (int i = 1; i < reportArr.Count(); i++)
                {
                    reportStr += (reportArr[i].ToString("X2") + " ");
                }
            }

            //Get time from last change
            double time = _HistroyLogTimer.GetTime();           

            HistoryItem item = new HistoryItem()
            {
                _Report_str = reportStr,
                _keyCode = keyCode,
                _UsagePage = usagePage,
                _Time = time,               
                _ReportName = Convert.ToString(HidUsagePage.pUsage(keyCode, usagePage) + "Down"),
                _Report_arr = reportArr,
                _keyState = ButtonState.Down,              
            };

            this._HistoryLog.AddItem(item);

            if (this.EnablePanel)
            {
                //foreach (ushort vk in pressedVirtualCode)
                //{
                //    if (vk != 0x00)
                //    {
                //        WinApiImport.SetForegroundWindow(this._watcher.LastWindowHandleExceptCurrentProcess);
                //        SendKeyboardEvent_Down(vk);
                //        MessageBox.Show(keyCode.ToString() + "down");
                //    }
                    
                //}
                if (this.EnablePanel)
                {
                    //foreach (ushort vk in pressedVirtualCode)
                    //{
                    //    WinApiImport.SetForegroundWindow(this._watcher.LastWindowHandleExceptCurrentProcess);
                    //    SendKeyboardEvent_Down(vk);
                    //}            
                    WinApiImport.SetForegroundWindow(this._watcher.LastWindowHandleExceptCurrentProcess);
                   // ushort 
                    SendKeyboardEvent_Down(HidUsagePage.VKconverter(keyCode, usagePage));
                   // MessageBox.Show(keyCode.ToString() + "down");
                }
            }

            //Clean Up
            pressedVirtualCode.Clear();
            pressedKeyCode.Clear();
            pressedUsagePage.Clear();
        }

        public void keyBoardButton_Up(ushort keyCode, ushort usagePage)
        {
            byte[] reportArr;           //report array of byte 
            string reportStr = "";      //report array converting to string
            bool islastButton = false; //Flag: 1 - no more pressed button , 0 - else
            //Search button with state Down
            foreach (keyboardButton _button in buttonArray)
            {
                if (_button._keyState == ButtonState.Down)
                {
                    pressedKeyCode.Add(_button._keyCode);
                    pressedUsagePage.Add(_button._usagePage);
                    pressedVirtualCode.Add(_button._virtualCode);
                }
            }

            //no more pressed buttons
            if (pressedKeyCode.Count < 1)
            {
                islastButton = true;
                pressedKeyCode.Add(keyCode);
                pressedUsagePage.Add(usagePage);
                //List must finished with 0x00
                pressedKeyCode.Add(0x00);
                pressedUsagePage.Add(0x00);
            }

            else
            {
                //has something with ButtonState.Down
                //List must finished with 0x00
                pressedKeyCode.Add(0x00);
                pressedUsagePage.Add(0x00);
                pressedVirtualCode.Add(0x00);
            }

            //Get report
            reportArr = _UsbDevice.GenerateReport(pressedKeyCode, pressedUsagePage);

            if (islastButton)
            {
                //no more pressed button
                if (reportArr[1] == 0)
                {
                    //zero interface
                    for (int i = 2; i < reportArr.Count(); i++)
                    {
                        reportStr += "00 ";
                    }
                }
                else
                {
                    //other interface
                    for (int i = 1; i < reportArr.Count(); i++)
                    {
                        reportStr += "00 ";
                    }
                }
            }
            else
            {
                //if this is report from zero interface, we don't show report ID
                //else, should show report ID            
                if (reportArr[1] == 0)
                {
                    //zero interface
                    for (int i = 2; i < reportArr.Count(); i++)
                    {
                        reportStr += (reportArr[i].ToString("X2") + " ");
                    }
                }
                else
                {
                    //other interface
                    for (int i = 1; i < reportArr.Count(); i++)
                    {
                        reportStr += (reportArr[i].ToString("X2") + " ");
                    }
                }
            }

            //Get time from last change
            double time = _HistroyLogTimer.GetTime(); 

            HistoryItem item = new HistoryItem()
            {
                _Report_str = reportStr,
                _keyCode = keyCode,
                _UsagePage = usagePage,
                _Time = time,                
                _ReportName = Convert.ToString(HidUsagePage.pUsage(keyCode, usagePage) + "Up"),
                _Report_arr = reportArr,
                _keyState = ButtonState.Up
            };

            this._HistoryLog.AddItem(item);

            if (this.EnablePanel)
            {
                //if (!islastButton)
                //{
                //    foreach (ushort vk in pressedVirtualCode)
                //    {
                //        WinApiImport.SetForegroundWindow(this._watcher.LastWindowHandleExceptCurrentProcess);
                //        SendKeyboardEvent_Down(vk);
                //    }      
                //}
                        
                WinApiImport.SetForegroundWindow(this._watcher.LastWindowHandleExceptCurrentProcess);
                SendKeyboardEvent_Up(HidUsagePage.VKconverter(keyCode, usagePage));
               // MessageBox.Show(keyCode.ToString()+"Up");
            }
            
            //Clean Up
            pressedKeyCode.Clear();
            pressedUsagePage.Clear();
            pressedVirtualCode.Clear();
        }

        public void keyBoardMagickButtonClick(object sender, RoutedEventArgs e)
        {
            if (!EnablePanel)
            {
                //Panel not active
                this._MagicKey.Background = new SolidColorBrush(Color.FromRgb(0x22, 0xB1, 0x4C));//GREEN
                this._MagicKey.ToolTip = "Deactivate panel";
                EnablePanel = true;
                this.Topmost = true;
            }
            else
            {
                //panel active
                this._MagicKey.Background = new SolidColorBrush(Color.FromRgb(133, 26, 26));//RED
                this._MagicKey.ToolTip = "Activate panel";
                EnablePanel = false;
                this.Topmost = false;
            }
        }

        public static void SendKeyboardEvent_Down(ushort virtualCode)
        {
            if (virtualCode > 0x00FF)
            {
                ushort vk = Convert.ToUInt16((virtualCode & 0xFF00) >> 8);
                WinApiImport.keybd_event(vk, 0, 0, 0);
                vk = Convert.ToUInt16(virtualCode & 0x00FF);
                WinApiImport.keybd_event(vk, 0, 0, 0);
            }
            else
            {
                WinApiImport.keybd_event(virtualCode, 0, 0, 0);
            }

        }

        public static void SendKeyboardEvent_Up(ushort virtualCode)
        {         
            if (virtualCode > 0x00FF)
            {
                ushort vk = Convert.ToUInt16(virtualCode & 0x00FF);
                WinApiImport.keybd_event(vk, 0, 0x0002, 0);
                vk = Convert.ToUInt16((virtualCode & 0xFF00) >> 8);
                WinApiImport.keybd_event(vk, 0, 0x0002, 0);
            }
            else
            {
                WinApiImport.keybd_event(virtualCode, 0, 0x0002, 0);
            }
        }
                     
    }
}
