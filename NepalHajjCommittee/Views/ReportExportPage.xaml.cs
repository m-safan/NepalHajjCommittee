using System;
using System.Windows.Controls;

namespace NepalHajjCommittee.Views
{
    /// <summary>
    /// Interaction logic for ReportExportPage
    /// </summary>
    public partial class ReportExportPage : UserControl
    {
        public ReportExportPage()
        {
            InitializeComponent();

            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\" + Constants.MainFolder + @"\printing.html";
            webBrowser.Source = new Uri("file:///" + filePath);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            webBrowser.Refresh();
        }
    }
}
