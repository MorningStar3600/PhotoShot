﻿using System;
using System.Drawing;
using System.IO;
using System.Security;
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

        public void Resize(double ratio)
        {
            Pixel[,] newPixels = new Pixel[(int)(Pixel.GetLength(0) * ratio), (int)(Pixel.GetLength(1) * ratio)];
            
            for (int i = 0; i < newPixels.GetLength(0); i++)
            {
                for (int j = 0; j < newPixels.GetLength(1); j++)
                {
                    var x = (int)(i / ratio);
                    var y = (int)(j / ratio);
                    newPixels[i, j] = Pixel[x, y];
                }
            }
            
            _pixels = newPixels;
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

        public ConsoleColor[] GetClosestColor(Pixel pixel)
        {
            ConsoleColor[] colors = new ConsoleColor[2];

            int minDistance = int.MaxValue;
            int secondMinDistance = int.MaxValue;
            
            for (int i = 0; i < 16; i++)
            {
                ConsoleColor color = (ConsoleColor)i;
                Color c = Color.FromName(color.ToString());
                
                int distance = Math.Abs(pixel.R - c.R) + Math.Abs(pixel.G - c.G) + Math.Abs(pixel.B - c.B);
                if (distance < minDistance)
                {
                    secondMinDistance = minDistance;
                    minDistance = distance;
                    colors[1] = colors[0];
                    colors[0] = color;
                }
                else if (distance < secondMinDistance)
                {
                    secondMinDistance = distance;
                    colors[1] = color;
                }
            }
            
            return colors;
        }
        public void Draw()
        {
            int newWidth = _pixels.GetLength(1);
            int newHeight = _pixels.GetLength(0)/2;

            double ratio = 1;
            
            if (_pixels.GetLength(0)> Console.WindowHeight|| _pixels.GetLength(1) > Console.WindowWidth)
            {
                double widthRatio = (double)_pixels.GetLength(1) / Console.WindowWidth;
                double heightRatio = (double)_pixels.GetLength(0) / (Console.WindowHeight);
                ratio = Math.Max(widthRatio, heightRatio);
                newWidth = (int)(_pixels.GetLength(1) / ratio);
                newHeight = (int)(_pixels.GetLength(0) / ratio);
            }
            


            for (int i = newHeight-1; i >=0 ; i--)
            {
                for (int j = 0; j < newWidth; j++)
                {
                    
                    ConsoleColor[] colors = GetClosestColor(_pixels[(int)(i*ratio), (int)(j*ratio)]);
                    Console.ForegroundColor = colors[1];
                    Console.BackgroundColor = colors[0];
                    Console.Write("O"); 
                    
                    
                    
                } 
                Console.WriteLine();
            }

            Console.ReadKey();
        }
        
        //Apply convolution matrix of size n to image
        public void ApplyConvolution(int[,] matrix)
        {
            if (matrix.GetLength(0) == matrix.GetLength(1))
            {
                Pixel[,] newPixels = new Pixel[Height, Width];
                int lostPixels = matrix.GetLength(0)/2;
                for (int i = lostPixels; i < _pixels.GetLength(0) - lostPixels; i++)
                {
                    for (int j = lostPixels; j < _pixels.GetLength(1) - lostPixels; j++)
                    {
                        int sumR = 0;
                        int sumG = 0;
                        int sumB = 0;
                        for (int k = 0; k < matrix.GetLength(0); k++)
                        {
                            for (int l = 0; l < matrix.GetLength(1); l++)
                            {
                                sumR += _pixels[i - lostPixels + k, j - lostPixels + l].R * matrix[k, l];
                                sumG += _pixels[i - lostPixels + k, j - lostPixels + l].G * matrix[k, l];
                                sumB += _pixels[i - lostPixels + k, j - lostPixels + l].B * matrix[k, l];
                            }
                        }
                        newPixels[i, j] = new Pixel(sumR / matrix.Length, sumB / matrix.Length, sumG / matrix.Length);
                    }
                }
                
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {

                        if (i < lostPixels || j < lostPixels || i >= Height - lostPixels || j >= Width - lostPixels)
                        {
                            newPixels[i, j] = new Pixel(0, 0, 0);
                        }
                    }
                }
                _pixels = newPixels;
            }
            
        }
        
        
        
        
    }
}