using System.Collections.Generic;
namespace FTGLdotNet.Atlas
{
    /// <summary>
    /// A texture atlas is used to pack several small regions into a single texture.
    /// </summary>
    public struct Texture_Atlas_T
    {
        /// <summary>
        /// Allocated nodes
        /// </summary>
        List<Node> nodes;

        /// <summary>
        /// Width (in pixels) of the underlying texture
        /// </summary>
        int width;

        /// <summary>
        /// Height (in pixels) of the underlying texture
        /// </summary>
        int height;

        /// <summary>
        /// Depth (in bytes) of the underlying texture
        /// </summary>
        int depth;

        /// <summary>
        /// Allocated surface size
        /// </summary>
        int used;

        /// <summary>
        /// Texture identity (OpenGL)
        /// </summary>
        uint id;

        /// <summary>
        /// Atlas data
        /// </summary>
        byte[] data;

        public int Width { get => width; set => width = value; }
        public List<Node> Nodes { get => nodes; set => nodes = value; }
        public int Height { get => height; set => height = value; }
        public int Depth { get => depth; set => depth = value; }
        public int Used { get => used; set => used = value; }
        public uint Id { get => id; set => id = value; }
        public byte[] Data { get => data; set => data = value; }
    }
}
