using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PhotoShot.AdvancedConsole;
using Image = PhotoShot.AdvancedConsole.Image;

namespace PhotoShot
{
    public static class AI
    {
        private static readonly string[] Determinants = new[] { "un", "une", "le", "la", "les", "des", "du", "de", "d", "a", "à", "en"};
        static string[][] Decompose(string str)
        {
            List<string[]> rslt= new List<string[]>();
            List<string> words = str.Split(' ', '\'').ToList();
            
            List<string> toRmve = new List<string>();

            foreach (string word in words)
            {
                foreach (string det in Determinants)
                {
                    if (word.ToLower() == det)
                    {
                        Console.WriteLine("Determinant !");
                        toRmve.Add(word);
                    }
                }
            }
            
            foreach (var toRmv in toRmve)
            {
                words.Remove(toRmv);
            }
            

            List<string> actual = new List<string>();
            for (int i = 0; i < words.Count; i++)
            {

                if (words[i].ToLower() == "et" && i < words.Count - 1)
                {
                    if (IsForm(words[i + 1]) != null)
                    {
                        rslt.Add(actual.ToArray());
                        actual.Clear();
                    }
                }
                
                if (IsForm(words[i]) != null)
                {
                    actual.Add(IsForm(words[i]));
                }
                else if (IsColor(words[i]))
                {
                    actual.Add(words[i]);
                }
            }

            if (actual.Count > 0) rslt.Add(actual.ToArray());
            
            Console.WriteLine(":::"+rslt.Count);

            return rslt.ToArray();
        }


        static string IsForm(string word)
        {
            string[][] forms = {new []{"circle", "cercle", "rond"}, new []{"square", "carre", "carré"}, new []{"triangle"}};
            
            foreach (string[] form in forms)
            {
                foreach (var t in form)
                {
                    if (word.ToLower() == t)
                    {
                        return form[0];
                    }
                }
            }

            return null;
        }

        static bool IsColor(string word)
        {
            string[] colors = new []{ "bleu", "rouge", "vert", "jaune", "noir", "blanc", "gris"};
            foreach (string color in colors)
            {
                if (word.ToLower() == color)
                {
                    return true;
                }
            }

            return false;
        }

        public static Image SmartImage()
        {
            Console.WriteLine("Donnez une taille a votre image : 'largeur * hauteur'");
            string size = Console.ReadLine();
            string[] sizes = size.Split(' ','*');
            int width = int.Parse(sizes[0]);
            int height = int.Parse(sizes[sizes.Length-1]);
            Console.WriteLine("Donnez une description de votre image. Vous pouvez dire : 'Un cercle bleu et blanc et un triangle rouge'");
            string description = Console.ReadLine();
            
            string[][] decomposition = Decompose(description);

            Image img = new Image
            {
                Pixel = new Pixel[height,width],
                Offset = 54,
                SizeHeader = 40,
                NbrBitPerColor = 24,
                FileSize = width * height * 24 + 54,
            };
            
            for (int i = 0; i < img.Pixel.GetLength(0); i++)
            {
                for (int j = 0; j < img.Pixel.GetLength(1); j++)
                {
                    if (img.Pixel[i, j] == null)
                    {
                        img.Pixel[i, j] = new Pixel(0, 0, 0);
                    }
                }
            }

            if (decomposition.Length > 0)
            {
                foreach(var decop in decomposition) ApplyForm(decop, img);
            }

            return img;
        }

        static void ApplyForm(string[] formProperties, Image img)
        {
            if (formProperties.Length == 0)
            {
                return;
            }
            
            

            string form = formProperties[0];
            string color = "white";
            if (formProperties.Length > 1) color = formProperties[1];
            string insideColor = "noir";
            if (formProperties.Length > 2)
            {
                insideColor = formProperties[2];
            }
            Hashtable colors = new Hashtable
            {
                {"bleu", Color.Blue},
                {"rouge", Color.Red},
                {"vert", Color.Green},
                {"jaune", Color.Yellow},
                {"noir", Color.Black},
                {"blanc", Color.White},
                {"gris", Color.Gray}
            };

            if (form == "square")
            {
                int thickness = img.Width/20;
                for (int i =  0; i < img.Pixel.GetLength(0); i++)
                {
                    for (int j = 0; j < img.Pixel.GetLength(1); j++)
                    {
                        if (i < thickness || i > img.Pixel.GetLength(0) - 1 - thickness || j < thickness || j > img.Pixel.GetLength(1) - 1-thickness)
                        {
                            img.Pixel[i, j] = new Pixel(((Color)colors[color]).B, ((Color)colors[color]).G, ((Color)colors[color]).R);
                        }
                        else
                        {
                            img.Pixel[i, j] = new Pixel(((Color)colors[insideColor]).B, ((Color)colors[insideColor]).G, ((Color)colors[insideColor]).R);;
                        }
                    }
                }
            } else if (form == "circle")
            {
                int radius = img.Pixel.GetLength(0) / 2 > img.Pixel.GetLength(1) / 2 ? img.Pixel.GetLength(1) / 2 : img.Pixel.GetLength(0) / 2;
                int centerY = img.Pixel.GetLength(0) / 2;
                int centerX = img.Pixel.GetLength(1) / 2;
                
                int thickness = img.Width/20;
                for (int i = 0; i < img.Pixel.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j < img.Pixel.GetLength(1) - 1; j++)
                    {
                        if (Math.Sqrt(Math.Pow(i - centerY, 2) + Math.Pow(j - centerX, 2)) < radius - thickness)
                        {
                            img.Pixel[i, j] = new Pixel(((Color)colors[color]).B, ((Color)colors[color]).G, ((Color)colors[color]).R);
                        }
                        else if (Math.Sqrt(Math.Pow(i - centerY, 2) + Math.Pow(j - centerX, 2)) < radius)
                        {
                            img.Pixel[i, j] = new Pixel(((Color)colors[insideColor]).B, ((Color)colors[insideColor]).G, ((Color)colors[insideColor]).R);;
                        }
                    }
                }
            } else if (form == "triangle")
            {
                int thickness = img.Width / 20;
                
                int width = img.Pixel.GetLength(1);
                
                double deltaX = (double)width/2/img.Pixel.GetLength(0);

                for (int i = 0; i < img.Pixel.GetLength(0); i++)
                {
                    for (int j = 0; j < img.Pixel.GetLength(1); j++)
                    {
                        //si le pixel est dans le triangle
                        if (j > width/2 - i*deltaX+thickness && j < width/2 + i*deltaX-thickness)
                        {
                            img.Pixel[i, j] = new Pixel(((Color)colors[insideColor]).B, ((Color)colors[insideColor]).G, ((Color)colors[insideColor]).R);
                        }
                        else if (j > width/2 - i*deltaX && j < width/2 + i*deltaX)
                        {
                            img.Pixel[i, j] = new Pixel(((Color)colors[color]).B, ((Color)colors[color]).G, ((Color)colors[color]).R);;
                        }
                    }
                }
            }
        }
    }
}