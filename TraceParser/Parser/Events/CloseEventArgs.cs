using TraceUI.Parser.Entries;

namespace TraceUI.Parser.Events
{
    public class CloseEventArgs : TraceEntryEventArgs<CloseEntry>
    {
        public CloseEventArgs(CloseEntry entry) : base(entry)
        {
        }
    }
}
