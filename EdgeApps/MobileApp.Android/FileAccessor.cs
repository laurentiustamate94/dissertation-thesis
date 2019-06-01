using System;
using System.IO;
using System.Linq;

using Android.App;
using MobileApp.Interfaces;

namespace MobileApp.Droid
{
    public class FileAccessor : IFileAccessor
    {
        public string[] GetLocalPublicKeyTemporaryPaths()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            return Application.Context.Assets.List("PgpKeys")
                .Where(k => k.Contains(".public."))
                .Select(k =>
                {
                    var outputFileName = Path.Combine(path, Path.GetFileName(k));
                    var tempStream = new MemoryStream();
                    var file = Application.Context.Assets.Open($"PgpKeys/{k}");

                    file.CopyTo(tempStream);
                    File.WriteAllBytes(outputFileName, tempStream.ToArray());

                    return outputFileName;
                })
                .ToArray();
        }
    }
}