using System.Collections.Generic;

public class HtmlElement
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<string> Attributes { get; set; } = new List<string>();
    public List<string> Classes { get; set; } = new List<string>();
    public string InnerHtml { get; set; }

    // מאפייני העץ
    public HtmlElement Parent { get; set; }
    public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();

    // מאפיין לשימוש ב-HashSet למניעת כפילויות בחיפוש (מומלץ בתיאור הפרויקט)
    // הערה: אין צורך במאפיין 'Visited' כפי שהוצע כדרך פרימיטיבית.
    // השימוש ב-HashSet במחלקת ה-Query ימנע כפילויות.

    public override bool Equals(object obj)
    {
        // בדיקת שוויון על סמך מופע (Reference Equality)
        return ReferenceEquals(this, obj);
    }

    public override int GetHashCode()
    {
        // שימוש ב-Reference Equality לקבלת Hash Code ייחודי לכל מופע
        return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this);
    }
}