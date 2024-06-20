
using Database.RepositoryInterfaces;
using Database.Repositories;
using Database;
using Microsoft.EntityFrameworkCore;
using Shared.Helper;
using Microsoft.AspNetCore.Identity;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Api.Services;
using Microsoft.OpenApi.Models;
using Api.Services.Interfaces;

namespace Api;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		AddRepositories(builder);
		AddServices(builder);

		builder.Services.AddControllers();
		builder.Services.AddIdentity<AppUser, IdentityRole>(
			options =>
			{
				options.User.AllowedUserNameCharacters =
					"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";
			})
			.AddEntityFrameworkStores<DataContext>();
		builder.Services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme =
		    options.DefaultChallengeScheme =
			options.DefaultForbidScheme =
		    options.DefaultScheme =
		    options.DefaultSignInScheme =
		    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
		})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					// validate the token based on the key we have provided 
					ValidateIssuerSigningKey = false,

					IssuerSigningKey =
						new SymmetricSecurityKey(
							Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY"))),

					// validate the issuer 
					ValidIssuer = builder.Configuration["JWT:Issuer"],
					ValidateIssuer = false,

					// validate the audience (angular side)
					ValidateAudience = false,
					ValidAudience = builder.Configuration["JWT:Audience"],

					ValidateLifetime = false
				};

			});

		builder.Services.AddTransient<Seed>();
		builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(MappingProfiles).Assembly);
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();
		builder.Services.AddSwaggerGen(option =>
		{
			option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
			option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				In = ParameterLocation.Header,
				Description = "Please enter a valid token",
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				BearerFormat = "JWT",
				Scheme = "Bearer"
			});
			option.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type=ReferenceType.SecurityScheme,
					Id="Bearer"
				}
			},
			new string[]{}
		}
	});
		});
		builder.Services.AddCors(options => options.AddPolicy(name: "Origins",
			policy =>
			{
				policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
			}));
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

		
		//.AddDefaultTokenProviders();
		//builder.Services.AddAuthorization();
		builder.Services.Configure<IdentityOptions>(options =>
		{
			options.Password.RequireDigit = true;
			options.Password.RequiredLength = 8;
			options.Password.RequireLowercase = true;
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequireUppercase = true;
		});

		builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

		var app = builder.Build();

		#region seeding the data
		if (args.Length == 1 && args[0].ToLower() == "seeddata")
			SeedData(app);
		#endregion


		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}
		 
		app.UseCors("Origins");
		app.UseHttpsRedirection();

		app.UseAuthentication();
		app.UseAuthorization();
		app.UseStaticFiles();


		app.MapControllers();

		app.Run();
	}

	private static void AddRepositories(WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<IBookRepository, BookRepository>();
		builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
		builder.Services.AddScoped<IStaticFileRepository<Document>, DocumentRepository>();
		builder.Services.AddScoped<IStaticFileRepository<Picture>, PictureRepository>();
		builder.Services.AddScoped<ITagRepository, TagRepository>();
	}
	private static void AddServices(WebApplicationBuilder builder)
	{
		builder.Services.AddScoped<ITokenService, TokenService>();
		builder.Services.AddScoped<IStaticFileService<Document>, StaticFileService<Document>>();
		builder.Services.AddScoped<IStaticFileService<Picture>, StaticFileService<Picture>>();
	}


	private static void SeedData(IHost app)
	{
		var scopedFactory = app.Services.GetService<IServiceScopeFactory>() ?? throw new NullReferenceException();

		using (var scope = scopedFactory.CreateScope())
		{
			var service = scope.ServiceProvider.GetService<Seed>() ?? throw new NullReferenceException();
			service.SeedDataContext();
		}
	}
}
