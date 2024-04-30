using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
//using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class DocumentController(IDocumentRepository repository, IMapper mapper, IWebHostEnvironment environment) : ControllerBase
{
	[HttpPost]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	public async Task<IActionResult> Create(IFormFile documentCreate)
	{
		if (documentCreate is null)
		{
			return BadRequest(ModelState);
		}
		if (!IsValidExtension(documentCreate))
		{
			ModelState.AddModelError("", "Wrong file type. File can not be uploaded.");
			return BadRequest(ModelState);
		}
		string name = HandleName(documentCreate.FileName);
		string path = "/Files/" + name;

		using (var fileStream = new FileStream(environment.WebRootPath + path, FileMode.Create))
		{
			await documentCreate.CopyToAsync(fileStream);
		}
		var document = new StaticFile()
		{
			Id = Guid.NewGuid(),
			Name = name,
			Path = path
		};

		var isCreated = repository.Create(document);
		if (!isCreated)
		{
			ModelState.AddModelError("", "Error while saving");
			return StatusCode(500, ModelState);
		}
		return Ok(document.Id);
	}


	[HttpGet("id/{id}")]
	public IActionResult Get(Guid id)
	{
		var document = repository.Get(id);
		var file = GetFile(document);
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(file);
	}


	[HttpGet("by-book/{parentId}")]
	[HttpGet("by-author/{parentId}")]
	public IActionResult GetByParentId(Guid parentId)
	{
		var documents = repository.GetByHolderId(parentId);
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
			"pdf",
			"epub",
			"fb2",
			"mobi",
			"djvu",
			"docx"];
		string extension = Path.GetExtension(file.Name).Trim().ToLower();
		if (extensions.Contains(extension))
		{
			return true;
		}
		return false;
	}
}
