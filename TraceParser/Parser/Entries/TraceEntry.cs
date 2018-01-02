using System.Collections.Generic;

namespace TraceUI.Parser.Entries
{
    public class TraceEntry
    {
        protected TraceEntry() : this("")
        {

        }

        protected TraceEntry(string cursorId)
        {
            CursorId = cursorId;
        }

        public Range PositionRange
        {
            get;
            set;
        }

        public Range LineRange
        {
            get;
            set;
        }

        public TraceEntryType Type
        {
            get;
            protected set;
        }

        public string CursorId
        {
            get;
            protected set;
        }

        internal virtual void SetProperties(List<StringProperty> properties)
        {
        }
    }
}
