using NepalHajjCommittee.Models;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace NepalHajjCommittee.ViewModels
{
    public class ReportExportPageViewModel : ViewModelBase
    {
        private readonly string _filePath;
        private ColumnVisibility _columnVisibility;
        private DelegateCommand _previewCommand;
        private DelegateCommand _printCommand;
        private List<SearchedResults> _searchedResults;

        public ReportExportPageViewModel(IRegionManager regionManager) : base(regionManager)
        {
            _filePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\" + Constants.MainFolder + @"\printing.html";
            ColumnVisibility = new ColumnVisibility();
        }

        public ColumnVisibility ColumnVisibility { get => _columnVisibility; set => SetProperty(ref _columnVisibility, value); }
        public List<SearchedResults> SearchedResults { get => _searchedResults; set => SetProperty(ref _searchedResults, value); }

        public ICommand PreviewCommand => _previewCommand ?? (_previewCommand = new DelegateCommand(ExecutePreviewCommand));

        public ICommand PrintCommand => _printCommand ?? (_printCommand = new DelegateCommand(ExecutePrintCommand));

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (navigationContext.Parameters.TryGetValue("Model", out List<SearchedResults> searchedResults))
                SearchedResults = searchedResults;
        }

        private void ExecutePrintCommand()
        {
            ExecutePreviewCommand();
            System.Diagnostics.Process.Start("file:///" + _filePath);
        }

        private void ExecutePreviewCommand()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<html><body><table><thead><tr><th><table style=\"width:100%\"><tr><td width=\"33%\"><img align=right height=100 style=\"width:2in\" src=\"embasy.jpg\"/></td><td width=\"auto\"><h1 align=center>Nepal Hajj Committee</h4></td><td width=\"auto\"><img align=left height=90 width=90 src=\"flag.png\"/></td></tr></table><hr/></th></tr></thead><tbody><tr><td>");
            stringBuilder.Append("<table border><thead><tr>");

            if (ColumnVisibility.Name)
                AddHeader(stringBuilder, "Name");
            if (ColumnVisibility.Gender)
                AddHeader(stringBuilder, "Gender");
            if (ColumnVisibility.PassportNo)
                AddHeader(stringBuilder, "Passport No");
            if (ColumnVisibility.ContactNo)
                AddHeader(stringBuilder, "Contact No");
            if (ColumnVisibility.State)
                AddHeader(stringBuilder, "State");
            if (ColumnVisibility.GroupName)
                AddHeader(stringBuilder, "Group Name");
            if (ColumnVisibility.BatchName)
                AddHeader(stringBuilder, "Batch Name");
            if (ColumnVisibility.ArrivalDateMakkah)
                AddHeader(stringBuilder, "Arrival Makkah");
            if (ColumnVisibility.DepartureDateMakkah)
                AddHeader(stringBuilder, "Departure Makkah");
            if (ColumnVisibility.ArrivalDateMadinah)
                AddHeader(stringBuilder, "Arrival Madinah");
            if (ColumnVisibility.DepartureDateMadinah)
                AddHeader(stringBuilder, "Departure Madinah");
            if (ColumnVisibility.MakkahRoomNo)
                AddHeader(stringBuilder, "Makkah Room No");
            if (ColumnVisibility.MadinahRoomNo)
                AddHeader(stringBuilder, "Madinah Room No");
            if (ColumnVisibility.MadinahToAirportBusNumber)
                AddHeader(stringBuilder, "Madinah To Airport Bus Number");
            if (ColumnVisibility.MakkahToAirportBusNumber)
                AddHeader(stringBuilder, "Makkah To Airport Bus Number");
            if (ColumnVisibility.MadinahToMakkahBusNumber)
                AddHeader(stringBuilder, "Madinah To Makkah Bus Number");
            if (ColumnVisibility.MakkahToMadinahBusNumber)
                AddHeader(stringBuilder, "Makkah To Madinah Bus Number");

            stringBuilder.Append("</tr></thead><tbody>");

            SearchedResults.ForEach(x =>
            {
                stringBuilder.Append("<tr>");
                if (ColumnVisibility.Name)
                    AddData(stringBuilder, x.Name);
                if (ColumnVisibility.Gender)
                    AddData(stringBuilder, x.Gender);
                if (ColumnVisibility.PassportNo)
                    AddData(stringBuilder, x.PassportNo);
                if (ColumnVisibility.ContactNo)
                    AddData(stringBuilder, x.ContactNo);
                if (ColumnVisibility.State)
                    AddData(stringBuilder, x.StateName);
                if (ColumnVisibility.GroupName)
                    AddData(stringBuilder, x.GroupName);
                if (ColumnVisibility.BatchName)
                    AddData(stringBuilder, x.BatchName);
                if (ColumnVisibility.ArrivalDateMakkah)
                    AddData(stringBuilder, x.ArrivalDateMakkah);
                if (ColumnVisibility.DepartureDateMakkah)
                    AddData(stringBuilder, x.DepartureDateMakkah);
                if (ColumnVisibility.ArrivalDateMadinah)
                    AddData(stringBuilder, x.ArrivalDateMadinah);
                if (ColumnVisibility.DepartureDateMadinah)
                    AddData(stringBuilder, x.DepartureDateMadinah);
                if (ColumnVisibility.MakkahRoomNo)
                    AddData(stringBuilder, x.MakkahRoomNo);
                if (ColumnVisibility.MadinahRoomNo)
                    AddData(stringBuilder, x.MadinahRoomNo);
                if (ColumnVisibility.MadinahToAirportBusNumber)
                    AddData(stringBuilder, x.MadinahToAirportBusNumber);
                if (ColumnVisibility.MakkahToAirportBusNumber)
                    AddData(stringBuilder, x.MakkahToAirportBusNumber);
                if (ColumnVisibility.MadinahToMakkahBusNumber)
                    AddData(stringBuilder, x.MadinahToMakkahBusNumber);
                if (ColumnVisibility.MakkahToMadinahBusNumber)
                    AddData(stringBuilder, x.MakkahToMadinahBusNumber);
                stringBuilder.Append("</tr>");
            });

            stringBuilder.Append("</tbody></table>");
            stringBuilder.Append("</td></tr></tbody></table></body></html>");

            if (File.Exists(_filePath))
                File.Delete(_filePath);

            var writer = File.AppendText(_filePath);
            writer.Write(stringBuilder.ToString());
            writer.Flush();
            writer.Close();
        }

        private void AddData(StringBuilder stringBuilder, object data)
        {
            stringBuilder.Append($"<td>{(data ?? string.Empty).ToString()}</td>");
        }

        private void AddHeader(StringBuilder stringBuilder, object data)
        {
            stringBuilder.Append($"<th>{(data ?? string.Empty).ToString()}</th>");
        }
    }
}
