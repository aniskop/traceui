namespace TraceUI.Reports
{
    public class ReportSettings
    {
        public ReportSettings()
        {
            IncludeSystemQueries = false;
            IncludeWaits = true;
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
