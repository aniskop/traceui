using TraceUI.Parser.Entries;

namespace TraceUI.Parser.Events
{
    public class BindsEventArgs : TraceEntryEventArgs<BindsEntry>
    {
        public BindsEventArgs(BindsEntry entry) : base(entry)
        {
        }
    }
}
