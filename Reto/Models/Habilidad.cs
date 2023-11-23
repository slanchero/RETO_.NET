using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reto.Models;

[Table("Habilidad")]
public partial class Habilidad
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [StringLength(750)]
    [Unicode(false)]
    public string? Descripcion { get; set; }

    [ForeignKey("HabilidadId")]
    [InverseProperty("Habilidads")]
    public virtual ICollection<Heroe> Heroes { get; set; } = new List<Heroe>();

    [ForeignKey("HabilidadId")]
    [InverseProperty("Habilidads")]
    public virtual ICollection<Villano> Villanos { get; set; } = new List<Villano>();
}
