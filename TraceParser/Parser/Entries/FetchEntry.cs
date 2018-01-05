using System.Collections.Generic;

namespace TraceUI.Parser.Entries
{
    /// <summary>
    /// Represents a fetch operation (<code>FETCH</code> entry) in Oracle trace.
    /// </summary>
    public class FetchEntry : TraceEntry
    {
        /// <summary>
        /// Creates an instance of <see cref="FetchEntry"/>.
        /// </summary>
        /// <param name="cursorId">Cursor ID of the statement.</param>
        public FetchEntry(string cursorId) : base(cursorId)
        {
            Type = TraceEntryType.Fetch;
        }

        /// <summary>
        /// CPU time consumed by the statement.
        /// In Oracle 9i and above in microseconds, below 9i in centiseconds.
        /// Value source is <see cref="Property.CPU_TIME"/> property.
        /// </summary>
        public LongProperty CpuTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Time consumed by the operation.
        /// In Oracle 9i and above in microseconds, below 9i in centiseconds.
        /// Value source is <see cref="Property.WALL_TIME_ELAPSED"/> property.
        /// </summary>
        public LongProperty Elapsed
        {
            get;
            private set;
        }

        /// <summary>
        /// Number of physical reads.
        /// Value source is <see cref="Property.PHYSICAL_READS"/> property.
        /// </summary>
        public LongProperty PhysicalReads
        {
            get;
            private set;
        }

        /// <summary>
        /// Number of buffers retrieved for consistent reads.
        /// Value source is <see cref="Property.CONSISTENT_READS"/> property.
        /// </summary>
        public LongProperty ConsistentReads
        {
            get;
            private set;
        }

        /// <summary>
        /// Number of buffers retrieved in current mode.
        /// Value source is <see cref="Property.CURRENT_READS"/> property.
        /// </summary>
        public LongProperty CurrentReads
        {
            get;
            private set;
        }

        /// <summary>
        /// Cursor missed in the cache.
        /// Value source is <see cref="Property.LIBRARY_CACHE_MISS"/> property.
        /// </summary>
        public LongProperty CursorMissed
        {
            get;
            private set;
        }

        /// <summary>
        /// Number of rows fetched.
        /// Value source is <see cref="Property.ROWS"/> property.
        /// </summary>
        public LongProperty Rows
        {
            get;
            private set;
        }

        /// <summary>
        /// Recursive call depth:
        /// <list type="bullet">
        /// <item>
        ///     <term>0</term>
        ///     <description>User call.</description>
        /// </item>
        /// <item>
        ///     <term>&gt;0</term>
        ///     <description>Recursive call.</description>
        /// </item>
        /// </list>
        /// Value source is <see cref="Property.RECURSIVE_DEPTH"/> property.
        /// </summary>
        public LongProperty Depth
        {
            get;
            private set;
        }

        /// <summary>
        /// Optimizer goal.
        /// See <see cref="Entries.OptimizerGoal"/> for possible values.
        /// Value source is <see cref="Property.OPTIMIZER_GOAL"/> property.
        /// </summary>
        public LongProperty OptimizerGoal
        {
            get;
            private set;
        }

        //todo: define purpose
        public LongProperty Plh
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

        internal override void SetProperties(List<StringProperty> properties)
        {
            foreach (StringProperty p in properties)
            {
                if (p.Name.Equals(Property.CPU_TIME))
                {
                    CpuTime = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.WALL_TIME_ELAPSED))
                {
                    Elapsed = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.PHYSICAL_READS))
                {
                    PhysicalReads = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.CONSISTENT_READS))
                {
                    ConsistentReads = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.CURRENT_READS))
                {
                    CurrentReads = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.LIBRARY_CACHE_MISS))
                {
                    CursorMissed = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.ROWS))
                {
                    Rows = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.RECURSIVE_DEPTH))
                {
                    Depth = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.OPTIMIZER_GOAL))
                {
                    OptimizerGoal = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.PLH))
                {
                    Plh = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.TIMESTAMP))
                {
                    Timestamp = ULongProperty.Convert(p);
                }
            }
        }
    }
}
