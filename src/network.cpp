#include "network.hpp"

Website::Website()
{
    this->curl = curl_easy_init();
}

Website::~Website()
{
    curl_easy_cleanup(curl);
}

bool Website::open(const std::string& url)
{
    this->htmlBuffer.clear();
    this->htmlBuffer.resize(0);
    CURLcode res;
    struct curl_slist *header = NULL;
    curl_slist_append(header, "Content-Type: text/html; charset=UTF-8");
    curl_easy_setopt(this->curl, CURLOPT_WRITEFUNCTION, Website::write_callback);
    curl_easy_setopt(this->curl, CURLOPT_WRITEDATA, &this->htmlBuffer);
    curl_easy_setopt(this->curl, CURLOPT_HTTPHEADER, header);
    curl_easy_setopt(this->curl, CURLOPT_FOLLOWLOCATION, 1L);
    curl_easy_setopt(this->curl, CURLOPT_REDIR_PROTOCOLS, "http");
    curl_easy_setopt(this->curl, CURLOPT_URL, url.c_str());
    res = curl_easy_perform(this->curl);
    curl_slist_free_all(header);

    if(res != CURLE_OK)
        std::cerr << "Error: " << curl_easy_strerror(res) << std::endl;

    std::cout << this->htmlBuffer << std::endl;
    try{
        // try to convert from GBK(936 is the code page for simplified Chinese) to UTF-8
        // mainly for chinese websites
        std::wstring temp;
        std::wstring_convert<std::codecvt_byname<wchar_t, char, std::mbstate_t>> cvt1(new std::codecvt_byname<wchar_t, char, std::mbstate_t>(".936"));
        temp = cvt1.from_bytes(this->htmlBuffer);
        std::wstring_convert<std::codecvt_utf8<wchar_t>> cvt2;
        std::string decoded;
        decoded = cvt2.to_bytes(temp);
        if(!decoded.length())
        {
            throw 1;
        }
        this->htmlBuffer = decoded;
    }catch(...){}

    return true;
}