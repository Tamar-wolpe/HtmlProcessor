# 🔍 Html Serializer & Crawler

A high-performance C# HTML parser that converts web pages into a hierarchical object tree. 
This project demonstrates advanced programming concepts such as **Recursive Algorithms**, **Singleton Pattern**, and **Regex Processing**.

## ✨ Key Features
* **HTML Tree Construction**: Builds a full DOM tree using a custom `HtmlElement` structure.
* **CSS Selector Engine**: Supports complex queries like `div#main.content a` using a custom `Selector` parser.
* **Memory Efficient**: Uses `Queue`-based BFS for traversing descendants to prevent Stack Overflow.
* **Singleton Pattern**: Efficient management of HTML tag metadata.

## 🚀 Getting Started
1. Clone the repository.
2. Ensure `HtmlTags.json` and `HtmlVoidTags.json` are present in the project directory.
3. Open `Html_Serializer.sln` in Visual Studio.
4. Press `F5` to run the demo.

## 🛠️ Built With
* C# / .NET 8
* System.Text.Json
* Regex
