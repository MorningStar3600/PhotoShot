using System;

namespace PhotoShot
{
    public class MenuItem
    {

        public string Name { get; set; }
        public string Content { get; set; }
        public Action Function { get; set; }
        
        //create menu item with name
        public MenuItem(string name)
        {
            Name = name;
        }
        //create menu item from image
        public MenuItem(string name, string content, Action function)
        {
            this.Name = name;
            this.Content = content;
            this.Function = function;
        }
        //create menu item from text
        public MenuItem(string name, string Content)
        {
            this.Name = name;
            this.Content = Content;
        }

        public void exe()
        {
            this.Function();
        }
        
    }
}