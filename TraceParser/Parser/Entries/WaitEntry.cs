using System.Collections.Generic;

namespace TraceUI.Parser.Entries
{
    /// <summary>
    /// Represents a wait event (<code>WAIT</code> entry) in Oracle trace.
    /// </summary>
    public class WaitEntry : TraceEntry
    {
        private Dictionary<string, string> waitProperties;
        private IReadOnlyDictionary<string, string> readOnlyWaitProperties;

        /// <summary>
        /// Value of <see cref="ObjectId"/> when <code>WAIT</code> is not on the database object.
        /// </summary>
        public const int NOT_A_DB_OBJECT = -1;

        /// <summary>
        /// Cursor ID in the <code>WAIT</code> entry, when the system (not the particular statement) is waiting.
        /// </summary>
        public const string SYSTEM_WAIT_CURSOR_ID = "0";

        /// <summary>
        /// Creates an instance of <code>WaitEntry</code>.
        /// </summary>
        /// <param name="cursorId">Cursor ID of the statement.</param>
        public WaitEntry(string cursorId) : base(cursorId)
        {
            Type = TraceEntryType.Wait;
            waitProperties = new Dictionary<string, string>();
            readOnlyWaitProperties = waitProperties;
        }

        /// <summary>
        /// Name of the wait.
        /// The wait events are the same as are in <code>V$SESSION_WAIT</code> view.
        /// Wait event names are in <code>V$EVENT_NAME</code> view.
        /// Value source is <see cref="Property.NAME"/> property.
        /// </summary>
        public StringProperty Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Waiting time.
        /// In Oracle 9i and above in microseconds, below 9i in centiseconds.
        /// <para>Value source is <see cref="Property.ELAPSED"/> property.</para>
        /// </summary>
        public LongProperty Elapsed
        {
            get;
            private set;
        }

        /// <summary>
        /// Database object id as in <code>DBA_OBJECTS.OBJECT_ID</code>.
        /// <see cref="NOT_A_DB_OBJECT"/> if wait is not on/for the database object.
        /// Value source is <see cref="Property.OBJECT_ID2"/> property.
        /// </summary>
        public LongProperty ObjectId
        {
            get;
            private set;
        }

        /// <summary>
        /// Timestamp.
        /// Can be used to determine the time between any 2 operations.
        /// In Oracle 9i and above in microseconds, below 9i in centiseconds.
        /// <para>
        /// Value source is <see cref="Property.TIMESTAMP"/> property.
        /// </para>
        /// </summary>
        public ULongProperty Timestamp
        {
            get;
            private set;
        }

        /// <summary>
        /// Properties, which depend on a type of wait.
        /// Use <see cref="Name"/> to query <code>V$EVENT_NAME</code> for properties of the wait.
        /// </summary>
        public IReadOnlyDictionary<string, string> CustomProperties
        {
            get
            {
                return readOnlyWaitProperties;
            }
        }

        /// <summary>
        /// Determines if the wait is because of the system or some SQL statement.
        /// <list type="bullet">
        /// <item>
        ///     <term>true</term>
        ///     <description>System wait.</description>
        /// </item>
        /// <item>
        ///     <term>false</term>
        ///     <description>Otherwise (some SQL statement is waiting).</description>
        /// </item>
        /// </list>
        /// </summary>
        public bool IsSystemWait
        {
            get
            {
                return (SYSTEM_WAIT_CURSOR_ID.Equals(CursorId));
            }
        }

        internal override void SetProperties(List<StringProperty> properties)
        {
            foreach (StringProperty p in properties)
            {
                if (p.Name.Equals(Property.NAME))
                {
                    Name = p;
                }
                else if (p.Name.Equals(Property.ELAPSED))
                {
                    Elapsed = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.OBJECT_ID2))
                {
                    ObjectId = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.TIMESTAMP))
                {
                    Timestamp = ULongProperty.Convert(p);
                }
                else
                {
                    waitProperties.Add(p.Name, p.Value);
                }
            }
        }
    }
}
