# 🔍 Html Serializer & Crawler

A high-performance C# HTML parser that converts web pages into a hierarchical object tree. This project demonstrates advanced programming concepts such as **Recursive Algorithms**, **Singleton Pattern**, and **Regex Processing**.

## ✨ Key Features

* **HTML Tree Construction:** Builds a full DOM tree using a custom `HtmlElement` structure.
* **CSS Selector Engine:** Supports complex queries like `div#main.content a` using a custom `Selector` parser.
* **Memory Efficient:** Uses **Queue-based BFS** for traversing descendants to prevent Stack Overflow.
* **Singleton Pattern:** Efficient management of HTML tag metadata via `HtmlHelper`.

## 💻 Usage Example

```csharp
var serializer = new HtmlSerializer();
var root = await serializer.Serialize("[https://example.com](https://example.com)");

// Find all links inside a specific div
var results = root.FindElements("div#main.content a");
🚀 Getting Started
Clone the repository: git clone https://github.com/Tamar-wolpe/HtmlProcessor.git

Setup: Ensure HtmlTags.json and HtmlVoidTags.json are present in the project directory.

Open: Load Html_Serializer.sln in Visual Studio.

Run: Press F5 to execute the demo.

🛠️ Built With
C# / .NET 8

System.Text.Json

Regular Expressions (Regex)
