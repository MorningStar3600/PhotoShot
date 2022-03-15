using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using PhotoShot.AdvancedConsole;
namespace PhotoShot.AdvancedConsole
{
    public class Image
    {
        private string _type;
        private int _fileSize;
        private int _offset;
        private int _nbrBitPerColor;
        
        Pixel[,] _pixels;

        public int Width
        {
            get => _pixels.GetLength(1);
        }
        
        public int Height
        {
            get => _pixels.GetLength(0);
        }

        public Image(string fileUrl)
        {
            _type = "";
            _fileSize = 0;
            _offset = 0;
            _nbrBitPerColor = 0;
            _pixels = new Pixel[0,0];
            Load(fileUrl);
        }
        
        public void Save(string fileUrl)
        {
            using (var file = new FileStream(fileUrl, FileMode.Create))
            {
                file.Write(BitConverter.GetBytes(Width), 0, 4);
                file.Write(BitConverter.GetBytes(Height), 0, 4);
                file.Write(BitConverter.GetBytes(_nbrBitPerColor), 0, 4);
                file.Write(BitConverter.GetBytes(_offset), 0, 4);
                //write string to file
                var typeBytes = Encoding.ASCII.GetBytes(_type);
                file.Write(BitConverter.GetBytes((int)typeBytes.Length), 0, 4);
                file.Write(typeBytes, 0, typeBytes.Length);
                file.Write(BitConverter.GetBytes(_fileSize), 0, 4);
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        file.Write(BitConverter.GetBytes(_pixels[i, j].R), 0, 1);
                        file.Write(BitConverter.GetBytes(_pixels[i, j].G), 0, 1);
                        file.Write(BitConverter.GetBytes(_pixels[i, j].B), 0, 1);
                    }
                }
            }
        }
        
        public void Load(string fileUrl)
        {
            if (File.Exists(fileUrl))
            {
                using (FileStream fs = new FileStream(fileUrl, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        _type = Encoding.ASCII.GetString(br.ReadBytes(2));
                        _fileSize = br.ReadInt32();
                        br.ReadBytes(4);
                        _offset = br.ReadInt32();

                        br.ReadBytes(4);
                        int width = br.ReadInt32();
                        int height = br.ReadInt32();
                        _pixels = new Pixel[height, width];
                        br.ReadBytes(2);
                        _nbrBitPerColor = br.ReadInt16();
                        //br.ReadBytes(24);
                        
                        /*for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                _pixels[i, j] = new Pixel(br.ReadByte(), br.ReadByte(), br.ReadByte());
                                //br.ReadBytes(_nbrBitPerColor-3);
                            }
                        }*/


                    }
                }
            }
        }

        public int EndianToInt(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }
        
        public byte[] IntToEndian(int value)
        {
            return BitConverter.GetBytes(value);
        }
        
        //tostring
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Type : " + _type);
            sb.AppendLine("File size : " + _fileSize);
            sb.AppendLine("Offset : " + _offset);
            sb.AppendLine("Nbr bit per color : " + _nbrBitPerColor);
            sb.AppendLine("Width : " + Width);
            sb.AppendLine("Height : " + Height);
            sb.AppendLine("Pixels : ");
            /*for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    sb.AppendLine(_pixels[i, j].ToString());
                }
            }*/
            return sb.ToString();
        }

        /*public Image(BitmapImage img)
        {
            int width = img.BitmapImageBySource.Width;
            int height = img.BitmapImageBySource.Height;
            
            pixels = new Pixel[height/4, width/4];
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    pixels[i/4, j/4] = new Pixel(img.BitmapImageBySource.GetPixel(j, i));
                }
            }
        }

        public void Draw()
        {
            int y = 0;
            for (int i = 0; i < pixels.GetLength(0); i++)
            {
                for (int j = 0; j < pixels.GetLength(1); j++)
                {
                    ColorHandler.SetScreenColors(pixels[i, j].Color, pixels[i, j].Color);
                    Console.Write(' ');
                    Thread.Sleep(1);
                }

                y++;
                Console.SetCursorPosition(0, y);
            }

            Console.ReadKey();


            ColorHandler.SetColor(ConsoleColor.Black, Color.Black);
            
        }*/
        
        
    }
}