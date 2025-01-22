using LibraryManagementDomain.Models;
using LibraryManagementApplication.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> AddStudent([FromBody] StudentModel model)
    {
        var createdStudent = await _mediator.Send(new AddStudentRequest { Model = model });
        return CreatedAtAction(nameof(AddStudent), new { id = createdStudent.StudentId }, createdStudent);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentModel>>> GetStudents()
    {
        var students = await _mediator.Send(new GetStudentsQuery());
        return Ok(students);
    }
}
