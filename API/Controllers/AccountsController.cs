using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using API.Data;
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[AllowAnonymous]
public class AccountsController(DataContext context, ITokenService tokerService) : BaseApiController
{
[HttpPost("register")]
public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto){

    if (await userExists(registerDto.Username)){ return BadRequest($"{registerDto.Username} is taken"); }
    return Ok();
    // using var hmac = new HMACSHA512();
    // var user = new AppUser(){
    //     UserName = registerDto.Username,
    //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
    //     PasswordSalt = hmac.Key
    // };

    // context.Users.Add(user);
    // await context.SaveChangesAsync();
    // return new UserDto{
    //     Username = registerDto.Username,
    //     Token = tokerService.CreateToken(user)
    // };
}

[HttpPost("login")]
public async Task<ActionResult<UserDto>> Login (LoginDTO loginDto){
    var user = await context.Users.FirstOrDefaultAsync(x => 
    x.UserName.ToLower() == loginDto.Username.ToLower());
    if (user == null){return Unauthorized("Invalid username");}

    using var hmac = new HMACSHA512(user.PasswordSalt);

    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
    for (int i = 0;i< computedHash.Length; i++){
        if (computedHash[i] != user.PasswordHash[i]){
            return Unauthorized("Invalid Password");
        }
    }
    return Ok( new UserDto{
        Username = loginDto.Username,
        Token = tokerService.CreateToken(user)
    });
}

private async Task<bool> userExists( string username){
return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
}
}
