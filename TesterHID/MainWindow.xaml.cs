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
using System.IO;

using UsbDeviceClassWrapperNamespace;
using UsbHostClassWrapperNamespace;
using WinUsbClassWrapperNamespace;

namespace TesterHID
{
    //key state
    public enum ButtonState { Up = 0, Down = 1, Wait = 2 };

    public class _WinUsbClass
    {

        private string _VID;
        private string _PID;

        private byte[] _ConfigDescriptor;
        private byte[] _ReportDescriptor;

        public _WinUsbClass(string VID, string PID, byte[] ConfigDescriptor, byte[] ReportDescriptor)
        {
            this._VID = VID;
            this._PID = PID;

            this._ConfigDescriptor = ConfigDescriptor;
            this._ReportDescriptor = ReportDescriptor;
        }

        unsafe public bool GetInfoFromDevice()
        {

            byte[] vid = Encoding.ASCII.GetBytes(this._VID);
            byte[] pid = Encoding.ASCII.GetBytes(this._PID);

            fixed (byte* _vid = vid)
            {
                fixed (byte* _pid = pid)
                {
                    fixed (byte* ConfigDescriptor = this._ConfigDescriptor)
                    {
                        fixed (byte* ReportDescriptor = this._ReportDescriptor)
                        {
                            sbyte* VID = (sbyte*)_vid;
                            sbyte* PID = (sbyte*)_pid;

                            WinUsbClassWrapper _winusb = new WinUsbClassWrapper(VID, PID, ConfigDescriptor, ReportDescriptor);
                            _winusb.InstallDriver();
                            return _winusb.GetInfoFromDevice();
                        }
                    }
                }
            }
        }
        unsafe public void UninstallDriver()
        {
            WinUsbClassWrapper _winusb = new WinUsbClassWrapper();
            _winusb.UninstallDriver();
        }
    }

    public class _UsbHostClass
    {
        private byte[] _ConfigDescriptor;
        private byte[] _ReportDescriptor;

        public _UsbHostClass(byte[] ConfigDescriptor, byte[] ReportDescriptor)
        {

            this._ConfigDescriptor = ConfigDescriptor;
            this._ReportDescriptor = ReportDescriptor;
        }

        unsafe public List<ushort> GenerateKeyboard()
        {
            List<ushort> _KeyBoard;

            fixed (byte* ConfigDescriptor = this._ConfigDescriptor)
            {
                fixed (byte* ReportDescriptor = this._ReportDescriptor)
                {
                    UsbHostClassWrapper _usbhost = new UsbHostClassWrapper(ConfigDescriptor, ReportDescriptor);
                    _KeyBoard = _usbhost.GenerateKeyboard();
                }
            }
            return _KeyBoard;
        }

        unsafe public List<ushort> HostProcess(byte[] KeyboardReport)
        {
            List<ushort> _KeyArray;

            fixed (byte* ConfigDescriptor = this._ConfigDescriptor)
            {
                fixed (byte* ReportDescriptor = this._ReportDescriptor)
                {

                    fixed (byte* _keyboardReport = KeyboardReport)
                    {
                        UsbHostClassWrapper _usbhost = new UsbHostClassWrapper(ConfigDescriptor, ReportDescriptor);
                        _KeyArray = _usbhost.HostProcess(_keyboardReport);
                    }

                }
            }
            return _KeyArray;
        }
    }

    public class _UsbDeviceClass
    {

        private byte[] _ConfigDescriptor;
        private byte[] _ReportDescriptor;

        public _UsbDeviceClass(byte[] ConfigDescriptor, byte[] ReportDescriptor)
        {

            this._ConfigDescriptor = ConfigDescriptor;
            this._ReportDescriptor = ReportDescriptor;
        }

        unsafe public byte[] GenerateReport(ushort keyCode, ushort usagePage)
        {
            fixed (byte* ConfigDescriptor = this._ConfigDescriptor)
            {
                fixed (byte* ReportDescriptor = this._ReportDescriptor)
                {
                    UsbDeviceClassWrapper _usbdev = new UsbDeviceClassWrapper(ConfigDescriptor, ReportDescriptor);
                    return _usbdev.GenerateReport(keyCode, usagePage);
                }
            }
        }

        unsafe public byte[] GenerateReport(List<ushort> keyCode, List<ushort> usagePage)
        {
            fixed (byte* ConfigDescriptor = this._ConfigDescriptor)
            {
                fixed (byte* ReportDescriptor = this._ReportDescriptor)
                {
                    ushort[] _keyCode = keyCode.ToArray();
                    ushort[] _usagePage = usagePage.ToArray();

                    fixed (ushort* KeyCode = _keyCode)
                    {
                        fixed (ushort* UsagePage = _usagePage)
                        {
                            UsbDeviceClassWrapper _usbdev = new UsbDeviceClassWrapper(ConfigDescriptor, ReportDescriptor);

                            return _usbdev.GenerateReport(KeyCode, UsagePage);
                        }
                    }

                }
            }
        }
    }

    public static class HidUsagePage
    {
        // 1 Generic Desktop 0x00 - 0xFF
        static string[] genericDesctop =
        {
             " Undefined " ,
	         " Pointer " ,
	         " Mouse " ,
	         " Reserved " ,
	         " Joystick " ,
	         " Game Pad " ,
	         " Keyboard " ,
	         " Keypad " ,
	         " Multi-axis Controller " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x0F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x1F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x2F
	         " X " ,
	         " Y " ,
	         " Z " ,
	         " Rx " ,
	         " Ry " ,
	         " Rz " ,
	         " Slider " ,
	         " Dial " ,
	         " Wheel " ,
	         " Hat switch " ,
	         " Counted Buffer " ,
	         " Byte Count " ,
	         " Motion Wakeup " ,
	         " Start " ,
	         " Select " ,
	         " Reserved " ,//0x3F
	         " Vx " ,
	         " Vy " ,
	         " Vz " ,
	         " Vbrx " ,
	         " Vbry " ,
	         " Vbrz " ,
	         " Vno " ,
	         " Feature Notification " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x4F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x5F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x6F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x7F
	         " Control " ,
	         " Power Down " ,
	         " Sleep " ,
	         " Wake Up " ,
	         " Context Menu " ,
	         " Main Menu " ,
	         " App Menu " ,
	         " Menu Help " ,
	         " Menu Exit " ,
	         " Menu Select " ,
	         " Menu Right " ,
	         " Menu Left " ,
	         " Menu Up " ,
	         " Menu Down " ,
	         " Cold Restart " ,
	         " Warm Restart " , //0x8F
	         " D-pad Up " ,
	         " D-pad Down " ,
	         " D-pad Right " ,
	         " D-pad Left " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x9F
	         " Dock " ,
	         " Undock " ,
	         " Setup " ,
	         " Break " ,
	         " Debugger Break " ,
	         " Application Break " ,
	         " Application Debugger Break " ,
	         " Speaker Mute " ,
	         " Hibernate " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0xAF
	         " Display Invert " ,
	         " Display Internal " ,
	         " Display External " ,
	         " Display Both " ,
	         " Display Dual " ,
	         " Display Toggle Int/Ext " ,
	         " Display Swap " ,
	         " Display LCD Autoscale " , // 0xB7 
             " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,//0xBF
             " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0xCF
             " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0xDF
             " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0xEF
             " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0xFF
        };


        // 2 Simulation Controls 0x00 - 0xD0
        static string[] simulationControls =
        {
	         " Undefined " ,
	         " Flight Sim Dev " ,
	         " Automobile Sim Dev " ,
	         " Tank Sim Dev " ,
	         " Spaceship Sim Dev " ,
	         " Submarine Sim Dev " ,
	         " Sailing Sim Dev " ,
	         " Motorcycle Sim Dev " ,
	         " Sports Sim Dev " ,
	         " Airplane Sim Dev " ,
	         " Helicopter Sim Dev " ,
	         " Magic Carpet Simulation " ,
	         " Bicycle Sim Dev " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " , // 0x0F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x1F
	         " Flight Control Stick " ,
	         " Flight Stick " ,
	         " Cyclic Control " ,
	         " Cyclic Trim " ,
	         " Flight Yoke " ,
	         " Track Control " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x2F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x3F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x4F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x5F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x6F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x7F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x8F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x9F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0xAF
	         " Aileron " ,
	         " Aileron Trim " ,
	         " Anti-Torque Control " ,
	         " Autopilot Enable " ,
	         " Chaff Release " ,
	         " Collective Control " ,
	         " Dive Brake " ,
	         " Electronic Countermeasures " ,
	         " Elevator " ,
	         " Elevator Trim " ,
	         " Rudder " ,
	         " Throttle " ,
	         " Flight Communications " ,
	         " Flare Release " ,
	         " Landing Gear " ,
	         " Toe Brake " ,//0xBF
	         " Trigger " ,
	         " Weapons Arm " ,
	         " Weapons Select " ,
	         " Wing Flaps " ,
	         " Accelerator " ,
	         " Brake " ,
	         " Clutch " ,
	         " Shifter " ,
	         " Steering " ,
	         " Turret Direction " ,
	         " Barrel Elevation " ,
	         " Dive Plane " ,
	         " Ballast " ,
	         " Bicycle Crank " ,
	         " Handle Bars " ,
	         " Front Brake " ,//0xCF
	         " Rear Brake "  //0xD0
        };

