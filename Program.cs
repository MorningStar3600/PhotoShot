using System;
using System.Drawing;

namespace PhotoShot
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            BitmapImage v = new BitmapImage("coco","myedit");
            v.MirrorImage();
        }
        
        
        
    }
}