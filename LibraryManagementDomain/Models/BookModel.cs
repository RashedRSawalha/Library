using Microsoft.EntityFrameworkCore;

namespace LibraryManagementDomain.Models
{
    public class BookModel
    {
    //internal string AuthorName;

        
        public int BookId { get; set; }
        public string Title { get; set; }
        public int YearPublished { get; set; }
        public string AuthorName { get; set; }
        public int AuthorId { get; set; }
       



    }
}

  






