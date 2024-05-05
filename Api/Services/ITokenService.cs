using Domain.Models;

namespace Api.Services;

public interface ITokenService
{
	string CreateToken(AppUser user, IList<string> roles);
}
