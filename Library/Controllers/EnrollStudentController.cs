using LibraryManagementDomain.Models;
using LibraryManagementApplication.Commands;
using MediatR;
using LibraryManagementApplication.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LibraryManagementAPI.Commands;


namespace LibraryManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollStudentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EnrollStudentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/enroll-student
        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollStudent([FromBody] StudentCourseModel model)
        {
            // Send the enroll command to MediatR
            var result = await _mediator.Send(new EnrollStudentCommand { Model = model });

            if (!result)
            {
                return BadRequest("Enrollment failed. Please ensure both the student and course exist.");
            }

            return Ok("Enrollment successful.");
        }

        // GET: api/enroll-student/students/{studentName}
        [HttpGet("students/{studentName}")]
        public async Task<ActionResult<StudentModel>> GetStudentWithCourses(string studentName)
        {
            var student = await _mediator.Send(new GetStudentsWithCoursesQuery { StudentName = studentName });

            if (student == null)
            {
                return NotFound($"Student with name '{studentName}' not found.");
            }

            return Ok(student);
        }

        // GET: api/enroll-student/courses/{courseTitle}
        [HttpGet("courses/{courseTitle}")]
        public async Task<ActionResult<CourseModel>> GetCourseWithStudents(string courseTitle)
        {
            var course = await _mediator.Send(new GetCourseWithStudentsQuery { CourseTitle = courseTitle });

            if (course == null)
            {
                return NotFound($"Course with title '{courseTitle}' not found.");
            }

            return Ok(course);
        }
    }
}
