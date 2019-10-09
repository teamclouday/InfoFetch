#pragma once
#include <string>
#include <codecvt>

inline std::string convert2string(const std::wstring& str)
{
    std::wstring_convert<std::codecvt_utf8<wchar_t>> converter;
    return converter.to_bytes(str);
}

inline std::wstring convert2wstring(const std::string& str)
{
    std::wstring_convert<std::codecvt_utf8<wchar_t>> converter;
    return converter.from_bytes(str);
}