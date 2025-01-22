namespace LibraryManagementDomain.Models
{
    public class AddPermissionRequest
    {
        public int RoleId { get; set; } // The ID of the role
        public string Permission { get; set; } // The permission name
    }
}
