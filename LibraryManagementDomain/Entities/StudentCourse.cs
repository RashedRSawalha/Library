namespace LibraryManagementDomain.Entities
{
    public class StudentCourse
    {
        // Foreign Key for Student
        public string StudentName { get; set; }
        public Student Student { get; set; } // Navigation property

        // Foreign Key for Course
        public string CourseTitle { get; set; }
        public Course Course { get; set; } // Navigation property
    }
}
