namespace Parallel_Monitor_of_Files_in_Directory.Models;

public class FileContext
{
    public string DirectoryPath { get; set; }
    public string FileName { get; set; }
    public List<string> FileHasKeyWords { get; set; }
    public IEnumerable<string>? KeyWords { get; set; }

    public FileContext(string directoryPath, string fileName, string fileHasKeyWords, IEnumerable<string> keyWords) 
    {
        DirectoryPath = directoryPath;
        FileName = fileName;
        FileHasKeyWords = new List<string> { fileHasKeyWords };
        KeyWords = keyWords;
    }


    public FileContext(string directoryPath, string fileName, List<string>? fileHasKeyWords, IEnumerable<string>? keyWords)
    {
        DirectoryPath = directoryPath;
        FileName = fileName;

        if(fileHasKeyWords is null)
        {

            FileHasKeyWords = new List<string>();

        }
        else
        {

            FileHasKeyWords = fileHasKeyWords;
            KeyWords = keyWords;

        }
    }


    public void AddKeyWord(string keyWord)
    {

        if (!FileHasKeyWords.Contains(keyWord))
        {

            FileHasKeyWords.Add(keyWord);

        }

    }


    public override string ToString()
    {
        Console.ResetColor();
        // Pick a unique color for each call, avoiding repeats in a single run
        var colors = Enum.GetValues<ConsoleColor>().Where(c => c != ConsoleColor.Black).ToList();
        var random = new Random();
        Console.ForegroundColor = colors[random.Next(colors.Count)];

        string kw = string.Empty;
        if (KeyWords != null)
        {
            foreach (string kwt in KeyWords)
                kw += $"{kwt}; ";
        }
        return $"\n\nDirectory: {DirectoryPath}\nFile Name: {FileName},\nKeyWords: [ {kw} ], \nFile with these KeyWords: [ {string.Join(", ", FileHasKeyWords)} ]";
    }
}
