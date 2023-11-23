namespace Reto.DTOs
{
    public class VillanoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public string? Origen { get; set; }

        public List<string> Habilidades { get; set; } = new List<string>();
    }

}
