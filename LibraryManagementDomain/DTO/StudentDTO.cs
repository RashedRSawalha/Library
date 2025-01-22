namespace LibraryManagementDomain.DTO
{
    public class StudentDTO
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        // List of course titles
        public List<string> Courses { get; set; } = new List<string>();
    }
}
