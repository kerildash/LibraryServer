using Api.Services;
using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class DocumentController(IDocumentRepository repository, IMapper mapper, IWebHostEnvironment environment, IStaticFileService<Document> staticFileService) : ControllerBase
{
	[HttpPost, DisableRequestSizeLimit]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	//[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Create()
	{
		var formCollection = await Request.ReadFormAsync();
		var documentCreate = formCollection.Files.First();
		if (documentCreate is null)
		{
			return BadRequest(ModelState);
		}
		if (!staticFileService.IsValidExtension(documentCreate))
		{
			ModelState.AddModelError("", "Wrong file type. File can not be uploaded.");
			return BadRequest(ModelState);
		}
		var document = await staticFileService.CreateAsync(documentCreate);

		var isCreated = await repository.Create(document);
		if (!isCreated)
		{
			ModelState.AddModelError("", "Error while saving");
			return StatusCode(500, ModelState);
		}
		return Ok(document.Id);
	}
	[HttpGet("id/{id}")]
	//[Authorize(Roles = "User")]
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
	public async Task<IActionResult> GetByParentId(Guid parentId)
	{
		var documents = await repository.GetByHolderId(parentId);

		var files = documents.Select(GetFile).ToList();
		return Ok(files);
	}

	private FileResult GetFile(StaticFile document)
	{

		var fullPath = environment.WebRootPath + document.Path;
		byte[] bytes = System.IO.File.ReadAllBytes(fullPath);
		return File(bytes, "application/octet-stream", Path.GetFileName(fullPath));
	}
}
