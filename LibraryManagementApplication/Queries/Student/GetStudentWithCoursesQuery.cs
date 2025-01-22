using LibraryManagementDomain.Entities;
using LibraryManagementDomain.DTO;
using LibraryManagementDomain.Mapping;
using Persistence.Interface;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryManagementApplication.Queries
{
    public class GetStudentsWithCoursesQuery : IRequest<IEnumerable<StudentDTO>>
    {
        public string StudentName { get; set; }
    }

    public class GetStudentsWithCoursesHandler : IRequestHandler<GetStudentsWithCoursesQuery, IEnumerable<StudentDTO>>
    {
        private readonly IRepository<Student> _studentRepository;

        public GetStudentsWithCoursesHandler(IRepository<Student> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<IEnumerable<StudentDTO>> Handle(GetStudentsWithCoursesQuery request, CancellationToken cancellationToken)
        {
            // Fetch all students from the repository
            var students = await _studentRepository.GetAllAsync();

            // Filter by StudentName if provided
            if (!string.IsNullOrEmpty(request.StudentName))
            {
                students = students.Where(s => s.StudentName == request.StudentName);
            }

            // Map all students to StudentDTO using the ToDTO() extension method
            return students.Select(student => student.ToDTO());
        }
    }
}
