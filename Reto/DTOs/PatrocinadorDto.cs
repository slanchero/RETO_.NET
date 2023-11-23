using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Reto.DTOs
{
    public class PatrocinadorDto
    {
        public string Nombre { get; set; }
        public string Origen { get; set; }
        public decimal Monto { get; set; }

    }

}
