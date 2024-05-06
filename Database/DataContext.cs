using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database;

public class DataContext : IdentityDbContext<AppUser>
{
	public DataContext(DbContextOptions<DataContext> options) : base(options)
	{
	}

	public DbSet<Picture> Pictures { get; set; }
	public DbSet<Document> Documents { get; set; }
	public DbSet<Book> Books { get; set; }
	public DbSet<Author> Authors { get; set; }
	public DbSet<Tag> Tags { get; set; }
	public DbSet<BookAuthor> BookAuthors { get; set; }
	public DbSet<BookTag> BookTags { get; set; }
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<BookTag>()
			.HasKey(bt => new { bt.BookId, bt.TagId });

		modelBuilder.Entity<BookTag>()
			.HasOne(bt => bt.Book)
			.WithMany(b => b.BookTags)
			.HasForeignKey(bt => bt.BookId);

		modelBuilder.Entity<BookTag>()
			.HasOne(bt => bt.Tag)
			.WithMany(a => a.BookTags)
			.HasForeignKey(ba => ba.TagId);


		modelBuilder.Entity<BookAuthor>()
			.HasKey(ba => new { ba.BookId, ba.AuthorId });

		modelBuilder.Entity<BookAuthor>()
			.HasOne(ba => ba.Book)
			.WithMany(b => b.BookAuthors)
			.HasForeignKey(ba => ba.BookId);

		
		modelBuilder.Entity<BookAuthor>()
			.HasOne(ba => ba.Author)
			.WithMany(a => a.BookAuthors)
			.HasForeignKey(ba => ba.AuthorId);


		//modelBuilder.Entity<Book>()
		//    .HasOne(b => b.Cover)
		//    .WithOne()
		//	.HasForeignKey<Picture>(p => p.HolderId)
		//	.IsRequired(false);

		//modelBuilder.Entity<Book>()
		//	.HasOne(b => b.Document)
		//	.WithOne()
		//	.HasForeignKey<Document>(d => d.HolderId)
		//	.IsRequired(false);
	}
}
