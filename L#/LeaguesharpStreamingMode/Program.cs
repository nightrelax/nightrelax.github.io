using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LeagueSharp;

namespace LeaguesharpStreamingMode
{
    class Program
    {
        static Assembly lib = Assembly.Load(LeaguesharpStreamingMode.Properties.Resources.LeaguesharpStreamingModelib);
        static string version = Game.Version[3] == '.' ? Game.Version.Substring(0, 3) : Game.Version.Substring(0, 4);
        static Int32 LeaguesharpCore = GetModuleAddress("Leaguesharp.Core.dll");
        static Dictionary<Int32, Int32> offsets;

        static Int32 GetModuleAddress(String ModuleName)
        {
            Process P = Process.GetCurrentProcess();
            for (int i = 0; i < P.Modules.Count; i++)
                if (P.Modules[i].ModuleName == ModuleName)
                    return (Int32)(P.Modules[i].BaseAddress);
            return 0;
        }

        static byte[] ReadMemory(Int32 address, Int32 length)
        {
            MethodInfo _ReadMemory = lib.GetType("LeaguesharpStreamingModelib.MemoryModule").GetMethods()[2];
            return (byte[])_ReadMemory.Invoke(null, new object[] { address, length });
        }

        static void WriteMemory(Int32 address, byte value)
        {
            MethodInfo _WriteMemory = lib.GetType("LeaguesharpStreamingModelib.MemoryModule").GetMethods()[4];
            _WriteMemory.Invoke(null, new object[] { address, value });
        }

        static void WriteMemory(Int32 address, byte[] array)
        {
            MethodInfo _WriteMemory = lib.GetType("LeaguesharpStreamingModelib.MemoryModule").GetMethods()[4];
            _WriteMemory.Invoke(null, new object[] { address, array });
        }

        static int SignatureScan(int start, int length, int[] pattern)
        {
            var buffer = ReadMemory(start, length);
            for (int i = 0; i < buffer.Length - pattern.Length; i++)
            {
                if ((int)buffer[i] == pattern[0])
                {
                    for (int i2 = 1; i2 < pattern.Length; i2++)
                    {
                        if (pattern[i2] >= 0 && (int)buffer[i + i2] != pattern[i2])
                            break;
                        if (i2 == pattern.Length - 1)
                            return i;
                    }
                }
            }
            return -1;
        }

        enum functionOffset : int
        {
            drawEvent = 0,
            printChat = 1,
            loadingScreenWatermark = 2
        }

        enum asm : byte
        {
            ret = 0xC3,
            push_ebp = 0x55,
            nop = 0x90
        }

        static void SetUpOffsets()
        {
            offsets = new Dictionary<Int32, Int32>();
            int[] p_drawEvent = { 0x55, 0x8B, 0xEC, 0x6A, 0xFF, 0x68, -1, -1, -1, -1, 0x64, 0xA1, 0, 0, 0, 0, 0x50, 0x83, 0xEC, 0x0C, 0x56, 0xA1, -1, -1, -1, -1, 0x33, 0xC5 };
            int[] p_printChat = { 0x55, 0x8B, 0xEC, 0x8D, 0x45, 0x14, 0x50 };
            
            int max_offset = 0x50000;
            int offset_drawEvent = SignatureScan(LeaguesharpCore, max_offset, p_drawEvent);
            int offset_printChat = SignatureScan(LeaguesharpCore, max_offset, p_printChat);

            offsets.Add((int)functionOffset.drawEvent, offset_drawEvent);
            offsets.Add((int)functionOffset.printChat, offset_printChat);
            offsets.Add((int)functionOffset.loadingScreenWatermark, offset_printChat - 0x7B);
        }

