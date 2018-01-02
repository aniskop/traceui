namespace TraceUI.Parser.Events
{
    public class ParserProgressChangedEventArgs
    {
        public ParserProgressChangedEventArgs(byte progress)
        {
            Progress = progress;
        }

        public byte Progress
        {
            private set;
            get;
        }
    }
}
