using Api.Services.Interfaces;
using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public abstract class StaticFileController<T>(IStaticFileRepository<T> repository,
											  IMapper mapper,
											  IWebHostEnvironment environment,
											  IStaticFileService<T> service)
											  : Controller where T : StaticFile
{
	[HttpPost, DisableRequestSizeLimit]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> CreateAsync()
	{
		IFormCollection formCollection = await Request.ReadFormAsync();
		IFormFile staticFileCreate = formCollection.Files.First();
		if (staticFileCreate is null)
		{
			return BadRequest(ModelState);
		}
		if (!service.IsValidExtension(staticFileCreate))
		{
			ModelState.AddModelError("", "Wrong file type. File can not be uploaded.");
			return BadRequest(ModelState);
		}
		T document = await service.CreateAsync(staticFileCreate);

		try
		{
			await repository.CreateAsync(document);
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", "Error while creating. " + ex.Message);
			return StatusCode(500, ModelState);
		}
		return Ok(document.Id);
	}
	[HttpGet("id/{id}")]
	public async Task<IActionResult> GetAsync(Guid id)
	{
		var document = await repository.GetAsync(id);
		var file = GetFile(document);
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(file);
	}
	[HttpGet("by-book/{parentId}")]
	[HttpGet("by-author/{parentId}")]
	public async Task<IActionResult> GetByParentId(Guid holderId)
	{
		var documents = await repository.GetByHolderIdAsync(holderId);

		var files = documents.Select(GetFile).ToList();
		return Ok(files);
	}
	private FileContentResult GetFile(T staticFile)
	{
		var fullPath = environment.WebRootPath + staticFile.Path;
		byte[] bytes = System.IO.File.ReadAllBytes(fullPath);
		return File(bytes, "application/octet-stream", Path.GetFileName(fullPath));
	}
}
