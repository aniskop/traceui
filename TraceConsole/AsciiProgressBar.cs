using System;

namespace TraceUI.CommandLine
{
    public class AsciiProgressBar
    {
        private const char PROGRESS_CHAR = '=';

        private const int LEFT_MARGIN = 0;
        private const int RIGHT_MARGIN = 2;
        private const int PERCENTS_PLACEHOLDER = 4;
        private const int BAR_FRAME_CHAR_COUNT = 2;

        private string emptyProgressBar;

        private int currentProgressPositionLeft = 0;
        private int percentsPositionLeft = 0;
        private byte currentProgress = 0;
        private int totalChunks = 0;
        private int chunksDrawn = 0;

        public AsciiProgressBar(int top, int left)
        {
            Top = top;
            Left = left;
            totalChunks = CalculateTotalChunks();
            Reset();
        }

        private int CalculateTotalChunks()
        {
            return Console.WindowWidth - Left - BAR_FRAME_CHAR_COUNT - PERCENTS_PLACEHOLDER - LEFT_MARGIN - RIGHT_MARGIN - 1;
        }

        public void Reset()
        {
            emptyProgressBar = string.Format("[{0}]    %", " ".PadLeft(totalChunks));
            Console.SetCursorPosition(Left, Top);
            Console.Write(emptyProgressBar);
            currentProgressPositionLeft = Left + 1; //+1 for leading [ char
            percentsPositionLeft = Left + emptyProgressBar.Length - PERCENTS_PLACEHOLDER;
            currentProgress = 0;
            chunksDrawn = 0;
        }

        public void Refresh()
        {
            RefreshBar();
            RefreshPercents();
        }

        private void RefreshBar()
        {
            int progressChunks = (Progress * totalChunks) / 100;
            int chunksToDraw = progressChunks - chunksDrawn;

            Console.SetCursorPosition(currentProgressPositionLeft, Top);

            if (chunksToDraw > 0)
            {
                if (chunksToDraw == 1)
                {
                    Console.Write(PROGRESS_CHAR);
                }
                else
                {
                    for (int i = 0; i < chunksToDraw; i++)
                    {
                        Console.Write(PROGRESS_CHAR);
                    }
                }
                currentProgressPositionLeft += chunksToDraw;
                chunksDrawn += chunksToDraw;
            }
        }

        private void RefreshPercents()
        {
            Console.SetCursorPosition(percentsPositionLeft, Top);
            Console.Write(Progress.ToString().PadLeft(3));
        }

        public byte Progress
        {
            get
            {
                return currentProgress;
            }
            set
            {
                if (value != currentProgress)
                {
                    currentProgress = value;
                    Refresh();
                }
            }
        }

        public int Top
        {
            get;
            set;
        }

        public int Left
        {
            get;
            set;
        }
    }
}
