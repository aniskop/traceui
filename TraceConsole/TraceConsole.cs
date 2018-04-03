using System;
using TraceUI.Reports;
using TraceUI.Parser;
using TraceUI.Reports.Events;
using System.IO;
using System.Reflection;


namespace TraceUI.CommandLine
{

    public class TraceConsole
    {
        private const string INCLUDE_SYSTEM_CALLS_OPTION = "--sys";
        private const string INCLUDE_WAITS_OPTION = "--wait";

        private AsciiProgressBar bar;

        private string traceFilePath;
        private string resultFilePath;

        public TraceConsole()
        {
        }

        public static void Main(string[] args)
        {
            TraceConsole app = new TraceConsole();
            ReportSettings settings = new ReportSettings();

            app.PrintHeader();

            if (args == null || args.Length == 0)
            {
                app.PrintUsage();
#if DEBUG
                Console.ReadKey();
#endif
                Environment.Exit(0);
            }
            else
            {
                app.ParseArguments(args, out settings);
            }

            Console.WriteLine("Processing:  {0}", app.traceFilePath);
            Console.WriteLine("Result file: {0}", app.resultFilePath);
            app.bar = new AsciiProgressBar(Console.CursorTop + 1, Console.CursorLeft + 0);

            AbstractReport rep = new TextReport(app.resultFilePath, new TraceParser(new TraceFileLexer(app.traceFilePath)), settings);
            rep.ProgressChanged += app.Report_ProgressChanged;
            rep.Generate();
#if DEBUG
            Console.ReadKey();
#endif
            Environment.Exit(0);
        }

        public void Report_ProgressChanged(object sender, ReportProgressChangedEventArgs e)
        {
            bar.Progress = e.Progress;
        }

        private void PrintUsage()
        {
            Console.WriteLine("Converts Oracle trace file into more readable format.\n");
            Console.WriteLine("Usage: traceuic [options] trace_file_path [result_file_path]\n");
            Console.WriteLine("Options:");
            Console.WriteLine(("  " + INCLUDE_SYSTEM_CALLS_OPTION + "=yes|no").PadRight(20, ' ') + "Include system queries. Default is no.");
            Console.WriteLine(("  " + INCLUDE_WAITS_OPTION + "=yes|no").PadRight(20, ' ') + "Include wait events or no. Default is no.");
        }

        private void ParseArguments(string[] args, out ReportSettings settings)
        {
            settings = new ReportSettings();
            bool sourceFilePresent = false;
            bool resultFilePresent = false;
            int i = 0;

            while (true)
            {
                if (i >= args.Length)
                {
                    break;
                }

                if (args[i].StartsWith("--"))
                {
                    if (args[i].StartsWith(INCLUDE_SYSTEM_CALLS_OPTION))
                    {
                        settings.IncludeSystemQueries = ParseBooleanProperty(args[i]);
                    }
                    else if (args[i].StartsWith(INCLUDE_WAITS_OPTION))
                    {
                        settings.IncludeWaits = ParseBooleanProperty(args[i]);
                    }
                    i++;
                }
                else
                {
                    break;
                }
            }

            if (i < args.Length)
            {
                traceFilePath = args[i];
                sourceFilePresent = true;
                i++;
            }

            if (i < args.Length)
            {
                resultFilePath = args[i];
                resultFilePresent = true;
            }
            else
            {
                resultFilePath = GenerateReportFilePath(traceFilePath);
                resultFilePresent = true;
            }

            if (!sourceFilePresent || !resultFilePresent)
            {
                //todo: raise error - insufficient parameters
            }
        }

        private string GenerateReportFilePath(string sourceFilePath)
        {
            string directory = Path.GetDirectoryName(sourceFilePath);
            if (string.IsNullOrEmpty(directory))
            {
                directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            }
            string name = Path.GetFileNameWithoutExtension(sourceFilePath);
            return directory + "\\" + name + ".report.txt";
        }

        private void PrintHeader()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)currentAssembly.GetCustomAttribute(typeof(AssemblyTitleAttribute));
            Console.WriteLine("{0} version {1}", titleAttribute.Title, currentAssembly.GetName().Version.ToString());
            Console.WriteLine();
        }

        private bool ParseBooleanProperty(string property)
        {
            string[] tokens = property.Split('=');
            return (tokens.Length == 2) ? tokens[1].Equals("yes", StringComparison.OrdinalIgnoreCase) : false;
        }
    }
}
