using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto.Domain;

namespace Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class AuthorController(IAuthorRepository repository, IMapper mapper) : ControllerBase
{
	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
	[ProducesResponseType(400)]
	[Authorize(Roles = "User", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public async Task<IActionResult> GetAllAsync(int pageNumber = 0, int pageSize = 12)
	{
		var authors = mapper.Map<List<AuthorDto>>(await repository.GetAllAsync(pageNumber, pageSize));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(authors);
	}
	[HttpGet("id/{id}")]
	[ProducesResponseType(200, Type = typeof(AuthorDto))]
	[ProducesResponseType(400)]
	[Authorize(Roles = "User")]

	public async Task<IActionResult> GetAsync(Guid id)
	{
		var author = mapper.Map<AuthorDto>(await repository.GetAsync(id));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(author);
	}
	[HttpGet("by-book/{bookId}")]
	[ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
	[ProducesResponseType(400)]
	[Authorize(Roles = "User")]
	public async Task<IActionResult> GetByBookIdAsync(Guid bookId)
	{
		var authors = mapper.Map<List<AuthorDto>>(await repository.GetByBookIdAsync(bookId));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(authors);
	}

	[HttpPost]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> CreateAsync([FromBody] AuthorDto authorCreate)
	{
		if (authorCreate is null)
		{
			return BadRequest(ModelState);
		}
		var authorMap = mapper.Map<Author>(authorCreate);
		try
		{
			await repository.CreateAsync(authorMap);
		}
		catch
		{
			ModelState.AddModelError("", "Error while saving");
			return StatusCode(500, ModelState);
		}
		return Ok("Author created");
	}

	[HttpPut("{authorId}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> UpdateAsync(Guid authorId, [FromBody] AuthorDto authorUpdate)
	{
		if (authorUpdate is null)
		{
			return BadRequest(ModelState);
		}
		if (authorId != authorUpdate.Id)
		{
			return BadRequest(ModelState);
		}
		if (!await repository.ExistsAsync(authorId))
		{
			return NotFound();
		}
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		var authorMap = mapper.Map<Author>(authorUpdate);
		try
		{
			await repository.UpdateAsync(authorMap);
		}
		catch
		{
			ModelState.AddModelError("", "Error while updating");
			return StatusCode(500, ModelState);
		}
		return NoContent();
	}

	[HttpDelete("{authorId}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> DeleteAsync(Guid authorId)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}
		try
		{
			await repository.DeleteAsync(authorId);
			return NoContent();
		}
		catch (ArgumentException ex)
		{
			ModelState.AddModelError("", ex.Message);
			return NotFound(ModelState);
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", ex.Message);
			return StatusCode(500, ModelState);
		}
	}
}
