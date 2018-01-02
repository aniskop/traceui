using System.Collections.Generic;

namespace TraceUI.Parser.Entries
{
    /// <summary>
    /// Represents a cursor entity in Oracle trace file.
    /// </summary>
    public class ParsingInCursorEntry : TraceEntry
    {
        public ParsingInCursorEntry(string cursorId) : base(cursorId)
        {
            Type = TraceEntryType.ParsingInCursor;
        }

        public LongProperty Length
        {
            get;
            private set;
        }

        public LongProperty Dep
        {
            get;
            private set;
        }

        public LongProperty Uid
        {
            get;
            private set;
        }

        public LongProperty Oct
        {
            get;
            private set;
        }

        public LongProperty Lid
        {
            get;
            private set;
        }

        public ULongProperty Tim
        {
            get;
            private set;
        }
        public LongProperty Hv
        {
            get;
            private set;
        }

        public StringProperty Address
        {
            get;
            private set;
        }

        public StringProperty SqlId
        {
            get;
            private set;
        }

        public string Statement
        {
            get;
            internal set;
        }

        public bool IsSystemCursor
        {
            get
            {
                return (Uid.Value == 0);
            }
        }

        internal override void SetProperties(List<StringProperty> properties)
        {
            foreach (StringProperty p in properties)
            {
                if (p.Name.Equals(Property.SQL_LENGTH))
                {
                    Length = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.RECURSIVE_DEPTH))
                {
                    Dep = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.CALLING_USER_ID))
                {
                    Uid = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.ORACLE_COMMAND_TYPE))
                {
                    Oct = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.OWNER_USER_ID))
                {
                    Lid = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.TIMESTAMP))
                {
                    Tim = ULongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.HASH_VALUE))
                {
                    Hv = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.LIBRARY_CACHE_ADDRESS))
                {
                    Address = p;
                }
                else if (p.Name.Equals(Property.SQLID))
                {
                    SqlId = p;
                }
            }
        }

        public override string ToString()
        {
            return "";
        }
    }
}
