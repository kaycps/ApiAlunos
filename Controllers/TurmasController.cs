using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiTeste;
using Microsoft.AspNetCore.Authorization;

namespace ApiTeste.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class TurmasController : ControllerBase
    {
        private readonly db_curso_inglesContext _context;

        public TurmasController(db_curso_inglesContext context)
        {
            _context = context;
        }

        // GET: api/Turmas
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IEnumerable<Turma>>> GetTurma()
        {
            return await _context.Turma.Include(a=>a.Alunos).Where(a=>a.Alunos!=null).ToListAsync();
        }

        // GET: api/Turmas/5
        [HttpGet("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<Turma>> GetTurma(int id)
        {
            var turma = await _context.Turma.Include(a => a.Alunos).Where(a => a.IdTurma == id).FirstOrDefaultAsync();

            if (turma == null)
            {
                ModelState.AddModelError(string.Empty, "Turma não encontrada!");
                return NotFound(ModelState);
                
            }

            return turma;
        }       

        // DELETE: api/Turmas/5
        [HttpDelete("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult<Turma>> DeleteTurma(int id)
        {
            var turma =  _context.Turma.Include(a=>a.Alunos).Where(t=>t.IdTurma==id).First();
            if (turma == null)
            {
                ModelState.AddModelError(string.Empty, "Turma não encontrada!");
                return NotFound(ModelState);
            }
            else
            {
                if (turma.Alunos.Count()>0)
                {
                    ModelState.AddModelError(string.Empty, "Não é possivel deletar turma com alunos vinculados!");
                    return BadRequest(ModelState);
                }
                else
                {
                    _context.Turma.Remove(turma);
                    await _context.SaveChangesAsync();

                    return turma;
                }
            }            
            
        }

        private bool TurmaExists(int id)
        {
            return _context.Turma.Any(e => e.IdTurma == id);
        }
    }
}
