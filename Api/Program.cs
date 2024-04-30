
using Database.RepositoryInterfaces;
using Database.Repositories;
using Database;
using Microsoft.EntityFrameworkCore;
using Shared.Helper;

namespace Api;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		AddRepositories(builder);
		

		builder.Services.AddControllers();
		builder.Services.AddTransient<Seed>();
		builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(MappingProfiles).Assembly);
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
		builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 500 * 1024 * 1024);

		#region AddDbContext
		builder.Services.AddDbContext<DataContext>(
			options =>
			options.UseSqlServer(
				builder.Configuration.GetConnectionString("DefaultConnection"),
				x => x.MigrationsAssembly(typeof(DataContext).Assembly.FullName)
				)
		);
		#endregion

		builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

		var app = builder.Build();

		# region seeding the data
		if (args.Length == 1 && args[0].ToLower() == "seeddata")
			SeedData(app);		
		#endregion


		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();

		app.UseAuthorization();
		app.UseStaticFiles();


		app.MapControllers();

		app.Run();
	}

	private static void AddRepositories(WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<IBookRepository, BookRepository>();
		builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
		builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
		builder.Services.AddScoped<IPictureRepository, PictureRepository>();
		builder.Services.AddScoped<ITagRepository, TagRepository>();
	}

	private static void SeedData(IHost app)
	{
		var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

		using (var scope = scopedFactory.CreateScope())
		{
			var service = scope.ServiceProvider.GetService<Seed>();
			service.SeedDataContext();
		}
	}
}
