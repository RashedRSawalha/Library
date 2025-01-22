namespace LibraryManagementDomain.Entities
{

    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public int AuthorAge { get; set; }
        public List<Book>? Books { get; set; }
        public short AuthorType { get; set; }
    }
}