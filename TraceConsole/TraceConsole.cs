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
        private const string COMMAND_SYSTEM = "-s";

        private AsciiProgressBar bar;

        private string traceFilePath;
        private string resultFilePath;

        public TraceConsole()
        {
        }

        public static void Main(string[] args)
        {
            TraceConsole app = new TraceConsole();
            ReportSettings settings = ReportSettings.DefaultSettings;

            app.PrintHeader();

            if (args == null || args.Length == 0)
            {
                app.PrintUsage();
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
            Console.WriteLine("  {0}   Include system queries.", COMMAND_SYSTEM);
        }

        private void ParseArguments(string[] args, out ReportSettings settings)
        {
            settings = ReportSettings.DefaultSettings;
            bool sourceFilePresent = false;
            bool resultFilePresent = false;
            int i = 0;

            while (true)
            {
                if (i >= args.Length)
                {
                    break;
                }

                if (args[i].StartsWith("-"))
                {
                    if (args[i].Equals(COMMAND_SYSTEM))
                    {
                        settings.IncludeSystemQueries = true;
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
                resultFilePath = args[i + 1];
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
    }
}
