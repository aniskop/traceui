using System;

namespace TraceUI.Parser.Events
{
    public class TraceEntryEventArgs<T> : EventArgs
    {
        public TraceEntryEventArgs(T entry) : base()
        {
            Value = entry;
        }

        public T Value
        {
            get;
            protected set;
        }
    }
}
