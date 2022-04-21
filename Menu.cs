using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using PhotoShot.AdvancedConsole;

namespace PhotoShot
{
    public class Menu
    {
        public List<MenuItem> MenuItems
        {
            get;
            set;
        }
        public string Title { get; set; }
        //create a new menu
        public Menu(string title)   //constructor
        {
            Title = title;
            MenuItems = new List<MenuItem>();
        }
        
        public void AddMenuItem(MenuItem menuItem)
        {
            //add a menu item
            MenuItems.Add(menuItem);
        }

        public void SelectImageMenu()
        {
            //select image from Images folders
            foreach (var file in Directory.GetFiles("../../Images/exemple/", "*.bmp"))
            {
                var x = file.Split('/');
                var y = x[x.Length-1].Split('.');
                AddMenuItem(new MenuItem(y[0], file));
            }
        }

        public int SelectedItem()
        {
            Console.Write(this);
            Console.WriteLine("Merci de saisir votre choix : ");
            var selecteImage = Int32.TryParse(Console.ReadLine(), out int itemSelected);
            if (!selecteImage || itemSelected > MenuItems.Count || itemSelected < 1)
            {
                Console.WriteLine("Please select a valid item");
                return SelectedItem();
            }
            Console.WriteLine("Réussi");
            return itemSelected;
        }

        public void SelectFunctionMenu(Image image)
        {
            AddMenuItem(new MenuItem("Resize","Resize",image.SelectResizeRation));
            AddMenuItem(new MenuItem("Noir et Blanc","WhiteAndBlack",image.BlackAndWhiteBurger));
            AddMenuItem(new MenuItem("Mirror","Mirror",image.Mirror));
            AddMenuItem(new MenuItem("Rotation","Rotation",image.SelectRotateSize));
            AddMenuItem(new MenuItem("Flou","Flou",image.flou));
            AddMenuItem(new MenuItem("Contraste","Contraste",image.Contraste));
            AddMenuItem(new MenuItem("Detection Contour","DetectionContour",image.DetectionContour));
            AddMenuItem(new MenuItem("Renforcement des bords","Renforcement des bords",image.RenforcementBord));
            AddMenuItem(new MenuItem("Smart Image","SmartImage",AI.SmartImage));
            AddMenuItem(new MenuItem("Quitté le program","Leave",image.Leave));
        }

        public override string ToString()
        {
            Console.WriteLine(Title);
            string result = "";
            for (int i = 1; i <= MenuItems.Count; i++)
            {
                result+=i+" - "+MenuItems[i-1].Name+"\n";
            }
            return result;
        }
    }
}