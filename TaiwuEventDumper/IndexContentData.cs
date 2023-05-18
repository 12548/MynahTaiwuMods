namespace TaiwuEventDumper;

public class IndexContentData
{
    public string Key;
    public string Name;
    public string Author;
    public bool Export = false;

    public Dictionary<string, Dictionary<object, string>> AllEventContent = new();

    public IndexContentData(string key, string name, string author)
    {
        Key = key;
        Name = name;
        Author = author;
    }
}
