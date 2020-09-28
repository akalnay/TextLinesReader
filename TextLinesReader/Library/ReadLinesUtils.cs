////////////////////////////////////////////////////////
// Copyright (c) Alejandro Kalnay                     //
// License: GNU GPLv3                                 //
////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;

namespace TextLinesReader
{
    public static class ReadLinesUtils
    {
        #region ReadLines<T>()

        /// <summary>
        /// Reads lines of text from a file and returns a sequence with instances of a generic class.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements in the returned sequence.
        /// </typeparam>
        /// <param name="path">
        /// Fully qualified file name of the text file that will be read to create a collection containing instances of a generic class.
        /// </param>
        /// <returns>A sequence containing instances of the generic class.</returns>
        /// <exception cref="ArgumentNullException">path is null
        /// </exception>
        /// <exception cref="ArgumentException">path is an empty string (""), contains only white space, or contains one or more invalid characters.
        /// <para></para>
        /// <para>or</para>
        /// <para></para>
        /// <para>path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in an NTFS environment.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a non-NTFS environment.
        /// </exception>
        /// <exception cref="FileNotFoundException">The file cannot be found</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <remarks>
        /// <para>
        /// The input file is a text file, the <see cref="ReadLines{T}(string)"/> method returns a sequence where each element in the sequence
        /// corresponds to a line in the input file.
        /// </para>
        /// <para>
        /// The generic type T must define a class with a constructor that takes a single string argument.  The T class' constructor must parse
        /// the string argument to create an instance of the T type.  There isn't a requirement on the layout of the value contained in the string 
        /// argument although typically it would be a string divided into comma separated values (CSV).  Alternatively, the string parameter could
        /// contain a string that can be divided into values of a fixed length.  It is up to the class' constructor to parse the string argument
        /// in a manner that corresponds to the layout of the input data.  The constructor may need to be chained into additional constructors to
        /// fully create the corresponding instance of the generic class.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// The example below reads data from an input file named "InputData.csv" located in the application's executable folder and containing
        /// the following 3 lines of text.
        /// </para>
        /// <para>Smith, John, jsmith@gmail.com,</para>
        /// <para>Jones, Mary, mjones @hotmail.com,</para>
        /// <para>Johnson, Steve, sjohnson @yahoo.com</para>
        /// <code>
        /// class Program
        /// {
        ///    static void Main(string[] args)
        ///    {
        ///       ProcessInputData();
        ///       Console.WriteLine("Press any key to end.");
        ///       Console.ReadKey();
        ///    }
        ///    
        ///    private static void ProcessInputData()
        ///    {
        ///       const string inputFileName    = "InputData.csv";
        ///       string dataFolder             = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        ///       string fullyQualifiedFileName = Path.Combine(dataFolder, inputFileName);
        ///       IEnumerable&lt;Person> values    = FileUtils.ReadLines&lt;Person>(fullyQualifiedFileName);
        ///       ShowValues(values);
        ///    }
        ///
        ///    private static void ShowValues(IEnumerable&lt;Person> values)
        ///    {
        ///       const int dividerLineLength = 50;
        ///       foreach (Person value in values)
        ///       {
        ///           Console.WriteLine($"First Name    = {value.FirstName}");
        ///           Console.WriteLine($"Last Name     = {value.LastName}");
        ///           Console.WriteLine($"Email Address = {value.EmailAddress}");
        ///           Console.WriteLine(new String('-', dividerLineLength));
        ///           Console.WriteLine();
        ///       }
        ///    }
        /// }
        /// 
        /// public class Person   
        /// {   
        ///    private const int _INDEXLASTNAME     = 0;   
        ///    private const int _INDEXFIRSTNAME    = 1;   
        ///    private const int _INDEXEMAILADDRESS = 2;   
        ///
        ///    public Person(string value) : this(value.Split(',')) { }   
        ///
        ///    public Person(string[] values) : this(values[_INDEXLASTNAME].Trim(), values[_INDEXFIRSTNAME].Trim(), values[_INDEXEMAILADDRESS].Trim()) { }   
        ///
        ///    public Person(string lastName, string firstName, string emailAddress)   
        ///    {   
        ///        LastName     = lastName;   
        ///        FirstName    = firstName;   
        ///        EmailAddress = emailAddress;   
        ///    }   
        ///
        ///    public string LastName { get; }   
        ///    public string FirstName { get; }   
        ///    public string EmailAddress { get; }   
        ///
        ///    public override string ToString() => $"{FirstName} {LastName} - {EmailAddress}";   
        /// }
        /// </code>
        /// <para><strong>Output:</strong></para>
        /// <para>First Name    = John</para>
        /// <para>Last Name     = Smith</para>
        /// <para>Email Address = jsmith@gmail.com</para>
        /// <para>---------------------------------------------</para>
        /// <para></para>
        /// <para>First Name    = Mary</para>
        /// <para>Last Name     = Jones</para>
        /// <para>Email Address = mjones@hotmail.com</para>
        /// <para>---------------------------------------------</para>
        /// <para></para>
        /// <para>First Name    = Steve</para>
        /// <para>Last Name     = Johnson</para>
        /// <para>Email Address = sjohnson@yahoo.com</para>
        /// <para>---------------------------------------------</para>
        /// </example>
        public static IEnumerable<T> ReadLines<T>(string path)
        {
            return ReadLines<T>(path, ReadLinesOptions.LinesToSkipDefault);
        }

