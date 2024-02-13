using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
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
	public IActionResult Get(Guid id)
	{
		var author = mapper.Map<AuthorDto>(repository.Get(id));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(author);
	}
	[HttpGet("by-book/{authorId}")]
	[ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
	[ProducesResponseType(400)]
	public IActionResult GetByAuthorID(Guid authorId)
	{
		var authors = mapper.Map<List<AuthorDto>>(repository.GetByBookId(authorId));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(authors);
	}
	[HttpPost]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
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
}
