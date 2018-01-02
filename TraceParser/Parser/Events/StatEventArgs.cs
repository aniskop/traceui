using TraceUI.Parser.Entries;

namespace TraceUI.Parser.Events
{
    public class StatEventArgs : TraceEntryEventArgs<StatEntry>
    {
        public StatEventArgs(StatEntry entry) : base(entry)
        {
        }
    }
}
