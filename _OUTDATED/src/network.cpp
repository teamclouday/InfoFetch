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
    this->html.clear();
    this->html.resize(0);
    this->htmlBuffer.clear();
    this->htmlBuffer.resize(0);
    CURLcode res;
    struct curl_slist *header = NULL;
    curl_slist_append(header, "Content-Type: text/html; charset=UTF-8");
    curl_easy_setopt(this->curl, CURLOPT_WRITEFUNCTION, Website::write_callback);
    curl_easy_setopt(this->curl, CURLOPT_WRITEDATA, &this->htmlBuffer);
    curl_easy_setopt(this->curl, CURLOPT_HTTPHEADER, header);
    curl_easy_setopt(this->curl, CURLOPT_FOLLOWLOCATION, 1L);
    curl_easy_setopt(this->curl, CURLOPT_MAXREDIRS, 3L);
    curl_easy_setopt(this->curl, CURLOPT_URL, url.c_str());
    res = curl_easy_perform(this->curl);
    curl_slist_free_all(header);

    // TODO: change this message box to a notification in the future
    if(res != CURLE_OK)
    {
        std::wstring message = L"网络连接错误: ";
        message += convert2wstring(std::string(curl_easy_strerror(res)));
        MessageBox(NULL, message.c_str(), L"InfoFetch Error", MB_OK | MB_ICONWARNING);
        return false;
    }

    this->html = this->decodeWebPage(this->htmlBuffer);
    if(!this->html.length())
        return false;

    return true;
}

std::string Website::decodeWebPage(const std::string& byteStream)
{
    // First try to convert to UTF-8
    std::wstring wtemp;
    std::string temp;
    int size = MultiByteToWideChar(CP_UTF8, 0, byteStream.c_str(), -1, NULL, 0);
    if(!size)
    {
        std::wstring message = L"编码转换错误(MultiByteToWideChar), 错误码: ";
        std::stringstream sstr;
        sstr << GetLastError();
        message += convert2wstring(sstr.str());
        MessageBox(NULL, message.c_str(), L"InfoFetch Error", MB_OK | MB_ICONWARNING);
        return "";
    }
    else
    {
        wtemp.resize(size*sizeof(wchar_t));
        MultiByteToWideChar(CP_UTF8, 0, byteStream.c_str(), -1, (LPWSTR)wtemp.c_str(), size);
        size = WideCharToMultiByte(CP_UTF8, 0, wtemp.c_str(), -1, NULL, 0, NULL, NULL);
        if(!size)
        {
            std::wstring message = L"编码转换错误(WideCharToMultiByte), 错误码: ";
            std::stringstream sstr;
            sstr << GetLastError();
            message += convert2wstring(sstr.str());
            MessageBox(NULL, message.c_str(), L"InfoFetch Error", MB_OK | MB_ICONWARNING);
            return "";
        }
        else
        {
            temp.resize(size);
            WideCharToMultiByte(CP_UTF8, 0, wtemp.c_str(), -1, (LPSTR)temp.c_str(), size, NULL, NULL);
        }
    }
    // Read charset of the webpage
    size_t pos = temp.find(std::string("charset"));
    std::string encoding;
    if(pos == std::string::npos)
    {
        std::wstring message = L"网页html中未定义charset, 将使用UTF-8";
        MessageBox(NULL, message.c_str(), L"InfoFetch Error", MB_OK | MB_ICONWARNING);
    }
    else
    {
        for(size_t i = pos + 7; i < temp.length(); i++)
        {
            if(temp[i] == ' ')
                continue;
            if(temp[i] == '\"')
            {
                if(encoding.length())
                    break;
                continue;
            }
            if(temp[i] == ';')
                break;
            if(temp[i] == '=')
                continue;
            encoding += temp[i];
        }
    }
    UINT CodePage = CP_UTF8;
    if(encoding == "utf-8" || encoding == "UTF-8")
    {
        return temp;
    }
    else if(encoding == "ASCII" || encoding == "ascii")
    {
        CodePage = CP_ACP; // Ascii
    }
    else if(encoding == "iso-8859-1" || encoding == "ISO-8859-1")
    {
        CodePage = 1252; // Latin
    }
    else if(encoding.substr(0, 2) == "GB" || encoding.substr(0, 2) == "gb")
    {
        CodePage = 936; // GBK
    }
    // If nothing matches, use UTF-8 as code page
    wtemp.clear();
    wtemp.resize(0);
    temp.clear();
    temp.resize(0);
    // now get the decoded web page
    size = MultiByteToWideChar(CodePage, 0, byteStream.c_str(), -1, NULL, 0);
    if(!size)
    {
        std::wstring message = L"编码转换错误(MultiByteToWideChar), 错误码: ";
        std::stringstream sstr;
        sstr << GetLastError();
        message += convert2wstring(sstr.str());
        MessageBox(NULL, message.c_str(), L"InfoFetch Error", MB_OK | MB_ICONWARNING);
        return "";
    }
    else
    {
        wtemp.resize(size*sizeof(wchar_t));
        MultiByteToWideChar(CodePage, 0, byteStream.c_str(), -1, (LPWSTR)wtemp.c_str(), size);
        size = WideCharToMultiByte(CP_UTF8, 0, wtemp.c_str(), -1, NULL, 0, NULL, NULL);
        if(!size)
        {
            std::wstring message = L"编码转换错误(WideCharToMultiByte), 错误码: ";
            std::stringstream sstr;
            sstr << GetLastError();
            message += convert2wstring(sstr.str());
            MessageBox(NULL, message.c_str(), L"InfoFetch Error", MB_OK | MB_ICONWARNING);
            return "";
        }
        else
        {
            temp.resize(size);
            WideCharToMultiByte(CP_UTF8, 0, wtemp.c_str(), -1, (LPSTR)temp.c_str(), size, NULL, NULL);
        }
    }

    return temp;
}