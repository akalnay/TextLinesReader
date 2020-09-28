////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System.Diagnostics;
using System.Text;

namespace TextLinesReader
{
    public class ReadLinesFileOptions
    {
        #region Backing Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private long _linesToSkip = ReadLinesOptions.LinesToSkipDefault;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Encoding _encoding = EncodingDefault;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _bufferSize = BufferSizeDefault;

        #endregion Backing Fields

        #region Property Defaults

        public static bool DetectEncodingFromByteOrderMarksDefault { get; } = true;

        public static Encoding EncodingDefault { get; } = Encoding.UTF8;

        public static int BufferSizeDefault { get; } = 1024;

        #endregion Property Defaults

        #region Properties

        public long LinesToSkip
        {
            get { return _linesToSkip; }
            set { _linesToSkip = value < 0 ? ReadLinesOptions.LinesToSkipDefault : value; }
        }

        public bool DetectEncodingFromByteOrderMarks { get; set; } = DetectEncodingFromByteOrderMarksDefault;

        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value ?? EncodingDefault; }
        }

        public int BufferSize
        {
            get { return _bufferSize; }
            set { _bufferSize = value < 1 ? BufferSizeDefault : value; }
        }

        #endregion Properties
    }
}
