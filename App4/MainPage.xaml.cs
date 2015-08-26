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
using OxyPlot;
using OxyPlot.Windows;
using OxyPlot.Series;
using OxyPlot.Axes;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace App4
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            setModel();
            this.DataContext = new MainViewModel();
        }

        MainViewModel doublelogModel = new MainViewModel();
        MainViewModel doublelineModel = new MainViewModel();
        MainViewModel logxModel = new MainViewModel();
        MainViewModel logyModel = new MainViewModel();

        int count = 0;

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

        //public void ChangeModel()
        //{
        //    myModel.MyModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "COS(X)"));
        //}

        private async void OnPickSingleFileClicked(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            //PlotModel myModel = new PlotModel();
            openPicker.FileTypeFilter.Add(".lis");
            StorageFile file = await openPicker.PickSingleFileAsync();
            IList<string> fileContents = await FileIO.ReadLinesAsync(file);
            count++;
            LineSeries doublelogLine = new LineSeries();
            LineSeries doublelineLine = new LineSeries();
            LineSeries logxLine = new LineSeries();
            LineSeries logyLine = new LineSeries();
            doublelogLine.Title = file.Name;
            doublelineLine.Title = file.Name;
            logxLine.Title = file.Name;
            logyLine.Title = file.Name;
            for (int i = 2; i < fileContents.Count; i++)
            {
                string currentLine = fileContents[i];
                string[] lineContents = currentLine.Split();
                string[] valuableLineContents = new string[4];
                int j = 0;
                for (int index = 0; index < lineContents.Length; index++)
                {
                    if (lineContents[index] != "")
                    {
                        valuableLineContents[j] = lineContents[index];
                        j++;
                    }
                }
                double x = Convert.ToDouble(valuableLineContents[1]) - Convert.ToDouble(valuableLineContents[0]);
                double y = Convert.ToDouble(valuableLineContents[2]) * x;
                doublelineLine.Points.Add(new DataPoint(x, y));
                doublelogLine.Points.Add(new DataPoint(x, y));
                logxLine.Points.Add(new DataPoint(x, y));
                logyLine.Points.Add(new DataPoint(x, y));
            }
            doublelogModel.MyModel.Series.Add(doublelogLine);
            doublelogModel.MyModel.Series.Remove(doublelogLine);
            doublelineModel.MyModel.Series.Add(doublelineLine);
            logxModel.MyModel.Series.Add(logxLine);
            logyModel.MyModel.Series.Add(logyLine);
            button1.Content = count.ToString();
            this.DataContext = new MainViewModel();
            this.DataContext = logxModel;
        }

        private void OnDoubleLineClicked(object sender, RoutedEventArgs e)
        {
            this.DataContext = new MainViewModel();
            this.DataContext = doublelineModel;
        }

        private void OnDoubleLogClicked(object sender, RoutedEventArgs e)
        {
            this.DataContext = new MainViewModel();
            this.DataContext = doublelogModel;
        }

        private void OnLogXClicked(object sender, RoutedEventArgs e)
        {
            this.DataContext = new MainViewModel();
            this.DataContext = logxModel;
        }

        private void OnLogYClicked(object sender, RoutedEventArgs e)
        {
            this.DataContext = new MainViewModel();
            this.DataContext = logyModel;
        }
    }
}
