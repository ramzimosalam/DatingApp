using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entites
{
    public class AppUser
    {
        public int Id { get; set; }
        public string? Name{get;set;}
        public string? UserName { get; set; }
        public byte[]? Password { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? StatusCode { get; set; }
    }
}