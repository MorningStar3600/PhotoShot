using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;

namespace PhotoShot.AdvancedConsole
{
    public class Image
    {
        private string _type;
        private int _fileSize;
        private int _offset;
        private int _nbrBitPerColor;
        private int _resolutionX;
        private int _resolutionY;
        private int _sizeheader;
        
        Pixel[,] _pixels;

        public int Width
        {
            get => _pixels.GetLength(1);
        }
        
        public int Height
        {
            get => _pixels.GetLength(0);
        }
        
        public Pixel[,] Pixel
        {
            get => _pixels;
            set => _pixels = value;
        }

        public Image()
        {
            _type = "";
            _fileSize = 0;
            _offset = 0;
            _nbrBitPerColor = 0;
            _pixels = new Pixel[0,0];
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
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileUrl, FileMode.Create)))
            {
                //convert string to bytes
                byte[] type = Encoding.ASCII.GetBytes(_type);
                writer.Write(type);
                writer.Write(IntToEndian(_fileSize, 4));
                writer.Write(new byte[4]);
                writer.Write(IntToEndian(_offset, 4));
                writer.Write(IntToEndian(_sizeheader, 4));
                writer.Write(IntToEndian(Width, 4));
                writer.Write(IntToEndian(Height, 4));
                writer.Write(new byte[2]);
                writer.Write(IntToEndian(_nbrBitPerColor, 2));
                writer.Write(new byte[8]);
                writer.Write(new byte[8]);
                //writer.Write(IntToEndian(_resolutionX, 4));
                //writer.Write(IntToEndian(_resolutionY, 4));
                writer.Write(new byte[_offset-38]);
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        writer.Write(IntToEndian(_pixels[i, j].R, _nbrBitPerColor/24));
                        writer.Write(IntToEndian(_pixels[i, j].G, _nbrBitPerColor/24));
                        writer.Write(IntToEndian(_pixels[i, j].B, _nbrBitPerColor/24));
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
                        _fileSize = EndianToInt(br.ReadBytes(4));
                        br.ReadBytes(4);
                        _offset = EndianToInt(br.ReadBytes(4));
                        _sizeheader = EndianToInt(br.ReadBytes(4));
                        int width = EndianToInt(br.ReadBytes(4));
                        int height = EndianToInt(br.ReadBytes(4));
                        _pixels = new Pixel[height, width];
                        br.ReadBytes(2);
                        _nbrBitPerColor = EndianToInt(br.ReadBytes(2));
                        br.ReadBytes(8);
                        br.ReadBytes(8);
                        //_resolutionX = EndianToInt(br.ReadBytes(4));
                        //_resolutionY = EndianToInt(br.ReadBytes(4));
                        br.ReadBytes(_offset-38);
                        
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                _pixels[i, j] = new Pixel(EndianToInt(br.ReadBytes(1)), EndianToInt(br.ReadBytes(1)),EndianToInt(br.ReadBytes(1)));
                                //br.ReadBytes(_nbrBitPerColor-3);
                            }
                        }


                    }
                }
            }
        }

        public static int EndianToInt(byte[] bytes)
        {
            var value = 0;
            for (int i = 0, j = 1; i < bytes.Length; i++, j *=256)
            {
                value += bytes[i] * j;
            }
            return value;
        }
        
        public static byte[] IntToEndian(int value, int nbrOfBytes)
        {
            //convert to little endian 
            byte[] bytes = new byte[nbrOfBytes];
            for (int i = 0; i < nbrOfBytes; i++)
            {
                bytes[i] = (byte)(value & 0xff);
                value >>= 8;
            }
            return bytes;
        }

        public void BlackAndWhiteBurger()
        {
            for (int i = 0; i < Pixel.GetLength(0); i++)
            {
                for (int j = 0; j < Pixel.GetLength(1); j++)
                {
                    var color = (Pixel[i, j].G + Pixel[i, j].B + Pixel[i, j].R) / 3;
                    Pixel[i, j].G = color;
                    Pixel[i, j].B = color;
                    Pixel[i, j].R = color;
                }
            }
        }

        public void Mirror()
        {
            var x = (Pixel[,]) _pixels.Clone();
            for (int i = 0; i < Pixel.GetLength(0); i++)
            {
                for (int j = 0; j < Pixel.GetLength(1); j++)
                {
                    var y = x[Pixel.GetLength(0) - i - 1,j];
                    Pixel[i, j] = y;
                }
            }
        }

        public Image ResizeDown(float top, float bottom)
        {
            var x = new Image();
            x._offset = _offset;
            x._sizeheader = _sizeheader;
            x._type = _type; 
            x._fileSize = _fileSize;
            x._nbrBitPerColor = _nbrBitPerColor;
            if (top == 0 || bottom == 0) { return x; }
            var a = Convert.ToInt32(Pixel.GetLength(0) * (top / bottom));
            var b = Convert.ToInt32(Pixel.GetLength(1) * (top / bottom));
            x.Pixel = new Pixel[a,b];
            for (int i = 0; i < x.Pixel.GetLength(0); i++)
            {
                for (int j = 0; j < x.Pixel.GetLength(1); j++)
                {
                    x.Pixel[i, j] = new Pixel(0,0,0);
                }
            }
            Console.Write(x);
            for (int i = 0; i < x.Pixel.GetLength(0); i++)
            {
                for (int j = 0; j < x.Pixel.GetLength(1); j++)
                {
                    x.Pixel[i, j].R = (Pixel[i, j].R+Pixel[i+1,j+1].R)/3;
                    x.Pixel[i, j].G = (Pixel[i, j].G+Pixel[i+1,j+1].G)/3;
                    x.Pixel[i, j].B = (Pixel[i, j].B+Pixel[i+1,j+1].B)/3;
                }
            }

            return x;
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
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (_pixels[i, j] != null)
                    {
                        sb.Append(_pixels[i, j].ToString());
                    }
                    
                }
                sb.AppendLine();
            }
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
        }*/

        public void Draw()
        {
            int y = 0;
            for (int i = 0; i < _pixels.GetLength(0); i++)
            {
                for (int j = 0; j < _pixels.GetLength(1); j++)
                {
                    ColorHandler.SetColor(ConsoleColor.Black, (uint)_pixels[i, j].R, (uint)_pixels[i, j].G,
                        (uint)_pixels[i, j].B);
                    Console.Write(' ');
                    Thread.Sleep(10);
                }

                y++;
                Console.SetCursorPosition(0, y);
            }
            
            ColorHandler.SetColor(ConsoleColor.Black, 0,0,0);

            Console.ReadKey();


            ColorHandler.SetColor(ConsoleColor.Black, Color.Black);
            
        }
        
        
    }
}