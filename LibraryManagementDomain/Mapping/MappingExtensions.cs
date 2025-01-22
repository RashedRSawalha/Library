using LibraryManagementDomain.Models;
using LibraryManagementDomain.Entities;
using LibraryManagementDomain.DTO;
using LibraryManagementDomain.Enums;



namespace LibraryManagementDomain.Mapping
{
    public static class MappingExtensions
    {
        // Map BookModel to Book Entity
        public static Book ToEntity(this BookModel model)
        {
            if (model == null) return null;

            return new Book
            {
                BookId = model.BookId,
                Title = model.Title,
                YearPublished = model.YearPublished,
                AuthorId = model.AuthorId,
                
            };
        }

        // Map Book Entity to BookModel
        public static BookModel ToModel(this Book entity)
        {
            if (entity == null) return null;

            return new BookModel
            {
                BookId = entity.BookId,
                Title = entity.Title,
                YearPublished = entity.YearPublished,
                AuthorId = entity.AuthorId,
                AuthorName = entity.Author?.Name // Include author's name
            };
        }

        // Map AuthorModel to Author Entity
        public static Author ToEntity(this AuthorModel model)
        {
            if (model == null) return null;

            return new Author
            {
                //AuthorId = model.AuthorId,
                AuthorAge = model.AuthorAge,
                Name = model.Name,
                AuthorType = (short)model.AuthorType
                //Books = model.Books?.Select(b => b.ToEntity()).ToList() // Map books if provided
            };
        }

        // Map Author Entity to AuthorModel
        public static AuthorModel ToModel(this Author entity)
        {
            if (entity == null) return null;

            return new AuthorModel
            {
                //AuthorId = entity.AuthorId,
                AuthorAge = entity.AuthorAge,
                Name = entity.Name,
                AuthorType = (short)entity.AuthorType
                // Books = entity.Books?.Select(b => b.ToModel()).ToList()
            };
        }

        // Map Book Entity to DTO
        public static BookDTO ToDTO(this Book entity)
        {
            if (entity == null) return null;

            return new BookDTO
            {
                BookId = entity.BookId,
                Title = entity.Title,
                YearPublished = entity.YearPublished,
                AuthorName = entity.Author?.Name,
                AuthorId = entity.AuthorId
            };
        }

        // Map Author Entity to DTO
        public static AuthorDTO ToDTO(this Author entity)
        {
            if (entity == null) return null;

            return new AuthorDTO
            {
                AuthorId = entity.AuthorId,
                Name = entity.Name,
                AuthorAge = entity.AuthorAge,
                AuthorType = (short)entity.AuthorType
            };
        }

        // Map IEnumerable<Book> to IEnumerable<BookDTO>
        public static IEnumerable<BookDTO> ToBookDTOs(this IEnumerable<Book> books)
        {
            return books.Select(b => b.ToDTO()).ToList();
        }

        // Map IEnumerable<Author> to IEnumerable<AuthorDTO>
        public static IEnumerable<AuthorDTO> ToAuthorDTOs(this IEnumerable<Author> authors)
        {
            return authors.Select(a => a.ToDTO()).ToList();
        }

        // Map StudentModel to Student Entity
        public static Student ToEntity(this StudentModel model)
        {
            if (model == null) return null;

            return new Student
            {
                StudentId = model.StudentId,
                StudentName = model.StudentName
            };
        }

        // Map Student Entity to StudentModel
        public static StudentModel ToModel(this Student entity)
        {
            if (entity == null) return null;

            return new StudentModel
            {
                StudentId = entity.StudentId,
                StudentName = entity.StudentName,
                Courses = entity.Courses?.Select(c => c.Title).ToList() ?? new List<string>() // Fetch course titles
            };
        }

        // Map CourseModel to Course Entity
        public static Course ToEntity(this CourseModel model)
        {
            if (model == null) return null;

            return new Course
            {
                CourseId = model.CourseId,
                Title = model.Title
            };
        }

        // Map Course Entity to CourseModel
        public static CourseModel ToModel(this Course entity)
        {
            if (entity == null) return null;

            return new CourseModel
            {
                CourseId = entity.CourseId,
                Title = entity.Title,
                Students = entity.Students?.Select(s => s.StudentName).ToList() ?? new List<string>() // Fetch student names
            };
        }

        // Map Student Entity to StudentDTO
        public static StudentDTO ToDTO(this Student entity)
        {
            if (entity == null) return null;

            return new StudentDTO
            {
                StudentId = entity.StudentId,
                StudentName = entity.StudentName,
                Courses = entity.Courses?.Select(c => c.Title).ToList() ?? new List<string>() // Fetch course titles
            };
        }

        // Map Course Entity to CourseDTO
        public static CourseDTO ToDTO(this Course entity)
        {
            if (entity == null) return null;

            return new CourseDTO
            {
                CourseId = entity.CourseId,
                Title = entity.Title,
                Students = entity.Students?.Select(s => s.StudentName).ToList() ?? new List<string>() // Fetch student names
            };
        }

        // Map IEnumerable<Student> to IEnumerable<StudentDTO>
        public static IEnumerable<StudentDTO> ToStudentDTOs(this IEnumerable<Student> students)
        {
            return students.Select(s => s.ToDTO()).ToList();
        }

        // Map IEnumerable<Course> to IEnumerable<CourseDTO>
        public static IEnumerable<CourseDTO> ToCourseDTOs(this IEnumerable<Course> courses)
        {
            return courses.Select(c => c.ToDTO()).ToList();
        }
    }
}
