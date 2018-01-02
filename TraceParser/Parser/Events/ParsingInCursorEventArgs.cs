using System;
using TraceUI.Parser.Entries;

namespace TraceUI.Parser.Events
{
    public class ParsingInCursorEventArgs : EventArgs
    {
        public ParsingInCursorEventArgs(ParsingInCursorEntry entry) : base()
        {
            Value = entry;
        }

        public ParsingInCursorEntry Value
        {
            get;
            private set;
        }
    }
}
