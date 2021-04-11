using FTGLdotNet.Atlas;
using SharpFont;
using System;
namespace FTGLdotNet.Font
{
    public static class Font
    {
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
        private static readonly float HRESF = 64.0f;
        private static readonly uint HRES = 64;
        private static readonly uint DPI = 72;

        private static Library library = null;
        private static Face face = null;
        public static Texture_Font_T LoadFontFromFile(Texture_Atlas_T atlas, float fontsize, string filename)
        {
            Texture_Font_T font = new Texture_Font_T();
            font.Atlas = atlas;
            font.Size = fontsize;
            font.Filename = filename;
            if (FontInit(ref font) == false) throw new Exception("error initialising Font");
            return font;
        }

        public static bool FontInit(ref Texture_Font_T font)
        {
            SizeMetrics sizemetrics = null;

            font.Glyphs = new System.Collections.Generic.List<Texture_Glyph_T>();
            font.Height = 0;
            font.Ascender = 0;
            font.Descender = 0;
            font.Outlinethickness = 0.0f;
            font.Rendermode = RenderMode.RENDER_NORMAL;
            font.Hinting = 1;
            font.Kerning = 1;
            font.Filtering = 1;
            font.Lcd_filter_weights = new byte[5] { 0x10, 0x40, 0x70, 0x40, 0x10 };
            if (!LoadFace(ref font, ref library, ref face, font.Size)) return false;
            font.Underlineposition = face.UnderlinePosition / (HRESF * HRESF) * font.Size;
            font.Underlineposition = System.MathF.Round(font.Underlineposition);
            if (font.Underlineposition < -2.0) font.Underlineposition = -2.0f;
            font.Underlinethickness = face.UnderlineThickness / (HRESF * HRESF) * font.Size;
            font.Underlinethickness = System.MathF.Round(font.Underlinethickness);
            if (font.Underlinethickness < 1) font.Underlinethickness = 1.0f;
            sizemetrics = face.Size.Metrics;
            font.Ascender = sizemetrics.Ascender.ToSingle();
            font.Descender = sizemetrics.Descender.ToSingle();
            font.Height = sizemetrics.Height.ToSingle();
            font.Linegap = font.Height - font.Ascender + font.Descender;

            GetGlyph(ref font, '\0');
            return true;

        }

        public static bool LoadFace(ref Texture_Font_T font, ref Library library, ref Face face, float size)
        {
            library = new Library();
            if (library == null) return false;
            face = new Face(library, font.Filename);
            if (face == null) return false;
            face.SelectCharmap(Encoding.Unicode);
            face.SetCharSize(Fixed26Dot6.FromSingle(size), 0, DPI * HRES, HRES);
            FTMatrix matrix = new FTMatrix((int)(1.0 / HRES * 0x10000L), (int)(0.0 * 0x10000L), (int)(0.0 * 0x10000L), (int)(1.0 * 0x10000L));
            face.SetTransform(matrix);
            return true;
        }
        public static Texture_Glyph_T GetGlyph(ref Texture_Font_T font, char codepoint)
        {
            Texture_Glyph_T glyph;
            if ((FindGlyph(ref font, codepoint, out glyph))) return glyph;
            if (LoadGlyph(ref font, codepoint))
            {
                FindGlyph(ref font, codepoint, out glyph);
                return glyph;
            }
            throw new Exception("Error retrieving the glyph!");
        }
        public static bool FindGlyph(ref Texture_Font_T font, char codepoint, out Texture_Glyph_T Glyph)
        {
            Texture_Glyph_T glyph;
            int uintcodepoint = char.ConvertToUtf32(codepoint.ToString(),0);
            for (int i = 0; i < font.Glyphs.Count; i++)
            {
                glyph = font.Glyphs[i];
                if((glyph.Codepoint == uintcodepoint)&& ((int)uintcodepoint ==-1)|| 
                    ((glyph.RenderMode == font.Rendermode)&& glyph.Outlinethickness == font.Outlinethickness))
                {
                    Glyph = glyph;
                    return true;
                }
            }
            Glyph = new Texture_Glyph_T();
            return false;
        }
        public static bool LoadGlyph(ref Texture_Font_T font, char codepoint)
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
            //check glyph has not already been loaded
            if(FindGlyph(ref font, codepoint, out glyph_T))
            {
                return true;
            }
            // handle null codepoint
            if(codepoint == '\0')
            {
                region = Atlas.Atlas.GetRegion(ref font.Atlas, 5, 5);
                glyph_T = InitGlyph();
                byte[] data = new byte[4 * 4 * 3] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                                                    0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
                if(region.X<0)
                {
                    //throw new Exception("texture atlas is full!");
                    return false;
                }
                Atlas.Atlas.SetRegion(ref font.Atlas, region.X, region.Y, 4, 4, data, 0);
                glyph_T.Codepoint = -1;
                glyph_T.S01 = (region.X + 2) / (float)font.Atlas.Width;
                glyph_T.T01 = (region.Y + 2) / (float)font.Atlas.Height;
                glyph_T.S11 = (region.X + 3) / (float)font.Atlas.Width;
                glyph_T.T11 = (region.Y + 3) / (float)font.Atlas.Height;
                font.Glyphs.Add(glyph_T);
                return true;
            }
            LoadFlags flags = 0;
            LoadTarget target = 0;
            glyphindex = face.GetCharIndex((uint)char.ConvertToUtf32(codepoint.ToString(),0));
            if(font.Rendermode != RenderMode.RENDER_NORMAL && font.Rendermode != RenderMode.RENDER_SIGNED_DISTANCE_FIELD)
            {
                flags |= LoadFlags.NoBitmap;
            }
            else
            {
                flags |= LoadFlags.Render;
            }
            if(!font.Hinting)
            {
                flags |= LoadFlags.NoHinting | LoadFlags.NoAutohint;
            }
            else
            {
                flags |= LoadFlags.ForceAutohint;
            }
            if(font.Atlas.Depth ==3)
            {
                library.SetLcdFilter(LcdFilter.Light);
                target |= LoadTarget.Lcd;
                if(font.Filtering)
                {
                    library.SetLcdFilterWeights(font.Lcd_filter_weights);
                }
            }
            else if(HRES ==1)
            {
                target |= LoadTarget.Light;
            }
            face.LoadGlyph(glyphindex, flags, target);
            if(font.Rendermode == RenderMode.RENDER_NORMAL || font.Rendermode == RenderMode.RENDER_SIGNED_DISTANCE_FIELD)
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

