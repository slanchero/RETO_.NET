using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reto.Models;

[Table("Lucha")]
public partial class Lucha
{
    [Key]
    public int Id { get; set; }

    public int HeroeId { get; set; }

    public bool Vencedor { get; set; }

    public int VillanoId { get; set; }

    [ForeignKey("HeroeId")]
    [InverseProperty("Luchas")]
    public virtual Heroe Heroe { get; set; } = null!;

    [ForeignKey("VillanoId")]
    [InverseProperty("Luchas")]
    public virtual Villano Villano { get; set; } = null!;
}
