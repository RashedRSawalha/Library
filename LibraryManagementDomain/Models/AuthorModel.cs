

using System.ComponentModel.DataAnnotations;

namespace LibraryManagementDomain.Models
{

    public class AuthorModel
    {
        public string Name { get; set; }
        public int AuthorAge { get; set; }
        public short AuthorType { get; set; }
    }
}