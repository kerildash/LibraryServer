using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Shared.Dto.Domain;
using Shared.Resolvers;

namespace Shared.Helper;

public class MappingProfiles : Profile
{
	public MappingProfiles()
	{
		CreateMap<Book, BookDto>()
			.ForMember(dest => dest.CoverId, opt => opt.MapFrom(src => src.Cover.Id))
			.ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.Document.Id))
			.ReverseMap()
			.ForMember(dest => dest.Cover, opt => opt.MapFrom<BookCoverResolver>())
			.ForMember(dest => dest.Document, opt => opt.MapFrom<BookDocumentResolver>());
		CreateMap<Author, AuthorDto>();		
		CreateMap<AuthorDto, Author>();

		CreateMap<Tag, TagDto>();
		CreateMap<TagDto, Tag>();
	}
}