        //3	VR Controls 0x00 - 0x21
        static string[] vrControls =
        {
	         " Unidentified " ,
	         " Belt " ,
	         " Body Suit " ,
	         " Flexor " ,
	         " Glove " ,
	         " Head Tracker " ,
	         " Head Mounted Display " ,
	         " Hand Tracker " ,
	         " Oculometer " ,
	         " Vest " ,
	         " Animatronic Device " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x0F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x1F
	         " Stereo Enable " ,
	         " Display Enable "  //0x21
        };


        //4	Sports Controls 0x00 - 0x63
        static string[] sportsControls =
        {
	         " Unidentified " ,
	         " Baseball Bat " ,
	         " Golf Club " ,
	         " Rowing Machine " ,
	         " Treadmill " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x0F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x1F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x2F
	         " Oar " ,
	         " Slope " ,
	         " Rate " ,
	         " Stick Speed " ,
	         " Stick Face Angle " ,
	         " Stick Heel/Toe " ,
	         " Stick Follow Through " ,
	         " Stick Tempo " ,
	         " Stick Type " ,
	         " Stick Height " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x3F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x4F
	         " Putter " ,
	         " 1 Iron " ,
	         " 2 Iron " ,
	         " 3 Iron " ,
	         " 4 Iron " ,
	         " 5 Iron " ,
	         " 6 Iron " ,
	         " 7 Iron " ,
	         " 8 Iron " ,
	         " 9 Iron " ,
	         " 10 Iron " ,
	         " 11 Iron " ,
	         " Sand Wedge " ,
	         " Loft Wedge " ,
	         " Power Wedge " ,
	         " 1 Wood " , //0x5F
	         " 3 Wood " ,
	         " 5 Wood " ,
	         " 7 Wood " ,
	         " 9 Wood "  //0x63
        };


        //5	Game Controls 0x00 - 0x38 (0x39)
        static string[] gameControls =
        {
	         " Undefined " ,
	         " 3D Game Controller " ,
	         " Pinball Device CA " ,
	         " Gun Device CA " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x0F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,//0x1F
	         " Point of View " ,
	         " Turn Right/Left " ,
	         " Pitch Forward/Backward " ,
	         " Roll Right/Left DV " ,
	         " Move Right/Left " ,
	         " Move Forward/Backward " ,
	         " Move Up/Down " ,
	         " Lean Right/Left " ,
	         " Lean Forward/Backward " ,
	         " Height of POV " ,
	         " Flipper " ,
	         " Secondary Flipper " ,
	         " Bump " ,
	         " New Game " ,
	         " Shoot Ball " ,
	         " Player " , //0x2F
	         " Gun Bolt " ,
	         " Gun Clip " ,
	         " Gun Selector " ,
	         " Gun Single Shot " ,
	         " Gun Burst " ,
	         " Gun Automatic " ,
	         " Gun Safety " ,
	         " Gamepad Fire/Jump " ,
	         " Gamepad Trigger " , // possible mistake in documentation, 0x38 is skipped
	         " Gamepad Trigger "  //0x39
        };

        //6	Generic Device Controls 0x00 - 0x22
        static string[] genericDeviceControls =
        {
	         " Unidentified " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x0F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x1F
	         " Battery Strength " ,
	         " Wireless Channel " ,
	         " Wireless ID "  //0x22
        };

        //7	Keyboard 0x00 - 0xFF
        static string[] keyBoard =
        {
             " Reserved " ,
	         " ErrorRollOver " ,
	         " POSTFail " ,
	         " ErrorUndefined " ,
	         " a A " ,
	         " b B " ,
	         " c C " ,
	         " d D " ,
	         " e E " ,
	         " f F " ,
	         " g G " ,
	         " h H " ,
	         " i I " ,
	         " j J " ,
	         " k K " ,
	         " l L " , //0x0F	
	         " m M " ,
	         " n N " ,
	         " o O " ,
	         " p P " ,
	         " q Q " ,
	         " r R " ,
	         " s S " ,
	         " t T " ,
	         " u U " ,
	         " v V " ,
	         " w W " ,
	         " x X " ,
	         " y Y " ,
	         " z Z " ,
	         " 1 ! " ,
	         "  2 @ " , //0x1F
	         " 3 # " ,
	         " 4 $ " ,
	         " 5 % " ,
	         " 6 ^ " ,
	         " 7 & " ,
	         " 8 * " ,
	         " 9 and( " ,
	         " 0 and) " ,
	         " ENTER " ,
	         " ESCAPE " ,
	         " <-(Backspace) " ,
	         " Tab " ,
	         " Spacebar " ,
	         " - _ " ,
	         " = + " ,
	         " [ { " , //0x2F
	         " ] } " ,
	         " \\ | " ,
	         " Non - US # ~ " ,
	         " ; : " ,
	         " ' \"  " ,
	         " ` ~ " ,
	         " , < " ,
        	 " . > " ,
	         " / ? " ,
	         " Caps Lock " ,
	         " F1 " ,
	         " F2 " ,
	         " F3 " ,
	         " F4 " ,
	         " F5 " ,
	         " F6 " , //0x3F
	         " F7 " ,
	         " F8 " ,
	         " F9 " ,
	         " F10 " ,
	         " F11 " ,  
	         " F12 " ,
	         " PrintScreen " ,
	         " Scroll Lock " ,
	         " Pause " ,
	         " Insert " ,
	         " Home " ,
	         " PageUp " ,
	         " Delete Forward " ,
	         " End " ,
	         " PageDown " ,
	         " RightArrow " ,//0x4F
	         " LeftArrow " ,
	         " DownArrow " ,
	         " UpArrow " ,
	         " Num Lock " ,
	         " / " ,
	         " * " ,
	         " - " ,
	         " + " ,
	         " ENTER " ,
	         " 1 End " ,
	         " 2 Down Arrow " ,
	         " 3 PageDn " ,
	         " 4 Left Arrow " ,
	         " 5 " ,
	         " 6 Right Arrow " ,
	         " 7 Home " , //0x5F
	         " 8 Up Arrow " ,
	         " 9 PageUp " ,
	         " 0 Insert " ,
	         " . Delete " ,
	         " Non - US \\ | " ,
	         " Application " ,
	         " Power " ,
	         " Keypad = " ,
	         " F13 " ,
	         " F14 " ,
	         " F15 " ,
	         " F16 " ,
	         " F17 " ,
	         " F18 " ,
	         " F19 " ,
	         " F20 " , //0x6F
	         " F21 " ,
	         " F22 " ,
	         " F23 " ,
	         " F24 " ,
	         " Execute " ,
	         " Help " ,
	         " Menu " ,
	         " Select " ,
	         " Stop " ,
	         " Again " ,
	         " Undo " ,
	         " Cut " ,
	         " Copy " ,
	         " Paste " ,
	         " Find " ,
	         " Mute " , //0x7F
	         " Volume Up " ,
	         " Volume Down " ,
	         " Locking Caps Lock " ,
	         " Locking Num Lock " ,
	         " Locking Scroll Lock " ,
	         " Keypad Comma " ,
	         " Keypad Equal Sign " ,
	         " International1 " ,
	         " International2 " ,
	         " International3 " ,
	         " International4 " ,
	         " International5 " ,
	         " International6 " ,
	         " International7 " ,
	         " International8 " ,
	         " International9 " , //0x8F
	         " LANG1 " ,
	         " LANG2 " ,
	         " LANG3 " ,
	         " LANG4 " ,
	         " LANG5 " ,
	         " LANG6 " ,
	         " LANG7 " ,
	         " LANG8 " ,
	         " LANG9 " ,
	         " Alternate Erase " ,
	         " SysReq / Attention " ,
	         " Cancel " ,
	         " Clear " ,
	         " Prior " ,
	         " Return " ,
	         " Separator " , //0x9F
        	 " Out " ,
	         " Oper " ,
	         " Clear / Again " ,
	         " CrSel / Props " ,
	         " ExSel " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0xAF
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,//0xBF
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,//0xCF
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,//0xDF
	         " LeftControl " ,
	         " LeftShift " ,
	         " LeftAlt " ,
	         " Left GUI " ,
	         " RightControl " ,
	         " RightShift " ,
	         " RightAlt " ,
	         " Right GUI " , //0xE7/////!!!!!!!!!!
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0xEF
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,//0xFF
        };


