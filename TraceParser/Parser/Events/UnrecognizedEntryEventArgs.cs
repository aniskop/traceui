using TraceUI.Parser.Entries;

namespace TraceUI.Parser.Events
{
    public class UnrecognizedEntryEventArgs : TraceEntryEventArgs<UnrecognizedEntry>
    {
        public UnrecognizedEntryEventArgs(UnrecognizedEntry entry) : base(entry)
        {
        }
    }
}