        /// <summary>
        /// Reads lines of text from a file and returns a sequence with instances of a generic class.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements in the returned sequence.
        /// </typeparam>
        /// <param name="path">
        /// Fully qualified file name of the text file that will be read to create a collection containing instances of a generic class.
        /// </param>
        /// <param name="linesToSkip">
        /// Number of lines to skip when reading from the input file.
        /// </param>
        /// <returns>A sequence containing instances of the generic class.</returns>
        /// <exception cref="ArgumentNullException">path is null
        /// </exception>
        /// <exception cref="ArgumentException">path is an empty string (""), contains only white space, or contains one or more invalid characters.
        /// <para></para>
        /// <para>or</para>
        /// <para></para>
        /// <para>path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in an NTFS environment.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a non-NTFS environment.
        /// </exception>
        /// <exception cref="FileNotFoundException">The file cannot be found</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <remarks>
        /// <para>
        /// The input file is a text file, the <see cref="ReadLines{T}(string, long)"/> method returns a sequence where each element in the sequence
        /// corresponds to a line in the input file.  The output sequence will exclude the first <see cref="linesToSkip"/> specified.
        /// </para>
        /// <para>
        /// The generic type T must define a class with a constructor that takes a single string argument.  The T class' constructor must parse
        /// the string argument to create an instance of the T type.  There isn't a requirement on the layout of the value contained in the string 
        /// argument although typically it would be a string divided into comma separated values (CSV).  Alternatively, the string parameter could
        /// contain a string that can be divided into values of a fixed length.  It is up to the class' constructor to parse the string argument
        /// in a manner that corresponds to the layout of the input data.  The constructor may need to be chained into additional constructors to
        /// fully create the corresponding instance of the generic class.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// The example below reads data from an input file named "InputData.csv" located in the application's executable folder and containing
        /// the following 3 lines of text.  Notice that the output below does not include the first input line.
        /// </para>
        /// <para>Smith, John, jsmith@gmail.com,</para>
        /// <para>Jones, Mary, mjones @hotmail.com,</para>
        /// <para>Johnson, Steve, sjohnson @yahoo.com</para>
        /// <code>
        /// class Program
        /// {
        ///    static void Main(string[] args)
        ///    {
        ///       ProcessInputData();
        ///       Console.WriteLine("Press any key to end.");
        ///       Console.ReadKey();
        ///    }
        ///    
        ///    private static void ProcessInputData()
        ///    {
        ///       const string inputFileName    = "InputData.csv";
        ///       string dataFolder             = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        ///       string fullyQualifiedFileName = Path.Combine(dataFolder, inputFileName);
        ///       IEnumerable&lt;Person> values    = FileUtils.ReadLines&lt;Person>(fullyQualifiedFileName, 1);
        ///       ShowValues(values);
        ///    }
        ///
        ///    private static void ShowValues(IEnumerable&lt;Person> values)
        ///    {
        ///       const int dividerLineLength = 50;
        ///       foreach (Person value in values)
        ///       {
        ///           Console.WriteLine($"First Name    = {value.FirstName}");
        ///           Console.WriteLine($"Last Name     = {value.LastName}");
        ///           Console.WriteLine($"Email Address = {value.EmailAddress}");
        ///           Console.WriteLine(new String('-', dividerLineLength));
        ///           Console.WriteLine();
        ///       }
        ///    }
        /// }
        /// 
        /// public class Person   
        /// {   
        ///    private const int _INDEXLASTNAME     = 0;   
        ///    private const int _INDEXFIRSTNAME    = 1;   
        ///    private const int _INDEXEMAILADDRESS = 2;   
        ///
        ///    public Person(string value) : this(value.Split(',')) { }   
        ///
        ///    public Person(string[] values) : this(values[_INDEXLASTNAME].Trim(), values[_INDEXFIRSTNAME].Trim(), values[_INDEXEMAILADDRESS].Trim()) { }   
        ///
        ///    public Person(string lastName, string firstName, string emailAddress)   
        ///    {   
        ///        LastName     = lastName;   
        ///        FirstName    = firstName;   
        ///        EmailAddress = emailAddress;   
        ///    }   
        ///
        ///    public string LastName { get; }   
        ///    public string FirstName { get; }   
        ///    public string EmailAddress { get; }   
        ///
        ///    public override string ToString() => $"{FirstName} {LastName} - {EmailAddress}";   
        /// }
        /// </code>
        /// <para>First Name    = Mary</para>
        /// <para>Last Name     = Jones</para>
        /// <para>Email Address = mjones@hotmail.com</para>
        /// <para>---------------------------------------------</para>
        /// <para></para>
        /// <para>First Name    = Steve</para>
        /// <para>Last Name     = Johnson</para>
        /// <para>Email Address = sjohnson@yahoo.com</para>
        /// <para>---------------------------------------------</para>
        /// </example>
        public static IEnumerable<T> ReadLines<T>(string path, long linesToSkip)
        {
            return ReadLines<T>(path, new ReadLinesFileOptions { LinesToSkip = linesToSkip });
        }

