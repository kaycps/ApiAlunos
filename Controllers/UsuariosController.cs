using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiTeste;
using ApiTeste.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ApiTeste.Controllers
{

    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly db_curso_inglesContext _context;
       

        public UsuariosController(db_curso_inglesContext context)
        {
            _context = context;            
        }

        // GET: api/Usuarios
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuario()
        {
            return await _context.Usuario.ToListAsync();
        }

        // POST: api/Usuarios        
        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> CadastrarUsuario(UsuarioSenhaPost post)
        {
            
            if (ValidadorEmail.IsValidEmail(post.Usuario.Email))
            {
                if (_context.Usuario.Any(u => u.Login.Equals(post.Usuario.Login))){

                    ModelState.AddModelError(string.Empty, "Login ja cadastrado!");
                    return BadRequest(ModelState);
                }

                byte[] s = Crypto.sha256encryptByte(post.SenhaForm);

                post.Usuario.Senha = s;
                
                _context.Usuario.Add(post.Usuario);

                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUsuario", new { id = post.Usuario.IdUsuario }, post.Usuario);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email invalido!");
                return BadRequest(ModelState);
            }
                 
                

        }        

       

    }
}