        //8	LEDs 0x00 - 0xFF
        static string[] LEDs =
        {
	         " Undefined  " ,
	         " Num Lock " ,
	         " Caps Lock " ,
	         " Scroll Lock " ,
	         " Compose " ,
	         " Kana " ,
	         " Power " ,
	         " Shift " ,
	         " Do Not Disturb " ,
	         " Mute " ,
	         " Tone Enable " ,
	         " High Cut Filter " ,
	         " Low Cut Filter " ,
	         " Equalizer Enable " ,
	         " Sound Field On " ,
	         " Surround On " , //0x0F
	         " Repeat " ,
	         " Stereo " ,
	         " Sampling Rate Detect " ,
	         " Spinning " ,
	         " CAV " ,
	         " CLV " ,
	         " Recording Format Detect " ,
	         " Off-Hook " ,
	         " Ring " ,
	         " Message Waiting " ,
	         " Data Mode " ,
	         " Battery Operation " ,
	         " Battery OK " ,
	         " Battery Low " ,
	         " Speaker " ,
	         " Head Set " , //0x1F
	         " Hold " ,
	         " Microphone " ,
	         " Coverage " ,
	         " Night Mode " ,
	         " Send Calls " ,
	         " Call Pickup " ,
	         " Conference " ,
	         " Stand-by " ,
	         " Camera On " ,
	         " Camera Off " ,
	         " On-Line " ,
	         " Off-Line " ,
	         " Busy " ,
	         " Ready " ,
	         " Paper-Out " ,
	         " Paper-Jam " , //0x2F
	         " Remote " ,
	         " Forward " ,
	         " Reverse " ,
	         " Stop " ,
	         " Rewind " ,
	         " Fast Forward " ,
	         " Play " ,
	         " Pause " ,
	         " Record " ,
	         " Error " ,
	         " Usage Selected Indicator " ,
	         " Usage In Use Indicator " ,
	         " Usage Multi Mode Indicator " ,
	         " Indicator On " ,
	         " Indicator Flash " ,
	         " Indicator Slow Blink " , //0x3F
	         " Indicator Fast Blink " ,
	         " Indicator Off " ,
	         " Flash On Time " ,
	         " Slow Blink On Time " ,
	         " Slow Blink Off Time " ,
	         " Fast Blink On Time " ,
	         " Fast Blink Off Time " ,
	         " Usage Indicator Color " ,
	         " Indicator Red " ,
	         " Indicator Green " ,
	         " Indicator Amber " ,
	         " Generic Indicator " ,
	         " System Suspend " ,
	         " External Power Connected " //0x4D
	        //0x4C - FFFF	Reserved  ,so shoudn't use
       };


        //9	Button 0x00 - 
        static string[] Button =
        {
	         " Button 0x " 
        };

        //10	Ordinal 0x00 - 
        static string[] Ordinal =
        {
	         " Ordinal 0x " 
        };

        //11	Telephony 0x00 - 0xBF
        static string[] telephony =
        {
	         " Unassigned " ,
	         " Phone " ,
	         " Answering Machine " ,
	         " Message Controls " ,
	         " Handset " ,
	         " Headset " ,
	         " Telephony Key Pad " ,
	         " Programmable Button " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x0F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x1F
	         " Hook Switch " ,
	         " Flash " ,
	         " Feature " ,
	         " Hold " ,
	         " Redial " ,
	         " Transfer " ,
	         " Drop " ,
	         " Park " ,
	         " Forward Calls " ,
	         " Alternate Function " ,
	         " Line " ,
	         " Speaker Phone " ,
	         " Conference " ,
	         " Ring Enable " ,
	         " Ring Select " ,
	         " Phone Mute " , //0x2F
	         " Caller ID " ,
	         " Send " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x3F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x4F
	         " Speed Dial " ,
	         " Store Number " ,
	         " Recall Number " ,
	         " Phone Directory " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x5F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,//0x6F
	         " Voice Mail " ,
	         " Screen Calls " ,
	         " Do Not Disturb " ,
	         " Message " ,
	         " Answer On/Off " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x7F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x8F
	         " Inside Dial Tone " ,
	         " Outside Dial Tone " ,
	         " Inside Ring Tone " ,
	         " Outside Ring Tone " ,
	         " Priority Ring Tone " ,
	         " Inside Ringback " ,
	         " Priority Ringback " ,
	         " Line Busy Tone " ,
	         " Reorder Tone " ,
	         " Call Waiting Tone " ,
	         " Confirmation Tone 1 " ,
	         " Confirmation Tone 2 " ,
	         " Tones Off " ,
	         " Outside Ringback " ,
	         " Ringer " ,
	         " Reserved " , // 0x9E is ringer, the doc has a mistake, this should be 0x9F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,//0xAF
	         " Phone Key 0 " ,
	         " Phone Key 1 " ,
	         " Phone Key 2 " ,
	         " Phone Key 3 " ,
	         " Phone Key 4 " ,
	         " Phone Key 5 " ,
	         " Phone Key 6 " ,
	         " Phone Key 7 " ,
	         " Phone Key 8 " ,
	         " Phone Key 9 " ,
	         " Phone Key Star " ,
	         " Phone Key Pound " ,
	         " Phone Key A " ,
	         " Phone Key B " ,
	         " Phone Key C " ,
	         " Phone Key D "  //0xBF
        };

