using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : BaseAPIController
    {
        private readonly DataContext context;
        public UsersController(DataContext context)
        {
            this.context = context;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task< ActionResult<IEnumerable<AppUser>>>GetUsers(){
            var result= await context.Users.ToListAsync();
            return await context.Users.ToListAsync();
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task< ActionResult<AppUser>>GetUsers(int id){
            
            return await context.Users.FindAsync(id);
        }
    }
}