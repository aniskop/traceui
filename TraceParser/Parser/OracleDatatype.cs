using System;

namespace TraceUI.Parser
{
    /// <summary>
    /// Oracle data type IDs and API to translate ID to a datatype name. 
    /// Source of values is "Call Interface Programmer's Guide", section "Data types".
    /// </summary>
    public sealed class OracleDatatype
    {
        public const int VARCHAR2_OR_NVARCHAR2 = 1;
        public const int NUMBER = 2;
        public const int LONG = 8;
        public const int DATE = 12;
        public const int RAW = 23;
        public const int LONG_RAW = 24;
        public const int ROWID = 69;
        public const int CHAR_OR_NCHAR = 96;
        public const int BINARY_FLOAT = 100;
        public const int BINARY_DOUBLE = 101;
        public const int USER_DEFINED_TYPE_OR_COLLECTION = 108;
        public const int REF = 111;
        public const int CLOB_OR_NCLOB = 112;
        public const int BLOB = 113;
        public const int BFILE = 114;
        public const int TIMESTAMP = 180;
        public const int TIMESTAMP_WITH_TIME_ZONE = 181;
        public const int INTERVAL_YEAR_TO_MONTH = 182;
        public const int INTERVAL_DAY_TO_SECOND = 183;
        public const int UROWID = 208;
        public const int TIMESTAMP_WITH_LOCAL_TIME_ZONE = 231;

        private OracleDatatype()
        {
        }

        public static string ToString(int id)
        {
            switch (id)
            {
                case VARCHAR2_OR_NVARCHAR2:
                    return "VARCHAR2 / NVARCHAR2";
                case NUMBER:
                    return "NUMBER";
                case LONG:
                    return "LONG";
                case DATE:
                    return "DATE";
                case RAW:
                    return "RAW";
                case LONG_RAW:
                    return "LONG RAW";
                case ROWID:
                    return "ROWID";
                case CHAR_OR_NCHAR:
                    return "CHAR / NCHAR";
                case BINARY_FLOAT:
                    return "BINARY FLOAT";
                case BINARY_DOUBLE:
                    return "BINARY DOUBLE";
                case USER_DEFINED_TYPE_OR_COLLECTION:
                    return "User defined type / object type / collection";
                case REF:
                    return "REF";
                case CLOB_OR_NCLOB:
                    return "CLOB";
                case BLOB:
                    return "BLOB";
                case BFILE:
                    return "BFILE";
                case TIMESTAMP:
                    return "TIMESTAMP";
                case TIMESTAMP_WITH_TIME_ZONE:
                    return "TIMESTAMP WITH TIME ZONE";
                case INTERVAL_YEAR_TO_MONTH:
                    return "INTERVAL YEAR TO MONTH";
                case INTERVAL_DAY_TO_SECOND:
                    return "INTERVAL DAY TO SECOND";
                case UROWID:
                    return "UROWID";
                case TIMESTAMP_WITH_LOCAL_TIME_ZONE:
                    return "TIMESTAMP WITH LOCAL TIME ZONE";

                default:
                    return Convert.ToString(id);
            }
        }

    }
}