        //12	Consumer 0x00 - 0x29C
        static string[] consumer =
        {
	         " Unassigned " ,
	         " Consumer Control " ,
	         " Numeric Key Pad " ,
	         " Programmable Buttons " ,
	         " Microphone " ,
	         " Headphone " ,
	         " Graphic Equalizer " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x0F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x1F
	         " +10 " ,
	         " +100 " ,
	         " AM/PM " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x2F doc error, should end at 0x2F
	         " Power " ,
	         " Reset " ,
	         " Sleep " ,
	         " Sleep After " ,
	         " Sleep Mode " ,
	         " Illumination " ,
	         " Function Buttons " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x3F
	         " Menu " ,
	         " Menu Pick " ,
	         " Menu Up " ,
	         " Menu Down " ,
	         " Menu Left " ,
	         " Menu Right " ,
	         " Menu Escape " ,
	         " Menu Value Increase " ,
	         " Menu Value Decrease " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x4F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x5F
	         " Data On Screen " ,
	         " Closed Caption " ,
	         " Closed Caption Select " ,
	         " VCR/TV " ,
	         " Broadcast Mode " ,
	         " Snapshot " ,
	         " Still " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x6F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x7F
	         " Selection " ,
	         " Assign Selection " ,
	         " Mode Step " ,
	         " Recall Last " ,
	         " Enter Channel " ,
	         " Order Movie " ,
	         " Channel " ,
	         " Media Selection " ,
	         " Media Select Computer " ,
	         " Media Select TV " ,
	         " Media Select WWW " ,
	         " Media Select DVD " ,
	         " Media Select Telephone " ,
	         " Media Select Program Guide " ,
	         " Media Select Video Phone " ,
	         " Media Select Games " , //0x8F
	         " Media Select Messages " ,
	         " Media Select CD " ,
	         " Media Select VCR " ,
	         " Media Select Tuner " ,
	         " Quit " ,
	         " Help " ,
	         " Media Select Tape " ,
	         " Media Select Cable " ,
	         " Media Select Satellite " ,
	         " Media Select Security " ,
	         " Media Select Home " ,
	         " Media Select Call " ,
	         " Channel Increment " ,
	         " Channel Decrement " ,
	         " Media Select SAP " ,
	         " Reserved " , //0x9F
	         " VCR Plus " ,
	         " Once " ,
	         " Daily " ,
	         " Weekly " ,
	         " Monthly " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  //0xAF
	         " Play " ,
	         " Pause " ,
	         " Record " ,
	         " Fast Forward " ,
	         " Rewind " ,
	         " Scan Next Track " ,
	         " Scan Previous Track " ,
	         " Stop " ,
	         " Eject " ,
	         " Random Play " ,
	         " Select Disc " ,
	         " Enter Disc " ,
	         " Repeat " ,
	         " Tracking " ,
	         " Track Normal " ,
	         " Slow Tracking " ,  //0xBF
	         " Frame Forward " ,
	         " Frame Back " ,
	         " Mark " ,
	         " Clear Mark " ,
	         " Repeat From Mark " ,
	         " Return To Mark " ,
	         " Search Mark Forward " ,
	         " Search Mark Backwards " ,
	         " Counter Reset " ,
	         " Show Counter " ,
	         " Tracking Increment " ,
	         " Tracking Decrement " ,
	         " Stop/Eject " ,
	         " Play/Pause " ,
	         " Play/Skip " ,
	         " Reserved " ,  //0xCF
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0xDF
	         " Volume " ,
	         " Balance " ,
	         " Mute " ,
	         " Bass " ,
	         " Treble " ,
	         " Bass Boost " ,
	         " Surround Mode " ,
	         " Loudness " ,
	         " MPX " ,
	         " Volume Increment " ,
	         " Volume Decrement " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  //0xEF
	         " Speed Select " ,
	         " Playback Speed " ,
	         " Standard Play " ,
	         " Long Play " ,
	         " Extended Play " ,
	         " Slow " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0xFF
	         " Fan Enable " ,
	         " Fan Speed " ,
	         " Light Enable " ,
	         " Light Illumination Level " ,
	         " Climate Control Enable " ,
	         " Room Temperature " ,
	         " Security Enable " ,
	         " Fire Alarm " ,
	         " Police Alarm " ,
	         " Proximity " ,
	         " Motion " ,
	         " Duress Alarm " ,
	         " Holdup Alarm " ,
	         " Medical Alarm " ,
	         " Reserved " ,  " Reserved " , //0x100
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x11F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x12F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x13F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x14F
	         " Balance Right " ,
	         " Balance Left " ,
	         " Bass Increment " ,
	         " Bass Decrement " ,
	         " Treble Increment " ,
	         " Treble Decrement " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x15F
	         " Speaker System " ,
	         " Channel Left " ,
	         " Channel Right " ,
	         " Channel Center " ,
	         " Channel Front " ,
	         " Channel Center Front " ,
	         " Channel Side " ,
	         " Channel Surround " ,
	         " Channel Low Frequency Enhancement " ,
	         " Channel Top " ,
	         " Channel Unknown " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x16F
	         " Sub-channel " ,
	         " Sub-channel Increment " ,
	         " Sub-channel Decrement " ,
	         " Alternate Audio Increment " ,
	         " Alternate Audio Decrement " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x17F
	         " Application Launch Buttons " ,
	         " AL Launch Button Configuration Tool " ,
	         " AL Programmable Button Configuration " ,
	         " AL Consumer Control Configuration " ,
	         " AL Word Processor " ,
	         " AL Text Editor " ,
	         " AL Spreadsheet " ,
	         " AL Graphics Editor " ,
	         " AL Presentation App " ,
	         " AL Database App " ,
	         " AL Email Reader " ,
	         " AL Newsreader " ,
	         " AL Voicemail " ,
	         " AL Contacts/Address Book " ,
	         " AL Calendar/Schedule " ,
	         " AL Task/Project Manager " , // 0x18F
	         " AL Log/Journal/Timecard " ,
	         " AL Checkbook/Finance " ,
	         " AL Calculator " ,
	         " AL A/V Capture/Playback " ,
	         " AL Local Machine Browser " ,
	         " AL LAN/WAN Browser " ,
	         " AL Internet Browser " ,
	         " AL Remote Networking/ISP Connect " ,
	         " AL Network Conference " ,
	         " AL Network Chat " ,
	         " AL Telephony/Dialer " ,
	         " AL Logon " ,
	         " AL Logoff " ,
	         " AL Logon/Logoff " ,
	         " AL Terminal Lock/Screensaver " ,
	         " AL Control Panel " , // 0x19F
	         " AL Command Line Processor/Run Sel " ,
	         " AL Process/Task Manager " ,
	         " AL Select Task/Application " ,
	         " AL Next Task/Application " ,
	         " AL Previous Task/Application " ,
	         " AL Preemptive Halt Task/Application " ,
	         " AL Integrated Help Center " ,
	         " AL Documents " ,
	         " AL Thesaurus " ,
	         " AL Dictionary " ,
	         " AL Desktop " ,
	         " AL Spell Check " ,
	         " AL Grammar Check " ,
	         " AL Wireless Status " ,
	         " AL Keyboard Layout " ,
	         " AL Virus Protection " , // 0x1AF
	         " AL Encryption " ,
	         " AL Screen Saver " ,
	         " AL Alarms " ,
	         " AL Clock " ,
	         " AL File Browser " ,
	         " AL Power Status " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x1BF
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x1CF
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x1DF
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x1EF
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x1FF
	         " Generic GUI Application Controls " ,
	         " AC New " ,
	         " AC Open " ,
	         " AC Close " ,
	         " AC Exit " ,
	         " AC Maximize " ,
	         " AC Minimize " ,
	         " AC Save " ,
	         " AC Print " ,
	         " AC Properties " , // 0x209, but the next one is 0x21A
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , // 0x20F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,
	         " AC Undo " ,
	         " AC Copy " ,
	         " AC Cut " ,
	         " AC Paste " ,
	         " AC Select All " ,
	         " AC Find " , // 0x21F
	         " AC Find and Replace " ,
	         " AC Search " ,
	         " AC Go To " ,
	         " AC Home " ,
	         " AC Back " ,
	         " AC Forward " ,
	         " AC Stop " ,
	         " AC Refresh " ,
	         " AC Previous Link " ,
	         " AC Next Link " ,
	         " AC Bookmarks " ,
	         " AC History " ,
	         " AC Subscriptions " ,
	         " AC Zoom In " ,
	         " AC Zoom Out " ,
	         " AC Zoom " , // 0x22F
	         " AC Full Screen View " ,
	         " AC Normal View " ,
	         " AC View Toggle " ,
	         " AC Scroll Up " ,
	         " AC Scroll Down " ,
	         " AC Scroll " ,
	         " AC Pan Left " ,
	         " AC Pan Right " ,
	         " AC Pan " ,
	         " AC New Window " ,
	         " AC Tile Horizontally " ,
	         " AC Tile Vertically " ,
	         " AC Format " ,
	         " AC Edit " ,
	         " AC Bold " ,
	         " AC Italics " , // 0x23F
	         " AC Underline " ,
	         " AC Strikethrough " ,
	         " AC Subscript " ,
	         " AC Superscript " ,
	         " AC All Caps " ,
	         " AC Rotate " ,
	         " AC Resize " ,
	         " AC Flip horizontal " ,
	         " AC Flip Vertical " ,
	         " AC Mirror Horizontal " ,
	         " AC Mirror Vertical " ,
	         " AC Font Select " ,
	         " AC Font Color " ,
	         " AC Font Size " ,
	         " AC Justify Left " ,
	         " AC Justify Center H " , // 0x24F
	         " AC Justify Right " ,
	         " AC Justify Block H " ,
	         " AC Justify Top " ,
	         " AC Justify Center V " ,
	         " AC Justify Bottom " ,
	         " AC Justify Block V " ,
	         " AC Indent Decrease " ,
	         " AC Indent Increase " ,
	         " AC Numbered List " ,
	         " AC Restart Numbering " ,
	         " AC Bulleted List " ,
	         " AC Promote " ,
	         " AC Demote " ,
	         " AC Yes " ,
	         " AC No " ,
	         " AC Cancel " , // 0x25F
	         " AC Catalog " ,
	         " AC Buy/Checkout " ,
	         " AC Add to Cart " ,
	         " AC Expand " ,
	         " AC Expand All " ,
	         " AC Collapse " ,
	         " AC Collapse All " ,
	         " AC Print Preview " ,
	         " AC Paste Special " ,
	         " AC Insert Mode " ,
	         " AC Delete " ,
	         " AC Lock " ,
	         " AC Unlock " ,
	         " AC Protect " ,
	         " AC Unprotect " ,
	         " AC Attach Comment " , // 0x26F
	         " AC Delete Comment " ,
	         " AC View Comment " ,
	         " AC Select Word " ,
	         " AC Select Sentence " ,
	         " AC Select Paragraph " ,
	         " AC Select Column " ,
	         " AC Select Row " ,
	         " AC Select Table " ,
	         " AC Select Object " ,
	         " AC Redo/Repeat " ,
	         " AC Sort " ,
	         " AC Sort Ascending " ,
	         " AC Sort Descending " ,
	         " AC Filter " ,
	         " AC Set Clock " ,
	         " AC View Clock " , // 0x27F
	         " AC Select Time Zone " ,
	         " AC Edit Time Zones " ,
	         " AC Set Alarm " ,
	         " AC Clear Alarm " ,
	         " AC Snooze Alarm " ,
	         " AC Reset Alarm " ,
	         " AC Synchronize " ,
	         " AC Send/Receive " ,
	         " AC Send To " ,
	         " AC Reply " ,
	         " AC Reply All " ,
	         " AC Forward Msg " ,
	         " AC Send " ,
	         " AC Attach File " ,
	         " AC Upload " ,
	         " AC Download (Save Target As) " , // 0x28F
	         " AC Set Borders " ,
	         " AC Insert Row " ,
	         " AC Insert Column " ,
	         " AC Insert File " ,
	         " AC Insert Picture " ,
	         " AC Insert Object " ,
	         " AC Insert Symbol " ,
	         " AC Save and Close " ,
	         " AC Rename " ,
	         " AC Merge " ,
	         " AC Split " ,
	         " AC Disribute Horizontally " ,
	         " AC Distribute Vertically "  // 0x29C
        };


