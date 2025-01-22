
namespace LibraryManagementDomain.Entities
{
    public class RolePermission
    {
        public int Id { get; set; }
        public int RoleId { get; set; } // Foreign key to Role table
        public string Permission { get; set; } // e.g., ViewAuthors, EditAuthors

    }
}
