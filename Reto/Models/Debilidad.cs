using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reto.Models;

[Table("Debilidad")]
public partial class Debilidad
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [StringLength(750)]
    [Unicode(false)]
    public string? Descripcion { get; set; }

    [ForeignKey("DebilidadId")]
    [InverseProperty("Debilidads")]
    public virtual ICollection<Heroe> Heroes { get; set; } = new List<Heroe>();
}
