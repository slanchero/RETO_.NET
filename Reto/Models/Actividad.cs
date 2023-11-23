using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reto.Models;

[Table("Actividad")]
public partial class Actividad
{
    [Key]
    public int Id { get; set; }

    public int? HeroeId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Titulo { get; set; }

    [Column(TypeName = "text")]
    public string? Descripcion { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Tipo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaHoraInicio { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FechaHoraFin { get; set; }

    [ForeignKey("HeroeId")]
    [InverseProperty("Actividads")]
    public virtual Heroe? Heroe { get; set; }
}
