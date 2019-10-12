#pragma once
#include <windows.h>
#include <curl/curl.h>
#include <string>
#include <sstream>
#include "tools.hpp"


// Get website content from URL
// Possibly call the Parser class to get selected info
class Website
{
private:
    CURL *curl;
    std::string htmlBuffer;

    std::string decodeWebPage(const std::string& byteStream);

public:
    Website();
    ~Website();

    bool open(const std::string& url);
    
    std::string html;

    static size_t write_callback(char *content, size_t size, size_t nmemb, void* user)
    {
        size_t realsize = size * nmemb;
        ((std::string*)user)->append(content, realsize);
        return realsize;
    }
};