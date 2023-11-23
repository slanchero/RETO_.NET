namespace Reto.DTOs
{
    public class HeroeDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string? Escuela { get; set; }

        // Listas de Nombres o DTOs simplificados para relaciones, habilidades, debilidades y patrocinadores
        public List<string> Habilidades { get; set; } = new List<string>();
        public List<string> Debilidades { get; set; } = new List<string>();
        public List<PatrocinadorDto> Patrocinadores { get; set; } = new List<PatrocinadorDto>();
        public List<RelacionDto> Relaciones { get; set; } = new List<RelacionDto>();
    }

}
