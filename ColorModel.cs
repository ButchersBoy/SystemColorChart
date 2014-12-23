using System.Windows.Media;

namespace SystemColorChart
{
    public class ColorModel
    {
        private readonly string _classification;
        private readonly string _name;
        private readonly Color _color;

        public ColorModel(string classification, string name, Color color)
        {
            _classification = classification;
            _name = name;
            _color = color;
        }        

        public string Name
        {
            get { return _name; }
        }

        public Color Color
        {
            get { return _color; }
        }

        public string Classification
        {
            get { return _classification; }
        }
    }
}