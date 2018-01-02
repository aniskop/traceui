using System.Collections.Generic;

namespace TraceUI.Parser.Entries
{
    /// <summary>
    /// Represents statement statistics in Oracle trace.
    /// It is a set of <see cref="StatEntryLine"/> (<code>STAT</code> entries), which report explain plan statistics for the cursor.
    /// </summary>
    public class StatEntry : TraceEntry
    {
        /// <summary>
        /// Creates an instance of <code>StatEntry</code>.
        /// </summary>
        /// <param name="cursorId">Cursor ID of the statement.</param>
        public StatEntry(string cursorId) : base(cursorId)
        {
            Type = TraceEntryType.Stat;
            Lines = new List<StatEntryLine>();
        }

        /// <summary>
        /// Collection of <code>STAT</code> lines.
        /// </summary>
        public List<StatEntryLine> Lines
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds <code>STAT</code> line to <see cref="Lines"/> collection.
        /// </summary>
        /// <param name="line"></param>
        internal void AddLine(StatEntryLine line)
        {
            Lines.Add(line);
        }
    }
}
