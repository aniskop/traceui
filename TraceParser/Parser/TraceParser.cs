using System;
using System.Collections.Generic;
using System.Text;
using TraceUI.Parser.Entries;
using TraceUI.Parser.Events;
using System.IO;

namespace TraceUI.Parser
{
    public class TraceParser
    {
        private const string PARSING_IN_CURSOR = "PARSING IN CURSOR #";
        private const string END_OF_STATEMENT = "END OF STMT";
        private const string PARSE = "PARSE #";
        private const string EXEC = "EXEC #";
        private const string FETCH = "FETCH #";
        private const string STAT = "STAT #";
        private const string CLOSE = "CLOSE #";
        private const string BINDS = "BINDS #";
        private const string BIND = " Bind#";
        private const string WAIT = "WAIT #";
        private const string XCTEND = "XCTEND ";

        private const string META = "*** ";
        private const string RPC_CALL = "RPC CALL:";
        private const string RPC_BINDS = "RPC BINDS:";
        private const string RPC_EXEC = "RPC EXEC:";
        private const string DOUBLE_SPACE = "  ";

        private const string NO_DATA_FOR_BIND = "  No oacdef for this bind.";

        private TraceLexer lexer;

        #region Events
        public event EventHandler<BindsEventArgs> BindsDetected;
        public event EventHandler<ParsingInCursorEventArgs> ParsingInCursorDetected;
        public event EventHandler<ExecEventArgs> ExecDetected;
        public event EventHandler<CloseEventArgs> CloseDetected;
        public event EventHandler<FetchEventArgs> FetchDetected;
        public event EventHandler<WaitEventArgs> WaitDetected;
        public event EventHandler<StatEventArgs> StatDetected;
        public event EventHandler<ParseEventArgs> ParseDetected;
        public event EventHandler<XctendEventArgs> XctendDetected;
        public event EventHandler<UnrecognizedEntryEventArgs> UnrecognizedDetected;
        public event EventHandler EndOfTraceReached;

        public event EventHandler<ParserProgressChangedEventArgs> ProgressChanged;
        #endregion

        public TraceParser(TraceLexer lexer)
        {
            this.lexer = lexer;
            TraceSize = -1;

            if (lexer is TraceFileLexer)
            {
                TraceFileLexer l = (TraceFileLexer)lexer;
                FileInfo traceFileInfo = new FileInfo(l.FilePath);
                TraceSize = traceFileInfo.Length;
            }
        }

