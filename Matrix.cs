using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralUnitTest
{

    /// <summary>
    /// Contains horz, vert, diag lines.  d0 is top left to bottom right.
    /// </summary>
    public struct Matrix
    {
        public Line h0;
        public Line h1;
        public Line h2;
        public Line v0;
        public Line v1;
        public Line v2;
        public Line d0;
        public Line d1;

        public Matrix(int[][] s)
        {
            h0 = new Line { LineValues = new List<int>(3) { s[0][0], s[0][1], s[0][2] }, Key = nameof(h0) };
            h1 = new Line { LineValues = new List<int>(3) { s[1][0], s[1][1], s[1][2] }, Key = nameof(h1) };
            h2 = new Line { LineValues = new List<int>(3) { s[2][0], s[2][1], s[2][2] }, Key = nameof(h2) };

            v0 = new Line { LineValues = new List<int>(3) { s[0][0], s[1][0], s[2][0] }, Key = nameof(v0) };
            v1 = new Line { LineValues = new List<int>(3) { s[0][1], s[1][1], s[2][1] }, Key = nameof(v1) };
            v2 = new Line { LineValues = new List<int>(3) { s[0][2], s[1][2], s[2][2] }, Key = nameof(v2) };

            // d0 is top left to bottom right
            d0 = new Line { LineValues = new List<int>(3) { s[0][0], s[1][1], s[2][2] }, Key = nameof(d0) };
            d1 = new Line { LineValues = new List<int>(3) { s[0][2], s[1][1], s[2][0] }, Key = nameof(d1) };
        }

        public Matrix Clone(Line replacedLine, int replacementValue)
        {
            Matrix me = this;

            if (replacedLine == null)
            {
                return this;
            }
            else
            {
                Line MyReplacedLine() // get ref to line that was replaced.
                {
                    switch (replacedLine.Key)
                    {
                        case nameof(h0): return me.h0;
                        case nameof(h1): return me.h1;
                        case nameof(h2): return me.h2;
                        case nameof(v0): return me.v0;
                        case nameof(v1): return me.v1;
                        case nameof(v2): return me.v2;
                        case nameof(d0): return me.d0;
                        case nameof(d1): return me.d1;
                        default: return null;
                    }
                }
                
                Line myReplacedLine = MyReplacedLine();
                int positionReplaced = myReplacedLine.ReplacedPositionIn(replacedLine, replacementValue);
                Debug.Assert(positionReplaced != -1);
                List<ReplaceLine> newIntersectingLines = GetNewIntersections(myReplacedLine.ToReplaceLine(replacementValue, positionReplaced));
                
                /// What to call for each new line to go in the cloned matrix.
                Line NewReplacedLine(Line existingLine)
                {
                    ReplaceLine intersectingLine = newIntersectingLines.FirstOrDefault(il => il.LineRef.Key == existingLine.Key);
                    if (myReplacedLine.Key == existingLine.Key)
                    {
                        return replacedLine; // line that was passed as param to clone.
                    }
                    else if (intersectingLine.LineRef != null)
                    {
                        return intersectingLine.LineRef; // an intersecting line of line that was passed as param to clone.
                    }
                    else
                    {
                        return existingLine;
                    }
                }

                var result = new Matrix
                {
                    h0 = NewReplacedLine(h0),
                    h1 = NewReplacedLine(h1),
                    h2 = NewReplacedLine(h2),
                    v0 = NewReplacedLine(v0),
                    v1 = NewReplacedLine(v1),
                    v2 = NewReplacedLine(v2),
                    d0 = NewReplacedLine(d0),
                    d1 = NewReplacedLine(d1),
                };
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>All of our lines horz, vert, then diag</returns>
        public IEnumerable<Line> Lines()
        {
            yield return h0;
            yield return h1;
            yield return h2;
            yield return v0;
            yield return v1;
            yield return v2;
            yield return d0;
            yield return d1;
        }

        public List<Line> GetLines()
        {
            var result = new List<Line>(8);
            foreach (Line line in Lines())
            {
                result.Add(line);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns>True if i is present anywhere in the matrix</returns>
        public bool Contains(int i) => Lines().Any(l => l.Contains(i));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="missingValue"></param>
        /// <returns>If found, a substitute line and the line it replaced.</returns>
        public ReplaceLine GetSubstituteForMissingFromHorzLines(int missingValue)
        {
            var result = h0.GetSubstituteForMissing(missingValue);
            if (result.LineRef == null)
            {
                result = h1.GetSubstituteForMissing(missingValue);
                if (result.LineRef == null)
                {
                    result = h2.GetSubstituteForMissing(missingValue);
                    if (result.LineRef == null)
                    {
                        return default;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A line with <paramref name="duplicateValue"/> replaced with a new value if possible.</returns>
        public ReplaceLine GetSubstituteForNextDuplicate()
        {
            int? duplicateValue = NextDuplicate();
            if (duplicateValue.HasValue)
            {
                Matrix me = this;

                ReplaceLine GetSubstituteForLine(Line candidateLine)
                {
                    ReplaceLine innerReplaceLine = candidateLine.GetSubstituteForDuplicate(duplicateValue.Value);
                    if ((innerReplaceLine.LineRef != null) && !me.Contains(innerReplaceLine.ReplacementValue)) // don't create another dupe.
                    {
                        return innerReplaceLine; // new line, adjusted value
                    }
                    else
                    {
                        return default;
                    }
                }

                // Attempt for horz then vert
                ReplaceLine result = GetSubstituteForLine(h0);
                if (result.LineRef != null)
                {
                    return result;
                }
                result = GetSubstituteForLine(h1);
                if (result.LineRef != null)
                {
                    return result;
                }
                result = GetSubstituteForLine(h2);
                if (result.LineRef != null)
                {
                    return result;
                }
                result = GetSubstituteForLine(v0);
                if (result.LineRef != null)
                {
                    return result;
                }
                result = GetSubstituteForLine(v1);
                if (result.LineRef != null)
                {
                    return result;
                }
                result = GetSubstituteForLine(v2);
                if (result.LineRef != null)
                {
                    return result;
                }
            }

            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Next number (consecutive 1..9) that should be in the matrix but isn't</returns>
        public int? NextMissingNumber()
        {
            foreach (var i in Enumerable.Range(1, 9))
            {
                if (!Contains(i))
                {
                    return i;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Next number (consecutive 1..9) that appears in the matrix more than once.</returns>
        private int? NextDuplicate()
        {
            foreach (var i in Enumerable.Range(1, 9))
            {
                bool alreadyFound = false;
                foreach (var line in Lines().Where(l => l.Key.Contains('h'))) // just check one dimension to iterate through all cells.
                {
                    if (line.Contains(i))
                    {
                        if (alreadyFound || line.LineValues.Count(l => l == i) > 1) // already found or appears twice in line.
                        {
                            return i;
                        }
                        else
                        {
                            alreadyFound = true;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rl"></param>
        /// <returns>Line(s) in the matrix affected by the current replacement containing the new replaced value in the proper position</returns>
        private List<ReplaceLine> GetNewIntersections(ReplaceLine rl)
        {
            int replace = rl.ReplacementValue;
            switch (rl.LineRef.Key)
            {
                case "h0" when rl.PositionReplaced == 0: return new List<ReplaceLine> { v0.ToReplaceLine(replace, 0), d0.ToReplaceLine(replace, 0) };
                case "h0" when rl.PositionReplaced == 1: return new List<ReplaceLine> { v1.ToReplaceLine(replace, 0) };
                case "h0" when rl.PositionReplaced == 2: return new List<ReplaceLine> { v2.ToReplaceLine(replace, 0), d1.ToReplaceLine(replace, 0) };
                case "h1" when rl.PositionReplaced == 0: return new List<ReplaceLine> { v0.ToReplaceLine(replace, 1) };
                // center
                case "h1" when rl.PositionReplaced == 1: return new List<ReplaceLine> { v1.ToReplaceLine(replace, 1), d0.ToReplaceLine(replace, 1), d1.ToReplaceLine(replace, 1) };
                case "h1" when rl.PositionReplaced == 2: return new List<ReplaceLine> { v2.ToReplaceLine(replace, 1) };
                case "h2" when rl.PositionReplaced == 0: return new List<ReplaceLine> { v0.ToReplaceLine(replace, 2), d1.ToReplaceLine(replace, 2) };
                case "h2" when rl.PositionReplaced == 1: return new List<ReplaceLine> { v1.ToReplaceLine(replace, 2) };
                case "h2" when rl.PositionReplaced == 2: return new List<ReplaceLine> { v2.ToReplaceLine(replace, 2), d0.ToReplaceLine(replace, 2) };

                case "v0" when rl.PositionReplaced == 0: return new List<ReplaceLine> { h0.ToReplaceLine(replace, 0), d0.ToReplaceLine(replace, 0) };
                case "v0" when rl.PositionReplaced == 1: return new List<ReplaceLine> { h1.ToReplaceLine(replace, 0) };
                case "v0" when rl.PositionReplaced == 2: return new List<ReplaceLine> { h2.ToReplaceLine(replace, 0), d1.ToReplaceLine(replace, 2) };
                case "v1" when rl.PositionReplaced == 0: return new List<ReplaceLine> { h0.ToReplaceLine(replace, 1) };
                // center
                case "v1" when rl.PositionReplaced == 1: return new List<ReplaceLine> { h1.ToReplaceLine(replace, 1), d0.ToReplaceLine(replace, 1), d1.ToReplaceLine(replace, 1) };
                case "v1" when rl.PositionReplaced == 2: return new List<ReplaceLine> { h2.ToReplaceLine(replace, 1) };
                case "v2" when rl.PositionReplaced == 0: return new List<ReplaceLine> { h0.ToReplaceLine(replace, 2), d1.ToReplaceLine(replace, 0) };
                case "v2" when rl.PositionReplaced == 1: return new List<ReplaceLine> { h1.ToReplaceLine(replace, 2) };
                case "v2" when rl.PositionReplaced == 2: return new List<ReplaceLine> { h2.ToReplaceLine(replace, 2), d0.ToReplaceLine(replace, 2) };

                case "d0" when rl.PositionReplaced == 0: return new List<ReplaceLine> { h0.ToReplaceLine(replace, 0), v0.ToReplaceLine(replace, 0) };
                // center
                case "d0" when rl.PositionReplaced == 1: return new List<ReplaceLine> { h1.ToReplaceLine(replace, 1), v1.ToReplaceLine(replace, 1), d1.ToReplaceLine(replace, 1) };
                case "d0" when rl.PositionReplaced == 2: return new List<ReplaceLine> { h2.ToReplaceLine(replace, 2), v2.ToReplaceLine(replace, 2) };

                case "d1" when rl.PositionReplaced == 0: return new List<ReplaceLine> { h0.ToReplaceLine(replace, 2), v2.ToReplaceLine(replace, 0) };
                // center
                case "d1" when rl.PositionReplaced == 1: return new List<ReplaceLine> { h1.ToReplaceLine(replace, 1), v1.ToReplaceLine(replace, 1), d0.ToReplaceLine(replace, 1) };
                case "d1" when rl.PositionReplaced == 2: return new List<ReplaceLine> { h2.ToReplaceLine(replace, 0), v0.ToReplaceLine(replace, 2) };

                default: throw new Exception($"Impossible line: {rl.LineRef.Key}");
            }
        }

    }

}
