using System;
using System.Collections.Generic;

namespace RESTfulAPI.Models;

public partial class Member
{
    public string Id { get; set; } = null!;

    public string? Username { get; set; }

    public string? Password { get; set; }

    public virtual Owner IdNavigation { get; set; } = null!;
}
