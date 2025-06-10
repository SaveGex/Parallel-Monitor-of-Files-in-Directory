using Parallel_Monitor_of_Files_in_Directory;

CancellationTokenSource cts = new CancellationTokenSource();
CancellationToken ct = cts.Token;

MonitorDirectory monitorDirectory = new MonitorDirectory(
    directoryPath: @"C:\Users\Opsik\Desktop\S\Ukraine language",
    keyWords: new List<string> { "що", "чому", "коли"},
    ct: ct
);


