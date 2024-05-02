using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static System.Reflection.Metadata.BlobBuilder;

namespace Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class AccountController(
	UserManager<AppUser> userManager,
	SignInManager<AppUser> signInManager,
	RoleManager<IdentityRole> roleManager) 
	: Controller
{
	[HttpPost("register")]
	public async Task<IActionResult> Register(
		[EmailAddress] string email,
		string userName,
		[MinLength(8)] string password, string confirmPassword)
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
			Email = email,
			UserName = userName
		};
		var result = await userManager.CreateAsync(user, password);
		
		if (result.Succeeded)
		{
			await userManager.AddToRoleAsync(user, "User");
			await signInManager.SignInAsync(user, isPersistent: false);
			return Ok($"user {user.UserName} created in a role of User");
		}
		ModelState.AddModelError("", "User could not be created");
		return BadRequest(ModelState);
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login(
		string userName,
		[MinLength(8)] string password)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		var result = await signInManager.PasswordSignInAsync(userName, password, true, false);
		if (result.Succeeded)
		{
			return Ok();
		}
		ModelState.AddModelError("", "Invalid email or password");
		return BadRequest(ModelState);
	}
	[HttpPost("logoff")]
	
	public async Task<IActionResult> LogOff()
	{
		if (signInManager.IsSignedIn(User))
		{
			await signInManager.SignOutAsync();
			return Ok("logged off");
		}
		return Ok("no need to log off");
	}
}
