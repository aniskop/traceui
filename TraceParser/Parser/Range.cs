namespace TraceUI.Parser
{
    public struct Range
    {
        public readonly int Start;
        public readonly int End;
        public readonly int Length;
        public readonly bool Empty;

        public Range(int start, int end)
        {
            Start = start;
            End = end;
            Length = end - start;
            Empty = (Length == 0);
        }

        public static readonly Range EMPTY_RANGE = new Range(-1, -1);
    }
}
