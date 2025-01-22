using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using LibraryManagementDomain.Models;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;


namespace LibraryManagementApplication.Commands
{
    public class AddStudentCommand : IRequest<StudentDTO>
    {
        public StudentModel Model { get; set; }

        public class AddStudentCommandHandler : IRequestHandler<AddStudentCommand, StudentDTO>
        {
            private readonly IRepository<Student> _studentRepository;

            public AddStudentCommandHandler(IRepository<Student> studentRepository)
            {
                _studentRepository = studentRepository;
            }

            public async Task<StudentDTO> Handle(AddStudentCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entity = request.Model.ToEntity();

                    await _studentRepository.AddAsync(entity);
                    return entity.ToDTO();
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}