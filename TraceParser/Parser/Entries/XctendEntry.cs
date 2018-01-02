using System.Collections.Generic;

namespace TraceUI.Parser.Entries
{
    /// <summary>
    /// Represents a transaction end marker (XCTEND entry) in Oracle trace.
    /// </summary>
    public class XctendEntry : TraceEntry
    {
        public const int ROLLBACK = 1;
        public const int NO_ROLLBACK = 0;

        public const int READ_ONLY = 1;
        public const int NO_READ_ONLY = 0;

        /// <summary>
        /// Creates an instance of <code>XctendEntry</code>.
        /// </summary>
        /// <param name="cursorId">Cursor ID in trace.</param>
        public XctendEntry() : base(string.Empty)
        {
            Type = TraceEntryType.Xctend;
        }

        /// <summary>
        /// Timestamp.
        /// Can be used to determine the time between any 2 operations.
        /// In Oracle 9i and above in microseconds, below 9i in centiseconds.
        /// </summary>
        public ULongProperty Timestamp
        {
            get;
            private set;
        }

        /// <summary>
        /// Specifies, whether a database transaction rolled back.
        /// <list type="bullet">
        /// <item>
        ///     <term><see cref="ROLLBACK"/></term>
        ///     <description>Rollback was performed.</description>
        /// </item>
        /// <item>
        ///     <term><see cref="NO_ROLLBACK"/></term>
        ///     <description>Commit.</description>
        /// </item>
        /// </list>
        /// </summary>
        public IntProperty Rollback
        {
            get;
            private set;
        }

        /// <summary>
        /// Specifies, whether a database transaction changed some data.
        /// <list type="bullet">
        /// <item>
        ///     <term><see cref="READ_ONLY"/></term>
        ///     <description>No changes, transaction was read only.</description>
        /// </item>
        /// <item>
        ///     <term><see cref="NO_READ_ONLY"/></term>
        ///     <description>Changes occurred.</description>
        /// </item>
        /// </list>
        /// </summary>
        public IntProperty ReadOnly
        {
            get;
            private set;
        }

        internal override void SetProperties(List<StringProperty> properties)
        {
            foreach (StringProperty p in properties)
            {
                if (p.Name.Equals(Property.ROLLBACK))
                {
                    Rollback = IntProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.READ_ONLY))
                {
                    ReadOnly = IntProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.TIMESTAMP))
                {
                    Timestamp = ULongProperty.Convert(p);
                }
            }
        }
    }
}