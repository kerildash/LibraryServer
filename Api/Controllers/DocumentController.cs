using Api.Services.Interfaces;
using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class DocumentController(IStaticFileRepository<Document> repository,
								IMapper mapper,
								IWebHostEnvironment environment, 
								IStaticFileService<Document> service)
								: StaticFileController<Document>(repository, mapper, environment, service)
{ }
