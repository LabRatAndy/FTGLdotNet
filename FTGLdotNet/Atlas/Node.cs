namespace FTGLdotNet.Atlas
{
    public  struct Node
    {
        int x;
        int y;
        int z;
        public Node(int X, int Y, int Z)
        {
            x = X;
            y = Y;
            z = Z;
        }
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        public int Z
        {
            get { return z; }
            set { z = value; }
        }
    }
}