using LibraryManagementDomain.Enums;

namespace LibraryManagementDomain.DTO
{
    public class AuthorDTO
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public int AuthorAge { get; set; }
        public short AuthorType { get; set; } 
    }
}
