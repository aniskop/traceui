using System.Collections.Generic;

namespace TraceUI.Parser.Entries
{
    /// <summary>
    /// Represents any entry, which has not been recognized by the <see cref="TraceParser"/>.
    /// </summary>
    public class UnrecognizedEntry : TraceEntry
    {
        /// <summary>
        /// Creates an instance of <code>UnrecognizedEntry</code>.
        /// </summary>
        public UnrecognizedEntry() : base()
        {
        }

        /// <summary>
        /// Text of the entry.
        /// </summary>
        public string Text
        {
            get;
            internal set;
        }

        internal override void SetProperties(List<StringProperty> properties)
        {
        }
    }
}
