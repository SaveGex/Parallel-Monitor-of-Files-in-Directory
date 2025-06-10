using Parallel_Monitor_of_Files_in_Directory.Interfaces;
using Parallel_Monitor_of_Files_in_Directory;
using Xunit;
using Xunit.Abstractions;


public class MyTest
{
    private readonly ITestOutputHelper _output;
    public MyTest(ITestOutputHelper output) => _output = output;

    [Fact]
    public void Test1()
    {
        _output.WriteLine("This will appear in the test output window.");
        Assert.True(true);
    }
}

public class MonitorDirectoryTests : IDisposable
{
    private readonly string _testDir;

    public MonitorDirectoryTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDir);
    }

    [Fact]
    public async Task ResearchByKeyWords_PopulatesFilesHasKeyWord()
    {
        // Arrange
        var file1 = Path.Combine(_testDir, "file1.txt");
        var file2 = Path.Combine(_testDir, "file2.txt");
        File.WriteAllText(file1, "hello world");
        File.WriteAllText(file2, "no keywords here");
        var keywords = new List<string> { "hello", "test" };
        var monitor = new MonitorDirectory(_testDir, keywords, null);

        // Act
        await Task.Delay(500); // Allow async void to run
        monitor.ResearchByKeyWords(keywords);
        await Task.Delay(500);

        // Assert
        Assert.Contains(monitor.FilesHasKeyWord, f => f.FileName == "file1.txt");
        Assert.DoesNotContain(monitor.FilesHasKeyWord, f => f.FileName == "file2.txt");
    }

    [Fact]
    public async Task FilesHasWords_YieldsCorrectFiles()
    {
        // Arrange
        var file1 = Path.Combine(_testDir, "a.txt");
        var file2 = Path.Combine(_testDir, "b.txt");
        File.WriteAllText(file1, "foo bar");
        File.WriteAllText(file2, "baz");
        var keywords = new List<string> { "foo", "baz" };
        var monitor = new MonitorDirectory(_testDir, keywords, null);

        // Act
        var results = new List<KeyValuePair<string, string>>();
        await foreach (var pair in monitor.FilesHasWords(new[] { file1, file2 }, keywords))
        {
            results.Add(pair);
        }

        // Assert
        Assert.Contains(results, p => p.Key == file1 && p.Value == "foo");
        Assert.Contains(results, p => p.Key == file2 && p.Value == "baz");
    }

    [Fact]
    public void Notify_CallsSubscriberUpdate()
    {
        // Arrange
        var monitor = new MonitorDirectory(_testDir, new List<string> { "x" }, null);
        var mockSubscriber = new MockSubscriber();
        var subscribersField = typeof(MonitorDirectory).GetField("subscribers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var list = (List<ISubscriber>)subscribersField.GetValue(monitor);
        list.Add(mockSubscriber);

        // Act
        monitor.Notify();

        // Assert
        Assert.True(mockSubscriber.Updated);
    }

    public void Dispose()
    {
        Directory.Delete(_testDir, true);
    }

    private class MockSubscriber : ISubscriber
    {
        public bool Updated { get; private set; }
        public void Update() => Updated = true;
    }
}
