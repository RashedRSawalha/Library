using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementDomain.Entities
{

    [Table("Books")]
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int YearPublished { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }


                            
    }




}