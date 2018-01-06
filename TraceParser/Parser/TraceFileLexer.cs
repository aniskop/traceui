using System;
using System.IO;

namespace TraceUI.Parser
{
    /// <summary>
    /// Sequential (one direction, top-to-bottom) lexer. Uses trace file as a source.
    /// </summary>
    public class TraceFileLexer : TraceLexer
    {
        private StreamReader traceStream;
        private string lineText = "";
        private string nextLineText = null;
        private long lineNumber = 0;
        private long absoluteLineOffsetInFile = 0;

        public TraceFileLexer(string filePath)
        {
            CurrentLineNumber = 0;
            traceStream = new StreamReader(filePath);
            FilePath = filePath;
        }

        public override string CurrentLine
        {
            get
            {
                return lineText;
            }
        }

        public override long CurrentLineNumber
        {
            get
            {
                return lineNumber;
            }

            set
            {
                if (lineNumber != value)
                {
                    lineNumber = value;
                };
            }
        }

        public override long CurrentLinePositionEnd
        {
            get
            {
                return absoluteLineOffsetInFile + CurrentLine.Length - 1;
            }
        }

        public override long CurrentLinePositionStart
        {
            get
            {
                return absoluteLineOffsetInFile;
            }
        }

        public override long CurrentPosition
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool EndOfSource()
        {
            return traceStream.EndOfStream;
        }

        public override void NextLine()
        {
            if (!EndOfSource())
            {
                string text = (nextLineText == null) ? traceStream.ReadLine() : nextLineText;
                lineNumber++;
                absoluteLineOffsetInFile += CurrentLine.Length;
                lineText = text;
                GoToLineStart();
            }
            if (!EndOfSource())
            {
                nextLineText = traceStream.ReadLine();
            }
            else
            {
                nextLineText = null;
            }
        }

        public override void PreviousLine()
        {
            // This is a sequential lexer - no need to go back.
            throw new NotSupportedException();
        }

        private void GoToLineStart()
        {
            PositionInLine = 0;
        }

        public override string NextLinePreview()
        {
            return nextLineText;
        }

        public string FilePath
        {
            get;
            private set;
        }
    }
}
