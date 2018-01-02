namespace TraceUI.Reports.Events
{
    public class ReportProgressChangedEventArgs
    {
        public ReportProgressChangedEventArgs(byte currentProgress)
        {
            Progress = currentProgress;
        }

        public byte Progress
        {
            private set;
            get;
        }
    }
}