        /// <summary>
        /// Reads lines of text from a file using the options specified and returns with sequence of instances of a generic class.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements in the returned sequence.
        /// </typeparam>
        /// <param name="path">
        /// Fully qualified file name of the text file that will be read to create a collection containing instances of a generic class.
        /// </param>
        /// <param name="readLinesFileOptions">
        /// <see cref="ReadLinesFileOptions"/> to use when reading the input file.
        /// </param>
        /// <returns>A sequence containing instances of the generic class.</returns>
        /// <exception cref="ArgumentNullException">path is null
        /// </exception>
        /// <exception cref="ArgumentException">path is an empty string (""), contains only white space, or contains one or more invalid characters.
        /// <para></para>
        /// <para>or</para>
        /// <para></para>
        /// <para>path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in an NTFS environment.</para>
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a non-NTFS environment.
        /// </exception>
        /// <exception cref="FileNotFoundException">The file cannot be found</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <remarks>
        /// <para>
        /// The input file is a text file, the <see cref="ReadLines{T}(string, ReadLinesFileOptions)"/> method returns a sequence where each element in the sequence
        /// corresponds to a line in the input file.
        /// </para>
        /// <para>
        /// The generic type T must define a class with a constructor that takes a single string argument.  The T class' constructor must parse
        /// the string argument to create an instance of the T type.  There isn't a requirement on the layout of the value contained in the string 
        /// argument although typically it would be a string divided into comma separated values (CSV).  Alternatively, the string parameter could
        /// contain a string that can be divided into values of a fixed length.  It is up to the class' constructor to parse the string argument
        /// in a manner that corresponds to the layout of the input data.  The constructor may need to be chained into additional constructors to
        /// fully create the corresponding instance of the generic class.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// The example below reads data from an input file named "InputData.csv" that has Unicode encoding, is located in the application's executable folder, and contains
        /// the following 3 lines of text.
        /// </para>
        /// <para>Smith, John, jsmith@gmail.com,</para>
        /// <para>Jones, Mary, mjones @hotmail.com,</para>
        /// <para>Johnson, Steve, sjohnson @yahoo.com</para>
        /// <code>
        /// class Program
        /// {
        ///    static void Main(string[] args)
        ///    {
        ///       ProcessInputData();
        ///       Console.WriteLine("Press any key to end.");
        ///       Console.ReadKey();
        ///    }
        ///    
        ///    private static void ProcessInputData()
        ///    {
        ///       const string inputFileName    = "InputData.csv";
        ///       string dataFolder             = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        ///       string fullyQualifiedFileName = Path.Combine(dataFolder, inputFileName);
        ///       IEnumerable&lt;Person> values    = FileUtils.ReadLines&lt;Person>(fullyQualifiedFileName, new ReadLinesFileOptions { Encoding = Encoding.Unicode });
        ///       ShowValues(values);
        ///    }
        ///
        ///    private static void ShowValues(IEnumerable&lt;Person> values)
        ///    {
        ///       const int dividerLineLength = 50;
        ///       foreach (Person value in values)
        ///       {
        ///           Console.WriteLine($"First Name    = {value.FirstName}");
        ///           Console.WriteLine($"Last Name     = {value.LastName}");
        ///           Console.WriteLine($"Email Address = {value.EmailAddress}");
        ///           Console.WriteLine(new String('-', dividerLineLength));
        ///           Console.WriteLine();
        ///       }
        ///    }
        /// }
        /// 
        /// public class Person   
        /// {   
        ///    private const int _INDEXLASTNAME     = 0;   
        ///    private const int _INDEXFIRSTNAME    = 1;   
        ///    private const int _INDEXEMAILADDRESS = 2;   
        ///
        ///    public Person(string value) : this(value.Split(',')) { }   
        ///
        ///    public Person(string[] values) : this(values[_INDEXLASTNAME].Trim(), values[_INDEXFIRSTNAME].Trim(), values[_INDEXEMAILADDRESS].Trim()) { }   
        ///
        ///    public Person(string lastName, string firstName, string emailAddress)   
        ///    {   
        ///        LastName     = lastName;   
        ///        FirstName    = firstName;   
        ///        EmailAddress = emailAddress;   
        ///    }   
        ///
        ///    public string LastName { get; }   
        ///    public string FirstName { get; }   
        ///    public string EmailAddress { get; }   
        ///
        ///    public override string ToString() => $"{FirstName} {LastName} - {EmailAddress}";   
        /// }
        /// </code>
        /// <para><strong>Output:</strong></para>
        /// <para>First Name    = John</para>
        /// <para>Last Name     = Smith</para>
        /// <para>Email Address = jsmith@gmail.com</para>
        /// <para>---------------------------------------------</para>
        /// <para></para>
        /// <para>First Name    = Mary</para>
        /// <para>Last Name     = Jones</para>
        /// <para>Email Address = mjones@hotmail.com</para>
        /// <para>---------------------------------------------</para>
        /// <para></para>
        /// <para>First Name    = Steve</para>
        /// <para>Last Name     = Johnson</para>
        /// <para>Email Address = sjohnson@yahoo.com</para>
        /// <para>---------------------------------------------</para>
        /// </example>
        public static IEnumerable<T> ReadLines<T>(string path, ReadLinesFileOptions readLinesFileOptions)
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                foreach (T dataInstance in ReadLines<T>(fileStream, new ReadLinesStreamOptions(readLinesFileOptions)))
                    yield return dataInstance;
        }

