using System;
using System.Collections.Generic;
namespace FTGLdotNet.Atlas
{
    /// <summary>
    /// A texture atlas is used to pack several small regions into a single texture.
    /// </summary>
    public partial class Atlas
    {
        /// <summary>
        /// Allocated nodes
        /// </summary>
        private List<Node> nodes;
        
        /// <summary>
        /// Width (in pixels) of the underlying texture
        /// </summary>
        private int width;

        /// <summary>
        /// Height (in pixels) of the underlying texture
        /// </summary>
        private int height;

        /// <summary>
        /// Allocated surface size
        /// </summary>
        private int used;

        /// <summary>
        /// Depth (in bytes) of the underlying texture
        /// </summary>
        private int depth;

        /// <summary>
        /// Atlas data
        /// </summary>
        private byte[] data;

        //properties
        public List<Node> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        public int Depth
        {
            get { return depth; }
            set { depth = value; }
        }
        public int Used
        {
            get { return used; }
            set { used = value; }
        }
        public byte[] Data
        {
            get { return data; }
        }
        //member functions 

        /// <summary>
        ///  Creates a new empty texture atlas.
        /// </summary>
        /// <param name="width">width of the atlas</param>
        /// <param name="height"> height of the atlas</param>
        /// <param name="depth">bit depth of the atlas</param>
        public Atlas(int width, int height, int depth)
        {
            if (!(depth == 1 || depth == 3 || depth == 4)) throw new ArgumentException("Invaild depth value must be 1,3 or 4");
            this.width = width;
            this.height = height;
            this.depth = depth;
            // We want a one pixel border around the whole atlas to avoid any artefact when
            // sampling texture
            Node node = new Node(1, 1, width - 2);
            nodes = new List<Node>();
            nodes.Add(node);
            used = 0;
            data = new byte[width * height * depth];
        }
        /// <summary>
        /// Allocate a new region in the atlas.
        /// </summary>
        /// <param name="width">width of the region to allocate</param>
        /// <param name="height">height of the region to allocate</param>
        /// <returns>Coordinates of the allocated region</returns>
        public Region GetRegion(int regionwidth, int regionheight)
        {
            int y;
            int bestindex = -1;
            int bestwidth = int.MaxValue;
            int bestheght = int.MaxValue;
            Region region = new Region(0, 0, regionwidth, regionheight);
            for (int i = 0; i < nodes.Count; i++)
            {
                y = Fit(i, regionwidth, regionheight); 
                if (y >= 0)
                {
                    if (((nodes[i].Y + regionheight) < bestheght) || ((nodes[i].Y + regionheight) == bestheght) && (nodes[i].Z > 0 &&  nodes[i].Z < bestwidth))
                    {
                        bestheght = y + regionheight;
                        bestindex = i;
                        bestwidth = nodes[i].Z;
                        region.X = nodes[i].X;
                        region.Y = y;
                    }
                }
            }
            if (bestindex == -1)
            {
                return new Region(-1, -1, 0, 0);
            }
            nodes.Insert(bestindex, new Node(region.X, region.Y + height, width));
            for (int i = bestindex + 1; i < nodes.Count; i++)
            {
                Node node = nodes[i];
                Node prev = nodes[i - 1];
                if (node.X < (prev.X + prev.Z)) 
                {
                    int shrink = prev.X + prev.Z - node.X;
                    node.X += shrink;
                    node.Z -= shrink;
                    if (node.Z <= 0)
                    {
                        nodes.RemoveAt(i);
                        --i;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            Merge();
            used += regionwidth * regionheight;
            return region;
        }

        /// <summary>
        /// Upload data to the specified atlas region.
        /// </summary>
        /// <param name="atlas">a texture atlas structure</param>
        /// <param name="x">a texture atlas structure</param>
        /// <param name="y">y coordinate the region</param>
        /// <param name="width">width of the region</param>
        /// <param name="height">height of the region</param>
        /// <param name="data">data to be uploaded into the specified region</param>
        /// <param name="stride">stride of the data</param>
        
        public void SetRegion(int x, int y, int regionwidth, int regionheight, byte[] regiondata, int stride)
        {
            int destinationstartpoint = ((y + 0) * width + x) * depth;
            Array.Copy(regiondata, 0, this.data, destinationstartpoint, regiondata.Length);
        }

        /// <summary>
        /// Remove all allocated regions from the atlas.
        /// </summary>
        public void Clear()
        {
            Node node = new Node(1, 1, 1);
            nodes.Clear();
            used = 0;
            node.Z = width - 2;
            nodes.Add(node);
            data = new byte[width * height * depth];
        }

        /// <summary>
        /// private function used to fit the next glyph into the texture atlas during the get region function
        /// </summary>
        /// <param name="index">node index</param>
        /// <param name="fitwidth">width of the glyph to fit</param>
        /// <param name="fitheight">height of the glyph to fit</param>
        /// <returns>The y coord of the region in the texture atlas</returns>
        private int Fit(int index, int fitwidth, int fitheight)
        {
            int x = nodes[index].X;
            int y = nodes[index].Y;
            int widthleft = fitwidth;
            int i = index;
            // can the new width be fitted into the atlas total width? 
            if ((fitwidth + x) > (width - 1)) return -1;
            while (widthleft > 0)
            {
                Node anode = nodes[i];
                if (anode.Y > y) y = anode.Y;
                if ((y + fitheight) > (height - 1)) return -1;
                widthleft -= anode.Z;
                i++;
            }
            return y;
        }
        /// <summary>
        /// private function used to merge nodes during the get region function
        /// </summary>
        private void Merge()
        {
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                Node next = nodes[i + 1];
                if (nodes[i].Y == next.Y)
                {
                    Node node = nodes[i];
                    node.Z += next.Z;
                    nodes[i] = node;
                    nodes.RemoveAt(i + 1);
                    --i;
                }
            }
        }
    }
}