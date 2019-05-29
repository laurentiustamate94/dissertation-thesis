using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MobileApp.Interfaces
{
    public interface IFileAccessor
    {
        string[] GetLocalPublicKeyTemporaryPaths();
    }
}
