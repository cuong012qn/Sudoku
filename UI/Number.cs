namespace UI
{
    using System.Diagnostics;
    [DebuggerDisplay("{Value}")]
    public class Number
    {
        public int Value { get; set; }
        public bool CanEdit { get; set; }
    }
}
