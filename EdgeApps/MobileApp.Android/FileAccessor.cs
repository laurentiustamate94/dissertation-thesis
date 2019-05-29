using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Views;
using Android.Widget;
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