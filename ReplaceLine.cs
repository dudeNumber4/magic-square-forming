using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralUnitTest
{

    /// <summary>
    /// A potential replacement line.
    /// </summary>
    public struct ReplaceLine
    {
        private int _adjustValue;
        
        public Line LineRef { get; set; }

        /// <summary>
        /// If non-zero, the value between the original value and the replacement value.
        /// </summary>
        public int AdjustValue { get => _adjustValue; set => _adjustValue = Math.Abs(value); }

        /// <summary>
        /// positive or negative.
        /// </summary>
        public int ReplacementValue;

        public int PositionReplaced;

    }

}
