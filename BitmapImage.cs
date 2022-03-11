using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace PhotoShot
{
    public class BitmapImage
    {
        public string Source { get; set; }
        public string DestininationName { get; set; }
        public Bitmap BitmapImageBySource { get; set; }
        
        public BitmapImage(string s, string dn)
        {
            Source = s;
            DestininationName = dn;
            LoadImage();
        }

        public void LoadImage()
        {
            BitmapImageBySource = new Bitmap("../../Images/" + Source + ".bmp");
        }

        public void SaveImage()
        {
            BitmapImageBySource.Save("../../Images/Edit/"+DestininationName+".bmp");
        }
        
        public BitmapImage MirrorImage()
        {
            var result = new BitmapImage(Source, DestininationName);
            Debug.WriteLine(BitmapImageBySource.Height);
            Debug.WriteLine(BitmapImageBySource.Width);
            for (var i = 0; i < BitmapImageBySource.Height; i++)
            {
                for (var j = 0; j < BitmapImageBySource.Width; j++)
                {
                    result.BitmapImageBySource.SetPixel(BitmapImageBySource.Width-1-j,i,BitmapImageBySource.GetPixel(j,i));
                }
            }
            result.SaveImage();
            return result;
        }
        
    }
}