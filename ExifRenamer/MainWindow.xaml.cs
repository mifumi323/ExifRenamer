using System;
using System.IO;
using System.Windows;

namespace ExifRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Properties.Settings.Default;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void txtLog_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
        }

        private void txtLog_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var inputFilePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                var pattern = txtOutputPath.Text;
                foreach (var inputFilePath in inputFilePaths)
                {
                    var outputFilePath = Renamer.TranslatePath(inputFilePath, pattern);
                    txtLog.Text += $"in: {inputFilePath}\nout: {outputFilePath}\n";
                    if (!string.IsNullOrEmpty(outputFilePath))
                    {
                        try
                        {
                            File.Move(inputFilePath, outputFilePath, false);
                        }
                        catch (Exception ex)
                        {
                            txtLog.Text += $"{ex.Message}\n";
                        }
                    }
                }
                e.Handled = true;
            }
        }
    }
}