        static void Enable()
        {
            WriteMemory(LeaguesharpCore + offsets[(int)functionOffset.drawEvent], (byte)asm.ret);
            WriteMemory(LeaguesharpCore + offsets[(int)functionOffset.printChat], (byte)asm.ret);
            WriteMemory(LeaguesharpCore + offsets[(int)functionOffset.loadingScreenWatermark], new byte[] { (byte)asm.nop, (byte)asm.nop, (byte)asm.nop, 
                                                                                                         (byte)asm.nop, (byte)asm.nop, (byte)asm.nop });
        }

        static void Disable()
        {
            WriteMemory(LeaguesharpCore + offsets[(int)functionOffset.drawEvent], (byte)asm.push_ebp);
            WriteMemory(LeaguesharpCore + offsets[(int)functionOffset.printChat], (byte)asm.push_ebp);
        }

        static bool IsEnabled() { return ReadMemory(LeaguesharpCore + offsets[(int)functionOffset.printChat], 1)[0] == (byte)asm.ret; }

        static uint[] hotkeys = { 0x24, 0x2D };  //home key, insert key
		static uint hotkey_overrideNames = 35; //end key
        static void OnWndProc(LeagueSharp.WndEventArgs args)
        {
            if (args.Msg == 0x100) //WM_KEYDOWN
            {
                if (hotkeys.Contains(args.WParam))
                {
                    if (IsEnabled())
                        Disable();
                    else
                        Enable();
                }
		if (args.WParam == hotkey_overrideNames)
			OverrideNames();
            }
        }

        static void OverrideNames()
        {
            var scanner = new MemoryScan(Process.GetCurrentProcess());
            int[] pattern = { 0x40, 0x73, 0x72, 0x63, 0x74, 0x72, 0x20, 0x6E, 0x61, 0x6D, 0x65, 0x40 };
            var results = scanner.Scan(pattern);
            if (results.Length == 0) return;
            foreach(MemoryScan.MemoryScanResult result in results)
            {
                for (int i = result.offset; i < result.offset + 0xFFFF; i++)
                    if (result.buffer[i] == 0x40) //&& result.buffer[i - 1] == 0x65
                        result.buffer[i] = 0x65; //result.buffer[i - 1] = 0x61;
                WriteMemory(result.BaseAddress, result.buffer);
                Console.WriteLine(result.Address.ToString("X"));
                break;
            }
        }

        static void OverrideNames2()
        {
            var scanner = new MemoryScan(Process.GetCurrentProcess());
            int[] pattern = { 0x3C, 0x74, 0x69, 0x74, 0x6C, 0x65, 0x4C, 0x65, 0x66, 0x74, 0x3E };
            var results = scanner.Scan(pattern);
            foreach (MemoryScan.MemoryScanResult result in results)
            {
                Console.WriteLine(result.BaseAddress.ToString("X"));
            }
        }

        static void Main(string[] args)
        {
            //OverrideNames();
            //OverrideNames2();


            SetUpOffsets();
            Enable();

            LeagueSharp.Game.OnWndProc += OnWndProc;
            AppDomain.CurrentDomain.DomainUnload += delegate
            {
                Disable();
            };
        }

    }
}

class MemoryScan
{
    const int PROCESS_QUERY_INFORMATION = 0x0400;
    const int MEM_COMMIT = 0x00001000;
    const int PAGE_READWRITE = 0x04;
    const int PROCESS_WM_READ = 0x0010;

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

    public struct MEMORY_BASIC_INFORMATION
    {
        public int BaseAddress;
        public int AllocationBase;
        public int AllocationProtect;
        public int RegionSize;
        public int State;
        public int Protect;
        public int lType;
    }

    public struct SYSTEM_INFO
    {
        public ushort processorArchitecture;
        ushort reserved;
        public uint pageSize;
        public IntPtr minimumApplicationAddress;
        public IntPtr maximumApplicationAddress;
        public IntPtr activeProcessorMask;
        public uint numberOfProcessors;
        public uint processorType;
        public uint allocationGranularity;
        public ushort processorLevel;
        public ushort processorRevision;
    }

    private Process process = null;

    public MemoryScan(Process P)
    {
        process = P;
    }

