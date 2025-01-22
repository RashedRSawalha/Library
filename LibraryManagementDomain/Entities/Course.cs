using Microsoft.EntityFrameworkCore;
using LibraryManagementDomain.Entities;

namespace LibraryManagementDomain.Entities
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }

        // Navigation property for many-to-many relationship
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
