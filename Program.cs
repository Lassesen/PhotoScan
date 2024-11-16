// See https://aka.ms/new-console-template for more information
using PhotoScan;

Console.WriteLine("Hello, World!");
string[] supportedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
foreach (var folder in Directory.GetDirectories(@"H:\"))
{
    Console.WriteLine(folder);    
    try
    {
        string[] photoFiles = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
            .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
            .ToArray();
        PhotoData.ScanFiles(photoFiles);
        Console.WriteLine($"{folder} {PhotoData.photos.Count}");
    }
    catch(Exception exc)
    {
        Console.WriteLine(exc.Message);
    }
}
Console.WriteLine("Done!");