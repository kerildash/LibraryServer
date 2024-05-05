using AutoMapper;
using Domain.Models;
using Shared.Dto.Domain;

namespace Shared.Helper;

public class MappingProfiles : Profile
{
	public MappingProfiles()
	{
		CreateMap<Book, BookDto>()
			.ForMember(dest => dest.PictureId, opt => opt.MapFrom(src => src.Cover.Id))
			.ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.Document.Id))
			.ReverseMap()
			.ForMember(dest => dest.Cover, opt => opt.MapFrom(src => new Picture { Id = src.PictureId }))
			.ForMember(dest => dest.Document, opt => opt.MapFrom(src => new Document { Id = src.DocumentId }));

		CreateMap<Author, AuthorDto>();		
		CreateMap<AuthorDto, Author>();

		CreateMap<Tag, TagDto>();
		CreateMap<TagDto, Tag>();
	}
}
