using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reto.Models;

[Table("Relacion")]
public partial class Relacion
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [Column("Relacion")]
    [StringLength(750)]
    [Unicode(false)]
    public string Relacion1 { get; set; } = null!;

    [ForeignKey("RelacionId")]
    [InverseProperty("Relacions")]
    public virtual ICollection<Heroe> Heroes { get; set; } = new List<Heroe>();
}
