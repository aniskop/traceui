using System;
using TraceUI.Parser;
using TraceUI.Reports.Events;
using TraceUI.Parser.Events;

namespace TraceUI.Reports
{
    /// <summary>
    /// A base class for all reports which are created from a trace file.
    /// Each derived class which is intended to generate a report in particular format (txt, html etc.)
    /// needs to implement the <see cref="Generate"/> method, which is an starting point for report generation,
    /// and methods, which handle parser events. These handlers are automatically registered by calling <see cref="RegisterListeners(TraceParser)"/>
    /// in <see cref="AbstractReport"/> constructor.
    /// </summary>
    public abstract class AbstractReport
    {

        public event EventHandler<ReportProgressChangedEventArgs> ProgressChanged;

        /// <summary>
        /// Creates an instance of report object and registers listeners of parser events by calling <see cref="RegisterListeners(TraceParser)"/>.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="settings"></param>
        protected AbstractReport(TraceParser parser, ReportSettings settings)
        {
            Parser = parser;
            Settings = settings;
            RegisterListeners(Parser);
        }

        /// <summary>
        /// Generates report of desired format.
        /// </summary>
        public abstract void Generate();

        /// <summary>
        /// Returns an instance of the parser, which is used to parse the trace file.
        /// </summary>
        public TraceParser Parser
        {
            protected set;
            get;
        }

        /// <summary>
        /// Returns settings, which are used to generate the report.
        /// </summary>
        public ReportSettings Settings
        {
            protected set;
            get;
        }

        /// <summary>
        /// Registers listeners for parser events.
        /// By default listeners for all events are registered since usually nothing can be missed from a trace file.
        /// Derived classes may override this behaviour to register only events of interest.
        /// </summary>
        /// <param name="p"></param>
        protected virtual void RegisterListeners(TraceParser p)
        {
            p.ParsingInCursorDetected += Parser_ParsingInCursorDetected;
            p.BindsDetected += Parser_BindsDetected;
            p.EndOfTraceReached += Parser_EndOfTraceReached;
            p.CloseDetected += Parser_CloseDetected;
            p.ExecDetected += Parser_ExecDetected;
            p.FetchDetected += Parser_FetchDetected;
            p.WaitDetected += Parser_WaitDetected;
            p.StatDetected += Report_StatDetected;
            p.ParseDetected += Parser_ParseDetected;
            p.XctendDetected += Parser_XctendDetected;
            p.UnrecognizedDetected += Parser_UnrecognizedDetected;
        }

        #region Parser events listeners
        protected abstract void Parser_ParsingInCursorDetected(object sender, ParsingInCursorEventArgs e);

        protected abstract void Parser_BindsDetected(object sender, BindsEventArgs e);

        protected abstract void Parser_EndOfTraceReached(object sender, EventArgs e);

        protected abstract void Parser_CloseDetected(object sender, CloseEventArgs e);

        protected abstract void Parser_ExecDetected(object sender, ExecEventArgs e);

        protected abstract void Parser_FetchDetected(object sender, FetchEventArgs e);

        protected abstract void Parser_WaitDetected(object sender, WaitEventArgs e);

        protected abstract void Report_StatDetected(object sender, StatEventArgs e);

        protected abstract void Parser_ParseDetected(object sender, ParseEventArgs e);

        protected abstract void Parser_XctendDetected(object sender, XctendEventArgs e);

        protected abstract void Parser_UnrecognizedDetected(object sender, UnrecognizedEntryEventArgs e);
        #endregion

        protected virtual void OnProgressChanged(object sender, byte progress)
        {
            ProgressChanged?.Invoke(sender, new ReportProgressChangedEventArgs(progress));
        }
    }
}
