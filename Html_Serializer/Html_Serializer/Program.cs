using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("--- Html Serializer & Query Project ---");

        // כתובת URL לדוגמה
        string url = "http://www.ynet.co.il"; // ניתן לשנות לכל אתר שתרצה לבדוק

        // שלב 1: Serialization (קריאה ובניית העץ)
        Console.WriteLine($"\n1. Loading and Serializing HTML from: {url}...");
        HtmlSerializer serializer = new HtmlSerializer();
        string htmlContent = await serializer.Load(url);

        if (string.IsNullOrEmpty(htmlContent))
        {
            Console.WriteLine("Failed to load HTML content. Exiting.");
            return;
        }

        HtmlElement rootElement = serializer.Serialize(htmlContent);

        if (rootElement == null)
        {
            Console.WriteLine("Failed to serialize HTML content.");
            return;
        }

        Console.WriteLine("HTML Tree built successfully.");

        // שלב 2: Query (תשאול העץ)

        // דוגמה 1: חיפוש כותרות h1
        string selector1 = "h1";
        Console.WriteLine($"\n2. Searching for elements matching selector: **{selector1}**");
        Selector h1Selector = Selector.Parse(selector1);
        HashSet<HtmlElement> h1Results = rootElement.FindElementsBySelector(h1Selector);

        Console.WriteLine($"Found {h1Results.Count} elements of type **{selector1}**:");
        foreach (var element in h1Results)
        {
            Console.WriteLine($"- Tag: <{element.Name}>, Id: {element.Id}, Classes: {string.Join(", ", element.Classes)}");
        }

        // דוגמה 2: חיפוש אלמנטים מורכבים (לפי class ו-id)
        // **יש לשנות את ה-ID וה-Class בהתאם לאתר שנטען!**
        string selector2 = "div#main_container .some-class";
        Console.WriteLine($"\n3. Searching for complex selector: **{selector2}**");
        Selector complexSelector = Selector.Parse(selector2);
        HashSet<HtmlElement> complexResults = rootElement.FindElementsBySelector(complexSelector);

        Console.WriteLine($"Found {complexResults.Count} elements matching the complex selector.");
        if (complexResults.Count > 0)
        {
            foreach (var element in complexResults)
            {
                // הצגת מידע על האלמנט שנמצא
                Console.WriteLine($"- Element Name: <{element.Name}>, Id: {element.Id}, Parent: {element.Parent?.Name}");
            }
        }

        // דוגמה 3: שימוש בפונקציית Descendants על אלמנט ספציפי
        Console.WriteLine("\n4. Using Descendants on the root element:");
        // לוקחים את 5 הצאצאים הראשונים (כולל ה'שורש' של הקריאה, אם הוא נכלל במימוש)
        var firstFiveDescendants = rootElement.Descendants().Take(5);
        Console.WriteLine("First 5 descendants (excluding root):");
        foreach (var d in firstFiveDescendants)
        {
            Console.WriteLine($"- <{d.Name}>, Parent: <{d.Parent?.Name}>");
        }
    }
}