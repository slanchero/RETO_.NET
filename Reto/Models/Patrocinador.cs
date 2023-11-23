using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reto.Models;

[Table("Patrocinador")]
public partial class Patrocinador
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Origen { get; set; }

    [Column(TypeName = "decimal(38, 2)")]
    public decimal Monto { get; set; }

    [ForeignKey("PatrocinadorId")]
    [InverseProperty("Patrocinadors")]
    public virtual ICollection<Heroe> Heroes { get; set; } = new List<Heroe>();
}
