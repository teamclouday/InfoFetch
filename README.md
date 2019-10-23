# InfoFetch  

Dependency:  
1. Windows SDK  
2. HtmlAgilityPack  
3. SQLite  

Data Flow:  
1. `FileManager` loads url and xpath from websites.txt  
2. `Database` removes redundant urls data that does not exist in the loaded urls  
4. For each url, `Website` downloads html from url  
5. For each html, `Parser` finds the required content  
6. `Database` compare the existing database with the one extracted by `Parser`  
7. `Database` updates content for new info, and reports to `InfoManager`  
8. `Database` adds new urls and new contents into the database, and reports to `InfoManager`


Because WIN API is not very easy using C++, and project does not require speed and memory efficiency  
Now using C#, building on Visual Studio 2019 Community  