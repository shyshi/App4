using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Storage;
using Windows.Storage.Pickers;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SQLite;
using SQLitePCL;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace App4
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PlotPage : Page
    {
        public PlotPage()
        {
            this.InitializeComponent();
            setModel();
            this.DataContext = new MainViewModel();
            AddNewLine(0);
            buttonAddLine.Visibility = (Visibility)0;
            buttonChangeStyle.Visibility = (Visibility)0;
            buttonDecLine.Visibility = (Visibility)0;
            buttonExport.Visibility = (Visibility)0;            
        }


        MainViewModel doublelogModel = new MainViewModel();
        MainViewModel doublelineModel = new MainViewModel();
        MainViewModel logxModel = new MainViewModel();
        MainViewModel logyModel = new MainViewModel();
        MainViewModel currentModel = new MainViewModel();

        private int numberOfLines = 0;
        private int numberOfExistLines = 0;

        private void setModel()
        {
            LinearAxis lineAxis1 = new LinearAxis();
            LinearAxis lineAxis2 = new LinearAxis();
            LinearAxis lineAxis3 = new LinearAxis();
            LinearAxis lineAxis4 = new LinearAxis();
            lineAxis1.Position = AxisPosition.Bottom;
            lineAxis2.Position = AxisPosition.Left;
            lineAxis3.Position = AxisPosition.Bottom;
            lineAxis4.Position = AxisPosition.Left;
            LogarithmicAxis logAxis1 = new LogarithmicAxis();
            LogarithmicAxis logAxis2 = new LogarithmicAxis();
            LogarithmicAxis logAxis3 = new LogarithmicAxis();
            LogarithmicAxis logAxis4 = new LogarithmicAxis();
            logAxis1.Position = AxisPosition.Bottom;
            logAxis2.Position = AxisPosition.Left;
            logAxis3.Position = AxisPosition.Bottom;
            logAxis4.Position = AxisPosition.Left;
            this.doublelogModel.MyModel.Axes.Add(logAxis1);
            this.doublelogModel.MyModel.Axes.Add(logAxis2);
            this.doublelineModel.MyModel.Axes.Add(lineAxis1);
            this.doublelineModel.MyModel.Axes.Add(lineAxis2);
            this.logxModel.MyModel.Axes.Add(logAxis3);
            this.logxModel.MyModel.Axes.Add(lineAxis4);
            this.logyModel.MyModel.Axes.Add(lineAxis3);
            this.logyModel.MyModel.Axes.Add(logAxis4);
        }


        private List<LineSeries> doublelineLines = new List<LineSeries>();
        private List<LineSeries> doublelogLines = new List<LineSeries>();
        private List<LineSeries> logxLines = new List<LineSeries>();
        private List<LineSeries> logyLines = new List<LineSeries>();

        private async void AddNewLine(int indexOfLines)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".lis");
            StorageFile file = await openPicker.PickSingleFileAsync();
            IList<string> fileContents = await FileIO.ReadLinesAsync(file);
            LineSeries doublelineLine = new LineSeries();
            LineSeries doublelogLine = new LineSeries();
            LineSeries logxLine = new LineSeries();
            LineSeries logyLine = new LineSeries();
            doublelineLines.Add(doublelineLine);
            doublelogLines.Add(doublelogLine);
            logxLines.Add(logxLine);
            logyLines.Add(logyLine);
            doublelineLines[indexOfLines].Title = file.Name;
            doublelineLines[indexOfLines].MarkerType = MarkerType.Triangle;
            logxLines[indexOfLines].Title = file.Name;
            logxLines[indexOfLines].MarkerType = MarkerType.Triangle;
            logyLines[indexOfLines].Title = file.Name;
            logyLines[indexOfLines].MarkerType = MarkerType.Triangle;
            doublelogLines[indexOfLines].Title = file.Name;
            doublelogLines[indexOfLines].MarkerType = MarkerType.Triangle; 
            for (int i = 2; i < fileContents.Count; i++)
            {
                string currentLine = fileContents[i];
                string[] lineContents = currentLine.Split();
                string[] valuableLineContents = new string[4];
                int k = 0;
                for (int index = 0; index < lineContents.Length; index++)
                {
                    if (lineContents[index] != "")
                    {
                        valuableLineContents[k] = lineContents[index];
                        k++;
                    }
                }
                double x = Convert.ToDouble(valuableLineContents[1]) - Convert.ToDouble(valuableLineContents[0]);
                double y = Convert.ToDouble(valuableLineContents[2]) * x;
                doublelineLines[indexOfLines].Points.Add(new DataPoint(x, y));
                doublelogLines[indexOfLines].Points.Add(new DataPoint(x, y));
                logxLines[indexOfLines].Points.Add(new DataPoint(x, y));
                logyLines[indexOfLines].Points.Add(new DataPoint(x, y));
            }
            doublelogModel.MyModel.Series.Add(doublelogLines[indexOfLines]);
            doublelineModel.MyModel.Series.Add(doublelineLines[indexOfLines]);
            logxModel.MyModel.Series.Add(logxLines[indexOfLines]);
            logyModel.MyModel.Series.Add(logyLines[indexOfLines]);
            this.DataContext = new MainViewModel();
            currentModel = logyModel;
            this.DataContext = currentModel;
            MenuFlyoutItem newitemtoDelete = new MenuFlyoutItem();
            newitemtoDelete.Text = file.Name;
            newitemtoDelete.Click += DecExistedLines_Click;
            DeleteExistedLines.Items.Add(newitemtoDelete);
            MenuFlyoutItem newitemtoAdd = new MenuFlyoutItem();
            newitemtoAdd.Text = file.Name;
            newitemtoAdd.Click += AddDeletedLine_Click;
            newitemtoAdd.Visibility = (Visibility)1;
            AddLine.Items.Add(newitemtoAdd);
        }

        //private async void buttonAddLine_Click(object sender, RoutedEventArgs e)
        //{
        //    //MessageDialog addLine = new MessageDialog("请选择希望添加的曲线来源");
        //    //UICommand commandAddNewLine = new UICommand("从新文件添加", new UICommandInvokedHandler(OnUICommandAddNewLineClicked));
        //    //UICommand commandAddExistLine = new UICommand("从已删除文件添加", new UICommandInvokedHandler(OnUICommandAddExistLineClicked));
        //    //addLine.Commands.Add(commandAddNewLine);
        //    //if (numberofNonExistLines != 0)
        //    //{
        //    //    addLine.Commands.Add(commandAddExistLine);
        //    //}
        //    //await addLine.ShowAsync();
        //}


        private async void buttonExport_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dlg = new MessageDialog("开发中...");
            await dlg.ShowAsync();
        }

        private void doubleLine_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = new MainViewModel();
            this.DataContext = doublelineModel;
        }

        private void doublelog_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = new MainViewModel();
            this.DataContext = doublelogModel;
        }

        private void xlog_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = new MainViewModel();
            this.DataContext = logxModel;
        }

        private void ylog_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = new MainViewModel();
            this.DataContext = logyModel;
        }

        private void DecExistedLines_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItemBase item = sender as MenuFlyoutItem;
            int indexOfItems = DeleteExistedLines.Items.IndexOf(item);
            item.Visibility = (Visibility)1;
            AddLine.Items[indexOfItems + 3].Visibility = (Visibility)0;
            doublelineLines[indexOfItems].IsVisible = false;
            doublelogLines[indexOfItems].IsVisible = false;
            logxLines[indexOfItems].IsVisible = false;
            logyLines[indexOfItems].IsVisible = false;
            this.DataContext = new MainViewModel();
            this.DataContext = logyModel;
        }

        private void AddNewLine_Click(object sender, RoutedEventArgs e)
        {
            AddNewLine(numberOfLines);
        }


        private void AddDeletedLine_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            int indexOfItems = AddLine.Items.IndexOf(item)-3;
            item.Visibility = (Visibility)1;
            DeleteExistedLines.Items[indexOfItems].Visibility = (Visibility)0;
            doublelineLines[indexOfItems].IsVisible = true;
            doublelogLines[indexOfItems].IsVisible = true;
            logxLines[indexOfItems].IsVisible = true;
            logyLines[indexOfItems].IsVisible = true;
            this.DataContext = new MainViewModel();
            this.DataContext = logyModel;
        }
    }
}
