using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Entity
{
    public class Organization
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string TaxNo { get; set; }

        public string TaxOffice { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSystemData { get; set; }
        public ICollection<OrganizationUser>? OrganizationUsers { get; set; }

    }
}
