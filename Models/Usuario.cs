using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace SalaDeReunião.Models
{
    public class Usuario
    {
        public Usuario()
        {
            Reservas = new Collection<Reserva>();
        }

        [Key]
        public int UsuarioId { get; set; }
        [Required]
        public string NomeDoUsuario { get; set; }
        [Required]
        public string Senha { get; set; }
        public string Regra { get; set; }
        public ICollection<Reserva> Reservas { get; set; }
    }
}