    public int SingleSignatureScan(int[] pattern, byte[] buffer)
    {
        for (int i = 0; i < buffer.Length - pattern.Length; i++)
        {
            if ((int)buffer[i] == pattern[0])
            {
                for (int i2 = 1; i2 < pattern.Length; i2++)
                {
                    if (pattern[i2] >= 0 && (int)buffer[i + i2] != pattern[i2])
                        break;
                    if (i2 == pattern.Length - 1)
                        return i;
                }
            }
        }
        return -1;
    }

    public MemoryScanResult SingleScan(int[] pattern)
    {
        SYSTEM_INFO sys_info = new SYSTEM_INFO();
        GetSystemInfo(out sys_info);

        IntPtr proc_min_address = sys_info.minimumApplicationAddress;
        IntPtr proc_max_address = sys_info.maximumApplicationAddress;

        long proc_min_address_l = (long)proc_min_address;
        long proc_max_address_l = (long)proc_max_address;

        IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_WM_READ, false, process.Id);

        MEMORY_BASIC_INFORMATION mem_basic_info = new MEMORY_BASIC_INFORMATION();

        int bytesRead = 0;

        while (proc_min_address_l < proc_max_address_l)
        {
            VirtualQueryEx(processHandle, proc_min_address, out mem_basic_info, 28);
            if (mem_basic_info.Protect == PAGE_READWRITE && mem_basic_info.State == MEM_COMMIT)
            {
                byte[] buffer = new byte[mem_basic_info.RegionSize];
                ReadProcessMemory((int)processHandle, mem_basic_info.BaseAddress, buffer, mem_basic_info.RegionSize, ref bytesRead);
              
                var result = SingleSignatureScan(pattern, buffer);
               
                if (result >= 0)
                    return new MemoryScanResult(mem_basic_info.BaseAddress, mem_basic_info.BaseAddress + result, result, buffer);   
            }

            proc_min_address_l += mem_basic_info.RegionSize;
            proc_min_address = new IntPtr(proc_min_address_l);
        }
        return null;
    }

    public MemoryScanResult[] Scan(int[] pattern)
    {
        SYSTEM_INFO sys_info = new SYSTEM_INFO();
        GetSystemInfo(out sys_info);

        IntPtr proc_min_address = sys_info.minimumApplicationAddress;
        IntPtr proc_max_address = sys_info.maximumApplicationAddress;

        long proc_min_address_l = (long)proc_min_address;
        long proc_max_address_l = (long)proc_max_address;

        IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_WM_READ, false, process.Id);

        MEMORY_BASIC_INFORMATION mem_basic_info = new MEMORY_BASIC_INFORMATION();

        int bytesRead = 0;

        var results = new List<MemoryScanResult>();
        while (proc_min_address_l < proc_max_address_l)
        {
            VirtualQueryEx(processHandle, proc_min_address, out mem_basic_info, 28);
            if (mem_basic_info.Protect == PAGE_READWRITE && mem_basic_info.State == MEM_COMMIT)
            {
                byte[] buffer = new byte[mem_basic_info.RegionSize];
                ReadProcessMemory((int)processHandle, mem_basic_info.BaseAddress, buffer, mem_basic_info.RegionSize, ref bytesRead);

                var result = SingleSignatureScan(pattern, buffer);

                if (result >= 0)
                    results.Add(new MemoryScanResult(mem_basic_info.BaseAddress, mem_basic_info.BaseAddress + result, result, buffer));
            }

            proc_min_address_l += mem_basic_info.RegionSize;
            proc_min_address = new IntPtr(proc_min_address_l);
        }
        return results.ToArray();
    }

    public class MemoryScanResult
    {
        public int BaseAddress;
        public int Address;
        public int offset;
        public byte[] buffer;

        public MemoryScanResult(int BaseAddress, int Address, int offset, byte[] buffer)
        {
            this.BaseAddress = BaseAddress;
            this.Address = Address;
            this.offset = offset;
            this.buffer = buffer;
        }
    }


}
