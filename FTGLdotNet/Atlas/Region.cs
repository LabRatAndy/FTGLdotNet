namespace FTGLdotNet.Atlas
{
    public struct Region
    {
        int x;
        int y;
        int width;
        int height;
        public Region(int X, int Y, int Width, int Height)
        {
            x = X;
            y = Y;
            width = Width;
            height = Height;
        }

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }
        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
    }
}