namespace StudentSystem.Models
{
    public class ResourceLicense
    {
        public int ResourceId { get; set; }
        public virtual Resource Resource { get; set; }

        public int LicenseId { get; set; }
        public virtual License License { get; set; }
    }
}