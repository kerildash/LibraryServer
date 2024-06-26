﻿using Domain.Models;

namespace Api.Services.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user, IList<string> roles);
}
