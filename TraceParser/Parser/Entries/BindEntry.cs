using System.Collections.Generic;

namespace TraceUI.Parser.Entries
{
    /// <summary>
    /// Represents each bind variable in SQL statement binds list.
    /// </summary>
    public class BindEntry : TraceEntry
    {
        /// <summary>
        /// Creates an instance of <see cref="BindEntry"/>.
        /// </summary>
        public BindEntry() : base()
        {
            Type = TraceEntryType.Bind;
        }

        /// <summary>
        /// Data type of the bind variable. See <see cref="OracleDatatype"/> for possible values.
        /// <para>Value source is <see cref="Property.ORACE_DATATYPE"/> property.</para>
        /// </summary>
        public IntProperty Datatype
        {
            get;
            private set;
        }

        /// <summary>
        /// Maximum length of the bind variable.
        /// <para>Value source is <see cref="Property.MAX_LENGTH"/> property.</para>
        /// </summary>
        public LongProperty MaxLength
        {
            get;
            private set;
        }

        /// <summary>
        /// Maximum length of the bind variable.
        /// <para>Value source is <see cref="Property.ACTUAL_VALUE_LENGTH"/> property.</para>
        /// </summary>   
        public LongProperty ActualLength
        {
            get;
            private set;
        }

        /// <summary>
        /// Array length.
        /// <para>Value source is <see cref="Property.ARRAY_LENGTH"/> property.</para>
        /// </summary>
        public LongProperty ArrayLength
        {
            get;
            private set;
        }

        //todo: determine which property is used for that value
        public LongProperty MaxLengthCharacters
        {
            get;
            private set;
        }

        /// <summary>
        /// Scale.
        /// <para>Value source is <see cref="Property.SCALE"/> property.</para>
        /// </summary>
        public LongProperty Scale
        {
            get;
            private set;
        }

        /// <summary>
        /// Precision.
        /// <para>Value source is <see cref="Property.PRECISION"/> property.</para>
        /// </summary>
        public LongProperty Precision
        {
            get;
            private set;
        }

        /// <summary>
        /// Special flag indicating bind options.
        /// <para>Value source is <see cref="Property.ORACLE_FLAG"/> property.</para>
        /// </summary>
        public LongProperty OracleFlag
        {
            get;
            private set;
        }

        //todo: define purpose end source.
        public LongProperty Fl2
        {
            get;
            private set;
        }

        //todo: define purpose end source.
        public LongProperty Frm
        {
            get;
            private set;
        }

        //todo: define purpose end source.
        public LongProperty Csi
        {
            get;
            private set;
        }

        /// <summary>
        /// Amount of memory to be allocated for this chunk.
        /// <para>Value source is <see cref="Property.MEMORY_SIZE"/> property.</para>
        /// </summary>
        public ULongProperty MemorySize
        {
            get;
            private set;
        }

        /// <summary>
        /// Value of the bind variable.
        /// <para>Value source is <see cref="Property.VALUE"/> property.</para>
        /// </summary>
        public string Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Bind variable position.
        /// <para>Value source is the number next to <code>Bind#</code>.</para>
        /// </summary>
        public int Index
        {
            get;
            internal set;
        }

        /// <summary>
        /// Offset into this chunk for this bind buffer.
        /// <para>Value source is <see cref="Property.OFFSET"/> property.</para>
        /// </summary>
        public ULongProperty Offset
        {
            get;
            private set;
        }

        //todo: define purpose end source.
        public string Kxsbbbfp
        {
            get;
            private set;
        }

        /// <summary>
        /// Bind buffer length.
        /// <para>Value source is <see cref="Property.BIND_BUFFER_LENGTH"/> property.</para>
        /// </summary>
        public ULongProperty BindBufferLength
        {
            get;
            private set;
        }

        /// <summary>
        /// Special flag indicating bind status.
        /// <para>Value source is <see cref="Property.STATUS_FLAG"/> property.</para>
        /// </summary>
        public ULongProperty StatusFlag
        {
            get;
            private set;
        }

        /// <summary>
        /// True, if trace contains data about bind variable, false otherwise.
        /// </summary>
        public bool HasMetadata
        {
            get;
            internal set;
        }

        internal override void SetProperties(List<StringProperty> properties)
        {
            int idx = 0;
            foreach (StringProperty p in properties)
            {
                if (p.Name.Equals(Property.ORACE_DATATYPE))
                {
                    Datatype = IntProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.MAX_LENGTH))
                {
                    //todo: parse it correctly
                    int pos = p.Value.IndexOf('(');
                    string maxLenStr = p.Value.Substring(0, pos);
                    string actualLenStr = p.Value.Substring(pos + 1, p.Value.Length - (pos + 1) - 1);

                    MaxLength = LongProperty.Convert(new StringProperty(p.Name, maxLenStr));
                }
                else if (p.Name.Equals(Property.MAX_LENGTH_CHARS))
                {
                    //todo: handle it correctly
                    //MaxLengthCharacters = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.ARRAY_LENGTH))
                {
                    ArrayLength = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.SCALE))
                {
                    Scale = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.PRECISION))
                {
                    Precision = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.ORACLE_FLAG))
                {
                    OracleFlag = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.FL2))
                {
                    Fl2 = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.FRM))
                {
                    Frm = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.CSI))
                {
                    Csi = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.MEMORY_SIZE))
                {
                    MemorySize = ULongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.OFFSET))
                {
                    Offset = ULongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.KXSBBBFP))
                {
                    Kxsbbbfp = p.Value;
                }
                else if (p.Name.Equals(Property.BIND_BUFFER_LENGTH))
                {
                    BindBufferLength = ULongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.ACTUAL_VALUE_LENGTH))
                {
                    ActualLength = LongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.STATUS_FLAG))
                {
                    StatusFlag = ULongProperty.Convert(p);
                }
                else if (p.Name.Equals(Property.VALUE))
                {
                    Value = p.Value;
                }
                Index = idx++;
            }
        }
    }
}