        /// <summary>
        /// Parses the entire trace file from top to bottom.
        /// Raises the events when a valid trace file entry is detected.
        /// </summary>
        public void Parse()
        {
            byte newProgress = 0;

            while (!lexer.EndOfSource())
            {
                lexer.NextLine();

                newProgress = (byte)(lexer.CurrentLinePositionStart * 100 / TraceSize);
                if (newProgress != Progress)
                {
                    Progress = newProgress;
                    OnProgressChanged(new ParserProgressChangedEventArgs(Progress));
                }

                if (lexer.CurrentLine.StartsWith(PARSING_IN_CURSOR))
                {
                    OnParsingInCursorDetected(new ParsingInCursorEventArgs(ParseParsingInCursor()));
                }
                else if (lexer.CurrentLine.StartsWith(PARSE))
                {
                    OnParseDetected(new ParseEventArgs(ParseParse()));
                }
                else if (lexer.CurrentLine.StartsWith(EXEC))
                {
                    OnExecDetected(new ExecEventArgs(ParseExec()));
                }
                else if (lexer.CurrentLine.StartsWith(CLOSE))
                {
                    OnCloseDetected(new CloseEventArgs(ParseClose()));
                }
                else if (lexer.CurrentLine.StartsWith(FETCH))
                {
                    OnFetchDetected(new FetchEventArgs(ParseFetch()));
                }
                else if (lexer.CurrentLine.StartsWith(WAIT))
                {
                    OnWaitDetected(new WaitEventArgs(ParseWait()));
                }
                else if (lexer.CurrentLine.StartsWith(STAT))
                {
                    OnStatDetected(new StatEventArgs(ParseStat()));
                }
                else if (lexer.CurrentLine.StartsWith(BINDS))
                {
                    OnBindsDetected(new BindsEventArgs(ParseBinds()));
                }
                else if (lexer.CurrentLine.StartsWith(XCTEND))
                {
                    OnXctendDetected(new XctendEventArgs(ParseXctend()));
                }
                else
                {
                    OnUnrecognizedDetected(new UnrecognizedEntryEventArgs(ParseUnrecognized()));
                }
            }
            OnEndOfTraceReached();
            OnProgressChanged(new ParserProgressChangedEventArgs(100));
        }
        /// <summary>
        /// Parses a single trace entry at the given line.
        /// If the line does not contain a beginning of a valid trace entry, moves backwards the trace source
        /// to locate the nearest valid entry. When entry has been located, parses and returns that entry.
        /// </summary>
        /// <remarks>This method is intended to be used by UI elements, which display trace contents: user can click any line in the trace file
        /// and see the entry details and entry highlighted.</remarks>
        /// <param name="lineNumber">Line number in the trace source where</param>
        /// <returns></returns>
        public TraceEntry ParseEntryAtLine(int lineNumber)
        {
            TraceEntry result = null;
            int startingLine = lexer.CurrentLineNumber;

            lexer.CurrentLineNumber = lineNumber;

            while (true)
            {
                //entryEnd = entryStart;

                if (lexer.CurrentLine.StartsWith(PARSING_IN_CURSOR))
                {
                    result = ParseParsingInCursor();
                }
                else if (lexer.CurrentLine.StartsWith(PARSE))
                {
                    result = ParseParse();
                }
                else if (lexer.CurrentLine.StartsWith(EXEC))
                {
                    result = ParseExec();
                }
                else if (lexer.CurrentLine.StartsWith(CLOSE))
                {
                    result = ParseClose();
                }
                else if (lexer.CurrentLine.StartsWith(FETCH))
                {
                    result = ParseFetch();
                }
                else if (lexer.CurrentLine.StartsWith(WAIT))
                {
                    result = ParseWait();
                }
                else if (lexer.CurrentLine.StartsWith(STAT))
                {
                    result = ParseStat();
                }
                else if (lexer.CurrentLine.StartsWith(BINDS))
                {
                    result = ParseBinds();
                }
                else if (lexer.CurrentLine.StartsWith(XCTEND))
                {
                    result = ParseXctend();
                }


                if (result != null || lexer.CurrentLineNumber == 0)
                {
                    break;
                }
                else
                {
                    lexer.PreviousLine();
                }
            }

            return result;
        }

        #region Entry parsers
        private ParsingInCursorEntry ParseParsingInCursor()
        {
            // Sample entry:
            // PARSING IN CURSOR #139816334997528 len=271 dep=1 uid=0 oct=3 lid=0 tim=2673974624982 hv=3876120609 ad='6176d328' sqlid='f0h5rpzmhju11'
            // select SYS_CONTEXT('USERENV', 'SERVER_HOST'), SYS_CONTEXT('USERENV', 'DB_UNIQUE_NAME') from v$instance
            // END OF STMT
            int startingLineNumber = lexer.CurrentLineNumber;
            int startingPosition = lexer.CurrentLinePositionStart;

            ParsingInCursorEntry result = new ParsingInCursorEntry(ReadCursorId(TraceLexer.SPACE));
            result.SetProperties(ReadProperties(TraceLexer.SPACE));

            StringBuilder stmt = new StringBuilder();
            while (true)
            {
                lexer.NextLine();
                if (lexer.EndOfSource() || lexer.CurrentLine.StartsWith(END_OF_STATEMENT))
                {
                    break;
                }
                stmt.AppendLine(lexer.CurrentLine);
            }
            result.Statement = stmt.ToString();

            result.LineRange = new Range(startingLineNumber, lexer.CurrentLineNumber);
            result.PositionRange = new Range(startingPosition, lexer.CurrentLinePositionEnd);
            return result;
        }

