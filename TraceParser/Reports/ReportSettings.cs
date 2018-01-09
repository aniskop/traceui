namespace TraceUI.Reports
{
    public class ReportSettings
    {
        public ReportSettings()
        {
            IncludeSystemQueries = false;
            IncludeWaits = false;
        }

        public bool IncludeSystemQueries
        {
            get;
            set;
        }

        public bool IncludeWaits
        {
            get;
            set;
        }
    }
}
