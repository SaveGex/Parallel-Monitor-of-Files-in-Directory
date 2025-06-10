using Parallel_Monitor_of_Files_in_Directory.Interfaces;

namespace Parallel_Monitor_of_Files_in_Directory;

internal class Printer : ISubscriber
{
    private IEnumerable<string> _filesNames;
    public Printer(IEnumerable<string> filesNames) 
    {
        _filesNames = filesNames;
        Print(filesNames);
    }

    public void Print(string message)
    {

        Console.WriteLine(message);
    }

    public void Print(IEnumerable<string> filesNames)
    {

        foreach (string fileName in filesNames)
        {
            Console.Write(fileName + "\n");
        }
        Console.WriteLine("\n\n\n");
    }


    public void ChangeFIlesNames(IEnumerable<string> filesNames)
    {
        _filesNames = filesNames;

    }


    public void Update()
    {
        Print(_filesNames);
    }
}
