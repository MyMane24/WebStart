using System;
using System.Collections.Generic;

namespace WebStart.Data;

public partial class Userinfo
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Email { get; set; }

    public string Password { get; set; } = null!;
}
