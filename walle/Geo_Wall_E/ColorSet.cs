namespace Geo_Wall_E
{
    public class ColorSet
    {
        public Stack<Color> colorStack = new Stack<Color>();
        public ColorSet()
        {
            colorStack.Push(Color.BLACK);
        }
        public Color PeekColorStack()
        {
            return colorStack.Peek();
        }
        public void PushColorStack(Color color)
        {
            colorStack.Push(color);
        }
        public Color PopColorStack()
        {
            return colorStack.Pop();
        }
        
    }
}