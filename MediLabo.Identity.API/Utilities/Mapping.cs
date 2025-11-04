using MediLabo.Identity.API.Models;
using MediLabo.Identity.API.Models.DTOs;

namespace MediLabo.Identity.API.Utilities;

public static class Mapping
{
    public static UserDto ToDto(ApplicationUser user, IList<string> roles)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles,
            CreatedAt = user.CreatedAt
        };
    }
}
