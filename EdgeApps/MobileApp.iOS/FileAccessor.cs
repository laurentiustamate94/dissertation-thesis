using System;
using System.IO;
using System.Linq;

using Foundation;
using MobileApp.Interfaces;

namespace MobileApp.iOS
{
    public class FileAccessor : IFileAccessor
    {
        public string[] GetLocalPublicKeyTemporaryPaths()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            return NSBundle.MainBundle.PathsForResources("")
                .Where(k => k.Contains(".public."))
                .Select(k =>
                {
                    var outputFileName = Path.Combine(path, Path.GetFileName(k));
                    var file = NSBundle.MainBundle.PathForResource(Path.GetFileNameWithoutExtension(k), "asc");
                    File.Copy(file, outputFileName);

                    return outputFileName;
                })
                .ToArray();
        }
    }
}