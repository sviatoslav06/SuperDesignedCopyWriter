using IOExtensions;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Navigation;

namespace CopyWriter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel model;
        public MainWindow()
        {
            InitializeComponent();
            model = new ViewModel()
            {
                Source = @"C:\Users\mka.itstep.000.001\Downloads\10GB.bin",
                Destination = @"C:\Users\mka.itstep.000.001\Desktop\TestFolder",
                Progress = 0
            };
            this.DataContext = model;
        }

        private async void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            #region Type 1 Type 2
            // type 1 - using File class
            string filename = Path.GetFileName(model.Source);
            string destFilePath = Path.Combine(model.Destination, filename);
            //File.Copy(Source, destFilePath, true);
            //MessageBox.Show("Complete");
            CopyProcessInfo info = new CopyProcessInfo(filename);
            model.AddProcess(info);
            await CopyFileAsync(model.Source, destFilePath, info);
            info.Percentage = 100;
            MessageBox.Show("Complete!");
            #endregion
        }
        private Task CopyFileAsync(string src, string dest, CopyProcessInfo info)
        {
            #region Type 1 Type 2
            //return Task.Run(() =>
            //{
            //    // type 2- using FileStream class
            //    using FileStream streamSource = new FileStream(src, FileMode.Open, FileAccess.Read);
            //    using FileStream streamDest = new FileStream(dest, FileMode.Create, FileAccess.Write);
            //    byte[] buffer = new byte[1024 * 8];//8KB   //12
            //    int bytes = 0;
            //    do
            //    {
            //        bytes = streamSource.Read(buffer, 0, buffer.Length);//0.5
            //        streamDest.Write(buffer, 0, bytes);//8
            //        //% = total  received 
            //        float procent = streamDest.Length / (streamSource.Length / 100);
            //        model.Progress = procent;
            //    } while (bytes > 0);
            //});
            #endregion

            return FileTransferManager.CopyWithProgressAsync(src, dest, (progress) =>
            {
                info.Percentage = progress.Percentage;
                info.BytesPerSecond = progress.BytesPerSecond;
            }, false);
        }

        private void OpenSourceBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if(dialog.ShowDialog() == true)
            {
                model.Source = dialog.FileName;
            }
        }

        private void OpenDestBtn_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                model.Destination = dialog.FileName;
            }
        }
    }
    [AddINotifyPropertyChangedInterface]
    class ViewModel
    {
        private ObservableCollection<CopyProcessInfo> processes;
        public string Source { get; set; }
        public string Destination { get; set; }
        public double Progress { get; set; }
        public bool IsWaiting => Progress == 0;
        public IEnumerable<CopyProcessInfo> Processes => processes;
        public ViewModel()
        {
            processes = new ObservableCollection<CopyProcessInfo>();
        }
        public void AddProcess(CopyProcessInfo info)
        {
            processes.Add(info);
        }
    }
    [AddINotifyPropertyChangedInterface]
    class CopyProcessInfo
    {
        public string FileName { get; set; }
        public double Percentage { get; set; }
        public int PercentageInt => (int)Percentage;
        public double BytesPerSecond { get; set; }
        public double MegaBytesPerSecond => Math.Round(BytesPerSecond / 1024 / 1024, 1);
        public CopyProcessInfo(string fileName)
        {
            this.FileName = fileName;
        }
    }
}
