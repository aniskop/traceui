using System.Collections.Generic;

namespace TraceUI.Parser.Entries
{
    /// <summary>
    /// Represents bind variables (<code>BINDS</code> entry) used for SQL statement.
    /// </summary>
    public class BindsEntry : TraceEntry
    {
        /// <summary>
        /// Creates an instance of <see cref="BindsEntry"/>.
        /// </summary>
        /// <param name="cursorId">Cursor ID of the statement.</param>
        public BindsEntry(string cursorId) : base(cursorId)
        {
            Type = TraceEntryType.Binds;
            Binds = new List<BindEntry>();
        }

        /// <summary>
        /// Collection of all bind variables (<code>Bind</code> entry) and their data.
        /// </summary>
        public List<BindEntry> Binds
        {
            get;
            private set;
        }
    }
}
