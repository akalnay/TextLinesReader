////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

namespace TextLinesReader
{
    public sealed class ReadLinesStreamOptions : ReadLinesFileOptions
    {
        #region Constructors

        public ReadLinesStreamOptions()
        {
        }

        public ReadLinesStreamOptions(ReadLinesFileOptions readLinesFileOptions)
        {
            if (readLinesFileOptions != null)
            {
                LinesToSkip                      = readLinesFileOptions.LinesToSkip;
                Encoding                         = readLinesFileOptions.Encoding;
                DetectEncodingFromByteOrderMarks = readLinesFileOptions.DetectEncodingFromByteOrderMarks;
                BufferSize                       = readLinesFileOptions.BufferSize;
            }
        }

        #endregion Constructors

        #region Property Defaults

        public static bool LeaveOpenDefault { get; } = true;

        #endregion Property Defaults

        #region Properties

        public bool LeaveOpen { get; set; } = LeaveOpenDefault;

        #endregion Properties
    }
}
