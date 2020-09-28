////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextLinesReader.Tests
{
    internal abstract class ReadLines_TestsBase
    {
        private class Person : IEquatable<Person>
        {
            private const int _INDEXLASTNAME     = 0;
            private const int _INDEXFIRSTNAME    = 1;
            private const int _INDEXEMAILADDRESS = 2;

            public Person() { }

            public Person(string value) : this(value.Split(',')) { }

            public Person(string[] values) : this(values[_INDEXLASTNAME].Trim(), values[_INDEXFIRSTNAME].Trim(), values[_INDEXEMAILADDRESS].Trim()) { }

            public Person(string lastName, string firstName, string emailAddress)
            {
                LastName     = lastName;
                FirstName    = firstName;
                EmailAddress = emailAddress;
            }

            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string EmailAddress { get; set; }

            public override bool Equals(object other)
            {
                return (other != null) && ReferenceEquals(this, other) || (GetType() == other.GetType() && Equals(other as Person));
            }

            public bool Equals(Person other)
            {
                return other != null && LastName == other.LastName && FirstName == other.FirstName && EmailAddress == other.EmailAddress;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    if (LastName != null)
                        hash = hash * 31 + LastName.GetHashCode(StringComparison.InvariantCulture);
                    if (FirstName != null)
                        hash = hash * 31 + FirstName.GetHashCode(StringComparison.InvariantCulture);
                    if (EmailAddress != null)
                        hash = hash * 31 + EmailAddress.GetHashCode(StringComparison.InvariantCulture);
                    return hash;
                }
            }

            public override string ToString() => $"{FirstName} {LastName} - {EmailAddress}";
        }

        protected static class TestCases
        {
            #region Happy Path Test Cases

            public static IEnumerable<TestCaseData> HappyPath()
            {
                string[] dataLines =
                {
                    "Smith, John, jsmith@gmail.com",
                    "Jones, Mary, mjones@hotmail.com",
                    "Johnson, Steve, sjohnson@yahoo.com"
                };
                IEnumerable<Person> expectedResult = dataLines.Select(dataLine => new Person(dataLine));
                Encoding[] encodings = new[] { Encoding.UTF8, Encoding.UTF7, Encoding.UTF32, Encoding.Unicode, Encoding.ASCII };
                int[] recordsToSkip = new[] { 0, 1 };
                Person typeInferer = new Person();
                for (int i = 0; i < encodings.Length; i++)
                    for (int j = 0; j < recordsToSkip.Length; j++)
                        yield return new TestCaseData(typeInferer, dataLines.Save(encodings[i]), recordsToSkip[j], encodings[i]).Returns(expectedResult.Skip(recordsToSkip[j]));
            }

            #endregion Happy Path Test Cases

            #region Unhappy Path Test Cases

            public static IEnumerable<TestCaseData> NullStream()
            {
                yield return new TestCaseData(new Person(), null, 0, Encoding.UTF8);
            }

            public static IEnumerable<TestCaseData> ClosedStream()
            {
                Stream stream = new MemoryStream();
                stream.Close();
                yield return new TestCaseData(new Person(), stream, 0, Encoding.UTF8);
            }

            #endregion Unhappy Path Test Cases
        }

        protected static IEnumerable<T> InvokeMethodReadLines<T>(Stream stream, ReadLinesStreamOptions readLinesStreamOptions)
        {
            return ReadLinesUtils.ReadLines<T>(stream, readLinesStreamOptions);
        }
    }

    [TestFixture]
    [Category("ReadLinesUtils - ReadLines")]
    internal sealed class ReadLines_Tests : ReadLines_TestsBase
    {
        #region Happy Path Tests

        // Verify that when the ReadLines method is invoked it returns the expected results.
        // Various iterations of this test apply different values to the 'recordsToSkip' and 'encoding' arguments
        [TestCaseSource(typeof(TestCases), nameof(TestCases.HappyPath))]
        public IEnumerable<T> WhenTheMethodIsInvoked_ThenTheResultIsTheExpectedValue<T>(T typeInferer, Stream stream, long recordsToSkip, Encoding encoding)
        {
            T[] result = InvokeMethodReadLines<T>(stream, new ReadLinesStreamOptions { LinesToSkip = recordsToSkip, Encoding = encoding }).ToArray();
            stream.Dispose();
            return result;
        }

        #endregion Happy Path Tests

        #region Unhappy Path Tests

        // Verify that an ArgumentNullException is thrown if the 'stream' argument is null
        [TestCaseSource(typeof(TestCases), nameof(TestCases.NullStream))]
        public void WhenTheMethodIsInvokedWithANullStream_ThenAnArgumentNullExceptionIsThrown<T>(T typeInferer, Stream stream, long recordsToSkip, Encoding encoding)
        {
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => InvokeMethodReadLines<T>(stream, new ReadLinesStreamOptions { LinesToSkip = recordsToSkip, Encoding = encoding }).ToArray());
            Assert.AreEqual(nameof(stream), exception.ParamName);
        }

        // Verify that an ArgumentException is thrown if the 'stream' argument refers to a closed stream
        [TestCaseSource(typeof(TestCases), nameof(TestCases.ClosedStream))]
        public void WhenTheMethodIsInvokedWithAClosedStream_ThenAnArgumentExceptionIsThrown<T>(T typeInferer, Stream stream, long recordsToSkip, Encoding encoding)
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(() => InvokeMethodReadLines<T>(stream, new ReadLinesStreamOptions { LinesToSkip = recordsToSkip, Encoding = encoding }).ToArray());
            /*
                Assert.AreEqual(nameof(stream), exception.ParamName);    // Can't verify that 'stream' is the argument generating the exception due to a
                                                                         // .NET bug that causes the exception.ParamName to have a Null value.
                                                                         // See related GitHub issue:
                                                                         // FileStream constructor throws an ArgumentException with a null ParamName property #41970
                                                                         // https://github.com/dotnet/runtime/issues/41970
            */
            stream.Dispose();
        }

        #endregion Unhappy Path Tests
    }
}
