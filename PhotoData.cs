using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.Design;
namespace PhotoScan
{
    internal class PhotoData
    {
        public static Dictionary<string, string> photos = new Dictionary<string, string>();
        public static void ScanFiles(string[] photoFiles)
        {

            foreach (string photoFile in photoFiles)
            {
                try
                {
                    var directories = ImageMetadataReader.ReadMetadata(photoFile);
                    var directory = directories.OfType<ExifIfd0Directory>().FirstOrDefault();
                    string outfolder = "";
                    if (directory != null && directory.Tags.Count > 0)
                    {
                        if (directory.ContainsTag(ExifDirectoryBase.TagDateTime))
                        {
                            try
                            {//may not be a valid date
                                var dateTaken = directory.GetDateTime(ExifDirectoryBase.TagDateTime);
                                outfolder = Path.Combine("W:\\Photos", $"{dateTaken.Year}\\{dateTaken.Month}\\{dateTaken.Day}");
                            }
                            catch { }
                        }
                        if (string.IsNullOrWhiteSpace(outfolder) && directory.ContainsTag(ExifDirectoryBase.TagMake))
                        {
                            var make = directory.GetString(ExifDirectoryBase.TagMake);
                            outfolder = Path.Combine("W:\\Photos", $"{make}");
                            if (directory.ContainsTag(ExifDirectoryBase.TagModel))
                            {
                                outfolder = Path.Combine("W:\\Photos", $"{make}\\{directory.GetString(ExifDirectoryBase.TagModel)}");
                            }
                            var fileInfo = new FileInfo(photoFile);
                            outfolder = Path.Combine(outfolder, $"{fileInfo.CreationTime.Year}\\{fileInfo.CreationTime.Month}\\{fileInfo.CreationTime.Day}");
                        }

                        if (string.IsNullOrWhiteSpace(outfolder) && directory.ContainsTag(ExifDirectoryBase.TagSoftware))
                        {
                            var software = directory.GetString(ExifDirectoryBase.TagSoftware).ToLower();
                            if (software.Contains("adobe", StringComparison.InvariantCultureIgnoreCase))
                            {
                            }
                            else
                            {
                                if (software.Length > 21)
                                    software = software.Substring(0, 20);
                                var fileInfo = new FileInfo(photoFile);
                                outfolder = Path.Combine("W:\\Photos", $"{software}\\{fileInfo.CreationTime.Year}\\{fileInfo.CreationTime.Month}\\{fileInfo.CreationTime.Day}");
                            }

                        }
                        if (!string.IsNullOrWhiteSpace(outfolder))
                        {
                            var fileInfo = new FileInfo(photoFile);
                            var folder = new DirectoryInfo(outfolder);
                            if (!folder.Exists)
                            {
                                folder.Create();
                            }
                            var outfile = Path.Combine(outfolder, fileInfo.Name);
                            if (!File.Exists(outfile))
                            {
                                File.Copy(photoFile, outfile);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {photoFile}: {ex.Message}");
                }
            }
        }
    }
}
