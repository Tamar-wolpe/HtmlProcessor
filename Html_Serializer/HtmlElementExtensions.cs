using System.Collections.Generic;
using System.Linq;

public static class HtmlElementExtensions
{
    // פונקצית Descendants (באמצעות Queue למניעת Stack Overflow)
    public static IEnumerable<HtmlElement> Descendants(this HtmlElement element)
    {
        // ה-yield return הופך את הפונקציה ל-Iterator
        if (element == null) yield break;

        var queue = new Queue<HtmlElement>();
        queue.Enqueue(element); // דוחף את האלמנט הנוכחי (this)

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            // הוספת האלמנט הנוכחי לרשימת הצאצאים (כולל ה'שורש' של הקריאה)
            if (current != element) // לא להחזיר את האלמנט שעליו הפעלנו את הפונקציה
            {
                yield return current;
            }

            // הוספת הבנים לתור
            foreach (var child in current.Children)
            {
                queue.Enqueue(child);
            }
        }
    }

    // פונקצית Ancestors
    public static IEnumerable<HtmlElement> Ancestors(this HtmlElement element)
    {
        var current = element.Parent; // מתחיל מהאבא
        while (current != null)
        {
            yield return current;
            current = current.Parent;
        }
    }

    // פונקצית הרחבה לחיפוש אלמנטים בעץ לפי סלקטור
    public static HashSet<HtmlElement> FindElementsBySelector(this HtmlElement element, Selector selector)
    {
        // שימוש ב-HashSet למניעת כפילויות בתוצאות
        var results = new HashSet<HtmlElement>();
        FindElementsRecursive(element, selector, results);
        return results;
    }

    // הפונקציה הריקורסיבית
    private static void FindElementsRecursive(HtmlElement currentElement, Selector currentSelector, HashSet<HtmlElement> results)
    {
        if (currentSelector == null)
        {
            // תנאי עצירה: הגעת לסלקטור האחרון
            // זה לא אמור לקרות אם תנאי העצירה מטופל ב-Descendants
            return;
        }

        // 1. קבלת רשימת כל הצאצאים
        var descendants = currentElement.Descendants().ToList();

        // כולל את האלמנט הנוכחי אם הוא לא השורש כדי להתחיל את החיפוש מהרמה הנוכחית
        if (currentElement.Parent != null || currentElement.Name == "html")
        {
            descendants.Insert(0, currentElement);
        }

        // 2. סינון הצאצאים לפי הקריטריונים של הסלקטור הנוכחי
        var filteredElements = descendants.Where(e => SelectorMatch(e, currentSelector)).ToList();

        // 3. תנאי עצירה - הגענו לסלקטור האחרון
        if (currentSelector.Child == null)
        {
            // הוספת הרשימה המסוננת לאוסף התוצאות (HashSet מטפל בכפילויות)
            foreach (var element in filteredElements)
            {
                results.Add(element);
            }
            return;
        }

        // 4. הפעלת ריקורסיה על הרשימה המסוננת עם הסלקטור הבא
        foreach (var nextElement in filteredElements)
        {
            FindElementsRecursive(nextElement, currentSelector.Child, results);
        }
    }

    // פונקצית עזר לבדיקת התאמה לקריטריונים
    private static bool SelectorMatch(HtmlElement element, Selector selector)
    {
        // בדיקת TagName
        if (!string.IsNullOrEmpty(selector.TagName) && element.Name.ToLower() != selector.TagName.ToLower())
        {
            return false;
        }

        // בדיקת Id
        if (!string.IsNullOrEmpty(selector.Id) && element.Id != selector.Id)
        {
            return false;
        }

        // בדיקת Classes
        if (selector.Classes.Count > 0)
        {
            foreach (var requiredClass in selector.Classes)
            {
                if (!element.Classes.Contains(requiredClass))
                {
                    return false;
                }
            }
        }

        return true;
    }
}