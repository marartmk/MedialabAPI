using System;
using System.Collections.Generic;

namespace MediaLabAPI.Models;

public partial class SysUser
{
    public Guid Id { get; set; }

    public Guid? IdWhr { get; set; }

    public Guid IdCompany { get; set; }

    public string Username { get; set; }

    public string PasswordHash { get; set; }

    public string Email { get; set; }

    public bool IsAdmin { get; set; }

    public bool IsEnabled { get; set; }

    public string AccessLevel { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }
}
