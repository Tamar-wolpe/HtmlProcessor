using System;
using System.IO;
using System.Text.Json;

public class HtmlHelper
{
    // מופע יחיד סטטי של המחלקה
    private static HtmlHelper _instance;
    // אובייקט נעילה לתמיכה ב-Threading
    private static readonly object _lock = new object();

    public string[] AllHtmlTags { get; private set; }
    public string[] SelfClosingTags { get; private set; }

    // בנאי פרטי למניעת יצירת מופעים מבחוץ
    private HtmlHelper(object jsonSerializer)
    {
        string allTagsPath = "HtmlTags.json";
        string selfClosingTagsPath = "SelfClosingTags.json";

        try
        {
            string allTagsJson = File.ReadAllText(allTagsPath);
            AllHtmlTags = System.Text.Json.JsonSerializer.Deserialize<string[]>(allTagsJson);

            string selfClosingTagsJson = File.ReadAllText(selfClosingTagsPath);
            SelfClosingTags = System.Text.Json.JsonSerializer.Deserialize<string[]>(selfClosingTagsJson);
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error loading HTML tags: {ex.Message}. Make sure the JSON files are in the correct directory.");
            AllHtmlTags = Array.Empty<string>();
            SelfClosingTags = Array.Empty<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during JSON deserialization: {ex.Message}");
            AllHtmlTags = Array.Empty<string>();
            SelfClosingTags = Array.Empty<string>();
        }

        JsonSerializer = jsonSerializer;
    }

    public HtmlHelper()
    {
    }

    // מאפיין סטטי להשגת המופע היחיד של המחלקה
    public static HtmlHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new HtmlHelper();
                    }
                }
            }
            return _instance;
        }
    }

    public object JsonSerializer { get; }
}