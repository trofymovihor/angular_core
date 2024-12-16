using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[AllowAnonymous]
public class AccountController(
    DataContext context,
     ITokenService tokenService,
      IConfiguration _config,
      IMapper mapper
) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {

        if (await userExists(registerDto.Username)) { return BadRequest($"{registerDto.Username} is taken"); }

        using var hmac = new HMACSHA512();

        var user = mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
           user.PasswordSalt = hmac.Key;
        // };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return new UserDto{
            Username = registerDto.Username,
            Token = tokenService.CreateToken(user),
            Gender = user.Gender,
            KnownAs = user.KnownAs,
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDTO loginDto)
    {
        var user = await context.Users
        .Include(u => u.Photos)
        .FirstOrDefaultAsync(x =>
        x.UserName.ToLower() == loginDto.Username.ToLower())
        ;
        if (user == null) { return Unauthorized("Invalid username"); }
        var test = _config["Movies:ServiceApiKey"];
        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
            {
                return Unauthorized("Invalid Password");
            }
        }
        return Ok(new UserDto
        {
            KnownAs = user.KnownAs,
            Username = loginDto.Username,
            Gender = user.Gender,
            Token = tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        });
    }

    private async Task<bool> userExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
