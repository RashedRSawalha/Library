using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Models;
using Persistence.Interface;
using MediatR;
using Persistence.UnitOfWork;

namespace LibraryManagementAPI.Commands
{
    public class EnrollStudentCommand : IRequest<bool>
    {
        public StudentCourseModel Model { get; set; }

        public class Handler : IRequestHandler<EnrollStudentCommand, bool>
        {
            private readonly IRepository<StudentCourse> _studentCourseRepository;
            private readonly IUnitOfWork _unitOfWork;

            public Handler(IRepository<StudentCourse> studentCourseRepository, IUnitOfWork unitOfWork)
            {
                _studentCourseRepository = studentCourseRepository;
                _unitOfWork = unitOfWork;
            }

            public async Task<bool> Handle(EnrollStudentCommand request, CancellationToken cancellationToken)
            {
                await _unitOfWork.BeginTransactionAsync();

                var entity = new StudentCourse
                {
                    StudentName = request.Model.StudentName,
                    CourseTitle = request.Model.Title
                };

                await _studentCourseRepository.AddAsync(entity);
                await _unitOfWork.SaveAndCommitAsync();

                return true;
            }
        }
    }
}
