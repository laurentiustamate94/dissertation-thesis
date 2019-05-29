using System;
using System.Collections.Generic;
using System.Text;

namespace Communication.Common.Interfaces
{
    public interface IUniqueIdGenerationService
    {
        string GenerateRandomId();
    }
}
