using System;

namespace TraceUI.Parser
{
    /// <summary>
    /// Long property of the <see cref="TraceEntry"/>.
    /// </summary>
    public struct LongProperty
    {
        /// <summary>
        /// Property name as in <see cref="Property"/>.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Property value.
        /// </summary>
        public readonly long Value;

        /// <summary>
        /// Creates and instance of the property.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public LongProperty(string name, long value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Converts provided <see cref="StringProperty"/> to an instance of <see cref="LongProperty"/>.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static LongProperty Convert(StringProperty p)
        {
            return new LongProperty(p.Name, System.Convert.ToInt64(p.Value));
        }
    }
}
