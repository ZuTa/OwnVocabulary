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
using System.Windows.Navigation;
using System.Windows.Shapes;
using OwnVocabulary.Database;
using System.Collections.ObjectModel;
using OwnVocabulary.Dialogs;
using System.Data;
using System.ComponentModel;

namespace OwnVocabulary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty DataListProperty = DependencyProperty.Register("Vocabularies", typeof(ObservableCollection<VocabularyItem>), typeof(MainWindow));
        
        private ObservableCollection<VocabularyItem> vocabularies;
        private DataTable phrases;
        private Translator translator;
        private Button btnAdd;
        private Button btnCancel;

        private VocabularyItem SelectedVocabulary
        {
            get { return (VocabularyItem)lvVocabularies.SelectedItem; }
        }

        public MainWindow()
        {
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, DeleteVocabulary, CanDelete));
        }

        private void DeleteVocabulary(object sender, ExecutedRoutedEventArgs e)
        {
            if (DataBase.IsConnected && SelectedVocabulary != null && MessageBox.Show(string.Format(Properties.Resources.AskRemoveVocabulary, SelectedVocabulary), Properties.Resources.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    DataBase.RemoveVocabulary(SelectedVocabulary.ID);
                    vocabularies.Remove(SelectedVocabulary);
                    ClearVocabularyData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Properties.Resources.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
        }

        private void ClearVocabularyData()
        {
            phrases.Clear();
            tbWord.Text = "";
            tbSearchPattern.Text = "";
            spTranslated.Children.Clear();
        }

        private void CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void miClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenConnectionToDatabase();

            if (DataBase.IsConnected)
            {
                vocabularies = DataBase.GetVocabularies();
                this.SetValue(MainWindow.DataListProperty, vocabularies);
            }
            this.DataContext = this;

            translator = new Translator();
        }

        private void OpenConnectionToDatabase()
        {
            try
            {
                DataBase.OpenConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void miNew_Click(object sender, RoutedEventArgs e)
        {
            DlgNewVocabularyWindow dlg = new DlgNewVocabularyWindow(this);
            dlg.ShowDialog();
            if (dlg.IsSuccessful)
                vocabularies.Add(DataBase.CreateVocabulary(dlg.Name, dlg.LanguageID));
        }

        private void icVocabularies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateVocabulary();
            tbSearchPattern.IsEnabled = lvVocabulary.IsEnabled = SelectedVocabulary != null;
            spTranslated.Children.Clear();
            tbWord.Text = "";
            tbWord.IsEnabled = true;
        }

        private void btnTranslate_Click(object sender, RoutedEventArgs e)
        {
            spTranslated.Children.Clear();

            bool visibleAddButton = false;
            try
            {
                translator.Translate(tbWord.Text, (Languages)SelectedVocabulary.LanguageID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Properties.Resources.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (translator.Translated.Count == 0)
            {
                if (translator.TranslatedPhrase != string.Empty)
                {
                    spTranslated.Children.Add(GetCheckBox(translator.TranslatedPhrase));
                    visibleAddButton = true;
                }
                else
                {
                    TextBlock tb = new TextBlock();
                    tb.Text = Properties.Resources.CantTranslate;
                    tb.FontSize = 12;
                    tb.Foreground = Brushes.Red;
                    tb.Margin = new Thickness(10, 10, 0, 0);
                    spTranslated.Children.Add(tb);
                }
            }
            else
            {
                visibleAddButton = true;
                foreach (TranslatedItem item in translator.Translated)
                {
                    StackPanel sp = new StackPanel();
                    foreach (string s in item.Phrases)
                    {
                        sp.Children.Add(GetCheckBox(s));
                    }

                    spTranslated.Children.Add(GetExpander(item.Type, sp));
                }
            }

            if (visibleAddButton && SelectedVocabulary != null)
            {
                tbWord.IsEnabled = false;

                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                
                btnAdd = new Button();
                btnAdd.Content = "Add";
                btnAdd.Margin = new Thickness(5, 10, 0, 0);
                btnAdd.Width = 60;
                btnAdd.Click += new RoutedEventHandler(btnAdd_Click);

                btnCancel = new Button();
                btnCancel.Content = "Cancel";
                btnCancel.Margin = new Thickness(5, 10, 0, 0);
                btnCancel.Width = 60;
                btnCancel.Click += new RoutedEventHandler(btnCancel_Click);

                sp.Children.Add(btnAdd);
                sp.Children.Add(btnCancel);

                spTranslated.Children.Add(sp);
            }
        }

        private void UpdateVocabulary()
        {
            if (DataBase.IsConnected && SelectedVocabulary != null)
            {
                phrases = DataBase.GetPhrases(SelectedVocabulary.ID, tbSearchPattern.Text);
                lvVocabulary.ItemsSource = phrases.DefaultView;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            spTranslated.Children.Clear();
            tbWord.IsEnabled = true;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            StringBuilder result = new StringBuilder();

            if (translator.Translated.Count > 0)
            {
                foreach (UIElement uie in spTranslated.Children)
                {
                    if (uie is Expander && ((Expander)uie).Content is StackPanel)
                    {
                        StringBuilder tmp = new StringBuilder();
                        bool flag = false;
                        tmp.Append("[").Append(((Expander)uie).Header).Append(",");
                        StackPanel sp = (StackPanel)((Expander)uie).Content;
                        foreach (UIElement element in sp.Children)
                        {
                            if (element is CheckBox && ((CheckBox)element).IsChecked == true)
                            {
                                tmp.Append(((CheckBox)element).Content).Append(",");
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            tmp.Remove(tmp.Length - 1, 1).Append("]");
                            result.Append(tmp);
                        }
                    }
                }
            }
            else
            {
                // type=""
                StringBuilder tmp = new StringBuilder();
                bool flag = false;
                tmp.Append("[,");

                foreach (UIElement uie in spTranslated.Children)
                {
                    if (uie is CheckBox && ((CheckBox)uie).IsChecked == true)
                    {
                        tmp.Append(((CheckBox)uie).Content).Append(",");
                        flag = true;
                    }
                }
                if (flag)
                {
                    tmp.Remove(tmp.Length - 1, 1).Append("]");
                    result.Append(tmp);
                }
            }
            if (DataBase.IsConnected && result.Length > 0)
            {
                DataBase.AddPhrase(tbWord.Text, result.ToString(), SelectedVocabulary.ID);
                btn.IsEnabled = false;
                spTranslated.Children.Clear();
                UpdateVocabulary();
                tbWord.Text = "";
            }
            else
                MessageBox.Show("Cant add phrase to vocabulary!", Properties.Resources.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            tbWord.IsEnabled = true;
        }

        private void tbWord_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnTranslate.IsEnabled = tbWord.Text.ToString().Trim().Length > 0;
        }

        private CheckBox GetCheckBox(string content)
        {
            CheckBox cb = new CheckBox();
            cb.Content = content;
            cb.Margin = new Thickness(20, 5, 0, 3);

            return cb;
        }

        private Expander GetExpander(string header, object content)
        {
            Expander exp = new Expander();
            exp.Header = header;
            exp.Content = content;
            exp.Margin = new Thickness(5, 5, 0, 5);

            return exp;
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateVocabulary();
        }

        private void btnRemovePhrase_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedPhrases();
        }

        private void DeleteSelectedPhrases()
        {
            if (MessageBox.Show(Properties.Resources.AskRemoveVocabularyItem, Properties.Resources.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    int[] ids = new int[lvVocabulary.SelectedItems.Count];
                    List<DataRow> removed = new List<DataRow>();
                    for (int i = 0; i < lvVocabulary.SelectedItems.Count; i++)
                    {
                        DataRow r = (DataRow)((DataRowView)lvVocabulary.SelectedItems[i]).Row;
                        ids[i] = int.Parse(r["ID"].ToString());
                        removed.Add(r);
                    }
                    DataBase.RemovePhrases(ids);

                    foreach (DataRow r in removed)
                    {
                        phrases.Rows.Remove(r);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Properties.Resources.AppName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }
        }

        private void lvVocabulary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnRemovePhrase.IsEnabled = lvVocabulary.SelectedItems.Count > 0;
        }

        private void miStartTest_Click(object sender, RoutedEventArgs e)
        {
            TestingWindow.CreateTest(GetPhraseOfCurrentVocabulary(), 20); // TODO: made custom count of phrases in test
        }

        private List<Testing.TestItem> GetPhraseOfCurrentVocabulary()
        {
            //TODO: choose by statistics
            List<Testing.TestItem> list = new List<Testing.TestItem>();

            foreach (DataRow r in phrases.Rows)
            {
                list.Add(new Testing.TestItem(r["Phrase"].ToString(), ParsingHelper.ParseToList(r["TranslatedPhrase"].ToString())));
            }

            return list;
        }

    }
}
