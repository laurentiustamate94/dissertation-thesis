using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CloudApp.Models
{
    public class KeyPairViewModel
    {
        public string Id { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$")]
        public string Purpose { get; set; }
    }
}
