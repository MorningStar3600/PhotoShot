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

            //ConsoleManager.SetCurrentFont("Consolas", 5);
            //ConsoleManager.SetFullScreen();

            //test();
            //BitmapImage v = new BitmapImage("coco","jjj");

            //v.WhiteAndBlackImage();

            Image img = new Image("../../Images/images/coco.bmp");
            
            int[,] matrix = new int[,] 
                {
                    {1,1,1},
                    {1,1,1},
                    {1,1,1}
                };
            img.ApplyConvolution(matrix);
            
            img.Rotate(-90);
            
            //img.Resize(2);
            
            //img.Resize((double)1/10);
            //img.Save("../../Images/images/1234.bmp");
            
            //img.Draw();
            //img.BlackAndWhiteBurger();
            
            img.Save("../../Images/images/12345.bmp");
            
            //img.Draw();
            
            //Console.ReadKey();
        }

        public static void test()
        {
            Color screenTextColor = Color.Orange;
            Color screenBackgroundColor = Color.FromArgb(0,100,100,100);
            int irc = ColorHandler.SetScreenColors(screenTextColor, screenBackgroundColor);
            //// these are relative to the buffer, not the screen:
            //Debug.WriteLine("WindowTop=" + Console.WindowTop + " WindowLeft=" + Console.WindowLeft);
            Console.WriteLine("Some text in a console window");
            Console.WriteLine("Nice !");
            screenBackgroundColor = Color.FromArgb(0,200,100,100);
            ColorHandler.SetScreenColors(screenTextColor, screenBackgroundColor);
            Console.WriteLine("Some text in a console window");
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Press ENTER to exit...");
            Console.ReadLine();
        }
    }
}