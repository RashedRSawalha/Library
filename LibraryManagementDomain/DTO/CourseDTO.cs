namespace LibraryManagementDomain.DTO
{
    public class CourseDTO
    {
        public int CourseId { get; set; }
        public string Title { get; set; }

        // List of student names
        public List<string> Students { get; set; } = new List<string>();
    }
}