        //13	Digitizer 0x00 - 0x46
        static string[] digitizers =
        {
	         " Undefined " ,
	         " Digitizer " ,
	         " Pen " ,
	         " Light Pen " ,
	         " Touch Screen " ,
	         " Touch Pad " ,
	         " White Board " ,
	         " Coordinate Measuring Machine " ,
	         " 3D Digitizer " ,
	         " Stereo Plotter " ,
	         " Articulated Arm " ,
	         " Armature " ,
	         " Multiple Point Digitizer " ,
	         " Free Space Wand " ,
	         " Reserved " ,  " Reserved " , //0x0F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x1F
	         " Stylus " ,
	         " Puck " ,
	         " Finger " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x2F
	         " Tip Pressure " ,
	         " Barrel Pressure " ,
	         " In Range " ,
	         " Touch " ,
	         " Untouch " ,
	         " Tap " ,
	         " Quality " ,
	         " Data Valid " ,
	         " Transducer Index " ,
	         " Tablet Function Keys " ,
	         " Program Change Keys " ,
	         " Battery Strength " ,
	         " Invert " ,
	         " X Tilt " ,
	         " Y Tilt " ,
	         " Azimuth " , //0x3F
	         " Altitude " ,
	         " Twist " ,
	         " Tip Switch " ,
	         " Secondary Tip Switch " ,
	         " Barrel Switch " ,
	         " Eraser " ,
	         " Tablet Pick "  //0x46
        };

        //14 Reserved
        //15 Physical Interface DeviceKeyboard LeftControl>

        //16	Unicode
        static string[] unicode =
        {
	         " Unicode 0x " 
        };

        //17 Undefined
        //18 Undefined
        //19 Undefined

        //20	Alphnumeric Display 0x00 - 0x4D
        static string[] alphnumericDisplay =
        {
	         " Undefined " ,
	         " Alphanumeric Display " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x0F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x1F
	         " Display Attributes Report " ,
	         " ASCII Character Set " ,
	         " Data Read Back " ,
	         " Font Read Back " ,
	         " Display Control Report " ,
	         " Clear Display " ,
	         " Display Enable " ,
	         " Screen Saver Delay " ,
	         " Screen Saver Enable " ,
	         " Vertical Scroll " ,
	         " Horizontal Scroll " ,
	         " Character Report " ,
	         " Display Data " ,
	         " Display Status " ,
	         " Stat Not Ready " ,
	         " Stat Ready " , //0x2F
	         " Err Not a loadable character " ,
	         " Err Font data cannot be read " ,
	         " Cursor Position Report " ,
	         " Row " ,
	         " Column " ,
	         " Rows " ,
	         " Columns " ,
	         " Cursor Pixel Positioning " ,
	         " Cursor Mode " ,
	         " Cursor Enable " ,
	         " Cursor Blink " ,
	         " Font Report " ,
	         " Font Data " ,
	         " Character Width " ,
	         " Character Height " ,
	         " Character Spacing Horizontal " , //0x3F
	         " Character Spacing Vertical " ,
	         " Unicode Character Set " ,
	         " Font 7-Segment " ,
	         " 7-Segment Direct Map " ,
	         " Font 14-Segment " ,
	         " 14-Segment Direct Map " ,
	         " Display Brightness " ,
	         " Display Contrast " ,
	         " Character Attribute " ,
	         " Attribute Readback " ,
	         " Attribute Data " ,
	         " Char Attr Enhance " ,
	         " Char Attr Underline " ,
	         " Char Attr Blink "  //0x4D
        };

        //21 - 63 Undefined
        //64	Medical Instrument 0x00 - 0xA1

        static string[] medicalInstrument =
        {
	         " Undefined " ,
	         " Medical Ultrasound " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x0F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x1F
	         " VCR/Acquisition " ,
	         " Freeze/Thaw " ,
	         " Clip Store " ,
	         " Update " ,
	         " Next " ,
	         " Save " ,
	         " Print " ,
	         " Microphone Enable " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x2F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x3F
	         " Cine " ,
	         " Transmit Power " ,
	         " Volume " ,
	         " Focus " ,
	         " Depth " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x4F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x5F
	         " Soft Step - Primary " ,
	         " Soft Step - Secondary " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x6F
	         " Depth Gain Compensation " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x7F
	         " Zoom Select " ,
	         " Zoom Adjust " ,
	         " Spectral Doppler Mode Select " ,
	         " Spectral Doppler Adjust " ,
	         " Color Doppler Mode Select " ,
	         " Color Doppler Adjust " ,
	         " Motion Mode Select " ,
	         " Motion Mode Adjust " ,
	         " 2-D Mode Select " ,
	         " 2-D Mode Adjust " ,
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x8F
	         " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " ,  " Reserved " , //0x9F
	         " Soft Control Select " ,
	         " Soft Control Adjust "  //0xA1
        };


