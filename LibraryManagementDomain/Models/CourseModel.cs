namespace LibraryManagementDomain.Models
{
    public class CourseModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; }

        // List of student names for simplicity
        public List<string>? Students { get; set; }
    }
}
