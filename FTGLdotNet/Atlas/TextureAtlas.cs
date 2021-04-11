using System;
namespace FTGLdotNet.Atlas
{
    public partial class Atlas
    { 
        /// <summary>
        ///  Creates a new empty texture atlas.
        /// </summary>
        /// <param name="width">width of the atlas</param>
        /// <param name="height"> height of the atlas</param>
        /// <param name="depth">bit depth of the atlas</param>
        /// <returns> a new empty texture atlas.</returns>
        public static Texture_Atlas_T NewAtlas(int width, int height, int depth)
        {
            Texture_Atlas_T atlas = new Texture_Atlas_T();
            // We want a one pixel border around the whole atlas to avoid any artefact when
            // sampling texture
            Node node = new Node(1, 1, width - 2);
            if (!(depth == 1 || depth == 3 || depth == 4)) throw new ArgumentException("Invaild depth value must be 1,3 or 4");
            atlas.Nodes = new System.Collections.Generic.List<Node>();
            atlas.Nodes.Add(node);
            atlas.Width = width;
            atlas.Height = height;
            atlas.Depth = depth;
            atlas.Used = 0;
            atlas.Id = 0;
            int arraysize =  width * height * depth;
            atlas.Data = new byte[arraysize];
            return atlas;
        }

        /// <summary>
        /// Allocate a new region in the atlas.
        /// </summary>
        /// <param name="atlas">a texture atlas structure</param>
        /// <param name="width">width of the region to allocate</param>
        /// <param name="height">height of the region to allocate</param>
        /// <returns>Coordinates of the allocated region</returns>
        public static Region GetRegion(ref Texture_Atlas_T atlas, int width, int height)
        {
            int y;
            int bestindex = -1;
            int bestwidth = int.MaxValue;
            int bestheght = int.MaxValue;
            Node node;
            Node prev;
            Region region = new Region(0, 0, width, height);
            for (int i = 0; i < atlas.Nodes.Count; i++)
            {
                y = Fit(ref atlas, i, width, height);
                if (y >= 0)
                {
                    node = atlas.Nodes[i];
                    if (((node.Y + height) < bestheght) || ((node.Y + height) == bestheght) && (node.Z > 0 && node.Z < bestwidth))
                    {
                        bestheght = y + height;
                        bestindex = i;
                        bestwidth = node.Z;
                        region.X = node.X;
                        region.Y = y;
                    }
                }
            }
            if(bestindex ==-1)
            {
                return new Region(-1, -1, 0, 0);
            }
            atlas.Nodes.Insert(bestindex, new Node(region.X, region.Y + height, width));
            for (int i = bestindex + 1; i < atlas.Nodes.Count; i++)
            {
                node = atlas.Nodes[i];
                prev = atlas.Nodes[i - 1];
                if(node.X < (prev.X +prev.Z))
                {
                    int shrink = prev.X + prev.Z - node.X;
                    node.X += shrink;
                    node.Z -= shrink;
                    if (node.Z <= 0)
                    {
                        atlas.Nodes.RemoveAt(i);
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
            Merge(ref atlas);
            atlas.Used += width * height;
            return region;
        }

        public static int Fit(ref Texture_Atlas_T atlas, int index, int width, int height)
        {
            Node node = atlas.Nodes[index];
            int x = node.X;
            int y = node.Y;
            int widthleft = width;
            int i = index;
            // can the new width be fitted into the atlas total width? 
            if ((width + x) > (atlas.Width - 1)) return -1;
            while (widthleft > 0)
            {
                Node anode = atlas.Nodes[i];
                if (node.Y > y) y = node.Y;
                if ((y + height) > (atlas.Height - 1)) return -1;
                widthleft -= node.Z;
                i++;
            }
            return y;
        }

        public static void Merge(ref Texture_Atlas_T atlas)
        {
            for (int i = 0; i < atlas.Nodes.Count - 1; i++) 
            {
                Node node = atlas.Nodes[i];
                Node next = atlas.Nodes[i + 1];
                if(node.Y == next.Y)
                {
                    node.Z += next.Z;
                    atlas.Nodes.RemoveAt(i + 1);
                    --i;
                }
            }
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
        public static void SetRegion(ref Texture_Atlas_T atlas, int x, int y, int width, int height, byte[] data, int stride)
        {
            int destinationstartpoint = ((y + 0) * atlas.Width + x) * atlas.Depth;
            Array.Copy(data, 0, atlas.Data, destinationstartpoint, data.Length);
        }
        /// <summary>
        /// Remove all allocated regions from the atlas.
        /// </summary>
        /// <param name="atlas">a texture atlas structure</param>
        public static void Clear(ref Texture_Atlas_T atlas)
        {
            Node node = new Node(1, 1, 1);
            atlas.Nodes.Clear();
            atlas.Used = 0;
            node.Z = atlas.Width - 2;
            atlas.Nodes.Add(node);
            atlas.Data = new byte[atlas.Width * atlas.Height * atlas.Depth];
        }
    }

}
