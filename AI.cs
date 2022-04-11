using System;
using System.Collections.Generic;
using System.Linq;
using PhotoShot.AdvancedConsole;

namespace PhotoShot
{
    public static class AI
    {
        private static string[] _determinants = new[] { "un", "une", "le", "la", "les", "des", "du", "de", "d", "a", "à"};
        static string[][] Decompose(string str)
        {
            List<string[]> rslt= new List<string[]>();
            List<string> words = str.Split(' ', '\'').ToList();

            foreach (string word in words)
            {
                foreach (string det in _determinants)
                {
                    if (word.ToLower() == det)
                    {
                        words.Remove(word);
                    }
                }
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

            return rslt.ToArray();
        }


        static string IsForm(string word)
        {
            string[][] forms = {new []{"circle", "cercle", "rond"}, new []{"square", "carre", "carré"}, new []{"triangle"}};
            
            foreach (string[] form in forms)
            {
                for (int i = 0; i < form.Length; i++)
                {
                    if (word.ToLower() == form[i])
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

        static void SmartImage()
        {
            Console.WriteLine("Donnez un nom a votre image");
            string name = Console.ReadLine();
            Console.WriteLine("Donnez une taille a votre image : 'largeur * hauteur'");
            string size = Console.ReadLine();
            string[] sizes = size.Split(' ');
            int width = int.Parse(sizes[0]);
            int height = int.Parse(sizes[sizes.Length-1]);
            
            Console.WriteLine("Donnez une description de votre image. Vous pouvez dire : 'Un cercle bleu et blanc et un triangle rouge'");
            string description = Console.ReadLine();
            
            string[][] decomposition = Decompose(description);

            Image img = new Image
            {
                Pixel = new Pixel[height,width]
            };
        }

        static void ApplyForm(string[] formProperties, Image img)
        {
            if (formProperties.Length == 0)
            {
                return;
            }
            
            string form = formProperties[0];
            string Color = formProperties[1];
            
            HashSet<string >

            if (form == "square")
            {
                
            }
        }
    }
}