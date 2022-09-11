using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

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
                    var outputFilePath = TranslatePath(inputFilePath, pattern);
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

        private static string? TranslatePath(string inputFilePath, string pattern)
        {
            try
            {
                using var stream = new MemoryStream(File.ReadAllBytes(inputFilePath));
                var frame = BitmapFrame.Create(stream);
                if (frame.Metadata is not BitmapMetadata metadata)
                {
                    return null;
                }
                var dateString = metadata.DateTaken;
                var date = DateTime.Parse(dateString);

                return pattern
                    .Replace("<yyyy>", date.Year.ToString("0000"))
                    .Replace("<yy>", (date.Year % 100).ToString("00"))
                    .Replace("<y>", date.Year.ToString())
                    .Replace("<MM>", date.Month.ToString("00"))
                    .Replace("<M>", date.Month.ToString())
                    .Replace("<dd>", date.Day.ToString("00"))
                    .Replace("<d>", date.Day.ToString())
                    .Replace("<HH>", date.Hour.ToString("00"))
                    .Replace("<H>", date.Hour.ToString())
                    .Replace("<mm>", date.Minute.ToString("00"))
                    .Replace("<m>", date.Minute.ToString())
                    .Replace("<ss>", date.Second.ToString("00"))
                    .Replace("<s>", date.Second.ToString())
                    ;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
