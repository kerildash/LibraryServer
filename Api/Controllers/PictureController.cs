using Api.Services.Interfaces;
using AutoMapper;
using Database.RepositoryInterfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]s")]
[ApiController]
public class PictureController(IStaticFileRepository<Picture> repository,
							   IMapper mapper,
							   IWebHostEnvironment environment,
							   IStaticFileService<Picture> service)
							   : StaticFileController<Picture> (repository, mapper, environment, service)
{ }
