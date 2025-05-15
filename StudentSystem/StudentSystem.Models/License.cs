using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentSystem.Models
{
    public class License
    {
        public License()
        {
            this.Resources = new HashSet<ResourceLicense>();
        }

        public int LicenseId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public virtual ICollection<ResourceLicense> Resources { get; set; }
    }
}