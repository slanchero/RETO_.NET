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
    public class ActividadesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActividadesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Actividades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActividadDto>>> GetActividades(string? heroe = null)
        {
            IQueryable<Actividad> query = _context.Actividads;

            // Filtrar por el nombre del héroe si se proporciona
            if (!string.IsNullOrEmpty(heroe))
            {
                query = query.Where(a => a.Heroe.Nombre.Contains(heroe));
            }

            var actividades = await query.Include(a => a.Heroe).ToListAsync();

            var actividadesDto = actividades.Select(a => new ActividadDto
            {
                Id = a.Id,
                Heroe = a.Heroe.Nombre,
                Titulo = a.Titulo,
                Descripcion = a.Descripcion,
                Tipo = a.Tipo,
                FechaHoraInicio = a.FechaHoraInicio,
                FechaHoraFin = a.FechaHoraFin
            }).ToList();

            return actividadesDto;
        }


        // GET: api/Actividades/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ActividadDto>> GetActividad(int id)
        {
            var actividad = await _context.Actividads
                                          .Include(a => a.Heroe)
                                          .FirstOrDefaultAsync(a => a.Id == id);

            if (actividad == null)
            {
                return NotFound();
            }

            var actividadDto = new ActividadDto
            {
                Id = actividad.Id,
                Heroe = actividad.Heroe?.Nombre, // Asegúrate de manejar el caso donde Heroe podría ser null
                Titulo = actividad.Titulo,
                Descripcion = actividad.Descripcion,
                Tipo = actividad.Tipo,
                FechaHoraInicio = actividad.FechaHoraInicio,
                FechaHoraFin = actividad.FechaHoraFin
            };

            return actividadDto;
        }


        // PUT: api/Actividades/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActividad(int id, ActividadDto actividadUpdateDto)
        {

            var actividad = await _context.Actividads.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }

            // Encuentra el héroe basado en el nombre proporcionado
            var heroe = await _context.Heroes.FirstOrDefaultAsync(h => h.Nombre == actividadUpdateDto.Heroe);
            if (heroe == null)
            {
                return NotFound("Héroe no encontrado.");
            }

            actividad.HeroeId = heroe.Id;
            actividad.Titulo = actividadUpdateDto.Titulo;
            actividad.Descripcion = actividadUpdateDto.Descripcion;
            actividad.Tipo = actividadUpdateDto.Tipo;
            actividad.FechaHoraInicio = actividadUpdateDto.FechaHoraInicio;
            actividad.FechaHoraFin = actividadUpdateDto.FechaHoraFin;

            _context.Entry(actividad).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActividadExists(id))
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

        // POST: api/Actividades
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ActividadDto>> PostActividad(ActividadDto actividadCreateDto)
        {
            // Encuentra el héroe basado en el nombre proporcionado
            var heroe = await _context.Heroes.FirstOrDefaultAsync(h => h.Nombre == actividadCreateDto.Heroe);
            if (heroe == null)
            {
                return NotFound("Héroe no encontrado.");
            }

            var actividad = new Actividad
            {
                HeroeId = heroe.Id,
                Titulo = actividadCreateDto.Titulo,
                Descripcion = actividadCreateDto.Descripcion,
                Tipo = actividadCreateDto.Tipo,
                FechaHoraInicio = actividadCreateDto.FechaHoraInicio,
                FechaHoraFin = actividadCreateDto.FechaHoraFin
            };

            _context.Actividads.Add(actividad);
            await _context.SaveChangesAsync();

            // Crear un DTO para la respuesta
            var actividadDto = new ActividadDto
            {
                Id = actividad.Id,
                Heroe = heroe.Nombre,
                Titulo = actividad.Titulo,
                Descripcion = actividad.Descripcion,
                Tipo = actividad.Tipo,
                FechaHoraInicio = actividad.FechaHoraInicio,
                FechaHoraFin = actividad.FechaHoraFin
            };

            return CreatedAtAction("GetActividad", new { id = actividad.Id }, actividadDto);
        }


        // DELETE: api/Actividades/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActividad(int id)
        {
            var actividad = await _context.Actividads.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }

            _context.Actividads.Remove(actividad);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool ActividadExists(int id)
        {
            return (_context.Actividads?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
