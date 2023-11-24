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
    public class HeroesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HeroesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Heroes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HeroeDto>>> GetHeroes(string? nombre = null, string? habilidad = null, string? relacion = null)
        {
            IQueryable<Heroe> query = _context.Heroes;

            // Filtrar por parte del nombre del héroe
            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(h => h.Nombre.Contains(nombre));
            }

            // Filtrar por habilidades
            if (!string.IsNullOrEmpty(habilidad))
            {
                query = query.Where(h => h.Habilidads.Any(hab => hab.Nombre == habilidad));
            }

            // Filtrar por nombre en relaciones personales
            if (!string.IsNullOrEmpty(relacion))
            {
                query = query.Where(h => h.Relacions.Any(rel => rel.Nombre.Contains(relacion)));
            }

            var heroes = await query.Include(h => h.Habilidads)
                                    .Include(h => h.Debilidads)
                                    .Include(h => h.Patrocinadors)
                                    .Include(h => h.Relacions)
                                    .ToListAsync();

            var heroesDto = heroes.Select(h => new HeroeDto
            {
                // Mapeo existente
            }).ToList();

            return heroesDto;
        }




        // GET: api/Heroes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HeroeDto>> GetHeroe(int id)
        {
            var heroe = await _context.Heroes
                                      .Include(h => h.Habilidads)
                                      .Include(h => h.Debilidads)
                                      .Include(h => h.Patrocinadors)
                                      .Include(h => h.Relacions)
                                      .FirstOrDefaultAsync(h => h.Id == id);

            if (heroe == null)
            {
                return NotFound();
            }

            var heroeDto = new HeroeDto
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
            };

            return heroeDto;
        }

        // PUT: api/Heroes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHeroe(int id, HeroeDto heroeDto)
        {

            var heroe = await _context.Heroes
                                      .Include(h => h.Habilidads)
                                      .Include(h => h.Debilidads)
                                      .Include(h => h.Relacions)
                                      .Include(h => h.Patrocinadors)
                                      .FirstOrDefaultAsync(h => h.Id == id);

            if (heroe == null)
            {
                return NotFound();
            }

            heroe.Nombre = heroeDto.Nombre;
            heroe.Edad = heroeDto.Edad;
            heroe.Escuela = heroeDto.Escuela;

            // Actualizar habilidades, debilidades, relaciones
            UpdateHabilidades(heroe, heroeDto.Habilidades);
            UpdateDebilidades(heroe, heroeDto.Debilidades);
            UpdateRelaciones(heroe, heroeDto.Relaciones);



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HeroeExists(id))
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

        private void UpdateHabilidades(Heroe heroe, List<string> habilidades)
        {
            // Obtener las habilidades actuales del héroe
            var habilidadesActuales = heroe.Habilidads.ToList();

            // Eliminar habilidades que ya no están en la nueva lista
            foreach (var habilidadActual in habilidadesActuales)
            {
                if (!habilidades.Contains(habilidadActual.Nombre))
                {
                    heroe.Habilidads.Remove(habilidadActual);
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
                    heroe.Habilidads.Add(nuevaHabilidad);
                }
            }
        }

        private void UpdateDebilidades(Heroe heroe, List<string> debilidades)
        {
            // Obtener las debilidades actuales del héroe
            var debilidadesActuales = heroe.Debilidads.ToList();

            // Eliminar habilidades que ya no están en la nueva lista
            foreach (var debilidadesActual in debilidadesActuales)
            {
                if (!debilidades.Contains(debilidadesActual.Nombre))
                {
                    heroe.Debilidads.Remove(debilidadesActual);
                }
            }

            // Añadir nuevas habilidades
            foreach (var nombreDebilidad in debilidades)
            {
                if (!debilidadesActuales.Any(d => d.Nombre == nombreDebilidad))
                {
                    var nuevaDebilidad = _context.Debilidads.FirstOrDefault(d => d.Nombre == nombreDebilidad);
                    if (nuevaDebilidad == null)
                    {
                        // Crear una nueva habilidad si no existe
                        nuevaDebilidad = new Debilidad { Nombre = nombreDebilidad };
                        _context.Debilidads.Add(nuevaDebilidad);
                    }
                    heroe.Debilidads.Add(nuevaDebilidad);
                }
            }
        }

        private void UpdateRelaciones(Heroe heroe, List<RelacionDto> relaciones)
        {
            // Convertir a lista de nombres para simplificar la búsqueda
            var nombresRelaciones = relaciones.Select(r => r.Nombre).ToList();

            // Eliminar relaciones que ya no están en la nueva lista
            var relacionesActuales = heroe.Relacions.ToList();
            foreach (var relacionActual in relacionesActuales)
            {
                if (!nombresRelaciones.Contains(relacionActual.Nombre))
                {
                    heroe.Relacions.Remove(relacionActual);
                }
            }

            // Añadir o actualizar relaciones
            foreach (var relacionDto in relaciones)
            {
                var relacionExistente = heroe.Relacions
                                             .FirstOrDefault(r => r.Nombre == relacionDto.Nombre);

                if (relacionExistente != null)
                {
                    // Actualizar la descripción si es necesario
                    relacionExistente.Relacion1 = relacionDto.Relacion;
                }
                else
                {
                    // Crear una nueva relación si no existe
                    var nuevaRelacion = new Relacion
                    {
                        Nombre = relacionDto.Nombre,
                        Relacion1 = relacionDto.Relacion
                    };
                    heroe.Relacions.Add(nuevaRelacion);
                }
            }
        }



        // POST: api/Heroes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Heroe>> PostHeroe(HeroeDto heroeDto)
        {
            var heroe = new Heroe
            {
                Nombre = heroeDto.Nombre,
                Edad = heroeDto.Edad,
                Escuela = heroeDto.Escuela
            };

            //lógica para manejar habilidades, debilidades y relaciones
            foreach (var nombreHabilidad in heroeDto.Habilidades)
            {
                var habilidad = await _context.Habilidads
                                              .FirstOrDefaultAsync(h => h.Nombre == nombreHabilidad)
                                 ?? new Habilidad { Nombre = nombreHabilidad };
                heroe.Habilidads.Add(habilidad);
            }
            foreach (var nombreDebilidad in heroeDto.Debilidades)
            {
                var debilidad = await _context.Debilidads
                                              .FirstOrDefaultAsync(d => d.Nombre == nombreDebilidad)
                                 ?? new Debilidad { Nombre = nombreDebilidad };
                heroe.Debilidads.Add(debilidad);
            }
            foreach (var Relacion in heroeDto.Relaciones)
            {
                var relacion = await _context.Relacions
                                              .FirstOrDefaultAsync(r => r.Nombre== Relacion.Nombre)
                                 ?? new Relacion { Nombre = Relacion.Nombre,Relacion1= Relacion.Relacion };
                heroe.Relacions.Add(relacion);
            }

            _context.Heroes.Add(heroe);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHeroe", new { id = heroe.Id }, heroe);
        }

        // DELETE: api/Heroes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHeroe(int id)
        {
            var heroe = await _context.Heroes
                                      .Include(h => h.Habilidads)
                                      .Include(h => h.Debilidads)
                                      .Include(h => h.Relacions)
                                      .Include(h => h.Patrocinadors)
                                      .FirstOrDefaultAsync(h => h.Id == id);

            if (heroe == null)
            {
                return NotFound();
            }

            // Eliminar relaciones con habilidades, debilidades, etc.
            heroe.Habilidads.Clear();
            heroe.Debilidads.Clear();
            heroe.Relacions.Clear();
            heroe.Patrocinadors.Clear();

            _context.Heroes.Remove(heroe);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool HeroeExists(int id)
        {
            return (_context.Heroes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
