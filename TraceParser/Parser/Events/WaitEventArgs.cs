using TraceUI.Parser.Entries;

namespace TraceUI.Parser.Events
{
    public class WaitEventArgs : TraceEntryEventArgs<WaitEntry>
    {
        public WaitEventArgs(WaitEntry entry) : base(entry)
        {
        }
    }
}
