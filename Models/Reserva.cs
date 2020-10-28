using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalaDeReunião.Models
{
    public class Reserva
    {
        [Key]
        public int ReservaId { get; set; }
        [Required]
        public string Titulo { get; set; }
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }
        [Required]
        public DateTime HorarioInicial  { get; set; }
        [Required]
        public DateTime HorarioFinal { get; set; }
        public string Detalhes { get; set; }
    }
}