        /// <summary>
        /// Reads lines of text from a stream and returns a sequence with instances of a generic class.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements in the returned sequence.
        /// </typeparam>
        /// <param name="stream">
        /// The stream to read from in order to create a collection containing instances of a generic class.
        /// There are no restrictions on which <see cref="Stream"/> descendant is used for the <see cref="stream"/> parameter, however unexpected
        /// results may occur if the method is used with something other than a <see cref="FileStream"/> or <see cref="MemoryStream"/>.
        /// </param>
        /// <returns>A sequence containing instances of the generic class.</returns>
        /// <exception cref="ArgumentNullException">stream is null</exception>
        /// <exception cref="ArgumentException">stream is closed</exception>
        /// <remarks>
        /// <para>
        /// The input stream contains text, the <see cref="ReadLines{T}(Stream)"/> method returns a sequence where each element in the sequence
        /// corresponds to a line in the input stream.
        /// </para>
        /// <para>
        /// The generic type T must define a class with a constructor that takes a single string argument.  The T class' constructor must parse
        /// the string argument to create an instance of the T type.  There isn't a requirement on the layout of the value contained in the string 
        /// argument although typically it would be a string divided into comma separated values (CSV).  Alternatively, the string parameter could
        /// contain a string that can be divided into values of a fixed length.  It is up to the class' constructor to parse the string argument
        /// in a manner that corresponds to the layout of the input data.  The constructor may need to be chained into additional constructors to
        /// fully create the corresponding instance of the generic class.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// The example below shows a method that reads from a stream to calculate payroll for a range of dates.  
        /// The method uses the <see cref="ReadLines&lt;PayrollRawData>(stream)"/> method to read from the stream in order to create a sequence of
        /// <see cref="PayrollRawData"/> elements that
        /// gets processed further to perform the required calculations.
        /// This method can be tested with unit tests; for a unit test the <see cref="Stream"/> instance would most likely be a <see cref="MemoryStream"/>,
        /// in a program the stream might be a <see cref="FileStream"/> instead.
        /// </para>
        /// <code>
        /// public static IEnumerable&lt;(int Id, string FirstName, string LastName, float TotalNetPay)> CalculatePayroll(Stream stream, DateTime startDate, DateTime endDate)
        /// {
        ///    if (stream == null) throw new ArgumentNullException(nameof(stream));
        ///    PayrollRawData[] payrollRawDatas = ReadLinesUtils.ReadLines&lt;PayrollRawData>(stream)
        ///                                                     .Where(payrollRawData => payrollRawData.Date >= startDate &amp;&amp; payrollRawData.Date &lt;= endDate)
        ///                                                     .ToArray();
        ///    var anon1 = payrollRawDatas.Select(payrollRawData =>
        ///        new {
        ///                payrollRawData.Id,
        ///                payrollRawData.FirstName,
        ///                payrollRawData.LastName,
        ///                NetPayForDate = payrollRawData.HourlyWage * payrollRawData.HoursWorked * (1 - payrollRawData.WithholdingPct)
        ///            }); 
        ///    var groupings = anon1.GroupBy(anon => anon.Id);
        ///    var anon2 = groupings.Select(groupItems => 
        ///        new {
        ///                Id = groupItems.Key,
        ///                groupItems.First().FirstName,
        ///                groupItems.First().LastName,
        ///                TotalNetPay = groupItems.Sum(groupItem => groupItem.NetPayForDate)
        ///            });
        ///    IEnumerable&lt;(int Id, string FirstName, string LastName, float TotalNetPay)> result = anon2.Select(a => (a.Id, a.FirstName, a.LastName, a.TotalNetPay));
        ///    return result;
        /// }
        /// 
        /// public sealed class PayrollRawData
        /// {
        ///    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ///    private const int _INDEXID             = 0,
        ///                      _INDEXFIRSTNAME      = 1,
        ///                      _INDEXLASTNAME       = 2,
        ///                      _INDEXDATE           = 3,
        ///                      _INDEXHOURSWORKED    = 4,
        ///                      _INDEXHOURLYWAGE     = 5,
        ///                      _INDEXWITHHOLDINGPCT = 6;
        ///
        ///    public PayrollRawData(string value) : this(value.Split(','))
        ///    {
        ///    }
        ///
        ///    public PayrollRawData(string[] values) : this(int.Parse(values[_INDEXID]), values[_INDEXFIRSTNAME].Trim(), values[_INDEXLASTNAME].Trim(),
        ///                                                    DateTime.Parse(values[_INDEXDATE]),
        ///                                                    float.Parse(values[_INDEXHOURSWORKED]), float.Parse(values[_INDEXHOURLYWAGE]),
        ///                                                    float.Parse(values[_INDEXWITHHOLDINGPCT]))
        ///    {
        ///    }
        ///
        ///    public PayrollRawData(int id, string firstName, string lastName, DateTime date, float hoursWorked, float hourlyWage, float withholdingPct)
        ///    {
        ///        Id             = id;
        ///        FirstName      = firstName;
        ///        LastName       = lastName;
        ///        Date           = date;
        ///        HoursWorked    = hoursWorked;
        ///        HourlyWage     = hourlyWage;
        ///        WithholdingPct = withholdingPct;
        ///    }
        ///
        ///    public int Id { get; }
        ///    public string FirstName { get; }
        ///    public string LastName { get; }
        ///    public DateTime Date { get; }
        ///    public float HoursWorked { get; }
        ///    public float HourlyWage { get; }
        ///    public float WithholdingPct { get; }
        /// }
        /// </code>
        /// </example>
        public static IEnumerable<T> ReadLines<T>(Stream stream)
        {
            return ReadLines<T>(stream, ReadLinesOptions.LinesToSkipDefault);
        }

