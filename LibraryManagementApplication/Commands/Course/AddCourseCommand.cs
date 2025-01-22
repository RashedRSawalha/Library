using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Mapping;
using LibraryManagementDomain.Models;
using LibraryManagementDomain.DTO;
using Persistence.Interface;
using MediatR;

namespace LibraryManagementApplication.Commands
{
    public class AddCourseCommand : IRequest<CourseDTO>
{
    public CourseModel Model { get; set; }

    public class AddCourseCommandHandler : IRequestHandler<AddCourseCommand, CourseDTO>
    {
        private readonly IRepository<Course> _courseRepository;
        

        public AddCourseCommandHandler(IRepository<Course> courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<CourseDTO> Handle(AddCourseCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var entity = request.Model.ToEntity();

                await _courseRepository.AddAsync(entity);
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
