using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalaDeReunião.Context;
using SalaDeReunião.Models;
using SalaDeReunião.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalaDeReunião.Controllers
{
    [Route("api/v1/[Controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsuariosController(AppDbContext contexto)
        {
            _context = contexto;
        }

        
        [HttpGet]
        //[Authorize(Roles = "manager")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Usuario>>> Get([FromServices] AppDbContext context)
        {
            var usuarios = await context
                .Usuarios
                .AsNoTracking()
                .ToListAsync();
            return usuarios;
        }

        [HttpPost]
        [AllowAnonymous]
        // [Authorize(Roles = "manager")]
        public async Task<ActionResult<Usuario>> Post(
            [FromServices] AppDbContext context,
            [FromBody]Usuario model)
        {
            // Verifica se os dados são válidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Força o usuário a ser sempre "funcionário"
                model.Regra = "employee";

                context.Usuarios.Add(model);
                await context.SaveChangesAsync();

                // Esconde a senha
                model.Senha = "";
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });

            }
        }

        [HttpPut("{usuarioId:int}")]
        //[Authorize(Roles = "manager")]
        [AllowAnonymous]
        public async Task<ActionResult<Usuario>> Put(
            [FromServices] AppDbContext context,
            int usuarioId,
            [FromBody]Usuario model)
        {
            // Verifica se os dados são válidos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifica se o ID informado é o mesmo do modelo
            if (usuarioId != model.UsuarioId)
                return NotFound(new { message = "Usuário não encontrada" });

            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return model;
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });

            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate(
                    [FromServices] AppDbContext context,
                    [FromBody]Usuario model)
        {
            var usuario = await context.Usuarios
                .AsNoTracking()
                .Where(x => x.NomeDoUsuario == model.NomeDoUsuario && x.Senha == model.Senha)
                .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(usuario);
            // Esconde a senha
            usuario.Senha = "";
            return new
            {
                usuario = usuario,
                token = token
            };
        }
    }
}