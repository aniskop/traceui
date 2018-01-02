using System;

namespace TraceUI
{
    /// <summary>
    /// Unsigned long property of the <see cref="TraceEntry"/>.
    /// </summary>
    public struct ULongProperty
    {
        /// <summary>
        /// Property name as in <see cref="Property"/>.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Property value.
        /// </summary>
        public readonly ulong Value;

        /// <summary>
        /// Creates and instance of the property.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ULongProperty(string name, ulong value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Converts provided <see cref="StringProperty"/> to an instance of <see cref="ULongProperty"/>.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static ULongProperty Convert(StringProperty p)
        {
            return new ULongProperty(p.Name, System.Convert.ToUInt64(p.Value));
        }
    }
}