                stroker.Set((int)(font.Outlinethickness * HRES), StrokerLineCap.Round, StrokerLineJoin.Round, 0);
                glyph = face.Glyph.GetGlyph();
                if(font.Rendermode == RenderMode.RENDER_OUTLINE_EDGE)
                {
                    glyph.Stroke(stroker, true);
                }
                else if(font.Rendermode == RenderMode.RENDER_OUTLINE_NEGATIVE)
                {
                    glyph.StrokeBorder(stroker, true, true);
                }
                else if(font.Rendermode == RenderMode.RENDER_OUTLINE_POSITIVE)
                {
                    glyph.StrokeBorder(stroker, false, true);
                }
                if (font.Atlas.Depth == 1)
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
            if(font.Rendermode == RenderMode.RENDER_SIGNED_DISTANCE_FIELD)
            {
                padding.Top = 1;
                padding.Left = 1;
            }
            if(font.Padding !=0)
            {
                padding.Left += font.Padding;
                padding.Right += font.Padding;
                padding.Top += font.Padding;
                padding.Bottom += font.Padding;
            }
            int width = (fTBitmap.Width/font.Atlas.Depth) + padding.Left + padding.Right;
            int height = fTBitmap.Rows + padding.Top + padding.Bottom;
            region = Atlas.Atlas.GetRegion(ref font.Atlas, width, height);
            if(region.X <0)
            {
                //throw new exception("texture atlas full");
                return false;
            }
            byte[] data = new byte[width * height * font.Atlas.Depth];
            data = fTBitmap.BufferData;
            if(font.Rendermode == RenderMode.RENDER_SIGNED_DISTANCE_FIELD)
            {
                // todo ? where is make distance map comming from
                throw new NotImplementedException();
            }
            Atlas.Atlas.SetRegion(ref font.Atlas, region.X, region.Y, width, height, data, width * font.Atlas.Depth);
            glyph_T = InitGlyph();
            glyph_T.Codepoint = (uint)char.ConvertToUtf32(codepoint.ToString(), 0);
            glyph_T.Width = (uint)width;
            glyph_T.Height = (uint)height;
            glyph_T.RenderMode = font.Rendermode;
            glyph_T.Outlinethickness = font.Outlinethickness;
            glyph_T.OffsetX = glyphleft;
            glyph_T.OffsetY = glyphtop;
            glyph_T.S01 = region.X / (float)font.Atlas.Width;
            glyph_T.T01 = region.Y / (float)font.Atlas.Height;
            glyph_T.S11 = (region.X + width) / (float)font.Atlas.Width;
            glyph_T.T11 = (region.Y + height) / (float)font.Atlas.Height;
            face.LoadGlyph(glyphindex, LoadFlags.Render | LoadFlags.NoHinting, target);
            glyphSlot = face.Glyph;
            glyph_T.Advancex = glyphSlot.Advance.X.ToSingle();
            glyph_T.Advancey = glyphSlot.Advance.Y.ToSingle();
            font.Glyphs.Add(glyph_T);
            if(font.Rendermode != RenderMode.RENDER_NORMAL && font.Rendermode != RenderMode.RENDER_SIGNED_DISTANCE_FIELD)
            {
                glyph.Dispose();
            }
            GenerateKerning(ref font);
            return true;
        }
        public static void GenerateKerning(ref Texture_Font_T font)
        {
            Texture_Glyph_T prevglyph;
            Texture_Glyph_T glyph;
            uint glyphindex;
            uint previndex;
            FTVector26Dot6 kerning;
            for (int x = 1; x < font.Glyphs.Count; x++)
            {
                glyph = font.Glyphs[x];
                glyphindex = face.GetCharIndex(glyph.Codepoint);
                glyph.Kerning.Clear();
                for (int y = 1; y < font.Glyphs.Count; y++)
                {
                    prevglyph = font.Glyphs[y];
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
        public static  Texture_Glyph_T InitGlyph()
        {
            Texture_Glyph_T glyph = new Texture_Glyph_T();
            glyph.Codepoint = -1;
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