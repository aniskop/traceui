using TraceUI.Parser.Entries;

namespace TraceUI.Parser.Events
{
    public class ParseEventArgs : TraceEntryEventArgs<ParseEntry>
    {
        public ParseEventArgs(ParseEntry entry) : base(entry)
        {
        }
    }
}
