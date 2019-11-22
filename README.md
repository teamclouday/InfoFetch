# InfoFetch 

**About this project**:  
This program runs in the background, and fetches desired content from urls (written in websites.txt), according to the XPaths given. If there are any updates, it will push notification to the desktop. It will also load a backup database to check for updates  

**Current Drawbacks**:  
1. Cannot correctly read dynamically loaded website content (by scripts)  
2. Cannot fully handle file and website encodings (currently UTF-8 is demanded)  
3. Single threaded, so cannot handle large databases with lots of query instructions  

**_OUTDATED**: contains old project code written in C++, not complete due to complex Windows API calls  
**_InfoFetchTest**: contains the source code written in C#, tested to be functional  
**_InfoFetchService**: contains the source code of the windows service version of this program (not functional)  

------

### The Final Program is in folder `InfoFetchConsole`  

The `InfoFetchConsole/icon.ico` is converted from an Icon made by Vectors Market at www.flaticon.com  