        private ParseEntry ParseParse()
        {
            // Sample entry:
            // PARSE #139816334997528:c=1000,e=1701,p=0,cr=0,cu=0,mis=1,r=0,dep=1,og=4,plh=2529664852,tim=2673974624981
            int startingLineNumber = lexer.CurrentLineNumber;
            int startingPosition = lexer.CurrentLinePositionStart;

            ParseEntry result = new ParseEntry(ReadCursorId(TraceLexer.COLON));
            result.SetProperties(ReadProperties(TraceLexer.COMMA));
            result.LineRange = new Range(startingLineNumber, lexer.CurrentLineNumber);
            result.PositionRange = new Range(startingPosition, lexer.CurrentLinePositionEnd);

            return result;
        }

        private ExecEntry ParseExec()
        {
            // Sample entry:
            // EXEC #139816334999680:c=2997,e=2458,p=0,cr=3,cu=2,mis=1,r=1,dep=1,og=4,plh=2447725225,tim=2673974623050
            int startingLineNumber = lexer.CurrentLineNumber;
            int startingPosition = lexer.CurrentLinePositionStart;

            ExecEntry result = new ExecEntry(ReadCursorId(TraceLexer.COLON));
            result.SetProperties(ReadProperties(TraceLexer.COMMA));
            result.LineRange = new Range(startingLineNumber, lexer.CurrentLineNumber);
            result.PositionRange = new Range(startingPosition, lexer.CurrentLinePositionEnd);

            return result;
        }

        private CloseEntry ParseClose()
        {
            // Sample entry:
            // CLOSE #139816334997528:c=0,e=61,dep=1,type=0,tim=2673974625375
            int startingLineNumber = lexer.CurrentLineNumber;
            int startingPosition = lexer.CurrentLinePositionStart;

            CloseEntry result = new CloseEntry(ReadCursorId(TraceLexer.COLON));
            result.SetProperties(ReadProperties(TraceLexer.COMMA));
            result.LineRange = new Range(startingLineNumber, lexer.CurrentLineNumber);
            result.PositionRange = new Range(startingPosition, lexer.CurrentLinePositionEnd);

            return result;
        }

        private FetchEntry ParseFetch()
        {
            // Sample entry:
            // FETCH #139816334996272:c=998,e=289,p=0,cr=3,cu=0,mis=0,r=0,dep=2,og=4,plh=2542797530,tim=2673974636017
            int startingLineNumber = lexer.CurrentLineNumber;
            int startingPosition = lexer.CurrentLinePositionStart;

            FetchEntry result = new FetchEntry(ReadCursorId(TraceLexer.COLON));
            result.SetProperties(ReadProperties(TraceLexer.COMMA));
            result.LineRange = new Range(startingLineNumber, lexer.CurrentLineNumber);
            result.PositionRange = new Range(startingPosition, lexer.CurrentLinePositionEnd);

            return result;
        }

        private WaitEntry ParseWait()
        {
            // Sample entry:
            // WAIT #0: nam='SQL*Net message to client' ela= 3 driver id=1413697536 #bytes=1 p3=0 obj#=-1 tim=2673974642876
            int startingLineNumber = lexer.CurrentLineNumber;
            int startingPosition = lexer.CurrentLinePositionStart;

            WaitEntry result = new WaitEntry(ReadCursorId(TraceLexer.COLON));

            lexer.SkipSpaces();
            result.SetProperties(ReadProperties(TraceLexer.SPACE));
            result.LineRange = new Range(startingLineNumber, lexer.CurrentLineNumber);
            result.PositionRange = new Range(startingPosition, lexer.CurrentLinePositionEnd);

            return result;
        }

