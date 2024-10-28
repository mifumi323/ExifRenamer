using System;
using System.Collections.Generic;
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
                TransratePaths(inputFilePaths, pattern);
                e.Handled = true;
            }
        }

        private void TransratePaths(IEnumerable<string> inputFilePaths, string pattern)
        {
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
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                var inputFilePaths = new List<string>();
                foreach (var arg in args[1..])
                {
                    if (arg.Contains('*') || arg.Contains('?'))
                    {
                        var dir = Path.GetDirectoryName(arg) ?? Directory.GetCurrentDirectory();
                        var filename = Path.GetFileName(arg);
                        foreach (var path in Directory.GetFiles(dir, filename))
                        {
                            inputFilePaths.Add(path);
                        }
                    }
                    else
                    {
                        inputFilePaths.Add(arg);
                    }
                }
                var pattern = txtOutputPath.Text;
                TransratePaths(inputFilePaths, pattern);
            }
        }
    }
}
