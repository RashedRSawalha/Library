namespace LibraryManagementDomain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; } // Foreign key
        //public Role Role { get; set; } // Navigation property
    }
}
