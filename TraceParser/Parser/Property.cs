namespace TraceUI.Parser.Entries
{
    /// <summary>
    /// Provides constants for trace entry property names.
    /// </summary>
    public sealed class Property
    {
        /// <summary>
        /// SQL statement text length.
        /// </summary>
        public const string SQL_LENGTH = "len";

        /// <summary>
        /// Recursive depth.
        /// </summary>
        public const string RECURSIVE_DEPTH = "dep";

        /// <summary>
        /// Id of the user, who's calling the statement.
        /// </summary>
        public const string CALLING_USER_ID = "uid";

        /// <summary>
        /// Oracle command type number in OCI.
        /// </summary>
        public const string ORACLE_COMMAND_TYPE = "oct";

        /// <summary>
        /// Id of the user, who owns the database object, where the statement resides.
        /// </summary>
        public const string OWNER_USER_ID = "lid";

        /// <summary>
        /// Timestamp when the statement ocurred.
        /// In Oracle 9i and above in microseconds, below 9i in centiseconds.
        /// </summary>
        public const string TIMESTAMP = "tim";

        /// <summary>
        /// Hash value of the statement.
        /// </summary>
        public const string HASH_VALUE = "hv";

        /// <summary>
        /// Library cache address assigned in V$SQL.
        /// </summary>
        public const string LIBRARY_CACHE_ADDRESS = "ad";

        /// <summary>
        /// Id of the SQL statement.
        /// </summary>
        public const string SQLID = "sqlid";

        /// <summary>
        /// CPU time consumed by the process.
        /// </summary>
        public const string CPU_TIME = "c";

        /// <summary>
        /// Amount of wall time that elapsed during the call.
        /// </summary>
        public const string WALL_TIME_ELAPSED = "e";

        /// <summary>
        /// Physical reads.
        /// </summary>
        public const string PHYSICAL_READS = "p";

        /// <summary>
        /// Consistent reads.
        /// </summary>
        public const string CONSISTENT_READS = "cr";

        /// <summary>
        /// Current reads.
        /// </summary>
        public const string CURRENT_READS = "cu";

        /// <summary>
        /// Library cache miss.
        /// </summary>
        public const string LIBRARY_CACHE_MISS = "mis";

        /// <summary>
        /// Number of rows returned (in FETCH entries).
        /// </summary>
        public const string ROWS = "r";

        /// <summary>
        /// Optimizer goal.
        /// </summary>
        public const string OPTIMIZER_GOAL = "og";

        public const string PLH = "plh";

        public const string TYPE = "type";

        /// <summary>
        /// Name (in <see cref="WaitEntry"/>).
        /// </summary>
        public const string NAME = "nam";

        /// <summary>
        /// Time elapsed.
        /// In Oracle 9i and above in microseconds, below 9i in centiseconds.
        /// </summary>
        public const string ELAPSED = "ela";

        /// <summary>
        /// Number of rows returned (in STAT entries).
        /// </summary>
        public const string ROW_COUNT = "cnt";

        /// <summary>
        /// Database object id.
        /// </summary>
        public const string OBJECT_ID = "obj";

        /// <summary>
        /// Database object id (in <see cref="WaitEntry"/>).
        /// </summary>
        public const string OBJECT_ID2 = "obj#";

        /// <summary>
        /// Unique ID within STAT line set
        /// </summary>
        public const string ID = "id";

        /// <summary>
        /// Number of rows returned
        /// </summary>
        public const string COUNT = "count";

        /// <summary>
        /// Parent ID.
        /// </summary>
        public const string PARENT_ID = "pid";

        /// <summary>
        /// Position in explain plan.
        /// </summary>
        public const string POSITION = "pos";

        /// <summary>
        /// Row source access operation.
        /// </summary>
        public const string OPERATION = "op";

        /// <summary>
        /// OCI data type id.
        /// </summary>
        public const string ORACE_DATATYPE = "oacdty";

        /// <summary>
        /// Value.
        /// </summary>
        public const string VALUE = "value";

        /// <summary>
        /// Maximum length for this datatype.
        /// </summary>
        public const string MAX_LENGTH = "mxl";

        //todo: define purpose.
        public const string MAX_LENGTH_CHARS = "mxlc";

        /// <summary>
        /// Array length.
        /// </summary>
        public const string ARRAY_LENGTH = "mal";

        /// <summary>
        /// Scale.
        /// </summary>
        public const string SCALE = "scl";

        /// <summary>
        /// Precision.
        /// </summary>
        public const string PRECISION = "pre";

        /// <summary>
        /// Special flag indicating bind options.
        /// </summary>
        public const string ORACLE_FLAG = "oacflg";

        //todo: define purpose.
        public const string FL2 = "fl2";

        //todo: define purpose.
        public const string FRM = "frm";

        //todo: define purpose.
        public const string CSI = "csi";

        /// <summary>
        /// Amount of memory to be allocated.
        /// </summary>
        public const string MEMORY_SIZE = "siz";

        /// <summary>
        /// Offset.
        /// </summary>
        public const string OFFSET = "off";

        //todo: define purpose.
        public const string KXSBBBFP = "kxsbbbfp";

        /// <summary>
        /// Binds buffer length.
        /// </summary>
        public const string BIND_BUFFER_LENGTH = "bln";

        /// <summary>
        /// Actual value length.
        /// </summary>
        public const string ACTUAL_VALUE_LENGTH = "avl";

        /// <summary>
        /// Special flag indicating bind status.
        /// </summary>
        public const string STATUS_FLAG = "flg";

        /// <summary>
        /// Whether transaction rolled back.
        /// </summary>
        public const string ROLLBACK = "rlbk";

        /// <summary>
        /// Whether transaction changed any data.
        /// </summary>
        public const string READ_ONLY = "rd_only";

        private Property()
        {
        }
    }
}
