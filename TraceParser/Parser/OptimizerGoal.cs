using System;

namespace TraceUI.Parser
{

    /// <summary>
    /// Oracle optimizer goal IDs and API to translate ID to a name. 
    /// </summary>
    public sealed class OptimizerGoal
    {
        public const int ALL_ROWS = 1;
        public const int FIRST_ROWS = 2;
        public const int RULE = 3;
        public const int CHOOSE = 4;

        private OptimizerGoal()
        {
        }

        /// <summary>
        /// Converts optimizer goal ID to its name.
        /// </summary>
        /// <param name="id">Optimizer goal ID.</param>
        /// <returns>Optimizer goal name.</returns>
        public static string ToString(int id)
        {
            switch (id)
            {
                case ALL_ROWS:
                    return "ALL_ROWS";
                case FIRST_ROWS:
                    return "FIRST_ROWS";
                case RULE:
                    return "RULE";
                case CHOOSE:
                    return "CHOOSE";
                default:
                    return Convert.ToString(id);
            }
        }
    }
}
