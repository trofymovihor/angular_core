using System;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager) : BaseApiController
{
    [Authorize(Policy = "RequiredAdminRole")]
    [HttpGet("users-with-roles")]
    public async  Task<ActionResult> GetUsersWithPolicy()
    {
        var users = await userManager.Users
            .OrderBy(x => x.UserName)
            .Select(x => new
            {
                x.Id,
                Username = x.UserName,
                Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequiredAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("you must select at least one role");
        var selectedRoles = roles.Split(",").ToArray();

        var user = await userManager.FindByNameAsync(username);
        if (user is null) return BadRequest("user not found");

        var userRoles = await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to add roles to user");

        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("failed to remove from roles");

        return Ok(await userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModerator()
    {
        return Ok("only moderators can see it");
    }
}