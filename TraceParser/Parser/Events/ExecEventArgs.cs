using TraceUI.Parser.Entries;

namespace TraceUI.Parser.Events
{
    public class ExecEventArgs : TraceEntryEventArgs<ExecEntry>
    {
        public ExecEventArgs(ExecEntry entry) : base(entry)
        {
        }
    }
}
