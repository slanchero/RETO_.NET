namespace Reto.DTOs
{
    public class ActividadDto
    {
        public int Id { get; set; }
        public string Heroe { get; set; }
        public string Titulo { get; set; }
        public string? Descripcion { get; set; }
        public string? Tipo { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }

    }

}
