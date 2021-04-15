using FTGLdotNet.Atlas;
using SharpFont;
using System;
using System.Collections.Generic;

namespace FTGLdotNet.Font
{
    public class Font
    {
        //private helper struct
        private struct Padding
        {
            int left;
            int right;
            int top;
            int bottom;
            internal Padding(int L,int R, int T,int B)
            {
                left = L;
                right = R;
                top = T;
                bottom = B;
            }
            internal int Left
            {
                get { return left; }
                set { left = value; }
            }
            internal int Top
            {
                get { return top; }
                set { top = value; }
            }
            internal int Bottom
            {
                get { return bottom; }
                set { bottom = value; }
            }
            internal int Right
            {
                get { return right; }
                set { right = value; }
            }
        }
        //constants
        private const float HRESF = 64.0f;
        private const uint HRES = 64;
        private const uint DPI = 72;

        //member variables 
        private Library library = null;
        private Face face = null;

        //member variables from  texture font t struct
        private List<Texture_Glyph_T> glyphs;
        private Atlas.Atlas atlas = null;
        private string filename;
        private float size;
        private bool hinting;
        private RenderMode rendermode;
        private float outlinethickness;
        private bool filtering;
        private byte[] lcdfilterweights;
        private bool kerning;
        private float height;
        private float linegap;
        private float ascender;
        private float decender;
        private float underlineposition;
        private float underlinethickness;
        private int padding;

        // properties
        //Vector of glyphs contained in this font.
        public List<Texture_Glyph_T> Glyphs
        {
            get { return glyphs; }
            set { glyphs = value; }
        }

        //Atlas structure to store glyphs data.
        public Atlas.Atlas TextureAtlas
        {
            get { return atlas; }
            set { atlas = value; }
        }

        //font location can only be loaded from file atleast at the momment
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        //font size
        public float Size
        {
            get { return size; }
            set { size = value; }
        }

        // Whether to use autohint when rendering font
        public bool Hinting
        {
            get { return hinting; }
            set { hinting = value; }
        }

