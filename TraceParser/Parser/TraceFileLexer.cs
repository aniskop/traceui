using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private int lineNumber = 0;
        private int absoluteLineOffsetInFile = 0;

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

        public override int CurrentLineNumber
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

        public override int CurrentLinePositionEnd
        {
            get
            {
                return absoluteLineOffsetInFile + CurrentLine.Length - 1;
            }
        }

        public override int CurrentLinePositionStart
        {
            get
            {
                return absoluteLineOffsetInFile;
            }
        }

        public override int CurrentPosition
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