        public static ushort[] keyboardVK =
{
//    0x00,
//    0x00,
//    0x00,
//    0x00,
//    0x41,
//    0x42,
//    0x43,
//    0x44,
//    0x45,
//    0x46,
//    0x47,
//    0x48,
//    0x49,
//    0x4A,
//    0x4B,
//    0x4C,
//    0x4D,
//    0x4E,
//    0x4F,
//    0x50,
//    0x51,
//    0x52,
//    0x53,
//    0x54,
//    0x55,
//    0x56,
//    0x57,
//    0x58,
//    0x59,
//    0x5A,
//    0x31,
//    0x32,
//    0x33,
//    0x34,
//    0x35,
//    0x36,
//    0x37,
//    0x38,
//    0x39,
//    0x30,
//    0x0D,
//    0x1B,
//    0x2E,
//    0x09,
//    0x20,
//    0xBD,
//    0xBB,
//    0xDB, //0x2F
//    0xDD,
//    0xDC,
//    0xC0,
//    0xBA,
//    0xDE,
//    0xC0,
//    0xBC,
//    0xBE,
//    0xBF,
//    0x14,
//    0x70,
//    0x71,
//    0x72,
//    0x73,
//    0x74,
//    0x75,
//    0x76,
//    0x77,
//    0x78,
//    0x79,
//    0x7A,
//    0x7B,
//    0x2C,
//    0x91,
//    0x13,
//    0x2D,
//    0x24,
//    0x21,
//    0x2E,
//    0x23,
//    0x22,
//    0x27,//0x4F
//    0x25,
//    0x28,
//    0x26,
//    0x90,//and clear
//    0x6F,
//    0x6A,
//    0x6D,
//    0x6B,
//    0x0D,
//    0x61,
//    0x62,
//    0x63,
//    0x64,
//    0x65,
//    0x66,
//    0x67,
//    0x68,
//    0x69,
//    0x60,	
//    0xBE,
//    0xE2,
//    0x5D,
//    0x00,
//    0x00,
//    0x7C,
//    0x7D,
//    0x7E,
//    0x7F,
//    0x80,
//    0x81,
//    0x82,
//    0x83,
//    0x84,
//    0x85,
//    0x86,
//    0x87,
//    0x2B,
//    0x2F,
//    0x00,
//    0x29,
//    0x00,
//    0x00,
//    0x115A,
//    0x1158,
//    0x1143,
//    0x1156,
//    0x1146,///!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//    0xAD, //0x7F
//    0xAF,
//    0xAE,
//    0x14,
//    0x90,
//    0x91,
//    0x6E,
//    0x00,
//    0x1110,
//    0x1110,
//    0x1110,
//    0x1110,
//    0x1110,
//    0x1110,
//    0x1110,
//    0x1110,
//    0x1110, //0x8F
//    0x1130,
//    0x1131,
//    0x1132,
//    0x1133,
//    0x1134,
//    0x1135,
//    0x1136,
//    0x1137,
//    0x1138,
//    0xF9,
//    0xF6,
//    0x03,
//    0xFE,
//    0x00,
//    0x00,
//    0x6C, //0x9F
//    0x00,
//    0x00,
//    0x0C,
//    0xF7,
//    0xF8,
//    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, //0xAF
//    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,//0xBF
//    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,//0xCF
//    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,//0xDF
//    0xA2,
//    0xA0,
//    0x12,
//    0x5B,
//    0xA3,
//    0xA1,
//    0x12,
//    0x5C, //0xE7/////!!!!!!!!!!
//    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, //0xEF
//    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,//0xFF
//};
///////////////////////////////////////////////////////////////////////////
             0x00 /*" Reserved "*/ ,
	         0xF9,//" ErrorRollOver " ,
	         0x00,//" POSTFail " ,
	         0x00,//" ErrorUndefined " ,//!!
	         0x41,//" a A " ,
	         0x42,//" b B " ,
	         0x43,//" c C " ,
	         0x44,//" d D " ,
	         0x45,//" e E " ,
	         0x46,//" f F " ,
	         0x47,//" g G " ,
	         0x48,//" h H " ,
	         0x49,//" i I " ,
	         0x4A,//" j J " ,
	         0x4B,//" k K " ,
	         0x4C,//" l L " , //0x0F	
	         0x4D,//" m M " ,
	         0x4E,//" n N " ,
	         0x4F,//" o O " ,
	         0x50,//" p P " ,
	         0x51,//" q Q " ,
	         0x52,//" r R " ,
	         0x53,//" s S " ,
	         0x54,//" t T " ,
	         0x55,//" u U " ,
	         0x56,//" v V " ,
	         0x57,//" w W " ,
	         0x58,//" x X " ,
	         0x59,//" y Y " ,
	         0x5A,//" z Z " ,
	         0x31,//" 1 ! " ,
	         0x32,//" 2 @ " , //0x1F
	         0x33,//" 3 # " ,
	         0x34,//" 4 $ " ,
	         0x35,//" 5 % " ,
	         0x36,//" 6 ^ " ,
	         0x37,//" 7 & " ,
	         0x38,//" 8 * " ,
	         0x39,//" 9 and( " ,
	         0x30,//" 0 and) " ,
	         0x0D,//" ENTER " ,
	         0x1B,//" ESCAPE " ,
	         0x08,//" <-(Backspace) " ,
	         0x09,//" Tab " ,
	         0x20,//" Spacebar " ,
	         0xBD,//" - _ " ,
	         0xBB,//" = + " ,
	         0xDB,//" [ { " , //0x2F
	         0xDD,//" ] } " ,
	         0xDC,//" \\ | " ,
	         0xC0,//" Non - US # ~ " ,//!!
	         0xBA,//" ; : " ,
	         0xDE,//" ' \"  " ,
	         0xC0,//" ` ~ " ,
	         0xBC,//" , < " ,
        	 0xBE,//" . > " ,
	         0xBF,//" / ? " ,
	         0x14,//" Caps Lock " ,
	         0X70,//" F1 " ,
	         0X71,//" F2 " ,
	         0X72,//" F3 " ,
	         0X73,//" F4 " ,
	         0X74,//" F5 " ,
	         0X75,//" F6 " , //0x3F
	         0X76,//" F7 " ,
	         0X77,//" F8 " ,
	         0X78,//" F9 " ,
	         0X79,//" F10 " ,
	         0X7A,//" F11 " ,  
	         0X7B,//" F12 " ,
	         0x2C,//" PrintScreen " ,
	         0x91,//" Scroll Lock " ,
	         0x03,//" Pause/Break " ,
	         0x2D,//" Insert " ,
	         0x24,//" Home " ,
	         0x21,//" PageUp " ,
	         0x2E,//" Delete Forward " ,
	         0x23,//" End " ,
	         0x22,//" PageDown " ,
	         0x27,//" RightArrow " ,//0x4F
	         0x25,//" LeftArrow " ,
	         0x28,//" DownArrow " ,
	         0x26,//" UpArrow " ,
	         0x90,//" Num Lock " ,
	         0x6F,//" / " ,
	         0X6A,//" * " ,
	         0x6D,//" - " ,
	         0x6B,//" + " ,
	         0x0D,//" ENTER " ,
	         0x61,//" 1 End " ,
	         0x62,//" 2 Down Arrow " ,
	         0x63,//" 3 PageDn " ,
	         0x64,//" 4 Left Arrow " ,
	         0x65,//" 5 " ,
	         0x66,//" 6 Right Arrow " ,
	         0x67,//" 7 Home " , //0x5F
	         0x68,//" 8 Up Arrow " ,
	         0x69,//" 9 PageUp " ,
	         0x60,//" 0 Insert " ,
	         0x6E,//" . Delete " ,
	         0xE2,//" Non - US \\ | " ,
	         0x5D,//" Application " ,
	         0x00,//" Power " ,//!!
	         0x00,//" Keypad = " ,//!!
	         0X7C,//" F13 " ,
	         0X7D,//" F14 " ,
	         0X7E,//" F15 " ,
	         0X7F,//" F16 " ,
	         0X80,//" F17 " ,
	         0X81,//" F18 " ,
	         0X82,//" F19 " ,
	         0X83,//" F20 " , //0x6F
	         0X84,//" F21 " ,
	         0X85,//" F22 " ,
	         0X86,//" F23 " ,
	         0X87,//" F24 " ,
	         0x2B,//" Execute " ,
	         0x2F,//" Help " ,
	         0x00,//" Menu " ,
	         0x29,//" Select " ,
	         0xB2,//" Stop " ,
	         0x00,//" Again " ,
	         0xA25A,//" Undo " ,
	         0xA258,//" Cut " ,
	         0xA243,//" Copy " , 
	         0xA256,//" Paste " ,
	         0xA246,//" Find " ,
	         0xAD,//" Mute " , //0x7F
	         0xAF,//" Volume Up " ,
	         0xAE,//" Volume Down " ,
	         0x14,//" Locking Caps Lock " ,
	         0x90,//" Locking Num Lock " ,
	         0x91,//" Locking Scroll Lock " ,
	         0x6E,//" Keypad Comma " ,
	         0x00,//" Keypad Equal Sign " ,
	         0xA230,//" International1 " ,
	         0xA231,//" International2 " ,
	         0xA232,//" International3 " ,
	         0xA233,//" International4 " ,
	         0xA234,//" International5 " ,
	         0xA235,//" International6 " ,
	         0xA236,//" International7 " ,
	         0xA237,//" International8 " ,
	         0xA238,//" International9 " , //0x8F
	         0xA230,//" LANG1 " ,
	         0xA231,//" LANG2 " ,
	         0xA232,//" LANG3 " ,
	         0xA233,//" LANG4 " ,
	         0xA234,//" LANG5 " ,
	         0xA235,//" LANG6 " ,
	         0xA236,//" LANG7 " ,
	         0xA237,//" LANG8 " ,
	         0xA238,//" LANG9 " ,
	         0xF9,//" Alternate Erase " ,
	         0xF6,//" SysReq / Attention " ,//!!0xF0
	         0x03,//" Cancel " ,
	         0xFE,//" Clear " ,//!!
	         0x00,//" Prior " ,
	         0x00,//" Return " ,
	         0x6C,//" Separator " , //0x9F
        	 0x00,//" Out " ,
	         0x00,//" Oper " ,
	         0xE6,//" Clear / Again " ,
	         0xF7,//" CrSel / Props " ,
	         0xF8,//" ExSel " ,
	         0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ , //0xAF
	         0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,//0xBF
	         0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,//0xCF
	         0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,//0xDF
	         0xA2,//" LeftControl " ,
	         0xA0,//" LeftShift " ,
	         0xA4,//" LeftAlt " ,
	         0x5B,//" Left GUI " ,
	         0xA3,//" RightControl " ,
	         0xA1,//" RightShift " ,
	         0xA5,//" RightAlt " ,
	         0x5C,//" Right GUI " , //0xE7/////!!!!!!!!!!
	         0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ , //0xEF
	         0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,  0x00 /*" Reserved "*/ ,//0xFF
        };


