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
    public class PatrocinadorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatrocinadorController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Patrocinador
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatrocinadorMDto>>> GetPatrocinadores(string? heroe = null)
        {
            IQueryable<Patrocinador> query = _context.Patrocinadors;

            if (!string.IsNullOrEmpty(heroe))
            {
                // Filtrar patrocinadores que tienen una relación con el héroe especificado
                query = query.Where(p => p.Heroes.Any(h => h.Nombre == heroe));
            }

            var patrocinadores = await query.Include(p => p.Heroes).ToListAsync();

            var patrocinadoresDto = patrocinadores.Select(p => new PatrocinadorMDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Origen = p.Origen,
                Monto = p.Monto,
                Heroes = p.Heroes.Select(h => h.Nombre).ToList(),
            }).ToList();

            return patrocinadoresDto;
        }


        // GET: api/Patrocinador/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PatrocinadorMDto>> GetPatrocinador(int id)
        {
            var patrocinador = await _context.Patrocinadors
                                      .Include(p => p.Heroes)
                                      .FirstOrDefaultAsync(p => p.Id == id);

            if (patrocinador == null)
            {
                return NotFound();
            }

            var patrocinadorDto = new PatrocinadorMDto
            {
                Id = patrocinador.Id,
                Nombre = patrocinador.Nombre,
                Origen = patrocinador.Origen,
                Monto = patrocinador.Monto,
                Heroes = patrocinador.Heroes.Select(h => h.Nombre).ToList(),
            };

            return patrocinadorDto;
        }

        // PUT: api/Patrocinador/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]

        public async Task<IActionResult> PutPatrocinador(int id, PatrocinadorMDto PatrocinadorDto)
        {

            var patrocinador = await _context.Patrocinadors
                                      .Include(p => p.Heroes)
                                      .FirstOrDefaultAsync(p => p.Id == id);

            if (patrocinador == null)
            {
                return NotFound();
            }

            patrocinador.Nombre = PatrocinadorDto.Nombre;
            patrocinador.Origen = PatrocinadorDto.Origen;
            patrocinador.Monto = PatrocinadorDto.Monto;

            // Actualizar heroes patrocinados
            UpdateHeroes(patrocinador, PatrocinadorDto.Heroes);



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatrocinadorExists(id))
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

        private void UpdateHeroes(Patrocinador patrocinador, List<string> heroes)
        {
            // Obtener las habilidades actuales del héroe
            var heroesActuales = patrocinador.Heroes.ToList();

            // Eliminar heroes que ya no están en la nueva lista
            foreach (var heroeActual in heroesActuales)
            {
                if (!heroes.Contains(heroeActual.Nombre))
                {
                    patrocinador.Heroes.Remove(heroeActual);
                }
            }

            //Agregar Nuevos heroes si existen
            foreach (var nombreHeroe in heroes)
            {
                var heroe = _context.Heroes.FirstOrDefault(h => h.Nombre == nombreHeroe);
                if (heroe != null && !heroesActuales.Any(r => r.Nombre == heroe.Nombre))
                {
                    patrocinador.Heroes.Add(heroe);
                }
            }

        }


        // POST: api/Patrocinador
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Patrocinador>> PostPatrocinador(PatrocinadorMDto patrocinadorDto)
        {
            var patrocinador = new Patrocinador
            {
                Nombre = patrocinadorDto.Nombre,
                Origen = patrocinadorDto.Origen,
                Monto = patrocinadorDto.Monto
            };

            // Relacionar con héroes existentes basándose en nombres
            foreach (var nombreHeroe in patrocinadorDto.Heroes)
            {
                var heroe = await _context.Heroes.FirstOrDefaultAsync(h => h.Nombre == nombreHeroe);
                if (heroe != null)
                {
                   patrocinador.Heroes.Add(heroe);
                }
            }

            _context.Patrocinadors.Add(patrocinador);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPatrocinador", new { id = patrocinador.Id }, patrocinador);
        }



        // DELETE: api/Patrocinador/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatrocinador(int id)
        {
            var patrocinador = await _context.Patrocinadors
                                             .Include(p => p.Heroes) 
                                             .FirstOrDefaultAsync(p => p.Id == id);

            if (patrocinador == null)
            {
                return NotFound();
            }

            // Eliminar relaciones con héroes
            patrocinador.Heroes.Clear();

            _context.Patrocinadors.Remove(patrocinador);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool PatrocinadorExists(int id)
        {
            return (_context.Patrocinadors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
