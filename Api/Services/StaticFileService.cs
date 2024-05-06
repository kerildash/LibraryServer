using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System;
using System.IO;

namespace Api.Services;

public class StaticFileService<T>(IWebHostEnvironment environment) : IStaticFileService<T> where T : StaticFile, new()
{
	async Task<T> IStaticFileService<T>.CreateAsync(IFormFile staticFileCreate)
	{
		string name = HandleName(staticFileCreate.FileName);
		string path = "/Files/" + name;
		string extension = GetExtension(staticFileCreate);
		await SaveInWebRootAsync(path, staticFileCreate);

		return new T()
		{
			Id = Guid.NewGuid(),
			Name = name,
			Path = path,
			Extension = extension,
		};
	}
	private async Task SaveInWebRootAsync(string path, IFormFile staticFileCreate)
	{
		using (var fileStream = new FileStream(environment.WebRootPath + path, FileMode.Create))
		{
			await staticFileCreate.CopyToAsync(fileStream);
		}
	}
	private string HandleName(string name)
	{
		if (!IsValidName(name))
		{
			return $"{Guid.NewGuid()}.{Path.GetExtension(name)}";
		}
		return Regex.Replace(name, @"\s+", "");
	}
	private bool IsValidName(
		string text,
		string pattern = @"^[a-zA-Z0-9](?:[a-zA-Z0-9 ._-]*[a-zA-Z0-9])?\.[a-zA-Z0-9_-]+$")
	{
		return Regex.IsMatch(text, pattern);
	}

	bool IStaticFileService<T>.IsValidExtension(IFormFile file)
	{
		List<string> extensions = [];
		if (typeof(T) == typeof(Document))
		{
			extensions = [
				".pdf",
				".epub",
				".fb2",
				".mobi",
				".djvu",
				".docx"];
		}
		if (typeof(T) == typeof(Picture))
		{
			extensions = [
				".jpg",
				".jpeg",
				".png",
			];
		}

		string extension = GetExtension(file);
		if (extensions.Contains(extension))
		{
			return true;
		}
		return false;
	}
	internal string GetExtension(IFormFile file)
	{
		return Path.GetExtension(file.FileName).Trim().ToLower();
	}


}
