using System;
using System.Collections.Generic;

namespace Epam.EmailManager.Models;

public partial class UserDetail
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;
}
