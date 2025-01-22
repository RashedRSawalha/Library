namespace LibraryManagementDomain.DTO
{
    public class BookDTO
    {
        public int BookId { get; set; }  // The ID of the book
        public string Title { get; set; }  // The name (title) of the book
        public int YearPublished { get; set; }
        public string AuthorName { get; set; }  // The name of the author
        public int AuthorId { get; set; }
    }
}
