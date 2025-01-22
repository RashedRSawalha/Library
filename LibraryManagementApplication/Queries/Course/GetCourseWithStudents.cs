using LibraryManagementDomain.Entities;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryManagementApplication.Queries
{
    public class GetCourseWithStudentsQuery : IRequest<CourseDTO>
    {
        public string CourseTitle { get; set; }

        public class Handler : IRequestHandler<GetCourseWithStudentsQuery, CourseDTO>
        {
            private readonly IRepository<Course> _courseRepository;

            public Handler(IRepository<Course> courseRepository)
            {
                _courseRepository = courseRepository;
            }

            public async Task<CourseDTO> Handle(GetCourseWithStudentsQuery request, CancellationToken cancellationToken)
            {
                // Fetch all courses from the repository and filter by title
                var course = (await _courseRepository.GetAllAsync())
                    .FirstOrDefault(c => c.Title == request.CourseTitle);

                // If the course is not found, return null
                if (course == null) return null;

                // Map the course entity to CourseDTO
                return new CourseDTO
                {
                    CourseId = course.CourseId,
                    Title = course.Title,

                };
            }
        }
    }
}
