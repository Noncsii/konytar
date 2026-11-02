using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KonyvtarBackend.Data;
using KonyvtarBackend.Models;

namespace KonyvtarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OlvasoController : ControllerBase
    {
        private readonly KonyvtarContext _context;

        public OlvasoController(KonyvtarContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Olvaso>>> GetOlvasok()
        {
            return await _context.Olvasok.ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Olvaso>> GetOlvaso(int id)
        {
            var olvaso = await _context.Olvasok.FindAsync(id);

            if (olvaso == null)
            {
                return NotFound(); 
            }

            return olvaso;
        }

        
        public async Task<ActionResult<IEnumerable<Konyv>>> GetOlvasoKonyvei(int id)
        {
            var olvasoExists = await _context.Olvasok.AnyAsync(o => o.Id == id);
            if (!olvasoExists)
            {
                return NotFound(); 
            }

            var konyvek = await _context.Konyvek.Where(k => k.OlvasoId == id).ToListAsync();
            return konyvek;
        }

        
        [HttpGet("kolcsonzok")]
        public async Task<ActionResult<IEnumerable<Olvaso>>> GetKolcsonzoOlvasok()
        {
            
            var kolcsonzottOlvasoIdk = await _context.Konyvek
                .Where(k => k.OlvasoId != null)
                .Select(k => k.OlvasoId)
                .Distinct()
                .ToListAsync();

            
            var olvasok = await _context.Olvasok
                .Where(o => kolcsonzottOlvasoIdk.Contains(o.Id))
                .ToListAsync();

            return olvasok;
        }

        
        [HttpPost]
        public async Task<ActionResult<Olvaso>> PostOlvaso(Olvaso olvaso)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Olvasok.Add(olvaso);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOlvaso), new { id = olvaso.Id }, olvaso);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOlvaso(int id, Olvaso olvaso)
        {
            if (id != olvaso.Id)
            {
                return BadRequest(); 
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(olvaso).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Olvasok.AnyAsync(o => o.Id == id))
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

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOlvaso(int id)
        {
            var olvaso = await _context.Olvasok.FindAsync(id);
            if (olvaso == null)
            {
                return NotFound(); 
            }

            
            var konyvek = await _context.Konyvek.Where(k => k.OlvasoId == id).ToListAsync();
            foreach (var konyv in konyvek)
            {
                konyv.OlvasoId = null;
            }

            _context.Olvasok.Remove(olvaso);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
