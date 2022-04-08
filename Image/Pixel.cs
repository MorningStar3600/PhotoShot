using System.Drawing;

namespace PhotoShot.AdvancedConsole
{
    public class Pixel
    {
        private int _r;
        private int _g;
        private int _b;
        
        public int R
        {
            get { return _r; }
            set { _r = value; }
        }
        
        public int G
        {
            get { return _g; }
            set { _g = value; }
        }
        
        public int B
        {
            get { return _b; }
            set { _b = value; }
        }
        
        
        public Pixel(int b, int r, int g)
        {
            _r = r;
            _g = g;
            _b = b;
        }
        
        //override tostring
        public override string ToString()
        {
            return "R: " + _r + " G: " + _g + " B: " + _b;
        }
    }
}