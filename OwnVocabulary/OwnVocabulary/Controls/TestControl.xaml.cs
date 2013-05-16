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
using OwnVocabulary.Testing;
using System.ComponentModel;

namespace OwnVocabulary.Controls
{
    /// <summary>
    /// Interaction logic for TestControl.xaml
    /// </summary>
    public partial class TestControl : UserControl
    {
        #region Delegates

        public delegate void OnTestFinished(object sender, TestFinishedEventArgs e);

        #endregion

        #region Events

        public event OnTestFinished TestFinished;

        #endregion

        #region Fields & Properties

        private List<TestItem> items;
        public List<TestItem> Items
        {
            get { return items; }
            set { items = value; }
        }

        private int countOfPhrases;
        public int CountOfPhrases
        {
            get { return countOfPhrases; }
            set { countOfPhrases = value; }
        }

        #endregion

        public TestControl()
        {
            InitializeComponent();
        }

        public DataHelper data;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CountOfPhrases = Math.Min(items.Count, CountOfPhrases);
            data = new DataHelper(Items, CountOfPhrases);
            this.DataContext = data;

            DoNextStep();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            DoNextStep();             
        }

        private void DoNextStep()
        {
            bool result = data.DoNextStep();

            if (!result)
            {
                if (TestFinished != null)
                    TestFinished(this, new TestFinishedEventArgs());
                btnNext.IsEnabled = false;
            }
        }
    }

    public class DataHelper : INotifyPropertyChanged
    {
        private int countOfPhrases;
        public int CountOfPhrases
        {
            get { return countOfPhrases; }
            set { countOfPhrases = value; }
        }

        private List<TestItem> items;
        public List<TestItem> Items
        {
            get { return items; }
            set { items = value; }
        }

        private string phrase;
        public string Phrase
        {
            get { return items[currentIndex].Phrase; }
        }   

        private int currentIndex;
        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                currentIndex = value;
                NotifyPropertyChanged("Phrase");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public DataHelper(List<TestItem> list, int count)
        {
            this.Items = list;
            this.CountOfPhrases = count;
            currentIndex = -1;
        }

        public bool DoNextStep()
        {
            bool result = true;

            CurrentIndex++;
            if (CurrentIndex == this.CountOfPhrases)
                result = false;

            return result;
        }
    }
}
