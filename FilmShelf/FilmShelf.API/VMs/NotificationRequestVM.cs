using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.VMs;

public class NotificationRequestVM
{
    [FromRoute(Name = "id")]
    public int Id { get; set; }
}
