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
using OwnVocabulary.Testing;

namespace OwnVocabulary.Dialogs
{
    /// <summary>
    /// Interaction logic for TestingWindow.xaml
    /// </summary>
    public partial class TestingWindow : Window
    {
        public TestingWindow()
        {
            InitializeComponent();
        }

        public static void CreateTest(List<TestItem> items, int countOfPhrase)
        {
            TestingWindow window = new TestingWindow();
            window.testControl.Items = items;
            window.testControl.CountOfPhrases = countOfPhrase;

            window.ShowDialog();
        }

        private void testControl_TestFinished(object sender, TestFinishedEventArgs e)
        {
            //TODO: show statistics for testing
            this.Close();
        }
    }
}
