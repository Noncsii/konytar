using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KonyvtarBackend.Data;
using KonyvtarBackend.Models;

namespace KonyvtarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KonyvController : ControllerBase
    {
        private readonly KonyvtarContext _context;

        public KonyvController(KonyvtarContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Konyv>>> GetKonyvek()
        {
            return await _context.Konyvek.ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Konyv>> GetKonyv(int id)
        {
            var konyv = await _context.Konyvek.FindAsync(id);

            if (konyv == null)
            {
                return NotFound(); 
            }

            return konyv;
        }

        
        [HttpGet("szabad")]
        public async Task<ActionResult<IEnumerable<Konyv>>> GetSzabadKonyvek()
        {
            return await _context.Konyvek.Where(k => k.OlvasoId == null).ToListAsync();
        }

        
        [HttpGet("kereses")]
        public async Task<ActionResult<IEnumerable<Konyv>>> Kereses([FromQuery] string szoveg)
        {
            if (string.IsNullOrEmpty(szoveg))
            {
                return await _context.Konyvek.ToListAsync();
            }

            var keresettSzoveg = szoveg.ToLower();

            return await _context.Konyvek
                .Where(k => k.Cim.ToLower().Contains(keresettSzoveg) || 
                            k.Szerzo.ToLower().Contains(keresettSzoveg))
                .ToListAsync();
        }

        
        [HttpPost]
        public async Task<ActionResult<Konyv>> PostKonyv(Konyv konyv)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            
            var isbnExists = await _context.Konyvek.AnyAsync(k => k.ISBN == konyv.ISBN);
            if (isbnExists)
            {
                return BadRequest("A megadott ISBN számmal már létezik könyv a rendszerben!"); 
            }

            
            if (konyv.OlvasoId != null)
            {
                var olvasoExists = await _context.Olvasok.AnyAsync(o => o.Id == konyv.OlvasoId);
                if (!olvasoExists)
                {
                    return BadRequest("A megadott olvasó nem létezik!"); 
                }
            }

            _context.Konyvek.Add(konyv);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetKonyv), new { id = konyv.Id }, konyv);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKonyv(int id, Konyv konyv)
        {
            if (id != konyv.Id)
            {
                return BadRequest(); 
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
        
            
            var aktualisKonyv = await _context.Konyvek.AsNoTracking().FirstOrDefaultAsync(k => k.Id == id);
            if (aktualisKonyv == null)
            {
                return NotFound(); 
            }

            
            var isbnExists = await _context.Konyvek.AnyAsync(k => k.ISBN == konyv.ISBN && k.Id != id);
            if (isbnExists)
            {
                return BadRequest("A megadott ISBN számmal már létezik másik könyv a rendszerben!");
            }

            
            if (konyv.OlvasoId != aktualisKonyv.OlvasoId)
            {
                
                if (konyv.OlvasoId != null)
                {
                    
                    var olvasoExists = await _context.Olvasok.AnyAsync(o => o.Id == konyv.OlvasoId);
                    if (!olvasoExists)
                    {
                        return BadRequest("Nem lehet nem létező olvasóhoz könyvet rendelni!");
                    }

                    
                    if (aktualisKonyv.OlvasoId != null)
                    {
                        return BadRequest("A könyv jelenleg ki van kölcsönözve, előbb vissza kell hozni!");
                    }
                }
                
            }
            _context.Entry(konyv).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Konyvek.AnyAsync(k => k.Id == id))
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
         async Task<IActionResult> DeleteKonyv(int id)
        {
            var konyv = await _context.Konyvek.FindAsync(id);
            if (konyv == null)
            {
                return NotFound(); 
            }

            _context.Konyvek.Remove(konyv);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}



