using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Localizer_WPF
{
    /// <summary>
    /// Logique d'interaction pour AddLangagePopup.xaml
    /// </summary>
    public partial class AddLangagePopup : Window
    {
        public delegate void AddLangageDelegate(string langageName);
        public event AddLangageDelegate AddLangageEvent;

        public AddLangagePopup()
        {
            InitializeComponent();
        }

        private void AddLangageButton_Click(object sender, RoutedEventArgs e)
        {
            AddLangageEvent.Invoke(LangageText.Text);

            Close();
        }
    }
}
