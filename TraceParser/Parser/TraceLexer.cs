namespace TraceUI.Parser
{
    /// <summary>
    /// A base class for all lexers to read Oracle trace from source (file, text string etc).
    /// </summary>
    /// Trace file can be read and parsed line by line. Thus <code>Lexer</code> provides required API based on that principle:
    /// <list type="bullet">
    /// <item><description>API to navigate between trace lines.</description></item>
    /// <item><description>API to navigate and read data inside the current line.</description></item>
    /// </list>
    public abstract class TraceLexer
    {
        /// <summary>
        /// Represents a hash character.
        /// </summary>
        public const char HASH = '#';

        /// <summary>
        /// Represents an equals character.
        /// </summary>
        public const char EQUALS = '=';

        /// <summary>
        /// Represents a space character.
        /// </summary>
        public const char SPACE = ' ';

        /// <summary>
        /// Represents a single quot character.
        /// </summary>
        public const char QUOT = '\'';

        /// <summary>
        /// Represents a double quot character.
        /// </summary>
        public const char DOUBLE_QUOT = '"';

        /// <summary>
        /// Represents a line feed.
        /// </summary>
        public const char LF = '\n';

        /// <summary>
        /// Represents a carriage return.
        /// </summary>
        public const char CR = '\r';

        /// <summary>
        /// Represents a colon character.
        /// </summary>
        public const char COLON = ':';

        /// <summary>
        /// Represents a comma character.
        /// </summary>
        public const char COMMA = ',';

        /// <summary>
        /// Creates an instance of lexer to navigate the trace source.
        /// </summary>
        protected TraceLexer()
        {

        }

        /// <summary>
        /// Gets or sets zero-based read offset in the <see cref="CurrentLine"/>.
        /// </summary>
        public virtual int PositionInLine
        {
            get;
            protected set;
        }

        /// <summary>
        /// Advances to the next line in the trace source.
        /// </summary>
        public abstract void NextLine();

        /// <summary>
        /// This method allows to preview the contents of the next line.
        /// In other words, it returns the contents of next line, but does not consume it (current position stays where it is).
        /// Must return null, if end of trace file reached.
        /// Can be called only after <see cref="NextLine"/> is called for the first time.
        /// </summary>
        public abstract string NextLinePreview();

        /// <summary>
        /// Goes to the previous line in the trace source.
        /// Must be implemented if lexer navigates a trace source backwards.
        /// </summary>
        public abstract void PreviousLine();

        /// <summary>
        /// Skips current character in <see cref="CurrentLine"/> and advances to the next character.
        /// </summary>
        public virtual void Skip()
        {
            if (!EndOfLine())
            {
                PositionInLine++;
            }
        }

        /// <summary>
        /// Skips the current character if it is equal to <paramref name="skipChar"/>.
        /// </summary>
        /// <param name="skipChar">Character to skip.</param>
        public virtual void SkipIf(char skipChar)
        {
            if (!EndOfLine() && CurrentChar == skipChar)
            {
                Skip();
            }
        }

        /// <summary>
        /// Skips all characters in <see cref="CurrentLine"/> starting from <see cref="PositionInLine"/> until hits <paramref name="stopChar"/>.
        /// </summary>
        /// <param name="stopChar">Stop character.</param>
        public virtual void SkipUntil(char stopChar)
        {
            while (PositionInLine < CurrentLine.Length && CurrentChar != stopChar)
            {
                PositionInLine++;
            }
        }

        /// <summary>
        /// Skips all characters in the <see cref="CurrentLine"/> starting from <see cref="PositionInLine"/> until hits a digit.
        /// </summary>
        public virtual void SkipUntilDigit()
        {
            while (!EndOfLine() && !char.IsDigit(CurrentChar))
            {
                PositionInLine++;
            }
        }

        /// <summary>
        /// Skips all characters in <see cref="CurrentLine"/> starting from <see cref="PositionInLine"/> until hits a letter.
        /// </summary>
        public virtual void SkipUntilLetter()
        {
            while (!EndOfLine() && !char.IsLetter(CurrentChar))
            {
                PositionInLine++;
            }
        }

        /// <summary>
        /// Skips all space characters in the <see cref="CurrentLine"/> starting from <see cref="PositionInLine"/> until hits non-space character.
        /// </summary>
        public virtual void SkipSpaces()
        {
            while (!EndOfLine() && CurrentChar == SPACE)
            {
                PositionInLine++;
            }
        }

        /// <summary>
        /// Reads all characters in the <see cref="CurrentLine"/> starting from <see cref="PositionInLine"/> until hits <paramref name="stopChar"/>.
        /// </summary>
        /// <param name="stopChar">Stop character.</param>
        /// <returns>Consumed characters as a string.</returns>
        /// <remarks><paramref name="stopChar"/> is not read.</remarks>
        public virtual string ReadUntil(char stopChar)
        {
            int startingOffset = PositionInLine;
            SkipUntil(stopChar);
            return CurrentLine.Substring(startingOffset, (PositionInLine - startingOffset));
        }

        /// <summary>
        /// Consumes all characters in the <see cref="CurrentLine"/> starting from <see cref="PositionInLine"/> until end of line.
        /// </summary>
        /// <returns>Consumed characters as string.</returns>
        public virtual string ReadLine()
        {
            string result = CurrentLine.Substring(PositionInLine);
            PositionInLine = CurrentLine.Length - 1;
            return result;
        }

        /// <summary>
        /// Reads <see cref="CurrentChar"/>.
        /// </summary>
        /// <returns><see cref="CurrentChar"/></returns>
        public virtual char Read()
        {
            char c = CurrentChar;
            Skip();
            return c;
        }

        /// <summary>
        /// Gets the text at the current line which is <see cref="CurrentLineNumber"/>.
        /// </summary>
        public abstract string CurrentLine
        {
            get;
        }

        /// <summary>
        /// Gets a zero-based offset from the beginning of the source where the <see cref="CurrentLine"/> begins.
        /// </summary>
        public abstract long CurrentLinePositionStart
        {
            get;
        }

        /// <summary>
        /// Gets a zero-based offset from the beginning of the source where the <see cref="CurrentLine"/> ends.
        /// </summary>
        public abstract long CurrentLinePositionEnd
        {
            get;
        }

        /// <summary>
        /// Gets or sets current line number in trace source.
        /// </summary>
        public abstract long CurrentLineNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets current caret position (zero-based from beginning of the source).
        /// </summary>
        public abstract long CurrentPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the character of the <see cref="CurrentLine"/> which is at <see cref="PositionInLine"/>.
        /// </summary>
        public virtual char CurrentChar
        {
            get
            {
                return CurrentLine[PositionInLine];
            }
        }

        /// <summary>
        /// Checks if <see cref="PositionInLine"/> has reached the end of line.
        /// </summary>
        /// <returns>true, if the <see cref="CurrentLine"/> is null or <see cref="PositionInLine"/> is >= <see cref="CurrentLine"/>.Length.</returns>
        public virtual bool EndOfLine()
        {
            return (CurrentLine == null) || (PositionInLine >= CurrentLine.Length);
        }

        /// <summary>
        /// Checks if the <see cref="CurrentLineNumber"/> exceeds the last line in the source.
        /// </summary>
        /// <returns></returns>
        public abstract bool EndOfSource();
    }
}
