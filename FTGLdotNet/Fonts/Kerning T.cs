namespace FTGLdotNet.Font
{
    /// <summary>
    /// A structure that hold a kerning value relatively to a Unicode
    /// codepoint.
    ///
    /// This structure cannot be used alone since the (necessary) right
    /// Unicode codepoint is implicitely held by the owner of this structure.
    /// </summary>
    public struct Kerning_t
    {
        /// <summary>
        /// Left Unicode codepoint in the kern pair in UTF-32 LE encoding.
        /// </summary>
        uint codepoint;

        /// <summary>
        /// Kerning value (in fractional pixels).
        /// </summary>
        float kerning;
        
        public uint Codepoint { get => codepoint; set => codepoint = value; }
        public float Kerning { get => kerning; set => kerning = value; }
    }
}