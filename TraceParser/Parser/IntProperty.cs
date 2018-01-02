using System;

namespace TraceUI.Parser
{
    /// <summary>
    /// Integer property of the <see cref="TraceEntry"/>.
    /// </summary>
    public struct IntProperty
    {
        /// <summary>
        /// Property name as in <see cref="Property"/>.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Property value.
        /// </summary>
        public readonly int Value;

        /// <summary>
        /// Creates and instance of the property.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public IntProperty(string name, int value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Converts provided <see cref="StringProperty"/> to an instance of <see cref="IntProperty"/>.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static IntProperty Convert(StringProperty p)
        {
            return new IntProperty(p.Name, System.Convert.ToInt32(p.Value));
        }
    }
}
