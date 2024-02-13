using AutoMapper;
using Domain.Models;
using Shared.Dto;

namespace Shared.Helper;

public class MappingProfiles : Profile
{
	public MappingProfiles()
	{
		CreateMap<Book, BookDto>();
		CreateMap<BookDto, Book>();
		CreateMap<Author, AuthorDto>();		
		CreateMap<AuthorDto, Author>();
	}
}
