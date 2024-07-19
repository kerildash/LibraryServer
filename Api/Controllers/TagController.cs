using Api.Services.Interfaces;
using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto.Domain;

namespace Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class TagController(ITagRepository repository, ITagService tagService, IMapper mapper) : ControllerBase
{
	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<TagDto>))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetAllAsync(int pageNumber = 0, int pageSize = 12)
	{
		var tags = mapper.Map<List<TagDto>>(await repository.GetAllAsync(pageNumber, pageSize));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(tags);
	}
	[HttpGet("id/{id}")]
	[ProducesResponseType(200, Type = typeof(TagDto))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetAsync(Guid id)
	{
		var tag = mapper.Map<TagDto>(await repository.GetAsync(id));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(tag);
	}
	[HttpGet("find/{query}")]
	[ProducesResponseType(200, Type = typeof(IEnumerable<TagDto>))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> FindAsync(string query, int pageNumber = 0, int pageSize = 12)
	{
		var tags = mapper.Map<List<TagDto>>(await repository.GetAsync(query, pageNumber, pageSize));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(tags);
	}
	[HttpGet("by-book/{bookId}")]
	[ProducesResponseType(200, Type = typeof(IEnumerable<TagDto>))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetByBookID(Guid bookId)
	{
		var tags = mapper.Map<List<TagDto>>(await repository.GetByBookIdAsync(bookId));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(tags);
	}
	
	[HttpPost]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	public async Task<IActionResult> Create([FromBody] TagDto tagCreate)
	{
		if (tagCreate is null)
		{
			return BadRequest(ModelState);
		}
		Tag tagMap = mapper.Map<Tag>(tagCreate);

		try
		{
			tagService.HandleName(tagMap);
			await repository.CreateAsync(tagMap);
		}
		catch (ArgumentException ex)
		{
			ModelState.AddModelError("", ex.Message);
		}
		catch
		{
			ModelState.AddModelError("", "Error while saving");
			return StatusCode(500, ModelState);
		}
		return Ok("Tag created");
	}
	[HttpPut("{tagId}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> Update(Guid tagId, [FromBody] TagDto tagUpdate)
	{
		if (tagUpdate is null)
		{
			return BadRequest(ModelState);
		}
		if (tagId != tagUpdate.Id)
		{
			return BadRequest(ModelState);
		}
		if (!await repository.ExistsAsync(tagId))
		{
			return NotFound();
		}
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		var tagMap = mapper.Map<Tag>(tagUpdate);
		try
		{
			tagService.HandleName(tagMap);
			await repository.UpdateAsync(tagMap);
		}
		catch (ArgumentException ex)
		{
			ModelState.AddModelError("", ex.Message);
		}
		catch
		{
			ModelState.AddModelError("", "Error while updating");
			return StatusCode(500, ModelState);
		}
		return NoContent();
	}

	[HttpDelete("{tagId}")]
	public async Task<IActionResult> Delete(Guid tagId)
	{
		if (!await repository.ExistsAsync(tagId))
		{
			return NotFound();
		}
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}
		try
		{
			await repository.DeleteAsync(tagId);
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