        //Mode the font is rendering its next glyph
        public RenderMode RenderMode
        {
            get { return rendermode; }
            set { rendermode = value; }
        }
        //Outline thickness
        public float OutlineThinknes
        {
            get { return outlinethickness; }
            set { outlinethickness = value; }
        }
        //Whether to use our own lcd filter
        public bool LCDFilter
        {
            get { return filtering; }
            set { filtering = value; }
        }
        //LCD filter weights
        public byte[] LCDFilterWeights
        {
            get { return lcdfilterweights; }
            set { lcdfilterweights = value; }
        }
        //Whether to use kerning if available
        public bool Kerning
        {
            get { return kerning; }
            set { kerning = value; }
        }
        /* This field is simply used to compute a default line spacing (i.e., the
        * baseline-to-baseline distance) when writing text with this font. Note
        * that it usually is larger than the sum of the ascender and descender
        * taken as absolute values. There is also no guarantee that no glyphs
        * extend above or below subsequent baselines when using this distance.*/
        public float Height
        {
            get { return height; }
            set { height = value; }
        }
        /* This field is the distance that must be placed between two lines of
        * text. The baseline-to-baseline distance should be computed as:
        * ascender - descender + linegap*/
        public float Linegap
        {
            get { return linegap; }
            set { linegap = value; }
        }
        /* The ascender is the vertical distance from the horizontal baseline to
        * the highest 'character' coordinate in a font face. Unfortunately, font
        * formats define the ascender differently. For some, it represents the
        * ascent of all capital latin characters (without accents), for others it
        * is the ascent of the highest accented character, and finally, other
        * formats define it as being equal to bbox.yMax.*/
        public float Ascender
        {
            get { return ascender; }
            set { ascender = value; }
        }
        /** The descender is the vertical distance from the horizontal baseline to
        * the lowest 'character' coordinate in a font face. Unfortunately, font
        * formats define the descender differently. For some, it represents the
        * descent of all capital latin characters (without accents), for others it
        * is the ascent of the lowest accented character, and finally, other
        * formats define it as being equal to bbox.yMin. This field is negative
        * for values below the baseline. */
        public float Desecnder
        {
            get { return decender; }
            set { decender = value; }
        }
        /* The position of the underline line for this face. It is the center of
        * the underlining stem. Only relevant for scalable formats.*/
        public float UnderLinePosition
        {
            get { return underlineposition; }
            set { underlineposition = value; }
        }
        //The thickness of the underline for this face.Only relevant for scalable formats.
        public float UnderlineThinkness
        {
            get { return underlinethickness; }
            set { underlinethickness = value; }
        }
        //The padding to be add to the glyph's texture that are loaded by this font. Usefull when adding effects with shaders.
        public int PaddingValue
        {
            get { return padding; }
            set { padding = value; }
        }
        /// <summary>
        /// Constructor for font class, creates an atlas of the supplied size
        /// </summary>
        /// <param name="fontsize">the font size in points</param>
        /// <param name="filename">the font ttf file to load from</param>
        /// <param name="atlaswidth">width of the atlas created in pixels</param>
        /// <param name="atlasheight">height of the atlas created in pixels</param>
        /// <param name="atlasdepth">byte depth of atlas in bytes must be 1, 3 or 4</param>
        public Font(float fontsize, string filename, int atlaswidth, int atlasheight, int atlasdepth)
        {
            size = fontsize;
            this.filename = filename;
            atlas = new Atlas.Atlas(atlaswidth, atlasheight, atlasdepth);
            if (FontInit() == false) throw new Exception("error initialising Font");
        }
        //initialises the font object and loads the face object
        private bool FontInit()
        {
            SizeMetrics sizemetrics = null;

            glyphs = new System.Collections.Generic.List<Texture_Glyph_T>();
            height = 0;
            ascender = 0;
            decender = 0;
            outlinethickness = 0.0f;
            rendermode = RenderMode.RENDER_NORMAL;
            hinting = true;
            kerning = true;
            filtering = true;
            LCDFilterWeights = new byte[5] { 0x10, 0x40, 0x70, 0x40, 0x10 };
            if (!LoadFace()) return false;
            underlineposition = face.UnderlinePosition / (HRESF * HRESF) * size;
            underlineposition = System.MathF.Round(underlineposition);
            if (underlineposition < -2.0) underlineposition = -2.0f;
            underlinethickness = face.UnderlineThickness / (HRESF * HRESF) * size;
            underlinethickness = System.MathF.Round(underlinethickness);
            if (underlinethickness < 1) underlinethickness = 1.0f;
            sizemetrics = face.Size.Metrics;
            ascender = sizemetrics.Ascender.ToSingle();
            decender = sizemetrics.Descender.ToSingle();
            height = sizemetrics.Height.ToSingle();
            linegap = height - ascender + decender;

            GetGlyph('\0');
            return true;

        }
        //loads the face of the font 
        private bool LoadFace()
        {
            library = new Library();
            if (library == null) return false;
            face = new Face(library, filename);
            if (face == null) return false;
            face.SelectCharmap(Encoding.Unicode);
            face.SetCharSize(Fixed26Dot6.FromSingle(size), 0, DPI * HRES, HRES);
            FTMatrix matrix = new FTMatrix((int)(1.0 / HRES * 0x10000L), (int)(0.0 * 0x10000L), (int)(0.0 * 0x10000L), (int)(1.0 * 0x10000L));
            face.SetTransform(matrix);
            return true;
        }
        /// <summary>
        /// returns the glyph of a given codepoint
        /// </summary>
        /// <param name="codepoint">the codepoint whose glyph wiil be returned </param>
        /// <returns>The glyph that matches the codepoint</returns>
        public Texture_Glyph_T GetGlyph(char codepoint)
        {
            Texture_Glyph_T glyph;
            if ((FindGlyph(codepoint, out glyph))) return glyph;
            if (LoadGlyph(codepoint))
            {
                FindGlyph(codepoint, out glyph);
                return glyph;
            }
            throw new Exception("Error retrieving the glyph!");
        }
        /// <summary>
        /// finds if the glyph for the codepoint has already been loaded into the glyphs list and if so returns it.
        /// </summary>
        /// <param name="codepoint">the codepoint to retreive</param>
        /// <param name="Glyph">the glyph if found</param>
        /// <returns>true if the glyph has been loaded and passes the glyph back as Glyph, false otherwise</returns>
        /// <remarks> don't use glyph if it has returned false as it will be invalid</remarks>
        private bool FindGlyph(char codepoint, out Texture_Glyph_T Glyph)
        {
            Texture_Glyph_T glyph;
            int uintcodepoint = char.ConvertToUtf32(codepoint.ToString(),0);
            for (int i = 0; i < glyphs.Count; i++)
            {
                glyph = glyphs[i];
                if((glyph.Codepoint == uintcodepoint)&& ((int)uintcodepoint ==-1)|| 
                    ((glyph.RenderMode == rendermode)&& glyph.Outlinethickness == outlinethickness))
                {
                    Glyph = glyph;
                    return true;
                }
            }
            Glyph = new Texture_Glyph_T();
            return false;
        }
        /// <summary>
        /// Loads the glyph assoicated with the given codepoint into the glyphs list
        /// </summary>
        /// <param name="codepoint">codepoint that needs to be loaded</param>
        /// <returns>true on sucessful loading of glyph, false otherwise</returns>
        private bool LoadGlyph(char codepoint)
        {
            int i;
            int x;
            int y;
            Glyph glyph;
            GlyphSlot glyphSlot;
            FTBitmap fTBitmap;
            uint glyphindex;
            Texture_Glyph_T glyph_T;
            int glyphtop = 0;
            int glyphleft = 0;
            Region region;
            int missed = 0;
            byte[] data = null;
            //check glyph has not already been loaded
            if(FindGlyph(codepoint, out glyph_T))
            {
                return true;
            }
            // handle null codepoint
            if(codepoint == '\0')
            {
                region = atlas.GetRegion(5, 5);
                glyph_T = InitGlyph();
                data = new byte[4 * 4 * 3] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
                if(region.X<0)
                {
                    //throw new Exception("texture atlas is full!");
                    return false;
                }
                atlas.SetRegion(region.X, region.Y, 4, 4, data, 0);
                glyph_T.Codepoint = uint.MaxValue; // can't set to -1 as uint will try max value as can't really see a code point of uint.maxvalue being valid 
                glyph_T.S01 = (region.X + 2) / (float)atlas.Width;
                glyph_T.T01 = (region.Y + 2) / (float)atlas.Height;
                glyph_T.S11 = (region.X + 3) / (float)atlas.Width;
                glyph_T.T11 = (region.Y + 3) / (float)atlas.Height;
                glyphs.Add(glyph_T);
                return true;
            }
            LoadFlags flags = 0;
            LoadTarget target = 0;
            glyphindex = face.GetCharIndex((uint)char.ConvertToUtf32(codepoint.ToString(),0));
            if(rendermode != RenderMode.RENDER_NORMAL && rendermode != RenderMode.RENDER_SIGNED_DISTANCE_FIELD)
            {
                flags |= LoadFlags.NoBitmap;
            }
            else
            {
                flags |= LoadFlags.Render;
            }
            if(!hinting)
            {
                flags |= LoadFlags.NoHinting | LoadFlags.NoAutohint;
            }
            else
            {
                flags |= LoadFlags.ForceAutohint;
            }
            if(atlas.Depth ==3)
            {
                library.SetLcdFilter(LcdFilter.Light);
                target |= LoadTarget.Lcd;
                if(filtering)
                {
                    library.SetLcdFilterWeights(LCDFilterWeights);
                }
            }
            else if(HRES == 1)//todo check if correct as if so, else statement is redundant
            {
                target |= LoadTarget.Light;
            }
            face.LoadGlyph(glyphindex, flags, target);
            if(rendermode == RenderMode.RENDER_NORMAL || rendermode == RenderMode.RENDER_SIGNED_DISTANCE_FIELD)
            {
                glyphSlot = face.Glyph;
                fTBitmap = glyphSlot.Bitmap;
                glyphtop = glyphSlot.BitmapTop;
                glyphleft = glyphSlot.BitmapLeft;
            }
            else
            {
                Stroker stroker = new Stroker(library);
                BitmapGlyph bitmapGlyph;

                stroker.Set((int)(outlinethickness * HRES), StrokerLineCap.Round, StrokerLineJoin.Round, 0);
                glyph = face.Glyph.GetGlyph();
                if(rendermode == RenderMode.RENDER_OUTLINE_EDGE)
                {
                    glyph.Stroke(stroker, true);
                }
                else if(rendermode == RenderMode.RENDER_OUTLINE_NEGATIVE)
                {
                    glyph.StrokeBorder(stroker, true, true);
                }
                else if(rendermode == RenderMode.RENDER_OUTLINE_POSITIVE)
                {
                    glyph.StrokeBorder(stroker, false, true);
                }
                if (atlas.Depth == 1)
                {
                    glyph.ToBitmap(SharpFont.RenderMode.Normal, new FTVector26Dot6(0,0), true);
                }
                else
                {
                    glyph.ToBitmap(SharpFont.RenderMode.Lcd, new FTVector26Dot6(0,0), true);
                }
                bitmapGlyph = glyph.ToBitmapGlyph();
                fTBitmap = bitmapGlyph.Bitmap;
                glyphtop = bitmapGlyph.Top;
                glyphleft = bitmapGlyph.Left;

                stroker.Dispose();
            }
            Padding padding = new Padding(0, 1, 0, 1);
            if(rendermode == RenderMode.RENDER_SIGNED_DISTANCE_FIELD)
            {
                padding.Top = 1;
                padding.Left = 1;
            }
            if(this.padding !=0)
            {
                padding.Left += PaddingValue;
                padding.Right += PaddingValue;
                padding.Top += PaddingValue;
                padding.Bottom += PaddingValue;
            }
            int width = (fTBitmap.Width/atlas.Depth) + padding.Left + padding.Right;
            int height = fTBitmap.Rows + padding.Top + padding.Bottom;
            region = atlas.GetRegion(width, height);
            if(region.X <0)
            {
                //throw new exception("texture atlas full");
                return false;
            }
            data = new byte[width * height * atlas.Depth];
            data = fTBitmap.BufferData;
            if(rendermode == RenderMode.RENDER_SIGNED_DISTANCE_FIELD)
            {
                // todo ? where is make distance map comming from
                throw new NotImplementedException();
            }
            atlas.SetRegion(region.X, region.Y, width, height, data, width * atlas.Depth);
            glyph_T = InitGlyph();
            glyph_T.Codepoint = (uint)char.ConvertToUtf32(codepoint.ToString(), 0);
            glyph_T.Width = (uint)width;
            glyph_T.Height = (uint)height;
            glyph_T.RenderMode = rendermode;
            glyph_T.Outlinethickness = outlinethickness;
            glyph_T.OffsetX = glyphleft;
            glyph_T.OffsetY = glyphtop;
            glyph_T.S01 = region.X / (float)atlas.Width;
            glyph_T.T01 = region.Y / (float)atlas.Height;
            glyph_T.S11 = (region.X + width) / (float)atlas.Width;
            glyph_T.T11 = (region.Y + height) / (float)atlas.Height;
            face.LoadGlyph(glyphindex, LoadFlags.Render | LoadFlags.NoHinting, target);
            glyphSlot = face.Glyph;
            glyph_T.Advancex = glyphSlot.Advance.X.ToSingle();
            glyph_T.Advancey = glyphSlot.Advance.Y.ToSingle();
            glyphs.Add(glyph_T);
            if(rendermode != RenderMode.RENDER_NORMAL && rendermode != RenderMode.RENDER_SIGNED_DISTANCE_FIELD)
            {
                glyph.Dispose();
            }
            GenerateKerning();
            return true;
        }
        private void GenerateKerning()
        {
            Texture_Glyph_T prevglyph;
            Texture_Glyph_T glyph;
            uint glyphindex;
            uint previndex;
            FTVector26Dot6 kerning;
            for (int x = 1; x < glyphs.Count; x++)
            {
                glyph = glyphs[x];
                glyphindex = face.GetCharIndex(glyph.Codepoint);
                glyph.Kerning.Clear();
                for (int y = 1; y < glyphs.Count; y++)
                {
                    prevglyph = glyphs[y];
                    previndex = face.GetCharIndex(prevglyph.Codepoint);
                    kerning = face.GetKerning(previndex, glyphindex, KerningMode.Unfitted);
                    if(kerning.X)
                    {
                        Kerning_t k = new Kerning_t();
                        k.Codepoint = prevglyph.Codepoint;
                        k.Kerning = kerning.X.ToSingle() / HRESF;
                        glyph.Kerning.Add(k);
                    }
                }
            }
        }
        public Texture_Glyph_T InitGlyph()
        {
            Texture_Glyph_T glyph = new Texture_Glyph_T();
            glyph.Codepoint = uint.MaxValue; // can't set to -1 as uint will try max value as can't really see a code point of uint.maxvalue being valid 
            glyph.Width = 0;
            glyph.Height = 0;
            glyph.RenderMode = RenderMode.RENDER_NORMAL;
            glyph.Outlinethickness = 0;
            glyph.OffsetX = 0;
            glyph.OffsetY = 0;
            glyph.Advancex = 0;
            glyph.Advancey = 0;
            glyph.S01 = 0;
            glyph.S11 = 0;
            glyph.T01 = 0;
            glyph.T11 = 0;
            glyph.Kerning = new System.Collections.Generic.List<Kerning_t>();
            return glyph;
        }

    }
}