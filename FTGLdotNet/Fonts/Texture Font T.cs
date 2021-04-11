using System.Collections.Generic;
using FTGLdotNet.Atlas;
namespace FTGLdotNet.Font
{
    public struct Texture_Font_T
    {
        //Vector of glyphs contained in this font.
        List<Texture_Glyph_T> glyphs; // ??? type of glyphs

        //Atlas structure to store glyphs data.
        Texture_Atlas_T atlas;

        //font location can only be loaded from file atleast at the momment
        string filename;
        
        //font size
        float size;

        // Whether to use autohint when rendering font
        int hinting; //might be better as a bool?

        //Mode the font is rendering its next glyph
        RenderMode rendermode;

        //Outline thickness
        float outlinethickness;

        //Whether to use our own lcd filter
        int filtering; //? use bool instead

        //LCD filter weights
        byte[] lcd_filter_weights;

        //Whether to use kerning if available
        int kerning; //? use bool instead

        /* This field is simply used to compute a default line spacing (i.e., the
        * baseline-to-baseline distance) when writing text with this font. Note
        * that it usually is larger than the sum of the ascender and descender
        * taken as absolute values. There is also no guarantee that no glyphs
        * extend above or below subsequent baselines when using this distance.*/
        float height;

        /* This field is the distance that must be placed between two lines of
        * text. The baseline-to-baseline distance should be computed as:
        * ascender - descender + linegap*/
        float linegap;

        /* The ascender is the vertical distance from the horizontal baseline to
        * the highest 'character' coordinate in a font face. Unfortunately, font
        * formats define the ascender differently. For some, it represents the
        * ascent of all capital latin characters (without accents), for others it
        * is the ascent of the highest accented character, and finally, other
        * formats define it as being equal to bbox.yMax.*/
        float ascender;

        /** The descender is the vertical distance from the horizontal baseline to
        * the lowest 'character' coordinate in a font face. Unfortunately, font
        * formats define the descender differently. For some, it represents the
        * descent of all capital latin characters (without accents), for others it
        * is the ascent of the lowest accented character, and finally, other
        * formats define it as being equal to bbox.yMin. This field is negative
        * for values below the baseline. */
        float descender;

        /* The position of the underline line for this face. It is the center of
        * the underlining stem. Only relevant for scalable formats.*/
        float underlineposition;

        //The thickness of the underline for this face.Only relevant for scalable formats.
        float underlinethickness;

        //The padding to be add to the glyph's texture that are loaded by this font. Usefull when adding effects with shaders.
        int padding;

        public Texture_Atlas_T Atlas { get => atlas; set => atlas = value; }
        public string Filename { get => filename; set => filename = value; }
        public float Size { get => size; set => size = value; }
        public int Hinting { get => hinting; set => hinting = value; }
        public float Outlinethickness { get => outlinethickness; set => outlinethickness = value; }
        public byte[] Lcd_filter_weights { get => lcd_filter_weights; set => lcd_filter_weights = value; }
        public int Kerning { get => kerning; set => kerning = value; }
        public float Height { get => height; set => height = value; }
        public float Linegap { get => linegap; set => linegap = value; }
        public float Ascender { get => ascender; set => ascender = value; }
        public float Descender { get => descender; set => descender = value; }
        public float Underlineposition { get => underlineposition; set => underlineposition = value; }
        public float Underlinethickness { get => underlinethickness; set => underlinethickness = value; }
        public int Padding { get => padding; set => padding = value; }
        public List<Texture_Glyph_T> Glyphs { get => glyphs; set => glyphs = value; }
        public int Filtering { get => filtering; set => filtering = value; }
        internal RenderMode Rendermode { get => rendermode; set => rendermode = value; }
    }
}