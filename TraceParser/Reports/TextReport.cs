﻿using System;
using System.Collections.Generic;
using TraceUI.Parser;
using System.IO;
using TraceUI.Parser.Events;
using TraceUI.Parser.Entries;


namespace TraceUI.Reports
{

    public class TextReport : AbstractReport
    {

        private const int STATEMENT_TRIM_LENGTH = 100;
        private static readonly ULongProperty EMPTY_TIMESTAMP = new ULongProperty("", 0UL);

        private StreamWriter reportFile;
        private string reportFilePath;

        private Dictionary<string, ParsingInCursorEntry> cursorsCache;
        private Dictionary<string, BindsEntry> bindsCache;

        private TraceEntry previousEntry;
        private TraceEntry currentEntry;

        private ulong traceStartTimestamp = 0UL;

        private const string HORIZONTAL_LINE = "=================================================";

        private const long SECOND = 1000000L; // Microseconds in 1 second
        private const long SECONDS_IN_MINUTE = 60L;
        private const long SECONDS_IN_HOUR = 60L * SECONDS_IN_MINUTE; // Seconds in 1 hour

        /// <summary>
        /// Creates an instance of text report generator.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="settings"></param>
        public TextReport(string reportPath, TraceParser parser, ReportSettings settings) : base(parser, settings)
        {
            reportFilePath = reportPath;
            cursorsCache = new Dictionary<string, ParsingInCursorEntry>();
            bindsCache = new Dictionary<string, BindsEntry>();
            Parser.ProgressChanged += Parser_ProgressChanged;
        }

        public TextReport(string reportPath, TraceParser parser) : this(reportPath, parser, new ReportSettings())
        {
        }

        public override void Generate()
        {
            OpenReportFileForWrite();
            cursorsCache.Clear();
            Parser.Parse();
            WriteFooter();
            CloseReportFile();
            OnProgressChanged(this, 100);
        }

        private void SetCurrentEntry(TraceEntry entry)
        {
            previousEntry = currentEntry;
            currentEntry = entry;
        }

        #region Output writers and auxiliary API
        private void WriteEntryHeader(string name, TraceEntry entry, ULongProperty entryTimestamp)
        {
            if (entryTimestamp.Value != EMPTY_TIMESTAMP.Value)
            {
                WriteLine(string.Format("{0} (line {1}, time offset {2})", name, entry.LineRange.Start, FormatDuration((long)(entryTimestamp.Value - traceStartTimestamp))));
            }
            else
            {
                // Entry has no timestamp
                WriteLine(string.Format("{0} (line {1})", name, entry.LineRange.Start));
            }
        }

        private void WriteEntryHeader(string name, TraceEntry entry)
        {
            WriteEntryHeader(name, entry, EMPTY_TIMESTAMP);
        }

        private string FormatDuration(long durationMicroSeconds)
        {
            long hours = 0;
            long minutes = 0;
            long seconds = 0;
            int microSeconds = 0;

            if (durationMicroSeconds < SECOND)
            {
                microSeconds = (int)durationMicroSeconds;
            }
            else
            {
                microSeconds = (int)(durationMicroSeconds % SECOND);
                seconds = durationMicroSeconds / SECOND;

                if (seconds >= SECONDS_IN_HOUR)
                {
                    hours = Math.DivRem(seconds, SECONDS_IN_HOUR, out seconds);
                }
                if (seconds >= SECONDS_IN_MINUTE)
                {
                    minutes = Math.DivRem(seconds, SECONDS_IN_MINUTE, out seconds);
                }
            }

            return string.Format("{0}:{1}:{2}.{3}", hours, minutes, seconds, Convert.ToString(microSeconds).PadLeft(6, '0'));
        }

        private void AddCursorToCache(ParsingInCursorEntry entry)
        {
            if (cursorsCache.ContainsKey(entry.CursorId))
            {
                cursorsCache.Remove(entry.CursorId);
            }
            cursorsCache.Add(entry.CursorId, entry);
        }

        private void RemoveCursorFromCache(ParsingInCursorEntry entry)
        {
            if (cursorsCache.ContainsKey(entry.CursorId))
            {
                cursorsCache.Remove(entry.CursorId);
            }
        }

