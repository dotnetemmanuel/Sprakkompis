﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprakkompis.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? Firstname { get; set; }
        public string? LastName { get; set; }
    }
}
