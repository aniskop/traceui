namespace TraceUI.Parser
{
    public struct Range
    {
        public readonly long Start;
        public readonly long End;
        public readonly long Length;
        public readonly bool Empty;

        public Range(long start, long end)
        {
            Start = start;
            End = end;
            Length = end - start;
            Empty = (Length == 0);
        }

        public static readonly Range EMPTY_RANGE = new Range(-1, -1);
    }
}