        private ParsingInCursorEntry GetCursorFromCache(string cursorId)
        {
            if (cursorsCache.ContainsKey(cursorId))
            {
                return cursorsCache[cursorId];
            }
            else
            {
                ParsingInCursorEntry fakeEntry = new ParsingInCursorEntry(cursorId);
                fakeEntry.Statement = "Statement is not in this trace file.";
                cursorsCache.Add(cursorId, fakeEntry);
                return fakeEntry;
            }
        }

        private void AddBindsToCache(BindsEntry entry)
        {
            if (bindsCache.ContainsKey(entry.CursorId))
            {
                bindsCache.Remove(entry.CursorId);
            }
            bindsCache.Add(entry.CursorId, entry);
        }

        private void RemoveBindsFromCache(BindsEntry entry)
        {
            RemoveBindsFromCache(entry.CursorId);
        }

        private void RemoveBindsFromCache(string cursorId)
        {
            if (bindsCache.ContainsKey(cursorId))
            {
                bindsCache.Remove(cursorId);
            }
        }

        private void WriteLine(string text)
        {
            reportFile.WriteLine(text);
        }

        private void WriteLine(string template, string value)
        {
            reportFile.WriteLine(template, value);
        }

        private void WriteLine(string template, long value)
        {
            reportFile.WriteLine(template, value);
        }

        private void WriteLine(string template, int value)
        {
            reportFile.WriteLine(template, value);
        }

        private void WriteLine(string template, ulong value)
        {
            reportFile.WriteLine(template, value);
        }

        private void NewLine()
        {
            reportFile.WriteLine();
        }

        private void SetTraceStartTimestamp(ULongProperty timestamp)
        {
            if (traceStartTimestamp == EMPTY_TIMESTAMP.Value)
            {
                traceStartTimestamp = timestamp.Value;
            }
        }
        #endregion

        #region Events listeners
        protected override void Parser_BindsDetected(object sender, BindsEventArgs e)
        {
            BindsEntry entry = e.Value;
            SetCurrentEntry(entry);
            AddBindsToCache(entry);
        }

        protected override void Parser_CloseDetected(object sender, CloseEventArgs e)
        {
            CloseEntry entry = e.Value;
            SetCurrentEntry(entry);
            SetTraceStartTimestamp(entry.Timestamp);
        }

        protected override void Parser_EndOfTraceReached(object sender, EventArgs e)
        {
        }

        protected override void Parser_ExecDetected(object sender, ExecEventArgs e)
        {
            ExecEntry entry = e.Value;
            SetCurrentEntry(entry);
            SetTraceStartTimestamp(entry.Timestamp);

            ParsingInCursorEntry cursor = GetCursorFromCache(entry.CursorId);

            if (MustBeIncluded(cursor))
            {
                Hr();
                WriteEntryHeader("EXECUTE", entry, entry.Timestamp);
                WriteLine("  cursor#        = {0}", entry.CursorId);
                WriteLine("  sqlid          = {0}", cursor.SqlId.Value);
                WriteLine("  elapsed        = {0} us", entry.Elapsed.Value);
                WriteLine("  rows processed = {0}", entry.RowsProcessed.Value);

                NewLine();
                WriteFullStatement(cursor);

                if (bindsCache.ContainsKey(entry.CursorId))
                {
                    BindsEntry binds = bindsCache[entry.CursorId];
                    WriteParams(binds);
                }
            }
        }

        private void WriteParams(BindsEntry binds)
        {
            if (binds != null && binds.Binds != null)
            {
                WriteEntryHeader("with params", binds);
                foreach (BindEntry bind in binds.Binds)
                {
                    if (bind.HasMetadata)
                    {
                        if (bind.Value == null || "".Equals(bind.Value))
                        {
                            WriteLine("  bind{0} is null", bind.Index);
                        }
                        else
                        {
                            WriteLine(string.Format("  bind{0} = {1}", bind.Index, bind.Value));
                        }
                        WriteLine("    datatype      = {0}", OracleDatatype.ToString(bind.Datatype.Value));
                        WriteLine("    actual length = {0} bytes", bind.ActualLength.Value);
                        WriteLine("    max length    = {0} bytes", bind.MaxLength.Value);
                    }
                    else
                    {
                        WriteLine("  bind{0}", bind.Index);
                        WriteLine("    No data about bind.");
                    }
                    NewLine();
                }
            }
        }

