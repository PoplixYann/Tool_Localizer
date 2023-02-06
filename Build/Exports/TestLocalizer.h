
#ifndef TestLocalizer__H
#define TestLocalizer__H

#include <unordered_map>
#include <string>

class TestLocalizer
{
public:
	static bool Contain(std::string id, std::string langage);
	static const std::string& Get(std::string id, std::string langage);

private:
	TestLocalizer() = default;

private:
	static const std::unordered_map<std::string, std::string> s_Values;
	static const std::string s_MissingTextString;
};

#endif
