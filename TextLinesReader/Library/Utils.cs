////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextLinesReader
{
    internal static class Utils
    {
        #region Save a text sequence to a MemoryStream

        public static MemoryStream Save(this IEnumerable<string> values)
        {
            Encoding defaultEncoding = Encoding.UTF8;
            return values.Save(defaultEncoding);
        }

        public static MemoryStream Save(this IEnumerable<string> values, Encoding encoding)
        {
            const int defaultBufferSize = 1024;
            return values.Save(encoding, defaultBufferSize);
        }

        public static MemoryStream Save(this IEnumerable<string> values, Encoding encoding, int bufferSize)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (encoding == null) throw new ArgumentNullException(nameof(encoding));
            if (bufferSize <= 0) throw new ArgumentOutOfRangeException(nameof(bufferSize));
            const bool leaveStreamOpen = true;
            MemoryStream memoryStream = new MemoryStream();
            using (StreamWriter streamWriter = new StreamWriter(memoryStream, encoding, bufferSize, leaveStreamOpen))
                foreach (string value in values)
                    streamWriter.WriteLine(value);
            memoryStream.Position = 0;
            return memoryStream;
        }

        #endregion Save a text sequence to a MemoryStream
    }
}
