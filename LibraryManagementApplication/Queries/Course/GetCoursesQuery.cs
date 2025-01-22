using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;
//using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryManagementApplication.Queries
{
    public class GetCoursesQuery : IRequest<IEnumerable<CourseDTO>> { }

    public class Handler : IRequestHandler<GetCoursesQuery, IEnumerable<CourseDTO>>
    {
        private readonly IRepository<Course> _courseRepository;

        public Handler(IRepository<Course> courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<CourseDTO>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = await _courseRepository.GetAllAsync();
            var dto = courses.Select(courses => courses.ToDTO());
            return dto;
        }
    }
}
