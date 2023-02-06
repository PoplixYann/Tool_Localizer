using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Linq;
using System.Xml.Serialization;
using System.Runtime.ConstrainedExecution;
using System.Net.Security;
using Microsoft.VisualBasic;

namespace Localizer_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog OpenFileDialog1;
        SaveFileDialog SaveFileDialog1;
        DataTable Data;

        string Filename;

        public MainWindow()
        {
            InitializeComponent();

            // Set default grid (like do a new)
            NewItem_Click(null, null);
        }

        private void NewItem_Click(object sender, RoutedEventArgs e)
        {
            Data = new DataTable();

            LocalizationData.Columns.Clear();

            var columnID = new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("ID"),
                Width = new DataGridLength(100, DataGridLengthUnitType.Star)
            };

            var columnEN = new DataGridTextColumn
            {
                Header = "en-EN",
                Binding = new Binding("en-EN"),
                Width = new DataGridLength(100, DataGridLengthUnitType.Star)
            };

            LocalizationData.Columns.Add(columnID);
            LocalizationData.Columns.Add(columnEN);
            Data.Columns.Add("ID");
            Data.Columns.Add("en-EN");

            LocalizationData.ItemsSource = Data.DefaultView;

            Filename = string.Empty;
        }

        private void OpenItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog1 = new OpenFileDialog();
            OpenFileDialog1.Filter = "All Files|*.csv;*.json;*.xml|CSV|*.csv|JSON|*.json|XML|*.xml";
            if (OpenFileDialog1.ShowDialog() == true)
            {
                Filename = OpenFileDialog1.FileName;
                string ext = System.IO.Path.GetExtension(OpenFileDialog1.FileName);
                if (ext == ".csv")
                {
                    ReadCSV(OpenFileDialog1.FileName);
                }
                else if (ext == ".json")
                {
                    ReadJSON(OpenFileDialog1.FileName);
                }
                else if (ext == ".xml")
                {
                    ReadXML(OpenFileDialog1.FileName);
                }
            }
        }

        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            if (Filename == string.Empty) return;

            string ext = System.IO.Path.GetExtension(Filename);
            if (ext == ".csv")
            {
                WriteCSV(Filename);
            }
            else if (ext == ".json")
            {
                WriteJSON(Filename);
            }
            else if (ext == ".xml")
            {
                WriteXML(Filename);
            }
        }

        private void SaveAsItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.Filter = "All Files|*.csv;*.json;*.xml|CSV|*.csv|JSON|*.json|XML|*.xml";
            if (SaveFileDialog1.ShowDialog() == true)
            {
                string ext = System.IO.Path.GetExtension(SaveFileDialog1.FileName);
                if (ext == ".csv")
                {
                    WriteCSV(SaveFileDialog1.FileName);
                }
                else if (ext == ".json")
                {
                    WriteJSON(SaveFileDialog1.FileName);
                }
                else if (ext == ".xml")
                {
                    WriteXML(SaveFileDialog1.FileName);
                }
            }
        }

        private void ExportAsCsharpItem_Click(object sender, RoutedEventArgs e)
        {
            WriteCsharp();
        }

        private void ExportAsCPPItem_Click(object sender, RoutedEventArgs e)
        {
            WriteCPP();
        }

        private void AddLangageItem_Click(object sender, RoutedEventArgs e)
        {
            AddLangagePopup addLangagePopup = new AddLangagePopup();

            addLangagePopup.AddLangageEvent += OnAddLangage;

            addLangagePopup.Show();
        }

        void OnAddLangage(string langageName)
        {
            if (langageName == string.Empty) return;

            var column = new DataGridTextColumn
            {
                Header = langageName,
                Binding = new Binding(langageName),
                Width = new DataGridLength(100, DataGridLengthUnitType.Star)
            };

            LocalizationData.Columns.Add(column);
            Data.Columns.Add(langageName);
        }

        void ReadCSV(string filename)
        {
            Data = new DataTable();
            string[] lines = File.ReadAllLines(filename);
            string[] languages = lines[0].Split(';');

            LocalizationData.Columns.Clear();
            for (int i = 0; i < languages.Length; i++)
            {
                var column = new DataGridTextColumn
                {
                    Header = languages[i],
                    Binding = new Binding(languages[i])
                };

                if (i != 0)
                {
                    column.Width = new DataGridLength(100, DataGridLengthUnitType.Star);
                }

                LocalizationData.Columns.Add(column);
                Data.Columns.Add(languages[i]);
            }

            for (int i = 1; i < lines.Length; i++)
            {
                object[] element = new object[languages.Length];
                string[] lineElem = lines[i].Split(';');
                for (int j = 0; j < element.Length; j++)
                {
                    element[j] = lineElem[j];
                }
                Data.Rows.Add(element);
            }

            LocalizationData.ItemsSource = Data.DefaultView;
        }

        void WriteCSV(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                for (int j = 0; j < LocalizationData.Columns.Count; j++)
                {
                    sw.Write(LocalizationData.Columns[j].Header);
                    if (j < LocalizationData.Columns.Count - 1)
                        sw.Write(";");
                }

                sw.Write("\n");

                for (int i = 0; i < LocalizationData.Items.Count - 1; i++)
                {
                    if ((LocalizationData.Columns[0].GetCellContent(LocalizationData.Items[i]) as TextBlock)?.Text == string.Empty) continue;
                    for (int j = 0; j < LocalizationData.Columns.Count; j++)
                    {
                        TextBlock? text = LocalizationData.Columns[j].GetCellContent(LocalizationData.Items[i]) as TextBlock;
                        sw.Write(text?.Text);
                        if (j < LocalizationData.Columns.Count - 1)
                            sw.Write(";");
                    }
                    sw.Write("\n");
                }
            }
        }

        void ReadJSON(string filename)
        {
            Dictionary<string, List<string>>? values = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(File.ReadAllText(filename));
            if (values == null) return;

            Data = new DataTable();
            LocalizationData.Columns.Clear();
            for (int i = 0; i < values?.Count; i++)
            {
                var column = new DataGridTextColumn
                {
                    Header = values.ElementAt(i).Key,
                    Binding = new Binding(values.ElementAt(i).Key)
                };

                if (i != 0)
                {
                    column.Width = new DataGridLength(100, DataGridLengthUnitType.Star);
                }

                LocalizationData.Columns.Add(column);
                Data.Columns.Add(values.ElementAt(i).Key);
            }

            int nbLanguage = values.Count;
            int nbElement = values.ElementAt(0).Value.Count;

            for (int i = 0; i < nbElement; i++)
            {
                object[] element = new object[nbLanguage];
                for (int j = 0; j < nbLanguage; j++)
                {
                    element[j] = values.ElementAt(j).Value[i];
                }
                Data.Rows.Add(element);
            }

            LocalizationData.ItemsSource = Data.DefaultView;
        }

        void WriteJSON(string filename)
        {
            Dictionary<string, List<string>> values = new Dictionary<string, List<string>>();

            for (int j = 0; j < LocalizationData.Columns.Count; j++)
            {
                values.Add(LocalizationData.Columns[j].Header.ToString(), new List<string>());

                for (int i = 0; i < LocalizationData.Items.Count - 1; i++)
                {
                    TextBlock? text = LocalizationData.Columns[j].GetCellContent(LocalizationData.Items[i]) as TextBlock;

                    if (text != null)
                        values[LocalizationData.Columns[j].Header.ToString()].Add(text.Text);
                }
            }

            File.WriteAllText(filename, JsonSerializer.Serialize(values));
        }

        void ReadXML(string filename)
        {
            Data = new DataTable();
            Data.ReadXml(filename);

            LocalizationData.Columns.Clear();

            for (int i = 0; i < Data.Columns.Count; i++)
            {
                var column = new DataGridTextColumn
                {
                    Header = Data.Columns[i].ColumnName,
                    Binding = new Binding(Data.Columns[i].ColumnName)
                };

                if (i != 0)
                {
                    column.Width = new DataGridLength(100, DataGridLengthUnitType.Star);
                }

                LocalizationData.Columns.Add(column);
            }

            LocalizationData.ItemsSource = Data.DefaultView;
        }

        void WriteXML(string filename)
        {
            Data.TableName = "MyData";
            Data.WriteXml(filename, XmlWriteMode.WriteSchema);
        }

        void WriteCsharp()
        {
            if (Filename == string.Empty) return;

            string folderName = @"Exports";

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            string filename = System.IO.Path.GetFileNameWithoutExtension(Filename) + "Localizer";

            Dictionary<string, string> values = new Dictionary<string, string>();

            for (int i = 0; i < LocalizationData.Items.Count - 1; i++)
            {
                string? keyBase =
                    (LocalizationData.Columns[0].GetCellContent(LocalizationData.Items[i]) as TextBlock)?.Text;

                if (keyBase == null || keyBase == string.Empty) continue;

                for (int j = 1; j < LocalizationData.Columns.Count; j++)
                {
                    string key = keyBase + "_" + LocalizationData.Columns[j].Header;
                    TextBlock? text = LocalizationData.Columns[j].GetCellContent(LocalizationData.Items[i]) as TextBlock;

                    if (text != null)
                        values.Add(key, text.Text);
                }
            }

            string result = @"
using System.Collections.Generic;

            static class " + filename +
        @"
            {
                static readonly Dictionary<string, string> values
                    = new Dictionary<string, string>
                    {";

            foreach (var value in values)
            {
                result += "{\"" + value.Key + "\", \"" + value.Value + "\"},\n";
            }

            result += @"};

                public static bool Contain(string id, string langage)
                {
                    string key = string.Concat(id, ""_"", langage);
                    return values.ContainsKey(key);
                }

                public static string Get(string id, string langage)
                {
                    string key = string.Concat(id, ""_"", langage);
                    if (values.ContainsKey(key))
                        return values[key];

                    return ""Missing Text !"";
                }
            }
            ";



            using (StreamWriter sw
                = new StreamWriter(System.IO.Path.Combine(folderName, string.Concat(filename, ".cs"))))
            {
                sw.Write(result);
            }
        }

        void WriteCPP()
        {
            if (Filename == string.Empty) return;

            string folderName = @"Exports";

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            string filename = System.IO.Path.GetFileNameWithoutExtension(Filename) + "Localizer";

            Dictionary<string, string> values = new Dictionary<string, string>();

            for (int i = 0; i < LocalizationData.Items.Count - 1; i++)
            {
                string? keyBase =
                    (LocalizationData.Columns[0].GetCellContent(LocalizationData.Items[i]) as TextBlock)?.Text;

                if (keyBase == null || keyBase == string.Empty) continue;

                for (int j = 1; j < LocalizationData.Columns.Count; j++)
                {
                    string key = keyBase + "_" + LocalizationData.Columns[j].Header;
                    TextBlock? text = LocalizationData.Columns[j].GetCellContent(LocalizationData.Items[i]) as TextBlock;

                    if (text != null)
                        values.Add(key, text.Text);
                }
            }

            string resultCPP = @"
#include """ + filename + @".h""

bool " + filename + @"::Contain(std::string id, std::string langage)
{
    std::string key = id + ""_"" + langage;
    return s_Values.find(key) != s_Values.end();
}

const std::string& " + filename + @"::Get(std::string id, std::string langage)
{
    std::string key = id + ""_"" + langage;
    std::unordered_map < std::string, std::string>::const_iterator value = s_Values.find(key);
    
    if (value != s_Values.end())
        return value->second;
    else
        return s_MissingTextString;
}

const std::unordered_map<std::string, std::string> " + filename + @"::s_Values = {
";

            foreach (var value in values)
            {
                resultCPP += "{\"" + value.Key + "\", \"" + value.Value + "\"},\n";
            }

            resultCPP += @"};

const std::string " + filename + @"::s_MissingTextString = ""Missing Text !""; 
";


            string resultH = @"
#ifndef " + filename + "__H" + @"
#define " + filename + "__H" + @"

#include <unordered_map>
#include <string>

class " + filename + @"
{
public:
	static bool Contain(std::string id, std::string langage);
	static const std::string& Get(std::string id, std::string langage);

private:
	" + filename + @"() = default;

private:
	static const std::unordered_map<std::string, std::string> s_Values;
	static const std::string s_MissingTextString;
};

#endif
";


            using (StreamWriter sw
                = new StreamWriter(System.IO.Path.Combine(folderName, string.Concat(filename, ".cpp"))))
            {
                sw.Write(resultCPP);
            }
            using (StreamWriter sw
                = new StreamWriter(System.IO.Path.Combine(folderName, string.Concat(filename, ".h"))))
            {
                sw.Write(resultH);
            }
        }
    }
}
