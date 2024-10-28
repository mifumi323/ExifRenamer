using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace ExifRenamer
{
    public class Renamer
    {
        public static string? TranslatePath(string inputFilePath, string pattern)
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
