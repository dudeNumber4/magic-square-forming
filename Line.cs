using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralUnitTest
{

    /// <summary>
    /// Ex: 'h0', [3, 4, 2] for top horz line.
    /// </summary>
    public class Line
    {
        public string Key { get; set; }
        public List<int> LineValues { get; set; }

        public ReplaceLine ToReplaceLine(int replacementValue, int positionReplaced)
        {
            // Here we have to turn the replacement value back into an offset because this is backwards and I'm not rewriting.
            // Offset is what BuildNewLine originally accepted when it made the test ReplaceLine that is now calling this for it's related replace lines.
            int offsetValue = replacementValue - LineValues[positionReplaced];
            var newLine = BuildNewLine(this, (offsetValue, positionReplaced));
            return new ReplaceLine { LineRef = newLine, PositionReplaced = positionReplaced, ReplacementValue = replacementValue };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalLine"></param>
        /// <param name="replacement"></param>
        /// <returns>A new line with replacements specified</returns>
        private static Line BuildNewLine(Line originalLine, (int replacementOffset, int positionReplaced) replacement)
        {
            var newLineValues = new List<int>(3);
            newLineValues.Add(replacement.positionReplaced == 0 ? (originalLine.LineValues[0] + replacement.replacementOffset) : originalLine.LineValues[0]);
            newLineValues.Add(replacement.positionReplaced == 1 ? (originalLine.LineValues[1] + replacement.replacementOffset) : originalLine.LineValues[1]);
            newLineValues.Add(replacement.positionReplaced == 2 ? (originalLine.LineValues[2] + replacement.replacementOffset) : originalLine.LineValues[2]);
            return new Line { Key = originalLine.Key, LineValues = newLineValues };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other">A line that was created from this line, but has had a replacement made.</param>
        /// <param name="replacementValue">The value of the replacement.</param>
        /// <returns>The position of the value in the line that was replaced (if found)</returns>
        public int ReplacedPositionIn(Line other, int replacementValue)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (LineValues[i] == other.LineValues[i])
                {
                    continue;
                }
                if (other.LineValues[i] == replacementValue)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns>True if i is present anywhere in the line</returns>
        public bool Contains(int i) => LineValues.Any(l => l == i);

        public bool ContainsDuplicates => LineValues.Distinct().Count() != 3;

        private bool MatchesMagicNumber => (LineValues[0] + LineValues[1] + LineValues[2]) == UnitTest.MAGIC_NUMBER;

        /// <summary>
        /// Use when looking for a substitute for a value that is missing in the matrix.
        /// Thus, overall matrix logic is mixed in here.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public ReplaceLine GetSubstituteForMissing(int number)
        {
            if (!ContainsDuplicates && !MatchesMagicNumber)
            {
                return GetAdjustedLine(number);
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="duplicateValue"></param>
        /// <returns>If possible, a new line with duplicateValue replaced with new value that will cause line to sum to magic number, new value that was inserted</returns>
        public ReplaceLine GetSubstituteForDuplicate(int duplicateValue)
        {
            ReplaceLine result = default;
            int newValue = 0;
            /// We don't mess with a duplicate *on the current line*
            /// We only return for replacing a duplicate that exists somewhere else in the matrix.
            if (LineValues.Count(l => l == duplicateValue) == 1)
            {
                int sumWithoutValueToReplace = LineValues.Where(l => l != duplicateValue).Sum();
                newValue = UnitTest.MAGIC_NUMBER - sumWithoutValueToReplace;
                if (newValue != duplicateValue)
                {
                    var newLine = new Line { Key = Key, LineValues = new List<int>(3) };
                    var positionReplaced = LineValues.IndexOf(duplicateValue);
                    newLine.LineValues.Add(LineValues[0] == duplicateValue ? newValue : LineValues[0]);
                    newLine.LineValues.Add(LineValues[1] == duplicateValue ? newValue : LineValues[1]);
                    newLine.LineValues.Add(LineValues[2] == duplicateValue ? newValue : LineValues[2]);
                    result = new ReplaceLine { LineRef = newLine, ReplacementValue = newValue, AdjustValue = newValue - duplicateValue, PositionReplaced = positionReplaced };
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adjustTo"></param>
        /// <returns>Line adjusted by adjustTo in any position to match magic number if possible</returns>
        private ReplaceLine GetAdjustedLine(int adjustTo)
        {
            var adjustedValues = new List<int> { adjustTo, LineValues[1], LineValues[2] };

            Line FinalLine() => new Line { Key = Key, LineValues = adjustedValues };

            if (adjustedValues.Sum() == UnitTest.MAGIC_NUMBER)
            {
                return new ReplaceLine { LineRef = FinalLine(), PositionReplaced = 0, ReplacementValue = adjustTo, AdjustValue = adjustTo - LineValues[0] };
            }
            adjustedValues = new List<int> { LineValues[0], adjustTo, LineValues[2] };
            if (adjustedValues.Sum() == UnitTest.MAGIC_NUMBER)
            {
                return new ReplaceLine { LineRef = FinalLine(), PositionReplaced = 1, ReplacementValue = adjustTo, AdjustValue = adjustTo - LineValues[1] };
            }
            adjustedValues = new List<int> { LineValues[0], LineValues[1], adjustTo };
            if (adjustedValues.Sum() == UnitTest.MAGIC_NUMBER)
            {
                return new ReplaceLine { LineRef = FinalLine(), PositionReplaced = 2, ReplacementValue = adjustTo, AdjustValue = adjustTo - LineValues[2] };
            }

            return default;
        }

    }

}
