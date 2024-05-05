using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Shared.Helper;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class BookController(IBookRepository repository, IMapper mapper) : ControllerBase
{
	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
	[ProducesResponseType(400)]
	[Authorize(Roles = "User")]
	public async Task<IActionResult> Get()
	{
		var books = mapper.Map<List<BookDto>>(await repository.GetAll());
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(books);
	}
	[HttpGet("id/{id}")]
	[ProducesResponseType(200, Type = typeof(BookDto))]
	[ProducesResponseType(400)]
	[Authorize(Roles = "User")]
	public async Task<IActionResult> Get(Guid id)
	{
		var book = mapper.Map<BookDto>(await repository.Get(id));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(book);
	}

	[HttpGet("by-author/{authorId}")]
	[ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
	[ProducesResponseType(400)]
	[Authorize(Roles = "User")]
	public async Task<IActionResult> GetByAuthorID(Guid authorId)
	{
		var books = mapper.Map<List<BookDto>>(await repository.GetByAuthorId(authorId));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(books);
	}

	[HttpPost]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Create([FromQuery] List<Guid> authorIds, [FromBody] BookDto bookCreate)
	{
		try
		{
			if (bookCreate is null)
			{
				return BadRequest(ModelState);
			}
			var bookMap = mapper.Map<Book>(bookCreate);
			var isCreated = await repository.Create(authorIds, bookMap);
			if (!isCreated)
			{
				ModelState.AddModelError("", "Error while saving");
				return StatusCode(500, ModelState);
			}
			return Ok("Book created");
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", ex.Message);
			return BadRequest(ModelState);
		}
	}

	[HttpPut("{bookId}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> Update(Guid bookId, [FromBody] BookDto bookUpdate)
	{
		if (bookUpdate is null)
		{
			return BadRequest(ModelState);
		}
		if (bookId != bookUpdate.Id)
		{
			return BadRequest(ModelState);
		}
		if (!await repository.Exists(bookId))
		{
			return NotFound();
		}
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}
		var bookMap = mapper.Map<Book>(bookUpdate);
		if (!await repository.Update(bookMap))
		{
			ModelState.AddModelError("", "Error while updating");
			return StatusCode(500, ModelState);
		}
		return NoContent();
	}
	[HttpPost("{bookId}/add-author/{authorId}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> AddAuthor(Guid bookId, Guid authorId)
	{
		try
		{
			if (!await repository.Exists(bookId))
			{
				return NotFound();
			}
			await repository.AddBookAuthor(bookId, authorId);
			return NoContent();
		}
		catch (InvalidOperationException ex)
		{
			ModelState.AddModelError("", ex.Message);
			return StatusCode(400, ModelState);
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", ex.Message);
			return StatusCode(500, ModelState);
		}
	}

	[HttpDelete("{bookId}/remove-author/{authorId}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> RemoveAuthor(Guid bookId, Guid authorId)
	{
		try
		{
			await repository.RemoveBookAuthor(bookId, authorId);
			return NoContent();
		}
		catch (InvalidOperationException ex)
		{
			ModelState.AddModelError("", ex.Message);
			return StatusCode(400, ModelState);
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", ex.Message);
			return StatusCode(500, ModelState);
		}
	}
}