        public static string pUsagePage(uint v)
        {
            string[] tbl =
            { 
                "Undefined",
	            "Generic Desktop Ctrls",
	            "Sim Ctrls",
	            "VR Ctrls",
	            "Sport Ctrls",
	            "Game Ctrls",
	            "Generic Dev Ctrls",
	            "Kbrd/Keypad",
	            "LEDs",
	            "Button",
	            "Ordinal",
	            "Telephony",
	            "Consumer",
	            "Digitizer",
	            "Reserved 0x0E",
	            "PID Page",
	            "Unicode",
	            "Reserved 0x11",
	            "Reserved 0x12",
	            "Reserved 0x13",
	            "Alphanumeric Display",
            };

            if (v < 21)
            {
                return tbl[v];
            }
            else if (v == 0x40)
            {
                return " (Medical Instruments)";
            }
            else if (v >= 0x80 && v <= 0x83)
            {
                return " (Monitor Pages)";
            }
            else if (v >= 0x84 && v <= 0x87)
            {
                return " (Power Pages)";
            }
            else if (v == 0x8C)
            {
                return " (Bar Code Scanner Page)";
            }
            else if (v == 0x8D)
            {
                return " (Scale Page)";
            }
            else if (v == 0x8E)
            {
                return " (MagStripe Reading Devices)";
            }
            else if (v == 0x8F)
            {
                return " (Rsv'ed Point-of-Sale Pages)";
            }
            else if (v == 0x90)
            {
                return " (Camera Control Page)";
            }
            else if (v == 0x91)
            {
                return " (Arcade Page)";
            }
            else if (v >= 0x92 && v <= 0xFEFF)
            {
                return "Reserved";
            }
            else if (v >= 0xFF00 && v <= 0xFFFFF)
            {
                return "Vendor Defined";
            }
            else
            {
                return "ERROR1";
            }
        }

        public static string pUsage(uint v, ushort u)
        {
            string str = "ERROR2";
            switch (u)
            {
                case 1: { str = genericDesctop[v]; break; }
                case 2: { str = simulationControls[v]; break; }
                case 3: { str = vrControls[v]; break; }
                case 4: { str = sportsControls[v]; break; }
                case 5: { str = gameControls[v]; break; }
                case 6: { str = genericDeviceControls[v]; break; }
                case 7: { str = keyBoard[v]; break; }
                case 8: { str = LEDs[v]; break; }
                case 9: { str = Button[v]; break; }
                case 10: { str = Ordinal[v]; break; }
                case 11: { str = telephony[v]; break; }
                case 12: { str = consumer[v]; break; }
                case 13: { str = digitizers[v]; break; }
                case 16: { str = unicode[v]; break; }
                case 20: { str = alphnumericDisplay[v]; break; }
                case 64: { str = medicalInstrument[v]; break; }
                default: { str = "Undefined"; break; }
            };
            return str;
        }

