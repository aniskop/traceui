using System.Collections.Generic;

namespace TraceUI.Parser.Entries
{
    /// <summary>
    /// Represents PARSE entry in Oracle trace.
    /// </summary>
    public class ParseEntry : TraceEntry
    {
        public ParseEntry(string cursorId) : base(cursorId)
        {
            Type = TraceEntryType.Parse;
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

        public LongProperty P
        {
            get;
            private set;
        }

        public LongProperty Cr
        {
            get;
            private set;
        }

        public LongProperty Cu
        {
            get;
            private set;
        }

        public LongProperty Mis
        {
            get;
            private set;
        }

        public LongProperty R
        {
            get;
            private set;
        }

        public LongProperty Dep
        {
            get;
            private set;
        }

        public LongProperty Og
        {
            get;
            private set;
        }

        public LongProperty Plh
        {
            get;
            private set;
        }

        public ULongProperty Tim
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
                else if (p.Name.Equals(Property.PHYSICAL_READS))
                {
                    P = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.CONSISTENT_READS))
                {
                    Cr = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.CURRENT_READS))
                {
                    Cu = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.LIBRARY_CACHE_MISS))
                {
                    Mis = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.ROWS))
                {
                    R = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.RECURSIVE_DEPTH))
                {
                    Dep = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.OPTIMIZER_GOAL))
                {
                    Og = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.PLH))
                {
                    Plh = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.TIMESTAMP))
                {
                    Tim = ULongProperty.Convert(p);
                }
            }
        }
    }
}
