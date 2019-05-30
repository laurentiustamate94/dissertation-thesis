using System.ComponentModel.DataAnnotations;

namespace CloudApp.Models
{
    public class KeyPairViewModel
    {
        public string Id { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$")]
        public string Purpose { get; set; }
    }
}
