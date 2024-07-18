using Database;
using Database.Repositories;
using Database.Services;
using Domain.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Database;

public class AuthorRepositoryTests
{
    private readonly DataContext context;

    public AuthorRepositoryTests()
    {
        DbContextOptionsBuilder<DataContext> options =
            new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());

        context = new DataContext(options.Options);
    }

    #region ExistsAsync
    [Fact]
    public async Task ExistsAsync_ExistingAuthorInDb_ReturnsTrue()
    {
        AuthorRepository repo = new(context, null);
        Author author = new Author { Name = "Test" , Bio = "Test" };
        context.Authors.Add(author);
        context.SaveChanges();

        bool result = await repo.ExistsAsync(author.Id);

        result.Should().BeTrue();
    }

	[Fact]
	public async Task ExistsAsync_IdOfAuthorWhichIsNotExist_ReturnsFalse()
	{
		AuthorRepository repo = new(context, null);
		Author author = new Author { Name = "Test", Bio = "Test" };
		context.Authors.Add(author);
        context.SaveChanges();

		bool result = await repo.ExistsAsync(Guid.NewGuid());

		result.Should().BeFalse();
	}
    #endregion

    #region GetAsync(Guid id)
    [Fact]
	public async Task GetAsync_IdOfExistingAuthor_ReturnsAuthor()
    {
		AuthorRepository repo = new(context, new SearchService<Author>());
		Author author = new Author { Name = "Test", Bio = "Test" };
		context.Authors.Add(author);
		context.SaveChanges(true);

		Author getAuthor = await repo.GetAsync(author.Id);

		getAuthor.Should().NotBeNull();
		getAuthor.Should().Be(author);
	}

	[Fact]
	public async Task GetAsync_IdOfNotExistingAuthor_ThrowsArgumentException()
	{
		AuthorRepository repo = new(context, new SearchService<Author>());
		Author author = new Author { Name = "Test", Bio = "Test" };
		context.Authors.Add(author);
		context.SaveChanges(true);

		Func<Task> action = async () => await repo.GetAsync(Guid.NewGuid());

		action.Should().ThrowAsync<ArgumentException>();
	}

	#endregion

	#region GetAllAsync
	[Fact]
	public async Task GetAllAsync_2AuthorsInDb_ReturnsCollectionOfThem()
	{
		AuthorRepository repo = new(context, null);
		Author author1 = new Author { Name = "Test1", Bio = "Test1" };
		Author author2 = new Author { Name = "Test2", Bio = "Test2" };
		context.Authors.Add(author1);
		context.Authors.Add(author2);
		context.SaveChanges();

		ICollection<Author> authors = await repo.GetAllAsync();

		authors.Should().NotBeNullOrEmpty();
		authors.Should().HaveCount(2);
		authors.Should().Contain(author1);
		authors.Should().Contain(author2);
	}

	[Fact]
	public async Task GetAllAsync_NoAuthorsInDb_ReturnsEmptyCollection()
	{
		AuthorRepository repo = new(context, null);
		context.SaveChanges();

		ICollection<Author> authors = await repo.GetAllAsync();

		authors.Should().BeEmpty();
	}
	#endregion

	#region GetAsync(string)

	[Fact]
	public async Task GetAsync__ReturnsCollectionOfAuthors()
	{

		Mock<ISearchService<Author>> search = new();
		AuthorRepository repo = new(context, search.Object);

		ICollection<Author> authors = [
			new Author { Name = "Test1", Bio = "Test1" },
			new Author { Name = "Test2", Bio = "Test2" }
			];

		search.Setup(s => s.FindAsync(It.IsAny<string>())).ReturnsAsync(authors);

		ICollection<Author> result = await repo.GetAsync("query");
		result.Should().NotBeNull();
		result.Should().HaveCount(2);
		result.Should().BeSameAs(authors);
	}
	#endregion

	#region GetByBookIdAsync
	[Fact]
	public async Task GetByBookIdAsync_3Authors2WithBookId_Returns2AuthorsByBookId()
	{
		AuthorRepository repo = new(context, new SearchService<Author>());
		Book book1 = new Book { Title = "Book1", Description = "Book1", Document = null };
		Book book2 = new Book { Title = "Book2", Description = "Book2", Document = null };
		Book book3 = new Book { Title = "Book3", Description = "Book3", Document = null };
		Author author1 = new Author { Name = "Test1", Bio = "Test1" };
		Author author2 = new Author { Name = "Test2", Bio = "Test2" };
		Author author3 = new Author { Name = "Test3", Bio = "Test3" };
		BookAuthor book1Author1 = new BookAuthor { Book = book1, Author = author1, BookId = book1.Id, AuthorId = author1.Id };
		BookAuthor book2Author3 = new BookAuthor { Book = book2, Author = author3, BookId = book2.Id, AuthorId = author3.Id };
		BookAuthor book3Author1 = new BookAuthor { Book = book3, Author = author1, BookId = book3.Id, AuthorId = author1.Id };
		BookAuthor book3Author2 = new BookAuthor { Book = book3, Author = author2, BookId = book3.Id, AuthorId = author2.Id };


		context.AddRange([book1, book2, book3]);
		context.AddRange([author1, author2, author3]);
		context.AddRange([book1Author1, book2Author3, book3Author1, book3Author2]);

		context.SaveChanges();

		ICollection<Author> authors = await repo.GetByBookIdAsync(book3.Id);

		authors.Should().HaveCount(2);
		authors.Should().Contain(author1);
		authors.Should().Contain(author2);

	}

	[Fact]
	public async Task GetByBookIdAsync_3Authors0WithBookId_ReturnsEmptyCollection()
	{
		AuthorRepository repo = new(context, new SearchService<Author>());
		Book book1 = new Book { Title = "Book1", Description = "Book1", Document = null };
		Book book2 = new Book { Title = "Book2", Description = "Book2", Document = null };
		Author author1 = new Author { Name = "Test1", Bio = "Test1" };
		BookAuthor book1Tag1 = new BookAuthor { Book = book1, Author = author1, BookId = book1.Id, AuthorId = author1.Id };


		context.Add(book1);
		context.Add(book2);
		context.Add(author1);
		context.Add(book1Tag1);

		context.SaveChanges();

		ICollection<Author> authors = await repo.GetByBookIdAsync(book2.Id);

		authors.Should().NotBeNull();
		authors.Should().BeEmpty();

	}

	[Fact]
	public async Task GetByBookIdAsync_IdOfNotExistingBook_ReturnsEmptyCollection()
	{

		AuthorRepository repo = new(context, new SearchService<Author>());
		Book book1 = new Book { Title = "Book1", Description = "Book1", Document = null };
		Author author1 = new Author { Name = "Test1", Bio = "Test1" };
		BookAuthor book1Tag1 = new BookAuthor { Book = book1, Author = author1, BookId = book1.Id, AuthorId = author1.Id };

		context.Add(book1);
		context.Add(author1);
		context.Add(book1Tag1);

		context.SaveChanges();

		ICollection<Author> authors = await repo.GetByBookIdAsync(Guid.NewGuid());

		authors.Should().NotBeNull();
		authors.Should().BeEmpty();

	}
	#endregion

	#region CreateAsync
	[Fact]
	public async Task CreateAsync_CorrectAuthor_AddsAuthorToDb()
	{
		AuthorRepository repo = new(context, null);
		Author author = new Author { Name = "Test", Bio = "Test" };

		await repo.CreateAsync(author);

		List<Author> authors = context.Authors.ToList();
		authors.Should().NotBeNullOrEmpty();
		authors.Should().HaveCount(1);
		authors.Should().Contain(author);
	}
	#endregion

	#region UpdateAsync
	[Fact]
	public async Task UpdateAsync_CorrectAuthor_UpdatesAuthorInDb()
	{
		AuthorRepository repo = new(context, new SearchService<Author>());
		Author author = new Author { Name = "Test", Bio = "Test" };
		context.Authors.Add(author);
		context.SaveChanges();

		Author updatedAuthor = context.Authors.First(a => a.Id == author.Id);
		updatedAuthor.Name = "Updated Author";

		await repo.UpdateAsync(updatedAuthor);

		List<Author> authors = context.Authors.ToList();

		authors.Should().NotBeNullOrEmpty();
		authors.Should().HaveCount(1);
		authors.Should().Contain(updatedAuthor);
	}
	#endregion

	#region DeleteAsync
	[Fact]
	public async Task DeleteAsync_CorrectAuthorId_DeletesAuthorFromDb()
	{
		AuthorRepository repo = new(context, new SearchService<Author>());

		Author author = new Author { Name = "Test", Bio = "Test" };
		context.Authors.Add(author);
		context.SaveChanges();

		await repo.DeleteAsync(author.Id);

		List<Author> authors = context.Authors.ToList();
		authors.Should().BeNullOrEmpty();
		authors.Should().NotContain(author);
	}
	[Fact]
	public async Task DeleteAsync_IncorrectAuthorId_ThrowsArgumentException()
	{
		AuthorRepository repo = new(context, new SearchService<Author>());

		Author author = new Author { Name = "Test", Bio = "Test" };
		context.Authors.Add(author);
		context.SaveChanges();

		Guid newGuid = Guid.NewGuid();
		Func<Task> action = async () => await repo.DeleteAsync(newGuid);

		action.Should().ThrowAsync<ArgumentException>();
		List<Author> authors = context.Authors.ToList();
		authors.Should().NotBeNullOrEmpty();
		authors.Should().HaveCount(1);
		authors.Should().Contain(author);
	}
	[Fact]
	public async Task DeleteAsync_BookAuthorWithSuchAuthorIdExist_ThrowsInvalidOperationException()
	{
		AuthorRepository repo = new(context, new SearchService<Author>());

		Author author = new Author { Name = "Test", Bio = "Test" };
		BookAuthor bookAuthor = new BookAuthor { Book = null, Author = author, BookId = Guid.NewGuid(), AuthorId = author.Id };
		context.BookAuthors.Add(bookAuthor);
		context.Authors.Add(author);
		context.SaveChanges();


		Func<Task> action = async () => await repo.DeleteAsync(author.Id);

		action.Should().ThrowAsync<InvalidOperationException>();
		List<Author> authors = context.Authors.ToList();
		authors.Should().NotBeNullOrEmpty();
		authors.Should().HaveCount(1);
		authors.Should().Contain(author);
	}
	#endregion
}
