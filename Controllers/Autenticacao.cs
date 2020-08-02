using ApiTeste.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiTeste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Autenticacao : ControllerBase
    {

        private readonly db_curso_inglesContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public Autenticacao(db_curso_inglesContext context, UserManager<IdentityUser> userManager
                                    , SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // Post: api/Autenticacao/Login
        [HttpPost("Login")]
        public async Task<ActionResult> Login(UsuarioDTO usuario)
        {

            if (!UsuarioExists(usuario))
            {
                ModelState.AddModelError(string.Empty, "Usuario Invalido");
                return NotFound(ModelState);
            }


            await Registrar(usuario);

            var result = await _signInManager.PasswordSignInAsync(usuario.Login, usuario.Senha, false, false);

            if (result.Succeeded)
            {
                return Ok(GerarToken(usuario));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Algo deu errado, entre em contato com o suporte!");
                return BadRequest(ModelState);
            }

        }

        private bool UsuarioExists(UsuarioDTO Usuario)
        {

            if (_context.Usuario.Any(u => u.Login.Equals(Usuario.Login)))
            {
                var senhaUsuario = Crypto.byteArrayToString(_context.Usuario.Where(u => u.Login == Usuario.Login)
                                                       .Select(u => u.Senha).FirstOrDefault());
                var senhaEntrada = Crypto.sha256encrypt(Usuario.Senha);

                if (senhaUsuario == senhaEntrada)
                {
                    return true;
                }

            }
            return false;

        }

        private async Task<ActionResult> Registrar(UsuarioDTO usuario)
        {
            var user = new IdentityUser
            {
                UserName = usuario.Login
            };

            var result = await _userManager.CreateAsync(user, usuario.Senha);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            await _signInManager.SignInAsync(user, false);

            return Ok(GerarToken(usuario));
        }

        private UsuarioToken GerarToken(UsuarioDTO usuario)        {
            

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiracao = DateTime.UtcNow.AddHours(double.Parse(_configuration["TokenConfiguration:ExpireHours"]));

            JwtSecurityToken token = new JwtSecurityToken(

               issuer: _configuration["TokenConfiguration:Issue"],
               audience: _configuration["TokenConfiguration:Audience"],
               expires: expiracao,
               //claims: claims,
               signingCredentials: credenciais);

            return new UsuarioToken()
            {
                Autenticado = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracao = expiracao,
                Mensagem = "Token jwt ok"
            };

        }
    }
}
