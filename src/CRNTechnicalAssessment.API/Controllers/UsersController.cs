using Asp.Versioning;
using CRNTechnicalAssessment.Application.DTOs;
using CRNTechnicalAssessment.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRNTechnicalAssessment.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequestDto request)
    {
        var user = await _userService.CreateUserAsync(request);

        return CreatedAtAction(
            nameof(CreateUser),
            new { id = user.Id },
            user);
    }
}