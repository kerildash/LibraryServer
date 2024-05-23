using AutoMapper;
using Shared.Dto.Domain;
using Domain.Models;
using Database.RepositoryInterfaces;

namespace Shared.Resolvers
{
    internal class BookDocumentResolver(IDocumentRepository repository) : IValueResolver<BookDto, Book, Document>
    {
        public Document Resolve(BookDto dto, Book book, Document document, ResolutionContext context)
        {
            return (Document)repository.Get(dto.DocumentId).Result;
        }
    }
}
