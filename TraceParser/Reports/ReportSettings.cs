namespace TraceUI.Reports
{
    public class ReportSettings
    {
        private static ReportSettings defaultSettings;

        public ReportSettings()
        {
        }

        public bool IncludeSystemQueries
        {
            get;
            set;
        }

        public static ReportSettings DefaultSettings
        {
            get
            {

                if (defaultSettings == null)
                {
                    defaultSettings = new ReportSettings();
                    defaultSettings.IncludeSystemQueries = false;
                }
                return defaultSettings;
            }
        }
    }
}
