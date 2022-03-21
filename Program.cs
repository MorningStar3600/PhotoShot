using System;
using System.Drawing;
using System.Threading;
using PhotoShot.AdvancedConsole;
using Image = PhotoShot.AdvancedConsole.Image;

namespace PhotoShot
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            
            ConsoleManager.SetFullScreen();
            BitmapImage v = new BitmapImage("coco","jjj");

            v.WhiteAndBlackImage();

            Image img = new Image("../../Images/images/coco.bmp");
            
            var x = img.ResizeDown(1,2);
            x.Save("../../Images/images/1234.bmp");
            //img.BlackAndWhiteBurger();
            
            //img.Save("../../Images/images/1234.bmp");
            
            //img.Draw();
            
            Console.ReadKey();
        }
    }
}