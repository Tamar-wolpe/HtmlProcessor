using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class Selector
{
    private const char V = '.';
    private const char S = '#';

    public string TagName { get; set; }
    public string Id { get; set; }
    public List<string> Classes { get; set; } = new List<string>();

    // מאפייני היררכיה של סלקטורים
    public Selector Parent { get; set; }
    public Selector Child { get; set; }

    // פונקציה סטטית להמרת מחרוזת שאילתה לאובייקט Selector
    public static Selector Parse(string query)
    {
        var parts = query.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return null;

        Selector rootSelector = null;
        Selector currentSelector = null;

        foreach (var part in parts)
        {
            Selector newSelector = new Selector();

            // פירוק המחרוזת לחלקים לפי המפרידים # ו-. (נקודה)
            var tokens = Regex.Split(part, @"([#.][^#.]*)").Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            foreach (var token in tokens)
            {
                if (token.StartsWith(S.ToString()))
                {
                    newSelector.Id = token.Substring(1);
                }
                else if (token.StartsWith(V.ToString()))
                {
                    newSelector.Classes.Add(token.Substring(1));
                }
                else // שם תגית
                {
                    // יש לבדוק אם הוא שם תקין של תגית HTML
                    if (HtmlHelper.Instance.AllHtmlTags.Contains(token.ToLower()))
                    {
                        newSelector.TagName = token.ToLower();
                    }
                }
            }

            // בניית ההיררכיה
            if (rootSelector == null)
            {
                rootSelector = newSelector;
                currentSelector = newSelector;
            }
            else
            {
                newSelector.Parent = currentSelector;
                currentSelector.Child = newSelector;
                currentSelector = newSelector;
            }
        }

        return rootSelector;
    }
}