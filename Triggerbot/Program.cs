using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Triggerbot
{
    class Program        
    {
        public static int oLocalPlayer = 0x00A323E4;
        public static int oEntityList = 0x04A4FCA4;
        public static int oCrosshair = 0x0000AA44;
        public static int oTeam = 0x0F0;
        public static int oHealth = 0x000000FC;
        public static int oAttack = 0x02E8FCDC;
        public static int oEntityListLoopDis = 0x10;
        public static int xpos = 0x0134;
        public static int yois = 0x0138;
        public static int zpos = 0x013c;

        public static string process = "csgo";
        public static int baseClient;
        static void Main(string[] args)
        {
            VAMemory vam = new VAMemory(process);


            if (GetModuleAddy())
            {
                int fAttack = baseClient + oAttack;

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Nothing...", Console.ForegroundColor = ConsoleColor.Red);

                    int address = baseClient + oLocalPlayer;
                    int LocalPlayer = vam.ReadInt32((IntPtr)address);

                    address = LocalPlayer + oTeam;
                    int MyTeam = vam.ReadInt32((IntPtr)address);

                    address = LocalPlayer + oCrosshair;
                    int PlayerInCrosshair = vam.ReadInt32((IntPtr)address);

                    //Console.ReadKey();

                    if (PlayerInCrosshair > 0 && PlayerInCrosshair < 65)
                    {
                        address = baseClient + oEntityList + (PlayerInCrosshair - 1) * oEntityListLoopDis;
                        int PtrToPic = vam.ReadInt32((IntPtr)address);

                        address = PtrToPic + oHealth;
                        int PicHealth = vam.ReadInt32((IntPtr)address);

                        address = PtrToPic + oTeam;
                        int PICTeam = vam.ReadInt32((IntPtr)address);
                        Console.WriteLine(vam.ReadInt32((IntPtr)address));

                        if ((PICTeam != MyTeam) && (PicHealth > 0))
                        {
                            Console.Clear();
                            Console.Write("Shooting!", Console.ForegroundColor = ConsoleColor.Green);
                            vam.WriteInt32((IntPtr)fAttack, 1);
                            Thread.Sleep(10);
                            vam.WriteInt32((IntPtr)fAttack, 4);
                        }
                        Thread.Sleep(10);
                    }   
                }
            }

        }
        static bool GetModuleAddy()
        {
            try
            {
                Process[] p = Process.GetProcessesByName(process);

                if (p.Length > 0)
                {
                    foreach (ProcessModule m in p[0].Modules) // kikker igennem alle filer under csgo (process)
                    {
                        if (m.ModuleName == "client.dll")
                        {
                            baseClient = (int)m.BaseAddress; // vi får addressen af client.dll, som er vores base address.
                            return true;
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
