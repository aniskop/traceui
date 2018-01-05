using System;
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

        private StreamWriter eventsTempFile;

        private string eventsTempFilePath;
        private string reportFilePath;

        private Dictionary<string, ParsingInCursorEntry> cursorsCache;
        private Dictionary<string, BindsEntry> bindsCache;

        private TraceEntry previousEntry;
        private TraceEntry currentEntry;

        private const string HORIZONTAL_LINE = "=================================================";

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

        public TextReport(string reportPath, TraceParser parser) : this(reportPath, parser, ReportSettings.DefaultSettings)
        {
        }

        public override void Generate()
        {
            GenerateTempFiles();
            OpenTempFilesForWrite();
            cursorsCache.Clear();
            Parser.Parse();
            WriteFooter();
            CloseTempFiles();
            OnProgressChanged(this, 100);
            if (File.Exists(reportFilePath))
            {
                File.Delete(reportFilePath);
            }
            File.Move(eventsTempFilePath, reportFilePath);
        }

        private void SetCurrentEntry(TraceEntry entry)
        {
            previousEntry = currentEntry;
            currentEntry = entry;
        }

        private void WriteNameAndPosition(string name, TraceEntry entry)
        {
            WriteLine(string.Format("{0} (line {1})", name, entry.LineRange.Start));
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
            eventsTempFile.WriteLine(text);
        }

        private void WriteLine(string template, string value)
        {
            eventsTempFile.WriteLine(template, value);
        }

        private void WriteLine(string template, long value)
        {
            eventsTempFile.WriteLine(template, value);
        }

        private void WriteLine(string template, int value)
        {
            eventsTempFile.WriteLine(template, value);
        }

        private void WriteLine(string template, ulong value)
        {
            eventsTempFile.WriteLine(template, value);
        }

        private void NewLine()
        {
            eventsTempFile.WriteLine();
        }

        #region Events listeners
        protected override void Parser_BindsDetected(object sender, BindsEventArgs e)
        {
            BindsEntry entry = e.Value;
            SetCurrentEntry(entry);
            AddBindsToCache(entry);
        }

        protected override void Parser_CloseDetected(object sender, CloseEventArgs e)
        {
            SetCurrentEntry(e.Value);
        }

        protected override void Parser_EndOfTraceReached(object sender, EventArgs e)
        {
        }

        protected override void Parser_ExecDetected(object sender, ExecEventArgs e)
        {
            ExecEntry entry = e.Value;
            SetCurrentEntry(entry);
            ParsingInCursorEntry cursor = GetCursorFromCache(entry.CursorId);

            if (MustBeIncluded(cursor))
            {
                Hr();
                WriteNameAndPosition("EXECUTE", entry);
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
                WriteLine("with params:");
                foreach (BindEntry bind in binds.Binds)
                {
                    if (bind.HasMetadata)
                    {
                        WriteLine(string.Format("  bind{0} = {1}", bind.Index, bind.Value));
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

            if (MustBeIncluded(entry.CursorId))
            {
                Hr();
                WriteNameAndPosition("FETCH", entry);
                WriteLine("  rows            = {0}", entry.Rows.Value.ToString());
                WriteLine("  elapsed         = {0} us", entry.Elapsed.Value.ToString());
                WriteLine("  recursive depth = {0}", entry.Depth.Value.ToString());
                WriteLine("  optimizer goal  = {0}", entry.OptimizerGoal.Value.ToString());
            }
        }

        protected override void Parser_ParseDetected(object sender, ParseEventArgs e)
        {
            ParseEntry entry = e.Value;
            SetCurrentEntry(e.Value);

            ParsingInCursorEntry cursor = GetCursorFromCache(entry.CursorId);

            if (MustBeIncluded(cursor))
            {
                Hr();
                WriteNameAndPosition("PARSE", entry);
                WriteLine("  cursor#          = {0}", cursor.CursorId);
                WriteLine("  sqlid            = {0}", cursor.SqlId.Value);
                WriteLine("  calling user id  = {0}", cursor.Uid.Value);
                WriteLine("  statement length = {0}", cursor.Length.Value);
                NewLine();
                WriteFullStatement(cursor);
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
                WriteNameAndPosition("STATS", entry);
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

            ParsingInCursorEntry cursor = GetCursorFromCache(entry.CursorId);

            if (MustBeIncluded(cursor))
            {
                Hr();
                WriteNameAndPosition("WAIT", entry);
                WriteLine("  name      = {0}", entry.Name.Value);
                //todo: depends on Oracle version, because <9 in centiseconds
                WriteLine("  elapsed   = {0} us", entry.Elapsed.Value);
                WriteLine("  cursor#   = {0}", entry.CursorId);
                WriteLine("  object id = {0}", entry.ObjectId.Value);
                NewLine();

                if (!WaitEntry.SYSTEM_WAIT_CURSOR_ID.Equals(cursor.CursorId))
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
            AddCursorToCache(entry);
            RemoveBindsFromCache(entry.CursorId);
        }

        protected override void Parser_XctendDetected(object sender, XctendEventArgs e)
        {
            XctendEntry entry = e.Value;
            SetCurrentEntry(e.Value);

            Hr();
            WriteNameAndPosition("END OF TRANSACTION", entry);
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

        private void GenerateTempFiles()
        {
            eventsTempFilePath = Path.GetTempFileName();
        }

        private void OpenTempFilesForWrite()
        {
            eventsTempFile = new StreamWriter(eventsTempFilePath);
        }

        private void DeleteTempFiles()
        {
            File.Delete(eventsTempFilePath);
        }

        private void CloseTempFiles()
        {
            if (eventsTempFile != null)
            {
                eventsTempFile.Close();
            }
        }

        private void WriteFooter()
        {
            eventsTempFile.WriteLine();
            eventsTempFile.WriteLine("For details on reading raw trace output refer to \"Interpreting Raw SQL_TRACE output (Doc ID 39817.1)\" at support.oracle.com (ex Metalink).");
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

        private void Hr()
        {
            eventsTempFile.WriteLine(HORIZONTAL_LINE);
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
            if (entry.Statement.Length > STATEMENT_TRIM_LENGTH)
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