using System.Windows;
using System.Windows.Controls;

namespace NepalHajjCommittee.Controls
{
    public class TextColumn : DataGridTextColumn
    {
        public TextColumn()
        {
            FontSize = (double)Application.Current.Resources["RegularFontSize"];
            HeaderStyle = (Style) Application.Current.Resources["DataGridColumnHeader"];
        }

        protected override void RefreshCellContent(FrameworkElement element, string propertyName)
        {
            base.RefreshCellContent(element, propertyName);
        }
    }
}
