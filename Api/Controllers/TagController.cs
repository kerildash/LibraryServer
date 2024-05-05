using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto;

namespace Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class TagController(ITagRepository repository, IMapper mapper) : ControllerBase
{
	[HttpGet]
	[ProducesResponseType(200, Type = typeof(IEnumerable<TagDto>))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> Get()
	{
		var tags = mapper.Map<List<TagDto>>(await repository.GetAll());
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(tags);
	}
	[HttpGet("id/{id}")]
	[ProducesResponseType(200, Type = typeof(TagDto))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> Get(Guid id)
	{
		var tag = mapper.Map<TagDto>(await repository.Get(id));
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		return Ok(tag);
	}
	[HttpGet("by-book/{bookId}")]
	[ProducesResponseType(200, Type = typeof(IEnumerable<TagDto>))]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetByTagID(Guid bookId)
	{
		var tags = mapper.Map<List<TagDto>>(await repository.GetByBookId(bookId));
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
		var tagMap = mapper.Map<Tag>(tagCreate);
		var isCreated = await repository.Create(tagMap);
		if (!isCreated)
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
		if (!await repository.Exists(tagId))
		{
			return NotFound();
		}
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
		var tagMap = mapper.Map<Tag>(tagUpdate);
		if (!await repository.Update(tagMap))
		{
			ModelState.AddModelError("", "Error while updating");
			return StatusCode(500, ModelState);
		}
		return NoContent();
	}

	[HttpDelete("{tagId}")]
	public async Task<IActionResult> Delete(Guid tagId)
	{
		if (!await repository.Exists(tagId))
		{
			return NotFound();
		}
		if (!ModelState.IsValid)
		{
			return BadRequest();
		}
		try
		{
			await repository.Delete(tagId);
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