        private void WriteCommitRollback(XctendEntry entry)
        {
            WriteLine("  rollback  = {0}", (entry.Rollback.Value == XctendEntry.ROLLBACK) ? "yes" : "no (commit)");
            WriteLine("  read only = {0}", (entry.ReadOnly.Value == XctendEntry.READ_ONLY) ? "yes (no data changed)" : "no (data has changed)");
        }

        protected override void Parser_FetchDetected(object sender, FetchEventArgs e)
        {
            FetchEntry entry = e.Value;
            SetCurrentEntry(e.Value);
            SetTraceStartTimestamp(entry.Timestamp);

            if (MustBeIncluded(entry.CursorId))
            {
                Hr();
                WriteEntryHeader("FETCH", entry, entry.Timestamp);
                WriteLine("  rows            = {0}", entry.Rows.Value.ToString());
                WriteLine("  cursor#         = {0}", entry.CursorId);
                WriteLine("  elapsed         = {0} us", entry.Elapsed.Value.ToString());
                WriteLine("  recursive depth = {0}", entry.Depth.Value.ToString());
                WriteLine("  optimizer goal  = {0}", OptimizerGoal.ToString(entry.OptimizerGoal.Value));
                NewLine();
                WriteTrimmedStatement(GetCursorFromCache(entry.CursorId));
            }
        }

        protected override void Parser_ParseDetected(object sender, ParseEventArgs e)
        {
            ParseEntry entry = e.Value;
            SetCurrentEntry(e.Value);
            SetTraceStartTimestamp(entry.Timestamp);

            ParsingInCursorEntry cursor = GetCursorFromCache(entry.CursorId);

            if (MustBeIncluded(cursor))
            {
                Hr();
                WriteEntryHeader("PARSE", entry, entry.Timestamp);
                WriteLine("  cursor#        = {0}", cursor.CursorId);
                WriteLine("  sqlid          = {0}", cursor.SqlId.Value);
                WriteLine("  elapsed        = {0} us", entry.Elapsed.Value);
                WriteLine("  optimizer goal = {0}", OptimizerGoal.ToString(entry.Og.Value));
                NewLine();
                WriteTrimmedStatement(cursor);
            }
        }

        protected override void Report_StatDetected(object sender, StatEventArgs e)
        {
            StatEntry entry = e.Value;
            SetCurrentEntry(entry);

            ParsingInCursorEntry cursor = GetCursorFromCache(entry.CursorId);

            if (MustBeIncluded(cursor))
            {
                NewLine();
                WriteEntryHeader("STATS", entry);
                WriteLine("  cursor# = {0}", entry.CursorId);
                NewLine();

                if (entry.Lines != null)
                {
                    foreach (StatEntryLine line in entry.Lines)
                    {
                        WriteLine(line.Operation.Value.PadLeft(line.Operation.Value.Length + (2 * (int)line.ParentId.Value)));
                    }
                }
            }
        }

        protected override void Parser_WaitDetected(object sender, WaitEventArgs e)
        {
            WaitEntry entry = e.Value;
            SetCurrentEntry(e.Value);
            SetTraceStartTimestamp(entry.Timestamp);

            ParsingInCursorEntry cursor = GetCursorFromCache(entry.CursorId);

            if (MustBeIncluded(entry) && MustBeIncluded(cursor))
            {
                Hr();
                WriteEntryHeader("WAIT", entry, entry.Timestamp);
                WriteLine("  name      = {0}", entry.Name.Value);
                //todo: depends on Oracle version, because <9 in centiseconds
                WriteLine("  elapsed   = {0} us", entry.Elapsed.Value);
                WriteLine("  cursor#   = {0}", entry.CursorId);
                WriteLine("  object id = {0}", entry.ObjectId.Value);
                NewLine();

                if (!entry.IsSystemWait)
                {
                    WriteTrimmedStatement(cursor);
                    NewLine();
                }
            }
        }

