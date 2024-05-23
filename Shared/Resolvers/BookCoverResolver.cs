using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Shared.Dto.Domain;

namespace Shared.Resolvers
{
	internal class BookCoverResolver(IPictureRepository repository) : IValueResolver<BookDto, Book, Picture>
	{
		public Picture Resolve(BookDto dto, Book book, Picture cover, ResolutionContext context)
		{
			return (Picture)repository.Get(dto.CoverId).Result;
		}
	}
}
