using Database;
using Database.Repositories;
using Database.Services;
using Domain.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Database;

public class TagRepositoryTests
{
	private readonly DataContext context;

	public TagRepositoryTests()
	{
		DbContextOptionsBuilder<DataContext> options =
			new DbContextOptionsBuilder<DataContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString());

		context = new DataContext(options.Options);
	}

	#region ExistAsync

	[Fact]
	public async Task ExistAsync_IdOfAnExistingTag_ReturnsTrue()
	{
		TagRepository repo = new(context, new SearchService<Tag>());
		Tag tag = new Tag { Name = "newTag" };
		context.Tags.Add(tag);
		context.SaveChanges();

		bool isExist = await repo.ExistsAsync(tag.Id);

		isExist.Should().BeTrue();
	}

	[Fact]
	public async Task ExistAsync_IdOfNotExistingTag_ReturnsFalse()
	{
		TagRepository repo = new(context, new SearchService<Tag>());
		Tag tag = new Tag { Name = "newTag" };
		context.Tags.Add(tag);
		context.SaveChanges();

		bool isExist = await repo.ExistsAsync(Guid.NewGuid());

		isExist.Should().BeFalse();
	}

	#endregion

	#region GetAsync(Guid id)

	[Fact]
	public async Task GetAsync_IdOfExistingTag_ReturnsTag()
	{
		TagRepository repo = new(context, new SearchService<Tag>());
		Tag tag = new Tag { Name = "Test" };
		context.Tags.Add(tag);
		context.SaveChanges(true);

		Tag getTag = await repo.GetAsync(tag.Id);

		getTag.Should().NotBeNull();
		getTag.Should().Be(tag);
	}

	[Fact]
	public async Task GetAsync_IdOfNotExistingTag_ThrowsArgumentException()
	{
		TagRepository repo = new(context, new SearchService<Tag>());
		Tag tag = new Tag { Name = "Test" };
		context.Tags.Add(tag);
		context.SaveChanges(true);

		Func<Task> action = async () => await repo.GetAsync(Guid.NewGuid());

		action.Should().ThrowAsync<ArgumentException>();
	}
	#endregion

	#region GetAllAsync()
	[Fact]
	public async Task GetAllAsync_ThreeTagsInADb_ReturnsTheseTags()
	{
		TagRepository repo = new(context, new SearchService<Tag>());
		Tag tag1 = new Tag { Name = "Test1" };
		Tag tag2 = new Tag { Name = "Test2" };
		Tag tag3 = new Tag { Name = "Test3" };
		context.Tags.Add(tag1);
		context.Tags.Add(tag2);
		context.Tags.Add(tag3);
		context.SaveChanges(true);

		ICollection<Tag> tags = await repo.GetAllAsync(pageNumber: 0, pageSize: 10);

		tags.Should().NotBeNullOrEmpty();
		tags.Should().HaveCount(3);
		tags.Should().Contain(tag1);
		tags.Should().Contain(tag2);
		tags.Should().Contain(tag3);
	}

	[Fact]
	public async Task GetAllAsync_NoTagsInADb_ReturnsEmptyCollection()
	{
		TagRepository repo = new(context, new SearchService<Tag>());

		ICollection<Tag> tags = await repo.GetAllAsync(pageNumber: 0, pageSize: 10);

		tags.Should().NotBeNull();
		tags.Should().BeEmpty();
	}

	#endregion

	#region GetAsync(string)
	[Fact]
	public async Task GetAsync__ReturnsCollectionOfTags()
	{
		Mock<ISearchService<Tag>> search = new();
		ICollection<Tag> tags = [
			new Tag { Name = "Test" },
			new Tag { Name = "Test2" }
			];
		search.Setup(s => s.FindAsync(It.IsAny<string>())).ReturnsAsync(tags);

		TagRepository repo = new(context, search.Object);

		ICollection<Tag> result = await repo.GetAsync("string", pageNumber: 0, pageSize: 10);
		result.Should().NotBeNull();
		result.Should().HaveCount(2);
		result.Should().BeSameAs(tags);
	}

	#endregion

	#region GetByBookIdAsync
	[Fact]
	public async Task GetByBookIdAsync_3Tags2WithBookId_Returns2TagsByBookId()
	{
		TagRepository repo = new(context, new SearchService<Tag>());
		Book book1 = new Book { Title = "Book1", Description = "Book1", Document = null };
		Book book2 = new Book { Title = "Book2", Description = "Book2", Document = null };
		Book book3 = new Book { Title = "Book3", Description = "Book3", Document = null };
		Tag tag1 = new Tag { Name = "Tag1" };
		Tag tag2 = new Tag { Name = "Tag2" };
		Tag tag3 = new Tag { Name = "Tag3" };
		BookTag book1Tag1 = new BookTag { Book = book1, Tag = tag1, BookId = book1.Id, TagId = tag1.Id };
		BookTag book2Tag3 = new BookTag { Book = book2, Tag = tag3, BookId = book2.Id, TagId = tag3.Id };
		BookTag book3Tag1 = new BookTag { Book = book3, Tag = tag1, BookId = book3.Id, TagId = tag1.Id };
		BookTag book3Tag2 = new BookTag { Book = book3, Tag = tag2, BookId = book3.Id, TagId = tag2.Id };


		context.Add(book1);
		context.Add(book2);
		context.Add(book3);
		context.Add(tag1);
		context.Add(tag2);
		context.Add(tag3);
		context.Add(book1Tag1);
		context.Add(book2Tag3);
		context.Add(book3Tag1);
		context.Add(book3Tag2);

		context.SaveChanges();

		ICollection<Tag> tags = await repo.GetByBookIdAsync(book3.Id);

		tags.Should().HaveCount(2);
		tags.Should().Contain(tag1);
		tags.Should().Contain(tag2);

	}
	[Fact]
	public async Task GetByBookIdAsync_3Tags0WithBookId_ReturnsEmptyCollection()
	{
		TagRepository repo = new(context, new SearchService<Tag>());
		Book book1 = new Book { Title = "Book1", Description = "Book1", Document = null };
		Book book2 = new Book { Title = "Book2", Description = "Book2", Document = null };
		Tag tag1 = new Tag { Name = "Tag1" };
		BookTag book1Tag1 = new BookTag { Book = book1, Tag = tag1, BookId = book1.Id, TagId = tag1.Id };


		context.Add(book1);
		context.Add(book2);
		context.Add(tag1);
		context.Add(book1Tag1);

		context.SaveChanges();

		ICollection<Tag> tags = await repo.GetByBookIdAsync(book2.Id);

		tags.Should().NotBeNull();
		tags.Should().BeEmpty();

	}

	[Fact]
	public async Task GetByBookIdAsync_IdOfNotExistingBook_ReturnsEmptyCollection()
	{
		TagRepository repo = new(context, new SearchService<Tag>());
		Book book1 = new Book { Title = "Book1", Description = "Book1", Document = null };
		Book book2 = new Book { Title = "Book2", Description = "Book2", Document = null };
		Tag tag1 = new Tag { Name = "Tag1" };
		BookTag book1Tag1 = new BookTag { Book = book1, Tag = tag1, BookId = book1.Id, TagId = tag1.Id };


		context.Add(book1);
		context.Add(tag1);
		context.Add(book1Tag1);

		context.SaveChanges();

		ICollection<Tag> tags = await repo.GetByBookIdAsync(book2.Id);

		tags.Should().NotBeNull();
		tags.Should().BeEmpty();

	}

	#endregion

	#region CreateAsync
	[Fact]
	public async Task CreateAsync_CorrectTag_AddsTagToDb()
	{
		TagRepository repo = new(context, new SearchService<Tag>());
		Tag tag = new Tag { Name = "newTag" };

		await repo.CreateAsync(tag);

		List<Tag> tags = context.Tags.ToList();
		tags.Should().NotBeNullOrEmpty();
		tags.Should().HaveCount(1);
		tags.Should().Contain(tag);
	}
	#endregion

	#region UpdateAsync
	[Fact]
	public async Task UpdateAsync_CorrectTag_UpdatesTagInDb()
	{
		TagRepository repo = new(context, new SearchService<Tag>());
		Tag tag = new Tag { Name = "Tag" };
		context.Tags.Add(tag);
		context.SaveChanges();

		Tag updatedTag = context.Tags.First(t => t.Id == tag.Id);
		updatedTag.Name = "Updated Tag";

		await repo.UpdateAsync(updatedTag);

		List<Tag> tags = context.Tags.ToList();

		tags.Should().NotBeNullOrEmpty();
		tags.Should().HaveCount(1);
		tags.Should().Contain(updatedTag);
	}
	#endregion

	#region DeleteAsync
	[Fact]
	public async Task DeleteAsync_CorrectTagId_DeletesTagFromDb()
	{
		TagRepository repo = new(context, new SearchService<Tag>());

		Tag tag = new Tag { Name = "newTag" };
		context.Tags.Add(tag);
		context.SaveChanges();

		await repo.DeleteAsync(tag.Id);

		List<Tag> tags = context.Tags.ToList();
		tags.Should().BeNullOrEmpty();
		tags.Should().NotContain(tag);
	}
	[Fact]
	public async Task DeleteAsync_IncorrectTagId_ThrowsArgumentException()
	{
		TagRepository repo = new(context, new SearchService<Tag>());

		Tag tag = new Tag { Name = "newTag" };
		context.Tags.Add(tag);
		context.SaveChanges();

		Guid newGuid = Guid.NewGuid();
		Func<Task> action = async () => await repo.DeleteAsync(newGuid);

		action.Should().ThrowAsync<ArgumentException>()
			.WithMessage($"Tag ID: \"{newGuid}\" does not exist");
		List<Tag> tags = context.Tags.ToList();
		tags.Should().NotBeNullOrEmpty();
		tags.Should().HaveCount(1);
		tags.Should().Contain(tag);
	}
	[Fact]
	public async Task DeleteAsync_BookTagWithSuchTagIdExist_ThrowsInvalidOperationException()
	{
		TagRepository repo = new(context, new SearchService<Tag>());

		Tag tag = new Tag { Name = "newTag" };
		BookTag bookTag = new BookTag { Book = null, Tag = tag, BookId = Guid.NewGuid(), TagId = tag.Id };
		context.BookTags.Add(bookTag);
		context.Tags.Add(tag);
		context.SaveChanges();


		Func<Task> action = async () => await repo.DeleteAsync(tag.Id);

		action.Should().ThrowAsync<InvalidOperationException>();
		List<Tag> tags = context.Tags.ToList();
		tags.Should().NotBeNullOrEmpty();
		tags.Should().HaveCount(1);
		tags.Should().Contain(tag);
	}
	#endregion
}
