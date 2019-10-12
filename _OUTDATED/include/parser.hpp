#pragma once
#include <string>
#include <deque>
#include <regex>

// Parser designed for website html data
class Parser
{
private:
    std::string location;
    std::string content;
    bool findTag(const std::string& tag);

public:
    Parser(const std::string& content);
    ~Parser();
};