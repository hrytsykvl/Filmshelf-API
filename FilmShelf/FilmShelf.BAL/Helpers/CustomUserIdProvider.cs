using Mailjet.Client.Resources;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FilmShelf.BAL.Helpers;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
