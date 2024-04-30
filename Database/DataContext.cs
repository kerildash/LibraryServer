using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database;

public class DataContext : DbContext
{
	public DataContext(DbContextOptions<DataContext> options) : base(options)
	{

	public DbSet<Picture> Pictures { get; set; }
	public DbSet<Document> Documents { get; set; }
	public DbSet<Book> Books { get; set; }
	public DbSet<Author> Authors { get; set; }
	//public DbSet<Tag> Tags { get; set; }
	public DbSet<BookAuthor> BookAuthors { get; set; }
	//public DbSet<BookTag> BookTags { get; set; }
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<BookAuthor>()
			.HasKey(ba => new { ba.BookId, ba.AuthorId });

		modelBuilder.Entity<BookAuthor>()
			.HasOne(b => b.Book)
			.WithMany(ba => ba.BookAuthors)
			.HasForeignKey(b => b.BookId);

		modelBuilder.Entity<BookAuthor>()
			.HasOne(a => a.Author)
			.WithMany(ba => ba.BookAuthors)
			.HasForeignKey(a => a.AuthorId);
	}
}
