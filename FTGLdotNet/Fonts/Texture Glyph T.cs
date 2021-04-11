/*
 * Glyph metrics:
 * --------------
 *
 *                       xmin                     xmax
 *                        |                         |
 *                        |<-------- width -------->|
 *                        |                         |
 *              |         +-------------------------+----------------- ymax
 *              |         |    ggggggggg   ggggg    |     ^        ^
 *              |         |   g:::::::::ggg::::g    |     |        |
 *              |         |  g:::::::::::::::::g    |     |        |
 *              |         | g::::::ggggg::::::gg    |     |        |
 *              |         | g:::::g     g:::::g     |     |        |
 *    offset_x -|-------->| g:::::g     g:::::g     |  offset_y    |
 *              |         | g:::::g     g:::::g     |     |        |
 *              |         | g::::::g    g:::::g     |     |        |
 *              |         | g:::::::ggggg:::::g     |     |        |
 *              |         |  g::::::::::::::::g     |     |      height
 *              |         |   gg::::::::::::::g     |     |        |
 *  baseline ---*---------|---- gggggggg::::::g-----*--------      |
 *            / |         |             g:::::g     |              |
 *     origin   |         | gggggg      g:::::g     |              |
 *              |         | g:::::gg   gg:::::g     |              |
 *              |         |  g::::::ggg:::::::g     |              |
 *              |         |   gg:::::::::::::g      |              |
 *              |         |     ggg::::::ggg        |              |
 *              |         |         gggggg          |              v
 *              |         +-------------------------+----------------- ymin
 *              |                                   |
 *              |------------- advance_x ---------->|
 */
using System.Collections.Generic;
using SharpFont;
namespace FTGLdotNet.Font
{
    /// <summary>
    /// A structure that describe a glyph.
    /// </summary>
    public struct Texture_Glyph_T
    {
        /// <summary>
        /// Unicode codepoint this glyph represents in UTF-32 LE encoding.
        /// </summary>
        uint codepoint;
        
        /// <summary>
        /// Glyph's width in pixels.
        /// </summary>
        uint width;

        /// <summary>
        /// Glyph's height in pixels
        /// </summary>
        uint height;

        /// <summary>
        /// Glyph's left bearing expressed in integer pixels.
        /// </summary>
        int offsetX;

        /// <summary>
        /// Glyphs's top bearing expressed in integer pixels.
        /// Remember that this is the distance from the baseline to the top-most
        /// glyph scanline, upwards y coordinates being positive.
        /// </summary>
        int offsetY;

        /// <summary>
        /// For horizontal text layouts, this is the horizontal distance (in
        /// fractional pixels) used to increment the pen position when the glyph is
        /// drawn as part of a string of text.
        /// </summary>
        float advancex;

        /// <summary>
        /// For vertical text layouts, this is the vertical distance (in fractional
        /// pixels) used to increment the pen position when the glyph is drawn as
        /// part of a string of text.
        /// </summary>
        float advancey;

        /// <summary>
        /// First normalized texture coordinate (x) of top-left corner
        /// </summary>
        float S0;

        /// <summary>
        /// Second normalized texture coordinate (y) of top-left corner
        /// </summary>
        float T0;

        /// <summary>
        /// First normalized texture coordinate (x) of bottom-right corner
        /// </summary>
        float S1;

        /// <summary>
        /// Second normalized texture coordinate (y) of bottom-right corner
        /// </summary>
        float T1;

        /// <summary>
        /// A vector of kerning pairs relative to this glyph.
        /// </summary>
        List<Kerning_t> kerning;

        /// <summary>
        /// Mode this glyph was rendered
        /// </summary>
        RenderMode renderMode;

        /// <summary>
        /// Glyph outline thickness
        /// </summary>
        float outlinethickness;

        public uint Codepoint { get => codepoint; set => codepoint = value; }
        public uint Width { get => width; set => width = value; }
        public uint Height { get => height; set => height = value; }
        public int OffsetX { get => offsetX; set => offsetX = value; }
        public int OffsetY { get => offsetY; set => offsetY = value; }
        public float Advancex { get => advancex; set => advancex = value; }
        public float Advancey { get => advancey; set => advancey = value; }
        public float S01 { get => S0; set => S0 = value; }
        public float T01 { get => T0; set => T0 = value; }
        public float S11 { get => S1; set => S1 = value; }
        public float T11 { get => T1; set => T1 = value; }
        public float Outlinethickness { get => outlinethickness; set => outlinethickness = value; }
        public List<Kerning_t> Kerning { get => kerning; set => kerning = value; }
        internal RenderMode RenderMode { get => renderMode; set => renderMode = value; }
    }
}