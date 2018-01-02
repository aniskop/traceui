using System.Collections.Generic;

namespace TraceUI.Parser.Entries
{
    /// <summary>
    /// Represents a single <code>STAT</code> line in a <see cref="StatEntry"/>.
    /// Line reports explain plan statistics for the numbered cursor.
    /// </summary>
    public class StatEntryLine : TraceEntry
    {
        /// <summary>
        /// Creates an instance of <code>StatEntryLine</code>.
        /// </summary>
        /// <param name="cursorId">Cursor ID of the statement.</param>
        public StatEntryLine(string cursorId) : base(cursorId)
        {
            Type = TraceEntryType.Stat;
        }

        /// <summary>
        /// Line ID (index) in the explain plan. Starts at 1.
        /// Value source is <see cref="Property.ID"/> property.
        /// </summary>
        public LongProperty Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Number of rows returned.
        /// Value source is <see cref="Property.ROW_COUNT"/> property.
        /// </summary>
        public LongProperty RowCount
        {
            get;
            private set;
        }

        /// <summary>
        /// <see cref="Id"/> of the parent line within <code>STAT</code> line set.
        /// Value source is <see cref="Property.PARENT_ID"/> property.
        /// </summary>
        public LongProperty ParentId
        {
            get;
            private set;
        }

        /// <summary>
        /// Position in explain plan.
        /// Value source is <see cref="Property.POSITION"/> property.
        /// </summary>
        public LongProperty Pos
        {
            get;
            private set;
        }

        /// <summary>
        /// Database object ID as in <code>DBA_OBJECTS.OBJECT_ID</code>.
        /// <code>-1</code> if operation is not on the database object.
        /// Value source is <see cref="Property.OBJECT_ID"/> property.
        /// </summary>
        public LongProperty ObjectId
        {
            get;
            private set;
        }

        /// <summary>
        /// Row source access operation.
        /// Value source is <see cref="Property.OPERATION"/> property.
        /// </summary>
        public StringProperty Operation
        {
            get;
            private set;
        }

        internal override void SetProperties(List<StringProperty> properties)
        {
            foreach (StringProperty p in properties)
            {
                if (p.Name.Equals(Property.ID))
                {
                    Id = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.ROW_COUNT))
                {
                    RowCount = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.PARENT_ID))
                {
                    ParentId = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.POSITION))
                {
                    Pos = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.OBJECT_ID))
                {
                    ObjectId = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.OPERATION))
                {
                    Operation = p;
                }
            }
        }
    }
}
