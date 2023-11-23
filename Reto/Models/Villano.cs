using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reto.Models;

[Table("Villano")]
public partial class Villano
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    public int Edad { get; set; }

    [Unicode(false)]
    public string? Origen { get; set; }

    [InverseProperty("Villano")]
    public virtual ICollection<Lucha> Luchas { get; set; } = new List<Lucha>();

    [ForeignKey("VillanoId")]
    [InverseProperty("Villanos")]
    public virtual ICollection<Habilidad> Habilidads { get; set; } = new List<Habilidad>();
}
