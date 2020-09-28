# ReadLines&lt;T>() #
`ReadLines<T>()` is a collection of [.NET Core](https://docs.microsoft.com/en-us/dotnet/core/introduction) method overloads dedicated to reading lines of text from a source and returning a sequence of strongly typed instances of a generic type.  The `ReadLines<T>()` methods provide functionality, testability, and configurability that goes beyond what the equivalent .NET alternatives provide.

The `.NET Framework` provides several options for reading text and returning a sequence of lines:  There is for example, [System.IO.File.ReadLines(string)](https://docs.microsoft.com/en-us/dotnet/api/system.io.file.readlines?view=netcore-3.1) and its cousin [System.IO.File.ReadAllLines(string)](https://docs.microsoft.com/en-us/dotnet/api/system.io.file.readalllines?view=netcore-3.1).  Both of those two methods return a sequence of strings, where they differ is in that `ReadAllLines()` needs to read the entire input file before returning a value, while `ReadLines()` allows the user to start enumerating the results right away.

 `.NET` also provides other options for reading text and returning a sequence of lines although they require a bit more effort to achieve the type of functionality being discussed here:  [System.IO.StringReader.ReadLine()](https://docs.microsoft.com/en-us/dotnet/api/system.io.stringreader.readline?view=netcore-3.1) and [System.IO.StreamReader.ReadLine()](https://docs.microsoft.com/en-us/dotnet/api/system.io.stringreader.readline?view=netcore-3.1).  Both of those methods return a single line of text, so for our purpose of generating a sequence of strongly typed instances of a generic type it is necessary to use them within a loop that yields the required objects and keeps reading from the input source until no more lines of text are available.
 
 Among the options mentioned above the one that is closer to our intended purpose is [System.IO.File.ReadLines(string)](https://docs.microsoft.com/en-us/dotnet/api/system.io.file.readlines?view=netcore-3.1) as it returns an `IEnumerable<String>` collection that can then be processed further with a [LINQ](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/) [Select](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/) statement that projects elements of the required type.  While [System.IO.File.ReadLines(string)](https://docs.microsoft.com/en-us/dotnet/api/system.io.file.readlines?view=netcore-3.1) might seem like a good choice for reading lines of text and ending up with a sequence of a generic type, it does have some drawbacks:
- It returns a sequence of strings, so further processing is required to construct instances of the generic class being sought.
- It shouldn't be used in unit tests as it interacts directly with the file system.
- The only configuration option it provides is for `Encoding` via an overload. 

The `ReadLines<T>()` overloads provided here try to overcome those issues by:
- Directly constructing instances of the expected generic type without requiring additional programming.
- Providing testability via method overloads that take a [System.IO.Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.file.readalllinesasync?view=netcore-3.1) as an input source.  Using a `Stream` has the advantage of offering flexibility:  A `Stream` can be constructed from a file by using a [System.IO.FileStream](https://docs.microsoft.com/en-us/dotnet/api/system.io.filestream?view=netcore-3.1) or in memory by using a [System.IO.MemoryStream](https://docs.microsoft.com/en-us/dotnet/api/system.io.memorystream?view=netcore-3.1) instead.  In a program a `FileStream` might be a good choice whereas a unit test is likely to use a `MemoryStream`.  Please note that while `.NET` provides many other `Stream` choices in addition to those two, the `ReadLines<T>()` methods that take a `Stream` argument do not restrict on the type of `Stream` they receive and there might be unpredictable results if `Stream` types other than `FileStream` or `MemoryStream` are used.   
- Testability is also improved by the separation of concerns that is intrinsic to the `ReadLines<T>()` overloads:  The generic class type provided to `ReadLines<T>()` is responsible for parsing each line of input data, this allows the data parsing to be tested separately from the business logic.
- More configuration options.  For example, the only option that [System.IO.File.ReadLines(string)](https://docs.microsoft.com/en-us/dotnet/api/system.io.file.readlines?view=netcore-3.1) has is an overload that takes an `Encoding` argument.  In addition to `Encoding`, some of the `ReadLines<T>()` overloads also provide for setting the number of lines of input text that should be skipped from processing, changing the input buffer size, and other options.

In summary, the `ReadLines<T>()` overloads provided here are a flexible alternative to what `.NET` offers for reading lines of text, they are a unified mechanism for reading from a text source and creating a strongly typed sequence of elements of a generic class; in addition they provide optional testability and configurability.

## Examples: ##

Below is an example of a method that calculates payroll for employess that worked within a given range of dates:  it reads data from a text file and returns a sequence with the pay amounts for each employee.  This method uses `.NET`'s `File.ReadLines()` to read the input data.  The method is not testable via unit tests.

The input file is a CSV file that has been `UTF7` encoded and has one header line.  The file is expected to have lines of text like the following:

Id, First Name, Last Name, Date, Hours Worked, Hourly Wage, Withholding Pct.  
15, John, Smith, 10/15/2020, 6.5, 22.25, .25  
15, John, Smith, 10/16/2020, 5, 22.25, .25  
15, John, Smith, 10/17/2020, 8, 22.25, .25  
 7, Mary, Jones, 9/12/2020, 8.5, 62.45, .33  
 7, Mary, Jones, 10/15/2020, 6.5, 62.45, .33  
99, Steve, Johnson, 10/15/2020, 4.5, 15.50, .20  

```C#
public static IEnumerable<(int Id, string FirstName, string LastName, float TotalNetPay)> CalculatePayroll(string fileName, DateTime startDate, DateTime endDate)
{
    (int id, string firstName, string lastName, DateTime date, float hoursWorked, float hourlyWage, float withHoldingPct) ParseElements(string[] elements)
    {
        const int _INDEXID             = 0,
                  _INDEXFIRSTNAME      = 1,
                  _INDEXLASTNAME       = 2,
                  _INDEXDATE           = 3,
                  _INDEXHOURSWORKED    = 4,
                  _INDEXHOURLYWAGE     = 5,
                  _INDEXWITHHOLDINGPCT = 6;

        int id               = int.Parse(elements[_INDEXID]);
        string firstName     = elements[_INDEXFIRSTNAME].Trim();
        string lastName      = elements[_INDEXLASTNAME].Trim();
        DateTime date        = DateTime.Parse(elements[_INDEXDATE]);
        float hoursWorked    = float.Parse(elements[_INDEXHOURSWORKED]);
        float hourlyWage     = float.Parse(elements[_INDEXHOURLYWAGE]);
        float withHoldingPct = float.Parse(elements[_INDEXWITHHOLDINGPCT]);
        return (id, firstName, lastName, date, hoursWorked, hourlyWage, withHoldingPct);
    }

    const int linesToSkip = 1;
    int linesSkipped      = 0;
    Dictionary<int, (int Id, string FirstName, string LastName, float TotalNetPay)> dictionary = new Dictionary<int, (int, string, string, float)>();
    foreach (string textLine in File.ReadLines(fileName, Encoding.UTF7))
    {
        if (linesSkipped >= linesToSkip)
        {
            string[] lineElements = textLine.Split(',');
            (int id, string firstName, string lastName, DateTime date, float hoursWorked, float hourlyWage, float withHoldingPct) = ParseElements(lineElements);
            if (date >= startDate && date <= endDate)
            {
                float totalNetPay = hourlyWage * hoursWorked * (1 - withHoldingPct);
                if (dictionary.ContainsKey(id))
                    dictionary[id] = (id, firstName, lastName, dictionary[id].TotalNetPay + totalNetPay);
                else
                    dictionary.Add(id, (id, firstName, lastName, totalNetPay));
            }
        }
        else
            linesSkipped++;
    }
    return dictionary.Values;
}
```
Below is an alternate version that uses one of the `ReadLines<T>()` overloads provided here.  This method can be tested with unit tests and the data parsing logic can be tested separately from the business logic:
```C#
public static IEnumerable<(int Id, string FirstName, string LastName, float TotalNetPay)> CalculatePayroll(Stream stream, DateTime startDate, DateTime endDate)
{
    if (stream == null) throw new ArgumentNullException(nameof(stream));
    PayrollRawData[] payrollRawDatas = ReadLinesUtils.ReadLines<PayrollRawData>(stream, new ReadLinesStreamOptions { LinesToSkip = 1, Encoding = Encoding.UTF7 })
                                                     .Where(payrollRawData => payrollRawData.Date >= startDate && payrollRawData.Date <= endDate)
                                                     .ToArray();
    var anon1 = payrollRawDatas.Select(payrollRawData => 
        new {
                payrollRawData.Id, payrollRawData.FirstName, payrollRawData.LastName,
                NetPayForDate = payrollRawData.HourlyWage * payrollRawData.HoursWorked * (1 - payrollRawData.WithholdingPct)
            });
    var groupings = anon1.GroupBy(anon => anon.Id);
    var anon2 = groupings.Select(groupItems => 
        new {
                Id = groupItems.Key,
                groupItems.First().FirstName,
                groupItems.First().LastName,
                TotalNetPay = groupItems.Sum(groupItem => groupItem.NetPayForDate)
            });
    IEnumerable<(int Id, string FirstName, string LastName, float TotalNetPay)> result = anon2.Select(a => (a.Id, a.FirstName, a.LastName, a.TotalNetPay));
    return result;
}
```
The input data in this example is parsed by the following class:
```C#
public sealed class PayrollRawData
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private const int _INDEXID             = 0,
                      _INDEXFIRSTNAME      = 1,
                      _INDEXLASTNAME       = 2,
                      _INDEXDATE           = 3,
                      _INDEXHOURSWORKED    = 4,
                      _INDEXHOURLYWAGE     = 5,
                      _INDEXWITHHOLDINGPCT = 6;

    public PayrollRawData(string value) : this(value.Split(','))
    {
    }

    public PayrollRawData(string[] values) : this(int.Parse(values[_INDEXID]), values[_INDEXFIRSTNAME].Trim(), values[_INDEXLASTNAME].Trim(), 
                                                    DateTime.Parse(values[_INDEXDATE]),
                                                    float.Parse(values[_INDEXHOURSWORKED]), float.Parse(values[_INDEXHOURLYWAGE]),
                                                    float.Parse(values[_INDEXWITHHOLDINGPCT]))
    {
    }

    public PayrollRawData(int id, string firstName, string lastName, DateTime date, float hoursWorked, float hourlyWage, float withholdingPct)
    {
        Id             = id;
        FirstName      = firstName;
        LastName       = lastName;
        Date           = date;
        HoursWorked    = hoursWorked;
        HourlyWage     = hourlyWage;
        WithholdingPct = withholdingPct;
    }

    public int Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime Date { get; }
    public float HoursWorked { get; }
    public float HourlyWage { get; }
    public float WithholdingPct { get; }
}
```