        /// <summary>
        /// Reads lines of text from a stream and returns a sequence with instances of a generic class.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements in the returned sequence.
        /// </typeparam>
        /// <param name="stream">
        /// The stream to read from in order to create a collection containing instances of a generic class.
        /// There are no restrictions on which <see cref="Stream"/> descendant is used for the <see cref="stream"/> parameter, however unexpected
        /// results may occur if the method is used with something other than a <see cref="FileStream"/> or <see cref="MemoryStream"/>.
        /// </param>
        /// <param name="linesToSkip">
        /// Number of lines to skip when reading from the input file.
        /// </param>
        /// <returns>A sequence containing instances of the generic class.</returns>
        /// <exception cref="ArgumentNullException">stream is null</exception>
        /// <exception cref="ArgumentException">stream is closed</exception>
        /// <remarks>
        /// <para>
        /// The input stream contains text, the <see cref="ReadLines{T}(string, long)"/> method returns a sequence where each element in the sequence
        /// corresponds to a line in the input stream.
        /// The output sequence will exclude the first <see cref="linesToSkip"/> specified.
        /// </para>
        /// <para>
        /// The generic type T must define a class with a constructor that takes a single string argument.  The T class' constructor must parse
        /// the string argument to create an instance of the T type.  There isn't a requirement on the layout of the value contained in the string 
        /// argument although typically it would be a string divided into comma separated values (CSV).  Alternatively, the string parameter could
        /// contain a string that can be divided into values of a fixed length.  It is up to the class' constructor to parse the string argument
        /// in a manner that corresponds to the layout of the input data.  The constructor may need to be chained into additional constructors to
        /// fully create the corresponding instance of the generic class.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// The example below shows a method that reads from a stream to calculate payroll for a range of dates.  
        /// The method uses the <see cref="ReadLines&lt;T>(Stream, long)"/> method to read from the stream in order to create a sequence of
        /// <see cref="PayrollRawData"/> elements that
        /// gets processed further to perform the required calculations.
        /// This method can be tested with unit tests; for a unit test the <see cref="Stream"/> instance would most likely be a <see cref="MemoryStream"/>,
        /// in a program the stream might be a <see cref="FileStream"/> instead.
        /// </para>
        /// <code>
        /// public static IEnumerable&lt;(int Id, string FirstName, string LastName, float TotalNetPay)> CalculatePayroll(Stream stream, DateTime startDate, DateTime endDate)
        /// {
        ///    if (stream == null) throw new ArgumentNullException(nameof(stream));
        ///    PayrollRawData[] payrollRawDatas = ReadLinesUtils.ReadLines&lt;PayrollRawData>(stream, 1)
        ///                                                     .Where(payrollRawData => payrollRawData.Date >= startDate &amp;&amp; payrollRawData.Date &lt;= endDate)
        ///                                                     .ToArray();
        ///    var anon1 = payrollRawDatas.Select(payrollRawData =>
        ///        new {
        ///                payrollRawData.Id,
        ///                payrollRawData.FirstName,
        ///                payrollRawData.LastName,
        ///                NetPayForDate = payrollRawData.HourlyWage * payrollRawData.HoursWorked * (1 - payrollRawData.WithholdingPct)
        ///            }); 
        ///    var groupings = anon1.GroupBy(anon => anon.Id);
        ///    var anon2 = groupings.Select(groupItems => 
        ///        new {
        ///                Id = groupItems.Key,
        ///                groupItems.First().FirstName,
        ///                groupItems.First().LastName,
        ///                TotalNetPay = groupItems.Sum(groupItem => groupItem.NetPayForDate)
        ///            });
        ///    IEnumerable&lt;(int Id, string FirstName, string LastName, float TotalNetPay)> result = anon2.Select(a => (a.Id, a.FirstName, a.LastName, a.TotalNetPay));
        ///    return result;
        /// }
        /// 
        /// public sealed class PayrollRawData
        /// {
        ///    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ///    private const int _INDEXID             = 0,
        ///                      _INDEXFIRSTNAME      = 1,
        ///                      _INDEXLASTNAME       = 2,
        ///                      _INDEXDATE           = 3,
        ///                      _INDEXHOURSWORKED    = 4,
        ///                      _INDEXHOURLYWAGE     = 5,
        ///                      _INDEXWITHHOLDINGPCT = 6;
        ///
        ///    public PayrollRawData(string value) : this(value.Split(','))
        ///    {
        ///    }
        ///
        ///    public PayrollRawData(string[] values) : this(int.Parse(values[_INDEXID]), values[_INDEXFIRSTNAME].Trim(), values[_INDEXLASTNAME].Trim(),
        ///                                                    DateTime.Parse(values[_INDEXDATE]),
        ///                                                    float.Parse(values[_INDEXHOURSWORKED]), float.Parse(values[_INDEXHOURLYWAGE]),
        ///                                                    float.Parse(values[_INDEXWITHHOLDINGPCT]))
        ///    {
        ///    }
        ///
        ///    public PayrollRawData(int id, string firstName, string lastName, DateTime date, float hoursWorked, float hourlyWage, float withholdingPct)
        ///    {
        ///        Id             = id;
        ///        FirstName      = firstName;
        ///        LastName       = lastName;
        ///        Date           = date;
        ///        HoursWorked    = hoursWorked;
        ///        HourlyWage     = hourlyWage;
        ///        WithholdingPct = withholdingPct;
        ///    }
        ///
        ///    public int Id { get; }
        ///    public string FirstName { get; }
        ///    public string LastName { get; }
        ///    public DateTime Date { get; }
        ///    public float HoursWorked { get; }
        ///    public float HourlyWage { get; }
        ///    public float WithholdingPct { get; }
        /// }
        /// </code>
        /// </example>
        public static IEnumerable<T> ReadLines<T>(Stream stream, long linesToSkip)
        {
            return ReadLines<T>(stream, new ReadLinesStreamOptions { LinesToSkip = linesToSkip });
        }

