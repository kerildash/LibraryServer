using Database;
using Database.Repositories;
using Database.Services;
using Domain.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Database;

public class PictureRepositoryTests
{
	private readonly DataContext context;

	public PictureRepositoryTests()
	{
		DbContextOptionsBuilder<DataContext> options = 
			new DbContextOptionsBuilder<DataContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString());

		context = new(options.Options);
	}

	#region ExistsAsync
	[Fact]
	public async Task ExistsAsync_ExistingPictureInDb_ReturnsTrue()
	{
		PictureRepository repo = new(context, null);
		Picture picture = new Picture { Path = "Test" };
		context.Pictures.Add(picture);
		context.SaveChanges();

		bool result = await repo.ExistsAsync(picture.Id);

		result.Should().BeTrue();
	}

	[Fact]
	public async Task ExistsAsync_IdOfPictureWhichIsNotExist_ReturnsFalse()
	{
		PictureRepository repo = new(context, null);
		Picture picture = new Picture { Path = "Test" };
		context.Pictures.Add(picture);
		context.SaveChanges();

		bool result = await repo.ExistsAsync(Guid.NewGuid());

		result.Should().BeFalse();
	}
	#endregion

	#region GetAsync(Guid id)
	[Fact]
	public async Task GetAsync_IdOfExistingPicture_ReturnsPicture()
	{
		PictureRepository repo = new(context, null);
		Picture picture = new Picture { Path = "Test" };
		context.Pictures.Add(picture);
		context.SaveChanges(true);

		Picture getPicture = await repo.GetAsync(picture.Id);

		getPicture.Should().NotBeNull();
		getPicture.Should().Be(picture);
	}

	[Fact]
	public async Task GetAsync_IdOfNotExistingPicture_ThrowsArgumentException()
	{
		PictureRepository repo = new(context, null);
		Picture picture = new Picture { Path = "Test" };
		context.Pictures.Add(picture);
		context.SaveChanges(true);

		Func<Task> action = async () => await repo.GetAsync(Guid.NewGuid());

		action.Should().ThrowAsync<ArgumentException>();
	}

	#endregion

	#region GetAsync(string)
	[Fact]
	public async Task GetAsync__ReturnsCollection()
	{
		ICollection<Picture> list = [
			new Picture { Path = "Test" },
			new Picture { Path = "Test2" }
			];
		Mock<ISearchService<Picture>> search = new();
		search.Setup(s => s.FindAsync(It.IsAny<string>())).ReturnsAsync(list);
		PictureRepository repo = new(context, search.Object);

		ICollection<Picture> result = await repo.GetAsync("string", pageNumber: 0, pageSize: 10);

		result.Should().NotBeNull();
		result.Should().HaveCount(2);
		result.Should().BeSameAs(list);
	}
	#endregion

	#region GetAllAsync
	[Fact]
	public async Task GetAllAsync_2PicturesInDb_ReturnsCollectionOfThem()
	{
		PictureRepository repo = new(context, null);
		Picture picture1 = new Picture { Path = "Test1"};
		Picture picture2 = new Picture { Path = "Test2"};
		context.Pictures.AddRange([picture1, picture2]);
		context.SaveChanges();

		ICollection<Picture> pictures = await repo.GetAllAsync(pageNumber: 0, pageSize: 10);

		pictures.Should().NotBeNullOrEmpty();
		pictures.Should().HaveCount(2);
		pictures.Should().Contain(picture1);
		pictures.Should().Contain(picture2);
	}

	[Fact]
	public async Task GetAllAsync_NoPicturesInDb_ReturnsEmptyCollection()
	{
		PictureRepository repo = new(context, null);
		context.SaveChanges();

		ICollection<Picture> pictures = await repo.GetAllAsync(pageNumber: 0, pageSize: 10);

		pictures.Should().BeEmpty();
	}
	#endregion


	#region GetByHolderIdAsync
	[Fact]
	public async Task GetByHolderIdAsync_IdOfExistingAuthorWithRelatedPicture_ReturnThisPicture()
	{
		PictureRepository repo = new(context, null);
		Author author = new Author { Bio = "Test", Name = "Test", Id = Guid.NewGuid() };
		Picture picture = new Picture { Path = "Test", HolderId = author.Id };
		Picture picture2 = new Picture { Path = "Test2", HolderId = Guid.NewGuid() };
		context.Add(author);
		context.Add(picture);
		context.Add(picture2);
		context.SaveChanges();

		ICollection<Picture> result = await repo.GetByHolderIdAsync(author.Id);
		result.Should().NotBeNullOrEmpty();
		result.Count().Should().Be(1);
		result.Should().Contain(picture);
	}
	[Fact]
	public async Task GetByHolderIdAsync_IdOfExistingAuthorWithoutRelatedPicture_ReturnsEmptyCollection()
	{
		PictureRepository repo = new(context, null);
		Author author = new Author { Bio = "Test", Name = "Test" };
		Picture picture = new Picture { Path = "Test", HolderId = Guid.NewGuid() };
		context.Add(author);
		context.Add(picture);
		context.SaveChanges();

		ICollection<Picture> result = await repo.GetByHolderIdAsync(author.Id);
		result.Should().NotBeNull();
		result.Should().BeEmpty();
	}
	[Fact]
	public async Task GetByHolderIdAsync_IdOfNotExistingHolder_ReturnsEmptyCollection()
	{
		PictureRepository repo = new(context, null);
		Author author = new Author { Bio = "Test", Name = "Test" };
		Picture picture = new Picture { Path = "Test", HolderId = author.Id };
		Picture picture2 = new Picture { Path = "Test2", HolderId = Guid.NewGuid() };
		context.Add(author);
		context.AddRange([picture, picture2]);
		context.SaveChanges();

		ICollection<Picture> result = await repo.GetByHolderIdAsync(Guid.NewGuid());
		result.Should().NotBeNull();
		result.Should().BeEmpty();
	}
	#endregion

	#region CreateAsync
	[Fact]
	public async Task CreateAsync_CorrectPicture_AddsPictureToDb()
	{
		PictureRepository repo = new(context, null);
		Picture picture = new Picture { Path = "Test"};

		await repo.CreateAsync(picture);

		List<Picture> pictures = context.Pictures.ToList();
		pictures.Should().NotBeNullOrEmpty();
		pictures.Should().HaveCount(1);
		pictures.Should().Contain(picture);
	}
	#endregion

	#region DeleteAsync
	[Fact]
	public async Task DeleteAsync_PictureWithoutHolderId_DeletesPicture()
	{
		PictureRepository repo = new(context, null);
		Picture picture = new Picture { Path = "Test" };
		context.Add(picture);
		context.SaveChanges();

		await repo.DeleteAsync(picture.Id);

		ICollection<Picture> result = context.Pictures.ToList();
		result.Should().NotBeNull();
		result.Should().BeEmpty();
	}
	[Fact]
	public async Task DeleteAsync_PictureWithHolderIdOfExistingHolder_ThrowsInvalidOperationException()
	{
		PictureRepository repo = new(context, null);
		Author author = new Author { Bio = "Test", Name = "Test" };
		Picture picture = new Picture { Path = "Test", HolderId = author.Id };
		context.Add(author);
		context.Add(picture);
		context.SaveChanges();

		Func<Task> action = async () => await repo.DeleteAsync(picture.Id);

		action.Should().ThrowAsync<InvalidOperationException>();
	}
	[Fact]
	public async Task DeleteAsync_PictureWithHolderIdOfNonExistingHolder_DeletesPicture()
	{
		PictureRepository repo = new(context, null);
		Picture picture = new Picture { Path = "Test", HolderId = Guid.NewGuid() };
		context.Add(picture);
		context.SaveChanges();

		await repo.DeleteAsync(picture.Id);

		ICollection<Picture> result = context.Pictures.ToList();
		result.Should().NotBeNull();
		result.Should().BeEmpty();
	}
	#endregion

	#region UpdateAsync
	[Fact]
	public async Task UpdateAsync_CorrectPicture_UpdatesPictureInDb()
	{
		PictureRepository repo = new(context, null);
		Picture picture = new Picture { Path = "Test" };
		context.Add(picture);
		context.SaveChanges();

		Picture updatedPicture = context.Pictures.Where(p =>  p.Id == picture.Id).FirstOrDefault();
		updatedPicture.Path = "New";
		await repo.UpdateAsync(updatedPicture);

		List<Picture> pictures = context.Pictures.ToList();
		pictures.Should().NotBeNullOrEmpty();
		pictures.Should().HaveCount(1);
		pictures.Should().Contain(updatedPicture);
	}
	#endregion
}
