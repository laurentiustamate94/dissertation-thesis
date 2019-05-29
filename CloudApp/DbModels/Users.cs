using System;
using System.Collections.Generic;

namespace CloudApp.DbModels
{
    public partial class Users
    {
        public string Id { get; set; }
        public string User { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }
}
