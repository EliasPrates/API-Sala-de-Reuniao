using SalaDeReunião.Context;
using SalaDeReunião.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;

namespace ApiCatalogo.Controllers
{
    [Route("api/v1/[Controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ReservasController(AppDbContext contexto)
        {
            _context = contexto;
        }

        //[HttpGet("primeiro"]
        //[HttpGet("/primeiro"]
        //[HttpGet("{valor:alpha:length(5)}")]
        //public ActionResult<Produto> Get2()
        //{
        //    return _context.Produtos.FirstOrDefault();
        //}
        //--------------------------------------------

        // api/produtos
        [HttpGet]
        [AllowAnonymous]
        //[ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<Reserva>>> Get()
        {
            return await _context.Reservas.AsNoTracking().OrderBy(r => r.HorarioInicial).Include(r => r.Usuario).ToListAsync();
        }

        //// api/reservas/1
        //[HttpGet("{id}/{param2}", Name = "ObterReserva")]
        //public ActionResult<Reserva> Get(int id, string param2)
        //{
        //    var meuParametro = param2;

        //    var reserva =  _context.Reservas.AsNoTracking()
        //                    .FirstOrDefault(p => p.ReservaId == id);

        //    if (reserva == null)
        //    {
        //        return NotFound();
        //    }
        //    return reserva;
        //}
        //-------------------------------------------------------------

        // api/reservas/1
        //[HttpGet("{id:int:min(1)}", Name = "ObterReserva")]
        [HttpGet("{reservaId}", Name = "ObterReserva")]
        [AllowAnonymous]
        public async Task<ActionResult<Reserva>> Get(int reservaId)
        {

            //throw new Exception("Exception ao retornar reserva pela reservaId");
            //string[] teste = null;
            //if (teste.Length > 0)
            //{ }

            var reserva = await _context.Reservas.AsNoTracking()
                .FirstOrDefaultAsync(p => p.ReservaId == reservaId);

            if (reserva == null)
            {
                return NotFound();
            }
            return reserva;
        }

        //Reservas de um usuário específico
        [HttpGet("reservasdousuario/{usuarioId:int}")]
        [AllowAnonymous]
        //[ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservasDoUsuario(int usuarioId)
        {
            return await _context.Reservas.AsNoTracking().OrderBy(r => r.HorarioInicial).Where(r => r.UsuarioId == usuarioId).Include(r => r.Usuario).ToListAsync();
        }

        //  api/reservas
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Post([FromBody]Reserva reserva)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Verifica se a data/horário não é passado
            if(reserva.HorarioInicial < DateTime.Now)
            {
                return BadRequest("Data ou horário inválido - Você não pode agendar para uma horário ou data passada");
            }

            //Verifica se o horário inicial é menor que horário final
            if(reserva.HorarioInicial >= reserva.HorarioFinal)
            {
                return BadRequest("Horário inicial deve ser menor que horário final");
            }
            
            //Verifica se a data/horário selecionado está disponível - Grava no DB se estiver disponível
            var reservado = _context.Reservas.Where(r => (reserva.HorarioInicial >= r.HorarioInicial && reserva.HorarioInicial < r.HorarioFinal) || (reserva.HorarioFinal > r.HorarioInicial && reserva.HorarioInicial < r.HorarioInicial)).ToList();
            if (reservado.Count == 0) 
            {
            _context.Reservas.Add(reserva);
            _context.SaveChanges();
            return new CreatedAtRouteResult("obterReserva", new {reservaId = reserva.ReservaId}, reserva);
            }

            return BadRequest("Horário indisponível! Consulte a agenda para verificar horários disponíveis");
        }

        // api/reservas/1
        [HttpPut("{reservaId}")]
        [AllowAnonymous]
        public ActionResult Put(int reservaId, [FromBody] Reserva reserva)
        {

            //if(!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            if (reservaId != reserva.ReservaId)
            {
                return BadRequest();
            }

            //Verifica se a data/horário não é passado
            if(reserva.HorarioInicial < DateTime.Now)
            {
                return BadRequest("Data ou horário inválido - Você não pode agendar para uma horário ou data passada");
            }

            //Verifica se o horário inicial é menor que horário final
            if(reserva.HorarioInicial >= reserva.HorarioFinal)
            {
                return BadRequest("Horário inicial deve ser menor que horário final");
            }
            
            //Verifica se a data/horário selecionado está disponível - Grava no DB se estiver disponível
            var reservado = _context.Reservas.Where(r => (reserva.HorarioInicial >= r.HorarioInicial && reserva.HorarioInicial < r.HorarioFinal && reserva.ReservaId != r.ReservaId) || (reserva.HorarioFinal > r.HorarioInicial && reserva.HorarioInicial < r.HorarioInicial && reserva.ReservaId != r.ReservaId)).ToList();
            if (reservado.Count == 0) 
            {
            _context.Entry(reserva).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
            }

            return BadRequest("Horário indisponível! Consulte a agenda para verificar horários disponíveis");
        }

        //  api/reservas/1
        [HttpDelete("{reservaId:int}")]
        [AllowAnonymous]
        public ActionResult<Reserva> Delete(int reservaId)
        {
            var reserva = _context.Reservas.FirstOrDefault(p => p.ReservaId == reservaId);
            //var reserva = _context.Reservas.Find(id);

            if (reserva == null)
            {
                return NotFound();
            }

            _context.Reservas.Remove(reserva);
            _context.SaveChanges();
            return reserva;
        }
    }
}