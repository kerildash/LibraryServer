using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Shared.Dto.Domain;

namespace Shared.Resolvers
{
	internal class BookCoverResolver(IStaticFileRepository<Picture> repository) : IValueResolver<BookDto, Book, Picture>
	{
		public Picture Resolve(BookDto dto, Book book, Picture cover, ResolutionContext context)
		{
			return (Picture)repository.GetAsync(dto.CoverId).Result;
		}
	}
}
