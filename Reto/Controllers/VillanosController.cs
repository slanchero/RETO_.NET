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
    public class VillanosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VillanosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Villanos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillanoDto>>> GetVillanos(string? nombre = null, string? origen = null, string? habilidad = null)
        {
            IQueryable<Villano> query = _context.Villanos;

            // Filtrar por parte del nombre del villano
            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(v => v.Nombre.Contains(nombre));
            }

            // Filtrar por origen
            if (!string.IsNullOrEmpty(origen))
            {
                query = query.Where(v => v.Origen == origen);
            }

            // Filtrar por debilidad
            // Asumiendo que tienes una relación con una entidad 'Debilidad' o similar
            if (!string.IsNullOrEmpty(habilidad))
            {
                query = query.Where(v => v.Habilidads.Any(d => d.Nombre == habilidad));
            }

            var villanos = await query.Include(v => v.Habilidads)
                                      .ToListAsync();

            var villanosDto = villanos.Select(v => new VillanoDto
            {
                Id = v.Id,
                Nombre = v.Nombre,
                Edad = v.Edad,
                Habilidades = v.Habilidads.Select(hab => hab.Nombre).ToList()
            }).ToList();

            return villanosDto;
        }

        // GET: api/Villanos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VillanoDto>> GetVillano(int id)
        {
            var villano = await _context.Villanos
                                      .Include(v => v.Habilidads)
                                      .FirstOrDefaultAsync(v => v.Id == id);

            if (villano == null)
            {
                return NotFound();
            }

            var villanoDto = new VillanoDto
            {
                Id = villano.Id,
                Nombre = villano.Nombre,
                Edad = villano.Edad,
                Habilidades = villano.Habilidads.Select(hab => hab.Nombre).ToList()
            };

            return villanoDto;
        }

        // GET: api/Villanos/adolecentes
        [HttpGet("adolecentes")]
        public async Task<ActionResult<VillanoDto>> GetVillanoConMasDerrotasContraAdolescentes()
        {
            var villanoConMasDerrotas = await _context.Luchas
                .Where(l => l.Heroe.Edad < 18 && !l.Vencedor) // Considera una lucha donde el héroe es adolescente y el villano perdió
                .GroupBy(l => l.VillanoId)
                .Select(group => new { VillanoId = group.Key, Derrotas = group.Count() })
                .OrderByDescending(x => x.Derrotas)
                .FirstOrDefaultAsync();

            if (villanoConMasDerrotas == null)
            {
                return NotFound("No se encontraron villanos que cumplan con el criterio.");
            }

            var villano = await _context.Villanos.FindAsync(villanoConMasDerrotas.VillanoId);
            if (villano == null)
            {
                return NotFound();
            }

            var villanoDto = new VillanoDto
            {
                Id = villano.Id,
                Nombre = villano.Nombre,
                Edad = villano.Edad,
                // Otros mapeos necesarios
            };

            return villanoDto;
        }


        // PUT: api/Villanos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVillano(int id, VillanoDto villanoDto)
        {

            var villano = await _context.Villanos
                                      .Include(v => v.Habilidads)
                                      .FirstOrDefaultAsync(v => v.Id == id);

            if (villano == null)
            {
                return NotFound();
            }

            villano.Nombre = villanoDto.Nombre;
            villano.Edad = villanoDto.Edad;
            villano.Origen = villanoDto.Origen;

            // Actualizar habilidades
            UpdateHabilidades(villano, villanoDto.Habilidades);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VillanoExists(id))
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

        private void UpdateHabilidades(Villano villano, List<string> habilidades)
        {
            // Obtener las habilidades actuales del héroe
            var habilidadesActuales = villano.Habilidads.ToList();

            // Eliminar habilidades que ya no están en la nueva lista
            foreach (var habilidadActual in habilidadesActuales)
            {
                if (!habilidades.Contains(habilidadActual.Nombre))
                {
                    villano.Habilidads.Remove(habilidadActual);
                }
            }

            // Añadir nuevas habilidades
            foreach (var nombreHabilidad in habilidades)
            {
                if (!habilidadesActuales.Any(h => h.Nombre == nombreHabilidad))
                {
                    var nuevaHabilidad = _context.Habilidads.FirstOrDefault(h => h.Nombre == nombreHabilidad);
                    if (nuevaHabilidad == null)
                    {
                        // Crear una nueva habilidad si no existe
                        nuevaHabilidad = new Habilidad { Nombre = nombreHabilidad };
                        _context.Habilidads.Add(nuevaHabilidad);
                    }
                    villano.Habilidads.Add(nuevaHabilidad);
                }
            }
        }

        // POST: api/Villanos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Villano>> PostVillano(VillanoDto villanoDto)
        {
            var villano = new Villano
            {
                Nombre = villanoDto.Nombre,
                Edad = villanoDto.Edad,
                Origen = villanoDto.Origen
            };

            //lógica para manejar habilidades, debilidades y relaciones
            foreach (var nombreHabilidad in villanoDto.Habilidades)
            {
                var habilidad = await _context.Habilidads
                                              .FirstOrDefaultAsync(h => h.Nombre == nombreHabilidad)
                                 ?? new Habilidad { Nombre = nombreHabilidad };
                villano.Habilidads.Add(habilidad);
            }

            _context.Villanos.Add(villano);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVillano", new { id = villano.Id }, villano);
        }

        // DELETE: api/Villanos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVillano(int id)
        {
            var villano = await _context.Villanos
                                      .Include(v => v.Habilidads)
                                      .FirstOrDefaultAsync(v => v.Id == id);

            if (villano == null)
            {
                return NotFound();
            }

            // Eliminar relaciones con habilidades, debilidades, etc.
            villano.Habilidads.Clear();

            _context.Villanos.Remove(villano);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VillanoExists(int id)
        {
            return (_context.Villanos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
