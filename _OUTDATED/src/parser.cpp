#include "parser.hpp"

Parser::Parser(const std::string& content)
{
    this->content = content;
}

Parser::~Parser()
{

}

bool Parser::findTag(const std::string& tag)
{
    std::regex rehead("<"+tag);
    std::regex retail("</"+tag+">");
    std::deque<size_t> lshead;
    std::deque<size_t> lstail;
    for(std::sregex_iterator iter = std::sregex_iterator(this->content.begin(), this->content.end(), rehead); iter != std::sregex_iterator(); iter++)
        lshead.push_back(iter->position);
    for(std::sregex_iterator iter = std::sregex_iterator(this->content.begin(), this->content.end(), retail); iter != std::sregex_iterator(); iter++)
        lstail.push_front(iter->position);
    while(lshead.size() > lstail.size())
    {
        lshead.pop_front();
    }
    while(lshead.size() < lstail.size())
    {
        lstail.pop_front();
    }
}