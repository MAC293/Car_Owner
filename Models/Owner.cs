using System;
using System.Collections.Generic;

namespace RESTfulAPI.Models;

public partial class Owner
{
    public string Id { get; set; } = null!;

    public string? Fullname { get; set; }

    public int? Age { get; set; }

    public virtual Member? Member { get; set; }

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
