﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace PhotoShot.AdvancedConsole
{
    class FastConsole
    {
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutputW(
            SafeFileHandle hConsoleOutput,
            CharInfo[] lpBuffer,
            Coord dwBufferSize,
            Coord dwBufferCoord,
            ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct CharUnion
        {
            [FieldOffset(0)] public ushort UnicodeChar;
            [FieldOffset(0)] public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {
            [FieldOffset(0)] public CharUnion Char;
            [FieldOffset(2)] public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }


        [STAThread]
        public static void Write(CharInfo[] buf, int x, int y, int width, int height)
        {

            SafeFileHandle h = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);


            if (!h.IsInvalid)
            {


                SmallRect rect = new SmallRect()
                    { Left = (short)x, Top = (short)y, Right = (short)(x + width), Bottom = (short)(y + height) };

                //Random r = new Random();
                /*for (int i = 0; i < buf.Length; ++i)
                {
                    buf[i].Attributes = (short)(r.Next(0,15) | (r.Next(0,15) << 4));
                    buf[i].Char.UnicodeChar = r.Next(0,9).ToString().ToCharArray()[0];
                }*/

                WriteConsoleOutputW(h, buf,
                    new Coord() { X = (short)width, Y = (short)height },
                    new Coord() { X = (short)x, Y = (short)y },
                    ref rect);



            }

            h.Close();
        }
    }
}