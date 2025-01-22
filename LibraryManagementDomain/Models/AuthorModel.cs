
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementDomain.Models
{

    public class AuthorModel
    {
        //public int AuthorId { get; set; }

        public string Name { get; set; }
        public int AuthorAge { get; set; }
        public short AuthorType { get; set; }
        //public List<BookModel> ?Books { get; set; }
    }
}