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
        public async Task<ActionResult<IEnumerable<PatrocinadorMDto>>> GetPatrocinadores()
        {
            if (_context.Patrocinadors == null)
            {
                return NotFound();
            }

            // Cargar los datos necesarios incluyendo relaciones si es necesario
            var patrocinadores = await _context.Patrocinadors
                                       .Include(p => p.Heroes)
                                       .ToListAsync();

            // Mapear a DTOs
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
        public async Task<IActionResult> PutPatrocinador(int id, PatrocinadorMDto patrocinadorDto)
        {
            var patrocinador = await _context.Patrocinadors
                                             .Include(p => p.Heroes)
                                             .FirstOrDefaultAsync(p => p.Id == id);

            if (patrocinador == null)
            {
                return NotFound();
            }

            // Actualizar los datos del patrocinador
            patrocinador.Nombre = patrocinadorDto.Nombre;
            patrocinador.Origen = patrocinadorDto.Origen;
            patrocinador.Monto = patrocinadorDto.Monto;

            // Actualizar la relación con los héroes
            UpdateRelacionConHeroes(patrocinador, patrocinadorDto.Heroes);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private void UpdateRelacionConHeroes(Patrocinador patrocinador, List<string> nombresHeroes)
        {
            var heroesActuales = _context.Set<Dictionary<string, object>>("Heroe_Patrocinador")
                                         .Where(r => (int)r["PatrocinadorId"] == patrocinador.Id)
                                         .ToList();

            foreach (var relacion in heroesActuales)
            {
                var heroeId = (int)relacion["HeroeId"];
                var heroe = _context.Heroes.Find(heroeId);
                if (heroe == null || !nombresHeroes.Contains(heroe.Nombre))
                {
                    _context.Set<Dictionary<string, object>>("Heroe_Patrocinador").Remove(relacion);
                }
            }

            foreach (var nombreHeroe in nombresHeroes)
            {
                var heroe = _context.Heroes.FirstOrDefault(h => h.Nombre == nombreHeroe);
                if (heroe != null && !heroesActuales.Any(r => (int)r["HeroeId"] == heroe.Id))
                {
                    var nuevaRelacion = new Dictionary<string, object>
                    {
                        ["HeroeId"] = heroe.Id,
                        ["PatrocinadorId"] = patrocinador.Id
                    };
                    _context.Set<Dictionary<string, object>>("Heroe_Patrocinador").Add(nuevaRelacion);
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

            _context.Patrocinadors.Add(patrocinador);
            await _context.SaveChangesAsync();

            // Relacionar con héroes existentes basándose en nombres
            foreach (var nombreHeroe in patrocinadorDto.Heroes)
            {
                var heroe = await _context.Heroes.FirstOrDefaultAsync(h => h.Nombre == nombreHeroe);
                if (heroe != null)
                {
                    var heroePatrocinador = new Dictionary<string, object>
                    {
                        ["HeroeId"] = heroe.Id,
                        ["PatrocinadorId"] = patrocinador.Id
                    };
                    _context.Set<Dictionary<string, object>>("Heroe_Patrocinador").Add(heroePatrocinador);
                }
            }

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
