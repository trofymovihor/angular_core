using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[AllowAnonymous]
public class AccountController(
    UserManager<AppUser> userManager,
     ITokenService tokenService,
      IMapper mapper
) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {

        if (await userExists(registerDto.Username)) { return BadRequest($"{registerDto.Username} is taken"); }

        var user = mapper.Map<AppUser>(registerDto);
        user.UserName = registerDto.Username.ToLower();

        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded) { return BadRequest(result.Errors);}
        return new UserDto
        {
            Username = registerDto.Username,
            Token = await tokenService.CreateToken(user),
            Gender = user.Gender,
            KnownAs = user.KnownAs,
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDTO loginDto)
    {
        var user = await userManager.Users
        .Include(u => u.Photos)
        .FirstOrDefaultAsync(x =>
        x.NormalizedUserName == loginDto.Username.ToUpper());

        if (user == null || user.UserName == null) { return Unauthorized("Invalid username"); }

        var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result) return Unauthorized();//BadRequest("Password is incorrect");

        return Ok(new UserDto
        {
            KnownAs = user.KnownAs,
            Username = user.UserName,
            Gender = user.Gender,
            Token = await tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        });
    }

    private async Task<bool> userExists(string username)
    {
        return await userManager.Users.AnyAsync(x => x.NormalizedUserName== username.ToUpper());
    }
}
