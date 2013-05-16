using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OwnVocabulary.Dialogs
{
    /// <summary>
    /// Interaction logic for DlgNewVocabularyWindow.xaml
    /// </summary>
    public partial class DlgNewVocabularyWindow : Window
    {
        private bool isSuccessful = false;
        public bool IsSuccessful
        {
            get { return isSuccessful; }
        }

        public string Name
        {
            get
            {
                return tbName.Text.ToString().Trim();
            }
        }

        public int LanguageID
        {
            get
            {
                return cbLanguages.SelectedIndex;
            }
        }

        public DlgNewVocabularyWindow(Window owner)
        {
            InitializeComponent();
            this.Owner = owner;
        }


        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (tbName.Text.ToString().Trim().Length > 0)
            {
                isSuccessful = true;
                this.Close();
            }
            else
                MessageBox.Show(Properties.Resources.ErrorCreateNewVocabulary, Properties.Resources.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            isSuccessful = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbLanguages.Items.Clear();
            foreach (Languages o in Enum.GetValues(typeof(Languages)))
            {
                cbLanguages.Items.Add(o.GetStringValue());
            }
        }

    }
}
