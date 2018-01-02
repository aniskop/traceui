using TraceUI.Parser.Entries;

namespace TraceUI.Parser.Events
{
    public class XctendEventArgs : TraceEntryEventArgs<XctendEntry>
    {
        public XctendEventArgs(XctendEntry entry) : base(entry)
        {
        }
    }
}
