using System.Security.Claims;

namespace FilmShelf.BAL.Helpers;

public static class UserClaimsHelper
{
    public static int GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null)
        {
            throw new UnauthorizedAccessException("User ID not found in claims.");
        }

        return int.Parse(userIdClaim);
    }
}