        protected override void Parser_ParsingInCursorDetected(object sender, ParsingInCursorEventArgs e)
        {
            ParsingInCursorEntry entry = e.Value;
            SetCurrentEntry(entry);
            SetTraceStartTimestamp(entry.Timestamp);
            AddCursorToCache(entry);
            RemoveBindsFromCache(entry.CursorId);

            if (MustBeIncluded(entry))
            {
                Hr();
                WriteEntryHeader("PARSING IN CURSOR", entry, entry.Timestamp);
                WriteLine("  cursor#          = {0}", entry.CursorId);
                WriteLine("  sqlid            = {0}", entry.SqlId.Value);
                WriteLine("  hash value       = {0}", entry.HashValue.Value);
                WriteLine("  address          = {0}", entry.Address.Value);
                WriteLine("  calling user id  = {0}", entry.CallingUserId.Value);
                WriteLine("  owning user id   = {0}", entry.OwningUserId.Value);
                WriteLine("  statement length = {0}", entry.Length.Value);
                NewLine();
                WriteFullStatement(entry);
            }
        }

        protected override void Parser_XctendDetected(object sender, XctendEventArgs e)
        {
            XctendEntry entry = e.Value;
            SetCurrentEntry(e.Value);
            SetTraceStartTimestamp(entry.Timestamp);

            Hr();
            WriteEntryHeader("END OF TRANSACTION", entry, entry.Timestamp);
            WriteCommitRollback(entry);
        }

        protected override void Parser_UnrecognizedDetected(object sender, UnrecognizedEntryEventArgs e)
        {
            UnrecognizedEntry entry = e.Value;
            SetCurrentEntry(entry);

            if (!entry.Text.StartsWith("====================="))
            {
                WriteLine(entry.Text);
            }
        }
        #endregion

        private void OpenReportFileForWrite()
        {
            reportFile = new StreamWriter(reportFilePath);
        }

        private void CloseReportFile()
        {
            if (reportFile != null)
            {
                reportFile.Close();
            }
        }

        private void WriteFooter()
        {
            reportFile.WriteLine();
            reportFile.WriteLine("For details on reading raw trace output refer to \"Interpreting Raw SQL_TRACE output (Doc ID 39817.1)\" at support.oracle.com (ex Metalink).");
        }
        /// <summary>
        /// Checks whether information realated to given cursor must be inluded in the report.
        /// </summary>
        /// <param name="cursorId">Cursor id from the trace file.</param>
        /// <returns>True - must be included in report, false - otherwise.</returns>
        private bool MustBeIncluded(ParsingInCursorEntry entry)
        {
            return (!entry.IsSystemCursor || (entry.IsSystemCursor && Settings.IncludeSystemQueries) || entry.CursorId == WaitEntry.SYSTEM_WAIT_CURSOR_ID);
        }

        private bool MustBeIncluded(string cursorId)
        {
            return MustBeIncluded(GetCursorFromCache(cursorId));
        }

        private bool MustBeIncluded(WaitEntry entry)
        {
            return Settings.IncludeWaits;
        }

        private void Hr()
        {
            reportFile.WriteLine(HORIZONTAL_LINE);
        }

        private void Parser_ProgressChanged(object sender, ParserProgressChangedEventArgs e)
        {
            // Rethrow parser event
            OnProgressChanged(this, e.Progress);
        }

        private int Progress
        {
            get;
            set;
        }

        private void WriteFullStatement(ParsingInCursorEntry entry)
        {
            WriteLine(entry.Statement);
        }

        private void WriteTrimmedStatement(ParsingInCursorEntry entry)
        {
            if (string.IsNullOrEmpty(entry.Statement))
            {
                WriteLine("  < Statement text not in this trace file. >");
            }
            else if (entry.Statement.Length > STATEMENT_TRIM_LENGTH)
            {
                WriteLine(entry.Statement.Substring(0, STATEMENT_TRIM_LENGTH) + "...");
            }
            else
            {
                WriteLine(entry.Statement);
            }

        }
    }
}