        public static ushort VKconverter(uint keyCode, ushort usagePage)
        {
            ushort virtualKeyCode = 0;

            try
            {
                switch (usagePage)
                {
                    case 0x07://keyboard
                        {
                            virtualKeyCode = keyboardVK[keyCode];
                            break;
                        }
                    case 0x01://generic desctop controller
                        {
                            switch (keyCode)
                            {
                                case 0x82: { virtualKeyCode = 0x5F; break; }//Sleep
                                default: { virtualKeyCode = 0x00; break; }
                            }
                            break;
                        }

                    case 0x12://consumer
                        {
                            switch (keyCode)
                            {
                                case 0xB5: { virtualKeyCode = 0xB0; break; } //Scan Next Track
                                case 0xB6: { virtualKeyCode = 0xB1; break; } //Scan Previous Track
                                case 0xB7: { virtualKeyCode = 0xB2; break; } //Stop
                                case 0xB0: { virtualKeyCode = 0xB3; break; } //Play
                                case 0xB1: { virtualKeyCode = 0x13; break; } //Pause
                                case 0x180: { virtualKeyCode = 0xB6; break; } //Appl
                                case 0x18A: { virtualKeyCode = 0xB4; break; } //Email Reader
                                case 0x87: { virtualKeyCode = 0xB5; break; } //media
                                case 0x22F: { virtualKeyCode = 0xFB; break; } //Zoom
                                case 0x80: { virtualKeyCode = 0xEF; break; } //Selection
                                case 0x31: { virtualKeyCode = 0xE9; break; } //Reset
                                case 0x1A1: { virtualKeyCode = 0xE5; break; } //Process
                                case 0x196: { virtualKeyCode = 0xAC; break; } //Browser

                                default: { virtualKeyCode = 0x00; break; }
                            }
                            break;
                        }

                    default:
                        {
                            virtualKeyCode = 0x00;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return virtualKeyCode;
        }
    }

    public class HistoryItem
    {
        public string _Report_str { get; set; }

        public ushort _keyCode { get; set; }

        public ushort _UsagePage { get; set; }

        public double _Time { get; set; }

        public string _ReportName { get; set; }

        public byte[] _Report_arr { get; set; }

        public ButtonState _keyState { get; set; }
    }

    public class LogTimer
    {
        //Start and stop time
        DateTime Start;
        DateTime Stoped;

        //Flag: 1 - timer is activated, 0 - else
        private bool isActive;

        //Constrtuctor
        public LogTimer()
        {
            isActive = false;
            Start = DateTime.Now;
        }

        //Get time
        public double GetTime()
        {
            if (this.isActive)
            {
                return GetContTime();
            }
            else
            {
                return GetFirstTime();
            }
        }

        public void StopTimer()
        {
            this.isActive = false;
        }

        //timer use not first time
        private double GetContTime()
        {
            Stoped = DateTime.Now;

            TimeSpan Elapsed = new TimeSpan();

            Elapsed = Stoped.Subtract(Start);
            Start = DateTime.Now;

            return Elapsed.TotalMilliseconds;
        }
        //timer first time use
        private double GetFirstTime()
        {
            isActive = true;
            Start = DateTime.Now;
            return 0;
        }
    }

    public class keyboardButton : Button
    {       
        public ushort _keyCode;         //keyCode from Usage Page table
        public ushort _usagePage;       //usage Page code
        public ushort _virtualCode;     //Virtual keyCode
        public ButtonState _keyState;   //Flag: 0 - button, 1 - button down, 2 - waiting

        public keybdPanel _keybdPanel;  //pointer to current Panel      

        //Constructor
        public keyboardButton(ushort keyCode, ushort usagePage, keybdPanel keybdPanel)
        {
            this.Style = this.FindResource("PanelKeyStyle") as Style;

            this.PreviewMouseLeftButtonDown += this.keyBoardButton_Down;
            this.PreviewMouseLeftButtonUp += this.keyBoardButton_Up;
            this.PreviewMouseRightButtonDown += keyBoardButton_Stick;

            this._keyCode = keyCode;
            this._usagePage = usagePage;
            this._keybdPanel = keybdPanel;
            this._keyState = ButtonState.Wait;
            this.Content = HidUsagePage.pUsage(keyCode, usagePage);
                       
            this._virtualCode = HidUsagePage.VKconverter(this._keyCode, this._usagePage);
           
        }
        public keyboardButton()
        {
            this.Style = this.FindResource("MagicButtonStyle") as Style;                     
        }
        //Button Down Event
        public void keyBoardButton_Down(object sender, RoutedEventArgs e)
        {
            if (this._keyState != ButtonState.Down)
            {//butoon is not pressed                      
                this._keyState = ButtonState.Down;
                this.BorderBrush = new SolidColorBrush(Color.FromRgb(166, 14, 14));//RED Colour
                this._keybdPanel.keyBoardButton_Down(this._keyCode, this._usagePage);            
            }
        }

        //Button Up Event
        public void keyBoardButton_Up(object sender, RoutedEventArgs e)
        {
            this._keyState = ButtonState.Up;
            this.BorderBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
            this._keybdPanel.keyBoardButton_Up(this._keyCode, this._usagePage);
            this._keyState = ButtonState.Wait;        
        }

        //Button stick
        public void keyBoardButton_Stick(object sender, RoutedEventArgs e)
        {
            if (this._keyState == ButtonState.Down)
            {
                //button is already pressed
                keyBoardButton_Up(sender, e);
            }
            else
            {
                keyBoardButton_Down(sender, e);
            }
        }

    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FocusWatcher _watcher;
        private uint _layout;


        //Flag: 1 =  ConfigDescriptor and ReportDescriptor loaded, 0 - else;
        private bool dataLoaded = false;
        
        //VID and PID device string
        public string VID = "";
        public string PID = "";

        //Device Configuration and Report descriptors buffers
        public byte[] ConfigDescriptor = new byte[512];
        public byte[] ReportDescriptor = new byte[4096];

        //Global variable
        //main modules
        public _WinUsbClass WinUsb;
        public _UsbHostClass UsbHost;
        public _UsbDeviceClass UsbDevice;

        //List of All keyboard Panels
        public List<keybdPanel> keyboardPanels;

        //History Log
        public WindowLog HistoryLog;

        //History Log Timer
        public LogTimer HistoryLogTimer;

        public MainWindow()
        {
            InitializeComponent();
            _watcher = new FocusWatcher();
            _watcher.LastActiveWindowChanged -= OnLastActiveWindowChanged;

            //Init Panel array
            keyboardPanels = new List<keybdPanel>();
        }

        private bool LoadData()
        {
            try
            {
                this.VID = (VID_TextBox.Text.ToString()).ToLower();
                this.PID = (PID_TextBox.Text.ToString()).ToLower();

                if (this.VID == "" || this.PID == "")
                {
                    throw new System.Exception("Error: VID or PID is not valid");
                }

                else
                {
                    //get Config and Report Descriptors from device
                    WinUsb = new _WinUsbClass(this.VID, this.PID, this.ConfigDescriptor, this.ReportDescriptor);
                    dataLoaded = WinUsb.GetInfoFromDevice();
                    
                    if (!dataLoaded)
                    {
                        throw new System.Exception("Error: Device not found");
                    }
                    else
                    {
                        //Init Host and Device modules
                        UsbHost = new _UsbHostClass(this.ConfigDescriptor, this.ReportDescriptor);
                        UsbDevice = new _UsbDeviceClass(this.ConfigDescriptor, this.ReportDescriptor);

                        //init Timer
                        HistoryLogTimer = new LogTimer();

                        return true;
                    }
                }                
            }                   
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void Border_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void MainWindow_OnExitButtonClick(object sender, RoutedEventArgs e)
        {
            this.ClosePanels();
            App.Current.Shutdown();
        }

        private void MainWindow_OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private bool GenerateKeyboard()
        {
            try
            {
                //Init Log Window
                HistoryLog = new WindowLog(HistoryLogTimer);

                //get keyboard array
                List<ushort> keyBoard = new List<ushort>();
                keyBoard = UsbHost.GenerateKeyboard();

                //cur Page Table
                ushort pageTable = 0;

                //look at each element
                for (int i = 0; i < keyBoard.Count(); i++)
                {

                    if (keyBoard[i] == 0x00)
                    {
                        if (keyBoard[i + 1] == 0x00)
                        {
                            //New Usage Page Item
                            //Create new Panel
                            i += 2;
                            pageTable = keyBoard[i];

                            if (pageTable == 0)
                            {
                                //Not valid usage Page = 0
                                continue;
                            }

                            keybdPanel _Panel = new keybdPanel(HistoryLog, UsbDevice, HistoryLogTimer, _watcher);

                            //_Panel.Title = HidUsagePage.pUsagePage(pageTable);

                            keyboardPanels.Add(_Panel);

                            continue;
                        }
                        else
                        {
                            //not valid usage = 0
                            continue;
                        }
                    }
                    else
                    {
                        //New Usage item 
                        //Create new button
                        keyboardButton _Button = new keyboardButton(keyBoard[i], pageTable, keyboardPanels[keyboardPanels.Count - 1]);

                        //Add button to array
                        keyboardPanels[keyboardPanels.Count - 1].buttonArray.Add(_Button);
                    }
                }//end of for(...)

                //Now create keyboard panels with buttons

                foreach (keybdPanel _panel in keyboardPanels)
                {
                    //Add button
                    foreach (keyboardButton _button in _panel.buttonArray)
                    {
                        _panel.wrapPanel.Children.Add(_button);
                    }

                    //show panel
                    _panel.Show();
                }

                //Show Log Window
                HistoryLog.Show();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void MainWindow_OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ClosePanels();

                if (!dataLoaded)
                {                
                    if (!this.LoadData())
                        throw new Exception("Error: Can't load data from device");                               
                }
                                
                //Generate keyboard
                this.GenerateKeyboard();             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void MainWindow_OnLoadButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                

                 WinApiImport.SetForegroundWindow(this._watcher.LastWindowHandleExceptCurrentProcess);

                //Create new Log Form
                WindowLog _windowLog = new WindowLog(HistoryLogTimer);
                _windowLog.Button_Clear.IsEnabled = false;
                _windowLog.Button_Clear.Visibility = System.Windows.Visibility.Hidden;
                _windowLog.Button_Save.IsEnabled = false;
                _windowLog.Button_Save.Visibility = System.Windows.Visibility.Hidden;
                _windowLog.Button_Capture.IsEnabled = false;
                _windowLog.Button_Capture.Visibility = System.Windows.Visibility.Hidden;
                _windowLog.Button_Minimize.Visibility = Visibility.Visible;
                _windowLog.Button_Exit.Visibility = Visibility.Visible;              

                FileStream fs = new FileStream(@"LOG.bin", FileMode.Open);
                BinaryReader br = new BinaryReader(fs);                   
                
                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    
                    //Data for each item in file
                    byte reportSize = Convert.ToByte(br.PeekChar());
                    byte[] reportArr = br.ReadBytes(reportSize);
                    double time = br.ReadDouble();
                    ushort keyCode = br.ReadUInt16();
                    ushort usagePage = br.ReadUInt16();
                    byte keyState = br.ReadByte();
                    string reportStr = "";                                  
                   
                    for (int i = 0; i < reportSize; i++)
                    {
                        reportStr += (reportArr[i].ToString("X2") + " ");
                    }

                    HistoryItem item = new HistoryItem()
                    {
                        _Report_str = reportStr,
                        _keyCode = keyCode,
                        _UsagePage = usagePage,
                        _Time = time,
                        _ReportName = Convert.ToString(HidUsagePage.pUsage(keyCode, usagePage) + (ButtonState)keyState),
                        _Report_arr = reportArr,
                        _keyState = (ButtonState)keyState,
                    };
                    
                    _windowLog.AddLoadedItem(item);                  

                    if ((ButtonState)keyState == ButtonState.Down)
                    {
                        System.Threading.Thread.Sleep(Convert.ToInt32(time));
                        keybdPanel.SendKeyboardEvent_Down(HidUsagePage.VKconverter(keyCode, usagePage));      

                    }
                    else
                    {
                        System.Threading.Thread.Sleep(Convert.ToInt32(time));
                        keybdPanel.SendKeyboardEvent_Up(HidUsagePage.VKconverter(keyCode, usagePage));                      
                    }                   
                }

                fs.Close();

                //show Log Window
                _windowLog.Show();

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            _layout = layout;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.VID != (VID_TextBox.Text.ToString()).ToLower() ||
                 this.PID != (PID_TextBox.Text.ToString()).ToLower())
            {
                this.dataLoaded = false;
            }
            else
            {
                this.dataLoaded = true;
            }

        }

        private void ClosePanels()
        {
            if (keyboardPanels.Count > 0)
            {
                foreach (keybdPanel panel in this.keyboardPanels)
                {
                    panel.Close();
                }
                
                keyboardPanels.Clear();
                this.HistoryLog.Close();
                this.WinUsb.UninstallDriver();
            }
        }

    }
}
