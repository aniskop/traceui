using System.Collections.Generic;

namespace TraceUI.Parser.Entries
{
    /// <summary>
    /// Represents EXEC entry in Oracle trace.
    /// </summary>
    public class ExecEntry : TraceEntry
    {
        public ExecEntry(string cursorId) : base(cursorId)
        {
            Type = TraceEntryType.Exec;
        }

        /// <summary>
        /// CPU time consumed by the statement.
        /// In Oracle 9i and above in microseconds, below 9i in centiseconds.
        /// </summary>
        public LongProperty CpuTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Wall time elapsed during the call.
        /// </summary>
        public LongProperty Elapsed
        {
            get;
            private set;
        }

        /// <summary>
        /// Physical reads performed by the statement.
        /// </summary>
        public LongProperty PhysicalReads
        {
            get;
            private set;
        }

        /// <summary>
        /// Consistent reads performed by the statement.
        /// </summary>
        public LongProperty ConsistentReads
        {
            get;
            private set;
        }

        /// <summary>
        /// Current reads performed by the statement (read of the current content of a block).
        /// </summary>
        public LongProperty CurrentReads
        {
            get;
            private set;
        }

        /// <summary>
        /// Library cache miss (each causes hard parse).
        /// </summary>
        public LongProperty LibraryCacheMiss
        {
            get;
            private set;
        }

        /// <summary>
        /// Number of rows returned.
        /// </summary>
        public LongProperty RowsProcessed
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
        /// </summary>
        public LongProperty Depth
        {
            get;
            private set;
        }

        /// <summary>
        /// Optimizer goal.
        /// See <see cref="Entries.OptimizerGoal"/> for possible values.
        /// </summary>
        public LongProperty OptimizerGoal
        {
            get;
            private set;
        }

        public LongProperty Plh
        {
            get;
            private set;
        }

        /// <summary>
        /// Time when event occured.
        /// </summary>
        public ULongProperty Time
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
                    LibraryCacheMiss = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.ROWS))
                {
                    RowsProcessed = LongProperty.Convert(p);
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
                    Time = ULongProperty.Convert(p);
                }
            }
        }
    }
}
