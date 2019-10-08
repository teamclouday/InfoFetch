#pragma once
#include <windows.h>
#include <curl/curl.h>
#include <vector>
#include <string>
#include <iostream>
#include <codecvt>

class Website
{
private:
    CURL *curl;
    std::string htmlBuffer;

public:
    Website();
    ~Website();

    bool open(const std::string& url);

    static size_t write_callback(void *content, size_t size, size_t nmemb, void* user)
    {
        size_t realsize = size * nmemb;
        ((std::string*)user)->append((char *)content, realsize);
        return realsize;
    }
};