        private StatEntry ParseStat()
        {
            // Entry is multiline. 
            // Sample entry:
            // STAT #139816334977760 id=1 cnt=24 pid=0 pos=1 obj=0 op='SORT ORDER BY (cr=3 pr=0 pw=0 time=102 us cost=2 size=732 card=12)'
            // STAT #139816334977760 id=2 cnt=24 pid=1 pos=1 obj=212040 op='TABLE ACCESS CLUSTER COL$ (cr=3 pr=0 pw=0 time=74 us cost=1 size=732 card=12)'
            // STAT #139816334977760 id=3 cnt=1 pid=2 pos=1 obj=212036 op='INDEX UNIQUE SCAN I_OBJ# (cr=2 pr=0 pw=0 time=13 us cost=1 size=0 card=1)'

            string cursorId = ReadCursorId(TraceLexer.SPACE);
            StatEntry result = new StatEntry(cursorId);

            GoToStartOfStat(cursorId);
            int startingLineNumber = lexer.CurrentLineNumber;
            int startingPosition = lexer.CurrentLinePositionStart;

            while (true)
            {
                StatEntryLine line = new StatEntryLine(cursorId);
                line.SetProperties(ReadProperties(TraceLexer.SPACE));
                line.LineRange = new Range(lexer.CurrentLineNumber, lexer.CurrentLineNumber);
                line.PositionRange = new Range(lexer.CurrentLinePositionStart, lexer.CurrentLinePositionEnd);
                result.AddLine(line);
                if (lexer.NextLinePreview() == null || !lexer.NextLinePreview().StartsWith(STAT + cursorId))
                {
                    //lexer.NextLine();
                    /*if (lexer.EndOfSource() || !lexer.CurrentLine.StartsWith(STAT + cursorId))
                    {*/
                    result.LineRange = new Range(startingLineNumber, line.LineRange.End);
                    result.PositionRange = new Range(startingPosition, line.PositionRange.End);
                    break;
                    //}
                }
                lexer.NextLine();
            }

            return result;
        }

        private void GoToStartOfStat(string cursorId)
        {
            // Search only for STAT of given cursor
            string searchString = STAT + cursorId;

            try
            {
                while (lexer.CurrentLine.StartsWith(searchString))
                {
                    lexer.PreviousLine();
                }
            }
            catch (NotSupportedException e)
            {
                // Do nothing. The lexer just does not support navigating back.
            }
            // while loop stops when current line <> STAT
            // Bring lexer back to STAT line.
            if (!lexer.CurrentLine.StartsWith(searchString))
            {
                lexer.NextLine();
            }
        }

        private BindsEntry ParseBinds()
        {
            /* Sample entry:
               BINDS #139816334891624:
                Bind#0
                 oacdty = 12 mxl = 07(07) mxlc = 00 mal = 00 scl = 00 pre = 00
                 oacflg = 10 fl2 = 0000 frm = 00 csi = 00 siz = 8 off = 0
                 kxsbbbfp = 847c41f8 bln = 07  avl = 07  flg = 09
                 value = "8/22/2016 11:8:57"
                Bind#1
                 No oacdef for this bind.
                Bind#2
                 oacdty = 02 mxl = 22(22) mxlc = 00 mal = 00 scl = 00 pre = 00
                 oacflg = 00 fl2 = 0000 frm = 00 csi = 00 siz = 24 off = 0
                 kxsbbbfp = 7f2986fb3c38  bln = 22  avl = 03  flg = 05
                 value = 238
               */
            int startingLineNumber = lexer.CurrentLineNumber;
            int startingPosition = lexer.CurrentLinePositionStart;

            BindsEntry result = new BindsEntry(ReadCursorId(TraceLexer.COLON));

            result.SetProperties(ReadProperties(TraceLexer.SPACE));

            while (true)
            {
                if (lexer.NextLinePreview() == null)
                {
                    break;
                }
                else if (lexer.NextLinePreview().StartsWith(BIND))
                {
                    while (lexer.NextLinePreview().StartsWith(BIND))
                    {
                        lexer.NextLine();
                        BindEntry bind = ParseSingleBind();
                        result.Binds.Add(bind);
                    }
                }
                else
                {
                    break;
                }
            }

            result.LineRange = new Range(startingLineNumber, lexer.CurrentLineNumber);
            result.PositionRange = new Range(startingPosition, lexer.CurrentLinePositionEnd);

            return result;
        }

