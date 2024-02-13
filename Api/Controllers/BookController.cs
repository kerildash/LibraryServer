using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Shared.Helper;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;

namespace Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class BookController(IBookRepository repository, IMapper mapper) : ControllerBase
{
	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
	[ProducesResponseType(400)]
	public IActionResult Get()
	{
		var books = mapper.Map<List<BookDto>>(repository.GetAll());
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(books);
	}
	[HttpGet("id/{id}")]
	[ProducesResponseType(200, Type = typeof(BookDto))]
	[ProducesResponseType(400)]
	public IActionResult Get(Guid id)
	{
		var book = mapper.Map<BookDto>(repository.Get(id));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(book);
	}

	[HttpGet("by-author/{authorId}")]
	[ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
	[ProducesResponseType(400)]
	public IActionResult GetByAuthorID(Guid authorId)
	{
		var books = mapper.Map<List<BookDto>>(repository.GetByAuthorId(authorId));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(books);
	}

	[HttpPost]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	public IActionResult Create([FromQuery] List<Guid> authorIds,[FromBody] BookDto bookCreate)
	{
		if (bookCreate is null)
		{
			return BadRequest(ModelState);
		}
		var bookMap = mapper.Map<Book>(bookCreate);
		var isCreated = repository.Create(authorIds, bookMap);
		if (!isCreated)
		{
			ModelState.AddModelError("", "Error while saving");
			return StatusCode(500, ModelState);
		}
		return Ok("Book created");
	}
}
