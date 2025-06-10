namespace Parallel_Monitor_of_Files_in_Directory;

internal class MonitorDirectory
{
    public string DirectoryPath { get; private set; }
    public IEnumerable<string> FilesNames { get; private set; }
    private Thread _monitorThread;
    private CancellationToken? ct;
    public MonitorDirectory(string directoryPath, CancellationToken? ct)
    {
        DirectoryPath = directoryPath;
        FilesNames = Directory.GetFiles(DirectoryPath);
        _monitorThread = new Thread(MonitorFiles);
        _monitorThread.Start(ct);
    }


    private void MonitorFiles()
    {
        while (true || )
        {

        }
    }
}