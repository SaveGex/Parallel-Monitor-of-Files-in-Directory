using Microsoft.VisualBasic;
using Parallel_Monitor_of_Files_in_Directory.Interfaces;
using Parallel_Monitor_of_Files_in_Directory.Models;

namespace Parallel_Monitor_of_Files_in_Directory;

public class MonitorDirectory : IObserver
{
    public string DirectoryPath { get; private set; }

    public IEnumerable<string> FilesNames { get; private set; }

    public List<FileContext> FilesHasKeyWord { get; private set; }

    public IEnumerable<string>? KeyWords { get; private set; }

    private List<ISubscriber> subscribers = new List<ISubscriber>();

    private Thread _monitorThread;

    private CancellationToken? ct;

    private Printer _printer;


    public MonitorDirectory(string directoryPath, IEnumerable<string>? keyWords, CancellationToken? ct)
    {
        FilesNames = Directory.GetFiles(directoryPath)
            .Where(t => t.EndsWith(".txt"))
            .Select(fn => Path.GetFileName(fn));

        DirectoryPath = directoryPath;

        KeyWords = keyWords;

        FilesHasKeyWord =   new List<FileContext>();
        _monitorThread =    new Thread(MonitorFiles);
        _printer =          new Printer(Directory.GetFiles(directoryPath)
                                        .Where(t => t.EndsWith(".txt")));


        _monitorThread.Start();
        subscribers.Add(_printer);

    }


    private void MonitorFiles()
    {

        while (true && (ct.HasValue && ct.Value.IsCancellationRequested) is false)
        {

            IEnumerable<string> currentFiles = Directory.GetFiles(DirectoryPath)
                .Where(t => t.EndsWith(".txt"));
            
            if(FilesNames.Except(currentFiles).Any() || currentFiles.Except(FilesNames).Any())
            {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.CursorTop = 0;
                Console.CursorLeft = 0;
                // idk why it sets cursor not at the beginning of the console window... It's annoying actually!
                _printer.ChangeFIlesNames(currentFiles);
                Notify();
                ResearchByKeyWords(KeyWords);
                FilesNames = currentFiles;
            }


            Thread.Sleep(1000); // Sleep for 1 second to reduce CPU usage

        }
    }


    public async void ResearchByKeyWords(IEnumerable<string>? keyWords)
    {
        if(keyWords is null)
        {
            return;
        }
        
        FilesHasKeyWord.Clear();
        
        
        await foreach(KeyValuePair<string, string> FileWordPair in FilesHasWords(Directory.GetFiles(DirectoryPath).Where(p => p.EndsWith(".txt")), keyWords))
        {

            if (FilesHasKeyWord.Any(f => string.Equals(f.FileName, Path.GetFileName(FileWordPair.Key), StringComparison.OrdinalIgnoreCase)))
            {

                FilesHasKeyWord.First(f => f.FileName == Path.GetFileName(FileWordPair.Key)).AddKeyWord(FileWordPair.Value);
                continue;

            }
            else
            {

                FilesHasKeyWord.Add(new FileContext(DirectoryPath, Path.GetFileName(FileWordPair.Key), FileWordPair.Value, keyWords));

            }

        }
        var snapshot = FilesHasKeyWord.ToList();
        snapshot.ForEach(f => Console.WriteLine(f.ToString()));

    }

    //<summary> returns absolute paths of files that contain at least one of the keywords </summary>
    public async IAsyncEnumerable<KeyValuePair<string, string>> FilesHasWords(IEnumerable<string> FilesPaths, IEnumerable<string> KeyWords)
    {

        foreach (string filePath in FilesPaths)
        {

            string content = await File.ReadAllTextAsync(filePath);

            foreach(string keyWord in KeyWords)
            {

                if (content.Contains(keyWord))
                {
                    
                    yield return new KeyValuePair<string, string>(filePath, keyWord);

                }

            }
        }

    }


    public void Notify()
    {

        foreach(ISubscriber subscriber in subscribers)
        {

            subscriber.Update();

        }
    }
}