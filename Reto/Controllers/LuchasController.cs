using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reto.DBContext;
using Reto.DTOs;
using Reto.Models;

namespace Reto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LuchasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LuchasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Luchas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LuchaDto>>> GetLuchas(string? heroe = null, string? villano = null, string? vencedor = null)
        {
            IQueryable<Lucha> query = _context.Luchas;

            // Filtrar por nombre de héroe
            if (!string.IsNullOrEmpty(heroe))
            {
                query = query.Where(l => l.Heroe.Nombre == heroe);
            }

            // Filtrar por nombre de villano
            if (!string.IsNullOrEmpty(villano))
            {
                query = query.Where(l => l.Villano.Nombre == villano);
            }

            // Filtrar por vencedor
            if (!string.IsNullOrEmpty(vencedor))
            {
                query = query.Where(l => (l.Vencedor && l.Heroe.Nombre == vencedor) || (!l.Vencedor && l.Villano.Nombre == vencedor));
            }

            var luchas = await query.Include(l => l.Heroe)
                                    .Include(l => l.Villano)
                                    .ToListAsync();

            var luchasDto = luchas.Select(l => new LuchaDto
            {
                Id = l.Id,
                Heroe = l.Heroe.Nombre,
                Villano = l.Villano.Nombre,
                Vencedor = l.Vencedor ? l.Heroe.Nombre : l.Villano.Nombre,
            }).ToList();

            return luchasDto;
        }


        // GET: api/Luchas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LuchaDto>> GetLucha(int id)
        {
            var lucha = await _context.Luchas
                                      .Include(l => l.Heroe)
                                       .Include(l => l.Villano)
                                      .FirstOrDefaultAsync(h => h.Id == id);

            if (lucha == null)
            {
                return NotFound();
            }

            var luchaDto = new LuchaDto
            {
                Id = lucha.Id,
                Heroe = lucha.Heroe.Nombre,
                Villano = lucha.Villano.Nombre,
                Vencedor = lucha.Vencedor ? lucha.Heroe.Nombre : lucha.Villano.Nombre,
            };

            return luchaDto;
        }

        // GET: api/Luchas/MasVictorias
        [HttpGet("MasVictorias")]
        public async Task<ActionResult<IEnumerable<HeroeDto>>> GetHeroesConMasVictorias()
        {
            var heroesConMasVictorias = await _context.Luchas
                .Where(l => l.Vencedor) // Suponiendo que Vencedor es verdadero si el héroe gana
                .GroupBy(l => l.HeroeId)
                .Select(group => new { HeroeId = group.Key, Victorias = group.Count() })
                .OrderByDescending(x => x.Victorias)
                .Take(3) // Tomar los tres primeros
                .ToListAsync();

            var heroesIds = heroesConMasVictorias.Select(h => h.HeroeId).ToList();
            var heroes = await _context.Heroes
                                       .Where(h => heroesIds.Contains(h.Id))
                                       .ToListAsync();

            var heroesDto = heroes.Select(heroe => new HeroeDto
            {
                Id = heroe.Id,
                Nombre = heroe.Nombre,
                Edad = heroe.Edad,
                Escuela = heroe.Escuela,
                Habilidades = heroe.Habilidads.Select(hab => hab.Nombre).ToList(),
                Debilidades = heroe.Debilidads.Select(deb => deb.Nombre).ToList(),
                Patrocinadores = heroe.Patrocinadors.Select(pat => new PatrocinadorDto
                {
                    Nombre = pat.Nombre,
                    Origen = pat.Origen,
                    Monto = pat.Monto,
                }).ToList(),
                Relaciones = heroe.Relacions.Select(rel => new RelacionDto
                {
                    Nombre = rel.Nombre,
                    Relacion = rel.Relacion1
                }).ToList()
            }).ToList();

            return heroesDto;
        }


        //// PUT: api/Luchas/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutLucha(int id, Lucha lucha)
        //{
        //    if (id != lucha.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(lucha).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!LuchaExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Luchas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Lucha>> PostLucha(LuchaDto luchaDto)
        {
            var heroe = await _context.Heroes.FirstOrDefaultAsync(h => h.Nombre == luchaDto.Heroe);
            var villano = await _context.Villanos.FirstOrDefaultAsync(v => v.Nombre == luchaDto.Villano);

            if (heroe == null || villano == null)
            {
                return NotFound("El héroe o el villano especificado no existe.");
            }

            var lucha = new Lucha
            {
                HeroeId = heroe.Id,
                VillanoId = villano.Id,
                Vencedor = luchaDto.Vencedor == heroe.Nombre
            };

            _context.Luchas.Add(lucha);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLucha", new { id = lucha.Id }, lucha);
        }


        // DELETE: api/Luchas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLucha(int id)
        {
            var lucha = await _context.Luchas.FindAsync(id);
            if (lucha == null)
            {
                return NotFound();
            }

            _context.Luchas.Remove(lucha);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool LuchaExists(int id)
        {
            return (_context.Luchas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
