using Kernal;
using LibraryManagementDomain.Entities;
using LibraryManagementDomain.Models;
using LibraryManagementApplication.Authentication;
using LibraryManagementApplication.Commands;
using LibraryManagementDomain.DTO;
using LibraryManagementApplication.Queries;
using Persistence.Interface;
using Persistence.UnitOfWork;
using LibraryManagementApplication.Authorization;
using LibraryManagementApplication.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Redis;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using LibraryManagementInfrastructure;
using System.Web.Helpers;
using BCrypt.Net;
//using LibraryManagementAPI.Data;
using Shared.AuthorizationMiddleware;
using System.ComponentModel.DataAnnotations;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly IMediator _mediator;

    public CourseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> AddCourse([FromBody] CourseModel model)
    {
        var createdCourse = await _mediator.Send(new AddCourseRequest { Model = model });
        return CreatedAtAction(nameof(AddCourse), new { id = createdCourse.CourseId }, createdCourse);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseModel>>> GetCourses()
    {
        var courses = await _mediator.Send(new GetCoursesQuery());
        return Ok(courses);
    }
}
