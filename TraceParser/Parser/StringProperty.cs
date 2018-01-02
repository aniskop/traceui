namespace TraceUI
{
    /// <summary>
    /// String property of the <see cref="TraceEntry"/>.
    /// </summary>
    public struct StringProperty
    {
        /// <summary>
        /// Property name as in <see cref="Property"/>.
        /// </summary>
        public readonly string Name;
        
        /// <summary>
        /// Property value.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Creates an instance of the property.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public StringProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
