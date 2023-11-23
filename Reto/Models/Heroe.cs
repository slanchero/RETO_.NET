using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reto.Models;

[Table("Heroe")]
public partial class Heroe
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    public int Edad { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Escuela { get; set; }

    [InverseProperty("Heroe")]
    public virtual ICollection<Actividad> Actividads { get; set; } = new List<Actividad>();

    [InverseProperty("Heroe")]
    public virtual ICollection<Lucha> Luchas { get; set; } = new List<Lucha>();

    [ForeignKey("HeroeId")]
    [InverseProperty("Heroes")]
    public virtual ICollection<Debilidad> Debilidads { get; set; } = new List<Debilidad>();

    [ForeignKey("HeroeId")]
    [InverseProperty("Heroes")]
    public virtual ICollection<Habilidad> Habilidads { get; set; } = new List<Habilidad>();

    [ForeignKey("HeroeId")]
    [InverseProperty("Heroes")]
    public virtual ICollection<Patrocinador> Patrocinadors { get; set; } = new List<Patrocinador>();

    [ForeignKey("HeroeId")]
    [InverseProperty("Heroes")]
    public virtual ICollection<Relacion> Relacions { get; set; } = new List<Relacion>();
}
