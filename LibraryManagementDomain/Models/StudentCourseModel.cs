namespace LibraryManagementDomain.Models
{
    public class StudentCourseModel
    {
        public string StudentName { get; set; } // Foreign Key for Student
        public string Title { get; set; }      // Foreign Key for Course

        // Navigation properties
        public StudentModel? Student { get; set; }
        public CourseModel? Course { get; set; }
    }
}
