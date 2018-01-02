using System.Collections.Generic;

namespace TraceUI.Parser.Entries
{
    /// <summary>
    /// Represents CLOSE entry in Oracle trace.
    /// </summary>
    /// <example>CLOSE #139816334997528:c=0,e=61,dep=1,type=0,tim=2673974625375</example>
    public class CloseEntry : TraceEntry
    {
        public CloseEntry(string cursorId) : base(cursorId)
        {
            Type = TraceEntryType.Close;
        }

        public LongProperty C
        {
            get;
            private set;
        }

        public LongProperty Elapsed
        {
            get;
            private set;
        }

        public LongProperty Dep
        {
            get;
            private set;
        }

        public LongProperty CloseType
        {
            get;
            private set;
        }

        public ULongProperty Timestamp
        {
            get;
            private set;
        }

        internal override void SetProperties(List<StringProperty> properties)
        {
            foreach (StringProperty p in properties)
            {
                if (p.Name.Equals(Property.CPU_TIME))
                {
                    C = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.WALL_TIME_ELAPSED))
                {
                    Elapsed = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.RECURSIVE_DEPTH))
                {
                    Dep = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.TYPE))
                {
                    CloseType = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.TIMESTAMP))
                {
                    Timestamp = ULongProperty.Convert(p);
                }
            }
        }
    }
}