        private BindEntry ParseSingleBind()
        {
            BindEntry result = new BindEntry();
            // Just reusing ReadCursorId to read bind index since the same logic applies
            result.Index = Convert.ToInt32(ReadCursorId(TraceLexer.COMMA));

            if (lexer.NextLinePreview() != null)
            {
                if (NO_DATA_FOR_BIND.Equals(lexer.NextLinePreview()))
                {
                    // There is no metadata - just consume the line.
                    result.HasMetadata = false;
                    lexer.NextLine();
                }
                else if (lexer.NextLinePreview().StartsWith(DOUBLE_SPACE))
                {
                    result.HasMetadata = true;
                    while (lexer.NextLinePreview().StartsWith(DOUBLE_SPACE))
                    {
                        lexer.NextLine();
                        lexer.SkipSpaces();
                        if (lexer.CurrentLine.StartsWith("  value="))
                        {
                            result.SetProperties(ReadBindValue(result));
                        }
                        else
                        {
                            result.SetProperties(ReadProperties(TraceLexer.SPACE));
                        }
                    }
                }
            }

            return result;
        }

        private List<StringProperty> ReadBindValue(BindEntry bind)
        {
            /* Value property has structure:
             * value="8/22/2016 11:8:57"
             * value=238
             * value="multi
             * line value"
             * 
             * Value property is absent, when value is null.
             */
            lexer.SkipUntil(TraceLexer.EQUALS);
            lexer.Skip();

            List<StringProperty> result = new List<StringProperty>();
            if (lexer.CurrentChar == TraceLexer.DOUBLE_QUOT)
            {
                // String property, can be multi-line.
                bool singleLineValue = (lexer.CurrentLine.EndsWith("\""));
                if (singleLineValue)
                {
                    result.Add(new StringProperty(Property.VALUE, lexer.ReadLine()));
                }
                else
                {
                    StringBuilder value = new StringBuilder();
                    while (!lexer.CurrentLine.EndsWith("\""))
                    {
                        value.AppendLine(lexer.ReadLine());
                        lexer.NextLine();
                    }
                    value.AppendLine(lexer.ReadLine()); // Read the last line of value
                    result.Add(new StringProperty(Property.VALUE, value.ToString()));
                }
            }
            else
            {
                result.Add(new StringProperty(Property.VALUE, lexer.ReadLine()));
            }

            return result;
        }

        private XctendEntry ParseXctend()
        {
            // Sample entry:
            // XCTEND rlbk=0, rd_only=0, tim=2673977182044
            int startingLineNumber = lexer.CurrentLineNumber;
            int startingPosition = lexer.CurrentLinePositionStart;

            XctendEntry result = new XctendEntry();

            lexer.SkipUntil(TraceLexer.SPACE);
            lexer.SkipSpaces();

            result.SetProperties(ReadProperties(TraceLexer.COMMA));
            result.LineRange = new Range(startingLineNumber, lexer.CurrentLineNumber);
            result.PositionRange = new Range(startingPosition, lexer.CurrentLinePositionEnd);

            return result;
        }

