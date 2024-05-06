using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
namespace Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class PictureController(IPictureRepository repository, IMapper mapper, IWebHostEnvironment environment) : ControllerBase
{
	[HttpPost]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Create(IFormFile pictureCreate)
	{
		if (pictureCreate is null)
		{
			return BadRequest(ModelState);
		}
		if (!IsValidExtension(pictureCreate))
		{
			ModelState.AddModelError("", "Wrong file type. File can not be uploaded.");
			return BadRequest(ModelState);
		}
		string name = HandleName(pictureCreate.FileName);
		string path = "/Files/" + name;
		string extension = GetExtension(pictureCreate);

		using (var fileStream = new FileStream(environment.WebRootPath + path, FileMode.Create))
		{
			await pictureCreate.CopyToAsync(fileStream);
		}
		var picture = new Picture()
		{
			Id = Guid.NewGuid(),
			Name = name,
			Path = path,
			Extension = extension
		};

		var isCreated = await repository.Create(picture);
		if (!isCreated)
		{
			ModelState.AddModelError("", "Error while saving");
			return StatusCode(500, ModelState);
		}
		return Ok(picture.Id);
	}


	[HttpGet("id/{id}")]
	[Authorize(Roles = "User")]
	public async Task<IActionResult> Get(Guid id)
	{
		var document = await repository.Get(id);
		var file = GetFile(document);
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(file);
	}


	[HttpGet("by-book/{parentId}")]
	[HttpGet("by-author/{parentId}")]
	[Authorize(Roles = "User")]
	public async Task<IActionResult> GetByHolderId(Guid parentId)
	{
		var documents = await repository.GetByHolderId(parentId);
		var files = documents.Select(GetFile).ToList();
		return Ok(files);
	}
	private PhysicalFileResult GetFile(StaticFile document)
	{
		var fullPath = environment.WebRootPath + document.Path;
		return PhysicalFile(fullPath, "application/octet-stream", Path.GetFileName(fullPath));
	}
	private string HandleName(string name)
	{
		if (!IsValidName(name))
		{
			return $"{Guid.NewGuid()}.{Path.GetExtension(name)}";
		}
		return Regex.Replace(name, @"\s+", "");
	}
	private bool IsValidName(
		string text,
		string pattern = @"^[a-zA-Z0-9](?:[a-zA-Z0-9 ._-]*[a-zA-Z0-9])?\.[a-zA-Z0-9_-]+$")
	{
		return Regex.IsMatch(text, pattern);
	}

	private bool IsValidExtension(IFormFile file)
	{
		List<string> extensions = [
			".jpg",
			".jpeg",
			".png",
		];
		string extension = GetExtension(file);
		if (extensions.Contains(extension))
		{
			return true;
		}
		return false;
	}
	private string GetExtension(IFormFile file)
	{
		return Path.GetExtension(file.FileName).Trim().ToLower();
	}
}
