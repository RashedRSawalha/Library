using LibraryManagementDomain.Models;

namespace LibraryManagementDomain.Models
{
    public class StudentModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        // List of course titles for simplicity
        public List<string>? Courses { get; set; }
    }
}