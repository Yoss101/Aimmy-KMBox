using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows;
using Visuality;

namespace MouseMovementLibraries.KMBox
{
    internal class KMBox
    {
        #region KMBox Variables

        [DllImport("KMBoxDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ConnectKMBOX();

        [DllImport("KMBoxDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void StartListening(int vkCode);

        [DllImport("KMBoxDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Move(int x, int y);

        [DllImport("KMBoxDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Up(int key);
        
        [DllImport("KMBoxDLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Down(int key);

        #endregion KMBox Variables

    }
}
