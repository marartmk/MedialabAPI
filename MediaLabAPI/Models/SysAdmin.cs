using System;
using System.Collections.Generic;

namespace MediaLabAPI.Models;

public partial class SysAdmin
{
    public Guid Id { get; set; }

    public Guid IdCompany { get; set; }

    public string Username { get; set; }

    public string PasswordHash { get; set; }

    public string Email { get; set; }

    public string FullName { get; set; }

    public bool? IsSuperAdmin { get; set; }

    public bool? IsEnabled { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? LastLogin { get; set; }
}
