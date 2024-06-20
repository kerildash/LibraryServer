using Api.Services.Interfaces;
using Domain.Models;
using System.Text.RegularExpressions;

namespace Api.Services;

public class TagService : ITagService
{
	public void HandleName(Tag tag)
	{
		if (!IsNameMatchThePrimaryFilter(tag))
		{
			throw new ArgumentException("Invalid tag name");
		}
		CorrectName(tag);
	}
	public bool IsNameCorrect(Tag tag)
	{
		return Regex.IsMatch(tag.Name, @"^[a-zа-яё0-9_]+$") || tag.Name.Length <= 30;
	}

	private bool IsNameMatchThePrimaryFilter(Tag tag)
	{
		return Regex.IsMatch(tag.Name, @"^[a-zA-Zа-яёА-ЯЁ0-9_]+$") || tag.Name.Length <= 30;
	}
	private void CorrectName(Tag tag) 
	{
		tag.Name = tag.Name.ToLower();
	}
}
