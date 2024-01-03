namespace Geo_Wall_E
{
    public interface IDrawable
    {
        public Type DrawableType { get; }
        public Color DrawableColor { get; }
        public string DrawableStr { get; }

    }
    public class Drawable : IDrawable
    {
        private Color Color;
        private string Name;
        private Type Type;
        
        public Type DrawableType => Type;

        public Color DrawableColor => Color;

        public string DrawableStr => Name;

        public Drawable(Type Type, Color Color, string Name)
        {
            this.Type = Type;
            this.Color = Color;
            this.Name = Name;
        }

        
    } 
}