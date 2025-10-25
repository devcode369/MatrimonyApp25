using System.Diagnostics.CodeAnalysis;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  
    //[Authorize]
    public class MembersController (AppDbContext appDbContext): BaseApiController
    {
        [HttpGet]
        public async  Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
        {
            var members = await appDbContext.Users.ToListAsync();
            return members;
        }

         [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetMember(string id)
        {
            var member = await appDbContext.Users.FindAsync(id);
            if (member == null) return NotFound();
            return member;
        }
    }
}