        private UnrecognizedEntry ParseUnrecognized()
        {
            int startingLineNumber = lexer.CurrentLineNumber;
            int startingPosition = lexer.CurrentLinePositionStart;

            UnrecognizedEntry result = new UnrecognizedEntry();
            result.Text = lexer.ReadLine();

            result.LineRange = new Range(startingLineNumber, lexer.CurrentLineNumber);
            result.PositionRange = new Range(startingPosition, lexer.CurrentLinePositionEnd);

            return result;
        }

        #endregion

        #region Common parser API
        private string ReadCursorId(char stopChar)
        {
            lexer.SkipUntil(TraceLexer.HASH);
            lexer.Skip();
            string cursorId = lexer.ReadUntil(stopChar);
            lexer.SkipIf(stopChar);
            return cursorId.TrimEnd();
        }

        /// <summary>
        /// Always reads entry properties as <see cref="StringProperty"/>.
        /// Later internal SetProperty method decides whether this property is string or numeric and converts as required.
        /// </summary>
        /// <param name="propertyDelimiter">Delimiter used to separate properties. Usually it is a space or comma character.</param>
        /// <returns></returns>
        private StringProperty ReadProperty(char propertyDelimiter)
        {
            string name = lexer.ReadUntil(TraceLexer.EQUALS);
            lexer.Skip(); // skips equals (=) character
            // In WAIT entry ela property has a space after = character, but all other properties do not.
            // So need to skip it, if any.
            lexer.SkipIf(TraceLexer.SPACE);

            string value = null;
            if (lexer.CurrentChar == TraceLexer.QUOT)
            {
                lexer.Skip();
                value = lexer.ReadUntil(TraceLexer.QUOT);
            }
            else if (Property.VALUE.Equals(name))
            {
                value = lexer.ReadLine();
            }
            else
            {
                value = lexer.ReadUntil(propertyDelimiter);
            }
            return new StringProperty(name, value);
        }

        /// <summary>
        /// Reads all current entry properties until the end of current line.
        /// </summary>
        /// <returns></returns>
        private List<StringProperty> ReadProperties(char propertyDelimiter)
        {
            List<StringProperty> props = new List<StringProperty>();

            while (!lexer.EndOfLine())
            {
                props.Add(ReadProperty(propertyDelimiter));
                lexer.SkipIf(propertyDelimiter);
                lexer.SkipUntilLetter();
            }

            return props;
        }
        #endregion


        #region Event raisers
        protected virtual void OnBindsDetected(BindsEventArgs e)
        {
            BindsDetected?.Invoke(this, e);
        }

        protected virtual void OnParsingInCursorDetected(ParsingInCursorEventArgs e)
        {
            ParsingInCursorDetected?.Invoke(this, e);
        }

        protected virtual void OnEndOfTraceReached()
        {
            EndOfTraceReached?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnParseDetected(ParseEventArgs e)
        {
            ParseDetected?.Invoke(this, e);
        }

        protected virtual void OnExecDetected(ExecEventArgs e)
        {
            ExecDetected?.Invoke(this, e);
        }

        protected virtual void OnCloseDetected(CloseEventArgs e)
        {
            CloseDetected?.Invoke(this, e);
        }

        protected virtual void OnFetchDetected(FetchEventArgs e)
        {
            FetchDetected?.Invoke(this, e);
        }

        protected virtual void OnWaitDetected(WaitEventArgs e)
        {
            WaitDetected?.Invoke(this, e);
        }

        protected virtual void OnStatDetected(StatEventArgs e)
        {
            StatDetected?.Invoke(this, e);
        }

        protected virtual void OnXctendDetected(XctendEventArgs e)
        {
            XctendDetected?.Invoke(this, e);
        }

        private void OnProgressChanged(ParserProgressChangedEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }

        protected virtual void OnUnrecognizedDetected(UnrecognizedEntryEventArgs e)
        {
            UnrecognizedDetected?.Invoke(this, e);
        }

        #endregion

        private byte Progress
        {
            get;
            set;
        }

        private long TraceSize
        {
            get;
            set;
        }
    }
}
