using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Shared.Dto.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class BookController(IBookRepository repository, IMapper mapper) : ControllerBase
{
	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
	[ProducesResponseType(400)]
	[Authorize(Roles = "User", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public async Task<IActionResult> GetAllAsync(int pageNumber = 0, int pageSize = 12)
	{
		var books = mapper.Map<List<BookDto>>(await repository.GetAllAsync(pageNumber, pageSize));
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
	public async Task<IActionResult> GetAsync(Guid id)
	{
		var book = mapper.Map<BookDto>(await repository.GetAsync(id));
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
	public async Task<IActionResult> GetByAuthorIDAsync(Guid authorId)
	{
		var books = mapper.Map<List<BookDto>>(await repository.GetByAuthorIdAsync(authorId));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(books);
	}

	[HttpPost]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public async Task<IActionResult> CreateAsync([FromQuery] List<Guid?> authorIds, [FromBody] BookDto bookCreate)
	{

		if (bookCreate is null)
		{
			return BadRequest(ModelState);
		}
		bookCreate.Id = Guid.NewGuid().ToString();
		var bookMap = mapper.Map<Book>(bookCreate);


		//todo: replace document and picture in a bookmap (get them from repos)
		try
		{
			await repository.CreateAsync(authorIds, bookMap);
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", "Error while saving. " + ex.Message);
			return StatusCode(500, ModelState);
		}
		return Ok("Book created");
	}

	[HttpPut("{bookId}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> UpdateAsync(Guid bookId, [FromBody] BookDto bookUpdate)
	{
		if (bookUpdate is null)
		{
			return BadRequest(ModelState);
		}
		if (bookId.ToString() != bookUpdate.Id)
		{
			return BadRequest(ModelState);
		}
		if (!await repository.ExistsAsync(bookId))
		{
			return NotFound();
		}
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}
		var bookMap = mapper.Map<Book>(bookUpdate);
		try
		{
			await repository.UpdateAsync(bookMap);
		}
		catch (Exception ex)
		{
			ModelState.AddModelError("", "Error while updating. " + ex.Message);
			return StatusCode(500, ModelState);
		}
		return NoContent();
	}
	[HttpPost("{bookId}/add-author/{authorId}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> AddAuthorAsync(Guid bookId, Guid authorId)
	{
		try
		{
			if (!await repository.ExistsAsync(bookId))
			{
				return NotFound();
			}
			await repository.AddBookAuthorAsync(bookId, authorId);
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
	public async Task<IActionResult> RemoveAuthorAsync(Guid bookId, Guid authorId)
	{
		try
		{
			await repository.RemoveBookAuthorAsync(bookId, authorId);
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
	[HttpDelete("{bookId}")]
	//	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> DeleteAsync(Guid bookId)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}
		try
		{
			await repository.DeleteAsync(bookId);
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
