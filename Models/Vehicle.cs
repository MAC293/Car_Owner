using System;
using System.Collections.Generic;

namespace RESTfulAPI.Models;

public partial class Vehicle
{
    public string Patent { get; set; } = null!;

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public string? Type { get; set; }

    public int? Year { get; set; }

    public string Driver { get; set; } = null!;

    public virtual Owner DriverNavigation { get; set; } = null!;
}
