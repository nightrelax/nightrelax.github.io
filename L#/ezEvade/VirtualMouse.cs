using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using SharpDX;
using LeagueSharp.Common;
using LeagueSharp;

namespace ezEvade
{

    public enum WindowsMessages
    {
        WM_LBUTTONDBLCLCK = 0x203,
        WM_RBUTTONDBLCLCK = 0x206,
        WM_MBUTTONDBLCLCK = 0x209,
        WM_MBUTTONDOWN = 0x207,
        WM_MBUTTONUP = 0x208,
        WM_MOUSEMOVE = 0x200,
        WM_LBUTTONDOWN = 0x201,
        WM_LBUTTONUP = 0x202,
        WM_RBUTTONDOWN = 0x204,
        WM_RBUTTONUP = 0x205,
        WM_KEYDOWN = 0x0100,
        WM_KEYUP = 0x101
    }

    public enum MouseEvents
    {
        MOUSEEVENTF_RIGHTDOWN = 0x0008,
        MOUSEEVENTF_RIGHTUP = 0x0010,
        MOUSEEVENTF_MOVE = 0x0001,
        MOUSEEVENTF_ABSOLUTE = 0x8000,
    }

    public enum KeyboardEvents
    {
        KEYBDEVENTF_SHIFTVIRTUAL = 0x10,
        KEYBDEVENTF_SHIFTSCANCODE = 0x2A,
        KEYBDEVENTF_KEYDOWN = 0,
        KEYBDEVENTF_KEYUP = 2
    }

    public enum VirtualCommand
    {
        RightClick,
        ShiftClick,
    }

    public struct MOUSEPOINT
    {
        public int X;
        public int Y;

        public static implicit operator Point(MOUSEPOINT point)
        {
            return new Point(point.X, point.Y);
        }
    }
    
    /// <summary>
    ///     This class offers real mouse clicks.
    /// </summary>
    public class VirtualMouse
    {
        public static int clickdelay;
        public static int attkdelay;
        public static bool disableOrbClick = false; //if set to true, orbwalker won't send right clicks - for other scripts
        public static int coordX;
        public static int coordY;

        // mouse event
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        
        // set mouse position
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetCursorPos(int X, int Y);

        // get mouse position
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out MOUSEPOINT lpPoint);

        // keyboard event
        [DllImport("user32.dll", EntryPoint = "keybd_event", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern void keybd_event(byte vk, byte scan, int flags, int extrainfo);
        // simulates a click-and-release action of the right mouse button at its current position

        public static LeagueSharp.Common.Menu menu;

        public VirtualMouse(LeagueSharp.Common.Menu mainMenu)
        {
            var fakeclicks = new LeagueSharp.Common.Menu("Fake Clicks", "fakeClicks");
            fakeclicks.AddItem(new LeagueSharp.Common.MenuItem("clickEnable", "Enable").SetValue(false));
            fakeclicks.AddItem(new LeagueSharp.Common.MenuItem("MoveMouseBack", "Move Mouse Back").SetValue(false));
            fakeclicks.AddItem(new LeagueSharp.Common.MenuItem("clickDelay", "Click Delay").SetValue(new Slider(200, 0, 2000)));
            fakeclicks.AddItem(new LeagueSharp.Common.MenuItem("randomDelay", "Random Delay").SetValue(new Slider(100, 0, 500)));            

            mainMenu.AddSubMenu(fakeclicks);

            menu = mainMenu;
        }

        public static int GetTickCount()
        {
            return (int)DateTime.Now.TimeOfDay.TotalMilliseconds; //Game.ClockTime * 1000;
        }

        public static void VirtualClick(VirtualCommand command)
        {
            VirtualClick(command, new Vector2(coordX, coordY));
        }

        public static void VirtualClick(VirtualCommand command, Vector3 position)
        {
            VirtualClick(command, Drawing.WorldToScreen(position));
        }

        public static void VirtualClick(VirtualCommand command, Vector2 position)
        {
            if (menu.SubMenu("fakeClicks").Item("clickEnable").GetValue<bool>() == false)
                return;

            
            bool shouldMoveMouseBack = menu.SubMenu("fakeClicks").Item("MoveMouseBack").GetValue<bool>();

            if (GetTickCount() - clickdelay > menu.SubMenu("fakeClicks").Item("clickDelay").GetValue<Slider>().Value)
            {
                if (command == VirtualCommand.RightClick)
                {
                    MOUSEPOINT oldCursorPos;
                    GetCursorPos(out oldCursorPos);
                    SetCursorPos((int)position.X, (int)position.Y);

                    mouse_event((int)(MouseEvents.MOUSEEVENTF_RIGHTDOWN), (int)position.X, (int)position.Y, 0, 0);                    
                    mouse_event((int)(MouseEvents.MOUSEEVENTF_RIGHTUP), (int)position.X, (int)position.Y, 0, 0);

                    if (shouldMoveMouseBack)
                    {
                        Utility.DelayAction.Add(1, () => SetCursorPos((int)oldCursorPos.X, (int)oldCursorPos.Y));
                    }                        
                }
                else if (command == VirtualCommand.ShiftClick)
                {
                    MOUSEPOINT oldCursorPos;
                    GetCursorPos(out oldCursorPos);
                    SetCursorPos((int)position.X, (int)position.Y);

                    keybd_event((int)KeyboardEvents.KEYBDEVENTF_SHIFTVIRTUAL, (int)KeyboardEvents.KEYBDEVENTF_SHIFTSCANCODE, (int)KeyboardEvents.KEYBDEVENTF_KEYDOWN, 0);
                    mouse_event((int)MouseEvents.MOUSEEVENTF_RIGHTDOWN, (int)position.X, (int)position.Y, 0, 0);
                    mouse_event((int)MouseEvents.MOUSEEVENTF_RIGHTUP, (int)position.X, (int)position.Y, 0, 0);
                    Utility.DelayAction.Add(200, () => { keybd_event((int)KeyboardEvents.KEYBDEVENTF_SHIFTVIRTUAL, (int)KeyboardEvents.KEYBDEVENTF_SHIFTSCANCODE, (int)KeyboardEvents.KEYBDEVENTF_KEYUP, 0); });

                    if (shouldMoveMouseBack)
                    {
                        Utility.DelayAction.Add(1, () => SetCursorPos((int)oldCursorPos.X, (int)oldCursorPos.Y));
                    }           
                }
                clickdelay = GetTickCount();
            }
        }
    }
}
