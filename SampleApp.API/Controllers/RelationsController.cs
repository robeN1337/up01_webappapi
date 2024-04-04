using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleApp.Domen.Models;

namespace SampleApp.API.Controllers
{
    [Route("api/relations/")]
    [ApiController]
    public class RelationsController : ControllerBase
    {
        private readonly SampleAppContext _context;

        public RelationsController(SampleAppContext context)
        {
            _context = context;
        }


        [HttpGet("Followeds/{id}")]
        public async Task<ActionResult<IEnumerable<User>>> GetFollowedsById(int id)
        {
            if (_context.Relations == null)
            {
                return NotFound();
            }
            var relations = await _context.Relations.Where(r => r.FollowerId == id).ToListAsync();

            if (relations == null)
            {
                return NotFound();
            }
            User user = null;
            List<User> followeds = new List<User>();

            foreach (var relation in relations)
            {
                user = await _context.Users.FindAsync(relation.FollowedId);
                user.RelationFolloweds = null;
                user.RelationFollowers = null;
                followeds.Add(user);
            }
            //var content = new StringContent(JsonConvert.SerializeObject(followers), Encoding.UTF8, "application/json"); 
            return followeds;
        }


        [HttpGet("Followers/{id}")]
        public async Task<ActionResult<IEnumerable<User>>> GetFollowersById(int id)
        {
            if (_context.Relations == null)
            {
                return NotFound();
            }

            var relations = await _context.Relations.Where(r => r.FollowedId == id).ToListAsync();

            if (relations == null)
            {
                return NotFound();
            }
            User user = null;
            List<User> followers = new List<User>();

            foreach (var relation in relations)
            {
                user = await _context.Users.FindAsync(relation.FollowerId);
                user.RelationFolloweds = null;
                user.RelationFollowers = null;
                followers.Add(user);
            }
            //var content = new StringContent(JsonConvert.SerializeObject(followers), Encoding.UTF8, "application/json"); 
            return followers;
        }

        // PUT: api/Relations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRelation(int id, Relation relation)
        {
            if (id != relation.Id)
            {
                return BadRequest();
            }

            _context.Entry(relation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RelationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Relations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Relation>> PostRelation(Relation relation)
        {
          if (_context.Relations == null)
          {
              return Problem("Entity set 'SampleAppContext.Relations'  is null.");
          }
            _context.Relations.Add(relation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRelation", new { id = relation.Id }, relation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRelation(int id)
        {
            if (_context.Relations == null)
            {
                return NotFound();
            }
            var relation = await _context.Relations.FindAsync(id);
            if (relation == null)
            {
                return NotFound();
            }

            _context.Relations.Remove(relation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Relations/id?FollowerId=A&FollowedId=B
        [HttpDelete("id")]
        public async Task<IActionResult> DeleteRelationById(int FollowerId, int FollowedId)
        {
            if (_context.Relations == null)
            {
                return NotFound();
            }
            var relation = await _context.Relations.Where(r => r.FollowerId == FollowerId && r.FollowedId == FollowedId).FirstOrDefaultAsync();
            if (relation == null)
            {
                return NotFound();
            }

            _context.Relations.Remove(relation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RelationExists(int id)
        {
            return (_context.Relations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
