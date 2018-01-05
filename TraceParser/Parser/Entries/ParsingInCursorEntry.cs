using System.Collections.Generic;

namespace TraceUI.Parser.Entries
{
    /// <summary>
    /// Represents a cursor entity in Oracle trace file.
    /// </summary>
    public class ParsingInCursorEntry : TraceEntry
    {
        /// <summary>
        /// Creates an instance of <see cref="ParsingInCursorEntry"/>.
        /// </summary>
        /// <param name="cursorId">Cursor ID of the statement.</param>
        public ParsingInCursorEntry(string cursorId) : base(cursorId)
        {
            Type = TraceEntryType.ParsingInCursor;
        }

        /// <summary>
        /// Length of SQL statement.
        /// Value source is <see cref="Property.SQL_LENGTH"/>.
        /// </summary>
        public LongProperty Length
        {
            get;
            private set;
        }

        /// <summary>
        /// Recursive depth of the cursor.
        /// Value source is <see cref="Property.RECURSIVE_DEPTH"/>.
        /// </summary>
        public LongProperty Depth
        {
            get;
            private set;
        }

        /// <summary>
        /// Schema id under which SQL was parsed e.g. calling user.
        /// Value source is <see cref="Property.CALLING_USER_ID"/>.
        /// </summary>
        public LongProperty CallingUserId
        {
            get;
            private set;
        }

        /// <summary>
        /// Oracle command type.
        /// Value source is <see cref="Property.ORACLE_COMMAND_TYPE"/>.
        /// </summary>
        public LongProperty CommandType
        {
            get;
            private set;
        }

        /// <summary>
        /// User id owning the statement e.g. package.
        /// Value source is <see cref="Property.OWNER_USER_ID"/>.
        /// </summary>
        public LongProperty OwningUserId
        {
            get;
            private set;
        }

        /// <summary>
        /// Timestamp.
        /// Can be used to determine the time between any 2 operations.
        /// In Oracle 9i and above in microseconds, below 9i in centiseconds.
        /// Value source is <see cref="Property.TIMESTAMP"/> property.
        /// </summary>
        public ULongProperty Timestamp
        {
            get;
            private set;
        }

        /// <summary>
        /// Statement's hash value.
        /// Value source is <see cref="Property.HASH_VALUE"/>.
        /// </summary>
        public LongProperty HashValue
        {
            get;
            private set;
        }

        /// <summary>
        /// SQL address (see <code>V$SQLAREA</code>, <code>V$SQLTEXT</code> views).
        /// Value source is <see cref="Property.ADDRESS"/>.
        /// </summary>
        public StringProperty Address
        {
            get;
            private set;
        }

        /// <summary>
        /// SQL statement ID.
        /// Value source is <see cref="Property.SQLID"/>.
        /// </summary>
        public StringProperty SqlId
        {
            get;
            private set;
        }

        /// <summary>
        /// SQL statement text.
        /// </summary>
        public string Statement
        {
            get;
            internal set;
        }

        /// <summary>
        /// Whether the statement is system statement (generated internally by the Oracle).
        /// </summary>
        public bool IsSystemCursor
        {
            get
            {
                return (CallingUserId.Value == 0);
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
                    Depth = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.CALLING_USER_ID))
                {
                    CallingUserId = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.ORACLE_COMMAND_TYPE))
                {
                    CommandType = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.OWNER_USER_ID))
                {
                    OwningUserId = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.TIMESTAMP))
                {
                    Timestamp = ULongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.HASH_VALUE))
                {
                    HashValue = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.ADDRESS))
                {
                    Address = p;
                }
                else if (p.Name.Equals(Property.SQLID))
                {
                    SqlId = p;
                }
            }
        }
    }
}
