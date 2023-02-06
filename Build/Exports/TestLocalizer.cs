
using System.Collections.Generic;

            static class TestLocalizer
            {
                static readonly Dictionary<string, string> values
                    = new Dictionary<string, string>
                    {{"START_en-EN", "Start"},
{"START_fr-FR", "Commencer"},
{"START_ja-JP", "始"},
{"QUIT_en-EN", "Quit"},
{"QUIT_fr-FR", "Quitter"},
{"QUIT_ja-JP", "出発"},
};

                public static bool Contain(string id, string langage)
                {
                    string key = string.Concat(id, "_", langage);
                    return values.ContainsKey(key);
                }

                public static string Get(string id, string langage)
                {
                    string key = string.Concat(id, "_", langage);
                    if (values.ContainsKey(key))
                        return values[key];

                    return "Missing Text !";
                }
            }
            