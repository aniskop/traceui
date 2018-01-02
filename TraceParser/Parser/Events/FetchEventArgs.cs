using TraceUI.Parser.Entries;

namespace TraceUI.Parser.Events
{
    public class FetchEventArgs : TraceEntryEventArgs<FetchEntry>
    {
        public FetchEventArgs(FetchEntry entry) : base(entry)
        {
        }
    }
}
