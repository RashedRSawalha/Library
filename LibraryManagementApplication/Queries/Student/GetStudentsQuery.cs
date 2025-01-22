using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;

namespace LibraryManagementApplication.Queries
{
    public class GetStudentsQuery : IRequest<IEnumerable<StudentDTO>> { }

    public class GetStudentsHandler : IRequestHandler<GetStudentsQuery, IEnumerable<StudentDTO>>
    {
        private readonly IRepository<Student> _studentRepository;

        public GetStudentsHandler(IRepository<Student> studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<IEnumerable<StudentDTO>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
        {
            var students = await _studentRepository.GetAllAsync();
            var dto = students.Select(student => student.ToDTO()); // Fixed the variable naming here
            return dto;
        }
    }
}
