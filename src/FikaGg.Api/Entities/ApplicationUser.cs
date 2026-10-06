using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Entities;

public class ApplicationUser : IdentityUser
{
    [MaxLength(50)] public string DisplayName { get; set; } = "";
    [MaxLength(500)] public string? AvatarUrl { get; set; }
}
