
#include "TestLocalizer.h"

bool TestLocalizer::Contain(std::string id, std::string langage)
{
    std::string key = id + "_" + langage;
    return s_Values.find(key) != s_Values.end();
}

const std::string& TestLocalizer::Get(std::string id, std::string langage)
{
    std::string key = id + "_" + langage;
    std::unordered_map < std::string, std::string>::const_iterator value = s_Values.find(key);
    
    if (value != s_Values.end())
        return value->second;
    else
        return s_MissingTextString;
}

const std::unordered_map<std::string, std::string> TestLocalizer::s_Values = {
{"START_en-EN", "Start"},
{"START_fr-FR", "Commencer"},
{"START_ja-JP", "始"},
{"QUIT_en-EN", "Quit"},
{"QUIT_fr-FR", "Quitter"},
{"QUIT_ja-JP", "出発"},
};

const std::string TestLocalizer::s_MissingTextString = "Missing Text !"; 
