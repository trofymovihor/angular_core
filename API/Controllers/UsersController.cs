using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Authorize]
public class UsersController(
    IUserRepository repository,
IMapper mapper,
IPhotoService photoService
) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await repository.GetMembersAsync();
        return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await repository.GetMemberAsync(username);

        if (user == null) return NotFound();

        return user;
    }
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberDto)
    {

        var user = await repository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("User not found");
        mapper.Map(memberDto, user);
        if (await repository.SaveAllAsync()) return NoContent();
        return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await repository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return BadRequest("Cannot update user");
        var result = await photoService.AddPhotoAsync(file);

        if (result.Error!= null){
            return BadRequest(result.Error.Message);
        }

        var photo = new Photo{
            Url=result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };
        user.Photos.Add(photo);

        if (await repository.SaveAllAsync()) 
        return CreatedAtAction(
            nameof(GetUser), 
        new {username=User.GetUsername()},
         mapper.Map<PhotoDto>(photo));

        return BadRequest("Problem adding photo");
    }
}
