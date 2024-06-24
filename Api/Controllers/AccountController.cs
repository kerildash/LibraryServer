using Api.Services.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Dto.Account;

namespace Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class AccountController(
	UserManager<AppUser> userManager,
	SignInManager<AppUser> signInManager,
	RoleManager<IdentityRole> roleManager,
	ITokenService tokenService) 
	: Controller
{
	[HttpPost("register")]
	public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto register)
	{
		//populating the db with roles
		//could be in seed data?
		if (!await roleManager.RoleExistsAsync("User"))
		{
			await roleManager.CreateAsync(new IdentityRole("User"));
			await roleManager.CreateAsync(new IdentityRole("Admin"));
		}



		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		var user = new AppUser
		{
			Email = register.Email,
			UserName = register.UserName
		};
		var result = await userManager.CreateAsync(user, register.Password);
		
		if (result.Succeeded)
		{
			await userManager.AddToRoleAsync(user, "User");
			await signInManager.SignInAsync(user, isPersistent: false);
			var roles = await userManager.GetRolesAsync(user);
			return Ok(new UserResponse
			{
				UserName = user.UserName,
				Email = user.Email,
				Token = tokenService.CreateToken(user, roles)
			});
		}
		ModelState.AddModelError("", "User could not be created");
		return BadRequest(ModelState);
	}

	[HttpPost("login")]
	public async Task<IActionResult> LoginAsync(
		[FromBody] LoginDto login)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		var user = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == login.UserName);
		if (user == null)
		{
			return Unauthorized("User not found");
		}
		var result = await signInManager.PasswordSignInAsync(user, login.Password, true, false);
		var roles = await userManager.GetRolesAsync(user);
		if (result.Succeeded)
		{
			return Ok(new UserResponse
			{
				UserName = user.UserName,
				Email = user.Email,
				Token = tokenService.CreateToken(user, roles)
			});
		}
		ModelState.AddModelError("", "Invalid email or password");
		return BadRequest(ModelState);
	}
	[HttpPost("logoff")]
	
	public async Task<IActionResult> LogOffAsync()
	{
		if (signInManager.IsSignedIn(User))
		{
			await signInManager.SignOutAsync();
			return Ok("logged off");
		}
		return Ok("no need to log off");
	}
}