        /// <summary>
        /// Reads lines of text from a stream and returns a sequence with instances of a generic class.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the elements in the returned sequence.
        /// </typeparam>
        /// <param name="stream">
        /// The stream to read from in order to create a collection containing instances of a generic class.
        /// There are no restrictions on which <see cref="Stream"/> descendant is used for the <see cref="stream"/> parameter, however unexpected
        /// results may occur if the method is used with something other than a <see cref="FileStream"/> or <see cref="MemoryStream"/>.
        /// </param>
        /// <param name="readLinesStreamOptions">
        /// <see cref="ReadLinesStreamOptions"/> to use when reading the input file.
        /// </param>
        /// <returns>A sequence containing instances of the generic class.</returns>
        /// <exception cref="ArgumentNullException">stream is null</exception>
        /// <exception cref="ArgumentException">stream is closed</exception>
        /// <remarks>
        /// <para>
        /// The input stream contains text, the <see cref="ReadLines{T}(Stream, ReadLinesStreamOptions)"/> method returns a sequence where each element in the sequence
        /// corresponds to a line in the input stream.
        /// </para>
        /// <para>
        /// The generic type T must define a class with a constructor that takes a single string argument.  The T class' constructor must parse
        /// the string argument to create an instance of the T type.  There isn't a requirement on the layout of the value contained in the string 
        /// argument although typically it would be a string divided into comma separated values (CSV).  Alternatively, the string parameter could
        /// contain a string that can be divided into values of a fixed length.  It is up to the class' constructor to parse the string argument
        /// in a manner that corresponds to the layout of the input data.  The constructor may need to be chained into additional constructors to
        /// fully create the corresponding instance of the generic class.
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// The example below shows a method that reads from a stream to calculate payroll for a range of dates.  
        /// The method uses the <see cref="ReadLines{T}(Stream, ReadLinesStreamOptions)"/> method to read from the stream in order to create a sequence of
        /// <see cref="PayrollRawData"/> elements that
        /// gets processed further to perform the required calculations.
        /// This method can be tested with unit tests; for a unit test the <see cref="Stream"/> instance would most likely be a <see cref="MemoryStream"/>,
        /// in a program the stream might be a <see cref="FileStream"/> instead.
        /// </para>
        /// <code>
        /// public static IEnumerable&lt;(int Id, string FirstName, string LastName, float TotalNetPay)> CalculatePayroll(Stream stream, DateTime startDate, DateTime endDate)
        /// {
        ///    if (stream == null) throw new ArgumentNullException(nameof(stream));
        ///    PayrollRawData[] payrollRawDatas = ReadLinesUtils.ReadLines&lt;PayrollRawData>(stream,  new ReadLinesStreamOptions { Encoding = Encoding.Unicode })
        ///                                                     .Where(payrollRawData => payrollRawData.Date >= startDate &amp;&amp; payrollRawData.Date &lt;= endDate)
        ///                                                     .ToArray();
        ///    var anon1 = payrollRawDatas.Select(payrollRawData =>
        ///        new {
        ///                payrollRawData.Id,
        ///                payrollRawData.FirstName,
        ///                payrollRawData.LastName,
        ///                NetPayForDate = payrollRawData.HourlyWage * payrollRawData.HoursWorked * (1 - payrollRawData.WithholdingPct)
        ///            }); 
        ///    var groupings = anon1.GroupBy(anon => anon.Id);
        ///    var anon2 = groupings.Select(groupItems => 
        ///        new {
        ///                Id = groupItems.Key,
        ///                groupItems.First().FirstName,
        ///                groupItems.First().LastName,
        ///                TotalNetPay = groupItems.Sum(groupItem => groupItem.NetPayForDate)
        ///            });
        ///    IEnumerable&lt;(int Id, string FirstName, string LastName, float TotalNetPay)> result = anon2.Select(a => (a.Id, a.FirstName, a.LastName, a.TotalNetPay));
        ///    return result;
        /// }
        /// 
        /// public sealed class PayrollRawData
        /// {
        ///    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ///    private const int _INDEXID             = 0,
        ///                      _INDEXFIRSTNAME      = 1,
        ///                      _INDEXLASTNAME       = 2,
        ///                      _INDEXDATE           = 3,
        ///                      _INDEXHOURSWORKED    = 4,
        ///                      _INDEXHOURLYWAGE     = 5,
        ///                      _INDEXWITHHOLDINGPCT = 6;
        ///
        ///    public PayrollRawData(string value) : this(value.Split(','))
        ///    {
        ///    }
        ///
        ///    public PayrollRawData(string[] values) : this(int.Parse(values[_INDEXID]), values[_INDEXFIRSTNAME].Trim(), values[_INDEXLASTNAME].Trim(),
        ///                                                    DateTime.Parse(values[_INDEXDATE]),
        ///                                                    float.Parse(values[_INDEXHOURSWORKED]), float.Parse(values[_INDEXHOURLYWAGE]),
        ///                                                    float.Parse(values[_INDEXWITHHOLDINGPCT]))
        ///    {
        ///    }
        ///
        ///    public PayrollRawData(int id, string firstName, string lastName, DateTime date, float hoursWorked, float hourlyWage, float withholdingPct)
        ///    {
        ///        Id             = id;
        ///        FirstName      = firstName;
        ///        LastName       = lastName;
        ///        Date           = date;
        ///        HoursWorked    = hoursWorked;
        ///        HourlyWage     = hourlyWage;
        ///        WithholdingPct = withholdingPct;
        ///    }
        ///
        ///    public int Id { get; }
        ///    public string FirstName { get; }
        ///    public string LastName { get; }
        ///    public DateTime Date { get; }
        ///    public float HoursWorked { get; }
        ///    public float HourlyWage { get; }
        ///    public float WithholdingPct { get; }
        /// }
        /// </code>
        /// </example>
        public static IEnumerable<T> ReadLines<T>(Stream stream, ReadLinesStreamOptions readLinesStreamOptions)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            string line;
            long linesSkipped = 0;
            if (readLinesStreamOptions == null)
                readLinesStreamOptions = new ReadLinesStreamOptions();
            using (StreamReader streamReader = new StreamReader(stream, readLinesStreamOptions.Encoding, 
                                                                    readLinesStreamOptions.DetectEncodingFromByteOrderMarks,
                                                                    readLinesStreamOptions.BufferSize, readLinesStreamOptions.LeaveOpen))
                while ((line = streamReader.ReadLine()) != null)
                    if (linesSkipped < readLinesStreamOptions.LinesToSkip)
                        linesSkipped++;
                    else
                        yield return (T)Activator.CreateInstance(typeof(T), line);
        }

        #endregion ReadLines<T>()
    }
}
