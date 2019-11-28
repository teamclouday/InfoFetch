# InfoFetch 

**About this project**:  
This program runs in the background, and fetches desired content from urls (written in websites.txt), according to the XPaths given. If there are any updates, it will push notification to the desktop. It will also load a backup database to check for updates  


**New Version Updates:**  
The new version now uses ChromeDriver powered by Selenium. This update meets the requirements to fetch dynamically loaded webpage content.  
When it runs, it actually powers up a chrome process at background, so it might be memory consuming. The good news is that now the fetching process becomes more accurate thanks to the chromedriver.  


**Current Drawbacks**:  
1. Cannot fully handle file and website encodings (currently UTF-8 is demanded)  
2. Single threaded, so cannot handle large databases with lots of query instructions  


------

**_OUTDATED**: contains old project code written in C++, not complete due to complex Windows API calls  
**_InfoFetchTest**: contains the source code written in C#, tested to be functional  
**_InfoFetchService**: contains the source code of the windows service version of this program (not functional)  

------

### The Final Program is in folder `InfoFetchConsole`  

The `InfoFetchConsole/icon.ico` is converted from an Icon made by Vectors Market at www.flaticon.com  

### Additional Requirements: a `Chrome` is installed properly on Windows  

Specifically, as required by Selenium, `Chrome` is expected to have been installed at:  
`C:\Program Files (x86)\Google\Chrome\Application\chrome.exe`