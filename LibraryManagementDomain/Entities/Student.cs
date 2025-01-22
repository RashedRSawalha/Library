using Microsoft.EntityFrameworkCore;


namespace LibraryManagementDomain.Entities
{
    public class Student
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        // Navigation property for many-to-many relationship
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
