using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class HtmlSerializer
{
    private readonly HtmlHelper _htmlHelper = HtmlHelper.Instance;

    // 1. קריאה לדף אינטרנט
    public async Task<string> Load(string url)
    {
        // הגדרת client כדי להתנהג כדפדפן רגיל (מניעת חסימות)
        HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // זורק שגיאה לקודי סטטוס לא מוצלחים
            var html = await response.Content.ReadAsStringAsync();
            return html;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
            return string.Empty;
        }
    }

    // 2. פירוק לפי תגיות
    private List<string> SplitHtml(string html)
    {
        // Regular Expression לזיהוי תגיות (החל מ-> ומסתיימת ב-<)
        // [^>] - כל תו שאינו >
        var regex = new Regex("<(.*?)>");
        var matches = regex.Matches(html);

        List<string> parts = new List<string>();
        int lastIndex = 0;

        foreach (Match match in matches)
        {
            // הוספת הטקסט הפנימי (InnerHtml) שלפני התגית
            if (match.Index > lastIndex)
            {
                string innerText = html.Substring(lastIndex, match.Index - lastIndex);
                string cleanText = innerText.Trim();

                // ניקוי מחרוזות ריקות, ירידות שורה ורווחים מיותרים
                if (!string.IsNullOrWhiteSpace(cleanText))
                {
                    parts.Add(cleanText);
                }
            }

            // הוספת התגית עצמה ללא הסוגריים (<>)
            string tagContent = match.Groups[1].Value.Trim();
            if (!string.IsNullOrWhiteSpace(tagContent))
            {
                parts.Add(tagContent);
            }

            lastIndex = match.Index + match.Length;
        }

        return parts;
    }

    // 3. בניית העץ
    public HtmlElement Serialize(string html)
    {
        var parts = SplitHtml(html);
        if (parts.Count == 0)
        {
            return null;
        }

        // אוביקט השורש
        HtmlElement root = new HtmlElement { Name = "html", Id = "root" };
        // האלמנט הנוכחי (משתנה עקב האיטרציה)
        HtmlElement currentElement = root;

        foreach (var part in parts)
        {
            // אופציה 1: טקסט פנימי (InnerHtml)
            if (!part.StartsWith("/") && !_htmlHelper.AllHtmlTags.Contains(part.Split(' ')[0].ToLower()))
            {
                currentElement.InnerHtml += part;
                continue;
            }

            string firstWord = part.Split(' ')[0].ToLower();

            // אופציה 2: סוף ה-html
            if (firstWord == "html/")
            {
                // הגעת לסוף העץ, סיום הלולאה
                break;
            }

            // אופציה 3: תגית סוגרת
            if (firstWord.StartsWith("/"))
            {
                // עלי לרמה הקודמת בעץ - שימי באלמנט הנוכחי את האבא
                if (currentElement.Parent != null)
                {
                    currentElement = currentElement.Parent;
                }
                continue;
            }

            // אופציה 4: תגית פותחת
            if (_htmlHelper.AllHtmlTags.Contains(firstWord))
            {
                HtmlElement newElement = CreateElement(firstWord, part);

                // הוספת האלמנט החדש כבן של האלמנט הנוכחי
                newElement.Parent = currentElement;
                currentElement.Children.Add(newElement);

                // בדיקה אם התגית סוגרת את עצמה
                bool isSelfClosing = part.EndsWith("/") || _htmlHelper.SelfClosingTags.Contains(firstWord);

                // אם אינה סוגרת את עצמה, הפוך אותה לאלמנט הנוכחי
                if (!isSelfClosing)
                {
                    currentElement = newElement;
                }
            }
        }
        return root;
    }

    private HtmlElement CreateElement(string tagName, string fullTagContent)
    {
        HtmlElement element = new HtmlElement { Name = tagName };

        // ביטוי רגולרי לחילוץ attribute="value"
        var attrRegex = new Regex("(?<key>[a-zA-Z0-9_-]+)=\"(?<value>[^\"]*)\"");
        string attributesString = fullTagContent.Substring(tagName.Length).Trim();

        // Regular Expression: Attribute Parsing
        // **הערה: הפירוק הרגולרי יכול להיות מורכב יותר עבור כל המקרים האפשריים ב-HTML**
        var attributeMatches = attrRegex.Matches(attributesString);

        foreach (Match match in attributeMatches)
        {
            string key = match.Groups["key"].Value;
            string value = match.Groups["value"].Value;

            // עדכון Attributes
            element.Attributes.Add($"{key}=\"{value}\"");

            // עדכון Id
            if (key.ToLower() == "id")
            {
                element.Id = value;
            }

            // עדכון Classes (פיצול לפי רווח)
            else if (key.ToLower() == "class")
            {
                element.Classes.AddRange(value.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries));
            }
        }

        return element;
    }
}