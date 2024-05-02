using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;

namespace Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class AuthorController(IAuthorRepository repository, IMapper mapper) : ControllerBase
{
	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
	[ProducesResponseType(400)]
	[Authorize(Roles = "User")]
	public IActionResult Get()
	{
		var authors = mapper.Map<List<AuthorDto>>(repository.GetAll());
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

	public IActionResult Get(Guid id)
	{
		var author = mapper.Map<AuthorDto>(repository.Get(id));
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
	public IActionResult GetByAuthorID(Guid bookId)
	{
		var authors = mapper.Map<List<AuthorDto>>(repository.GetByBookId(bookId));
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
	public IActionResult Create([FromBody] AuthorDto authorCreate)
	{
		if (authorCreate is null)
		{
			return BadRequest(ModelState);
		}
		var authorMap = mapper.Map<Author>(authorCreate);
		var isCreated = repository.Create(authorMap);
		if (!isCreated)
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
	public IActionResult Update(Guid authorId, [FromBody] AuthorDto authorUpdate)
	{
		if (authorUpdate is null)
		{
			return BadRequest(ModelState);
		}
		if (authorId != authorUpdate.Id)
		{
			return BadRequest(ModelState);
		}
		if (!repository.Exists(authorId))
		{
			return NotFound();
		}
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		var authorMap = mapper.Map<Author>(authorUpdate);
		if (!repository.Update(authorMap))
		{
			ModelState.AddModelError("", "Error while updating");
			return StatusCode(500, ModelState);
		}
		return NoContent();
	}

	[HttpDelete("{authorId}")]
	[Authorize(Roles = "Admin")]
	public IActionResult Delete(Guid authorId)
	{
		if (!repository.Exists(authorId))
		{
			return NotFound();
		}
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}
		try
		{
			repository.Delete(authorId);
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
