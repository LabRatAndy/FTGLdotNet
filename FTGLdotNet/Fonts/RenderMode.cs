namespace FTGLdotNet.Font
{
    //A list of possible ways to render a glyph
    internal enum RenderMode
    {
        RENDER_NORMAL,
        RENDER_OUTLINE_EDGE,
        RENDER_OUTLINE_POSITIVE,
        RENDER_OUTLINE_NEGATIVE,
        RENDER_SIGNED_DISTANCE_FIELD
    }
}