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
    public class AlunosController : ControllerBase
    {
        private readonly db_curso_inglesContext _context;

        public AlunosController(db_curso_inglesContext context)
        {
            _context = context;
        }

        // GET: api/Alunos
        [Produces("application/json")]
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions),
            nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IEnumerable<Aluno>>> GetAluno()
        {
            
            return await _context.Aluno.Include(t => t.Turma.Alunos).OrderBy(a => a.IdAluno).ToListAsync();
        }

        // GET: api/Alunos/5
        [HttpGet("{id:int:min(1)}")]
        [ApiConventionMethod(typeof(DefaultApiConventions),
            nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<Aluno>> GetAluno(int id)
        {
            var aluno = await _context.Aluno.FindAsync(id);

            if (aluno == null)
            {
                ModelState.AddModelError(string.Empty, "Aluno não encontrado!");
                return NotFound(ModelState);
                
            }

            return aluno;
        }

        // PUT: api/Alunos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<IActionResult> AlteracaoAluno(int id, Aluno aluno)
        {
            if (id != aluno.IdAluno)
            {
                
                return BadRequest();
            }

            _context.Entry(aluno).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetAluno", new { id = aluno.IdAluno }, aluno);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlunoExists(id))
                {
                    ModelState.AddModelError(string.Empty, "Aluno não encontrado!");
                    return NotFound(ModelState);
                    
                }
                
            }

            return NoContent();
        }              

        // POST: api/Alunos        
        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult> CadastrarAluno(Aluno aluno )
        {                       
            

            if (_context.Aluno.Any(a => a.Matricula == aluno.Matricula))
            {
                ModelState.AddModelError(string.Empty, "Matricula ja cadastrada no sistema");
                return BadRequest(ModelState);
            }
            else
            {
                var turma = _context.Turma.Include(a => a.Alunos).Where(t=>t.IdTurma==aluno.IdTurma).First();

                if (turma.Alunos.Count <= 5)
                {
                    _context.Aluno.Add(aluno);

                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetAluno", new { id = aluno.IdAluno }, aluno);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Turma atingiu o limite de alunos!");
                    return BadRequest(ModelState);
                }
                
            }
            
            
        }

        // DELETE: api/Alunos/5
        [HttpDelete("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult<Aluno>> ExclusaoAluno(int id)
        {
            var aluno = await _context.Aluno.FindAsync(id);
            if (aluno == null)
            {
                ModelState.AddModelError(string.Empty, "Aluno Invalido!");
                return NotFound(ModelState);
            }
            if (aluno.IdTurma == null)
            {
                _context.Aluno.Remove(aluno);
                await _context.SaveChangesAsync();

                return aluno;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Não é possivel excluir alunos associados a turmas");
                return BadRequest(ModelState);
                
            }            

            
        }

        // DELETE: api/Alunos/5
        [HttpDelete("AlunoTurma/{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Delete))]
        public async Task<ActionResult<Aluno>> ExclusaoAlunoTurma(int id)
        {
            var aluno = await _context.Aluno.Where(a => a.IdAluno == id).FirstAsync();
            

            if (aluno == null)
            {
                ModelState.AddModelError(string.Empty, "Aluno Invalido!");
                return NotFound(ModelState);
            }
            
            var turma = await _context.Turma.Where(a => a.IdTurma == aluno.IdTurma).FirstAsync();
            

            //aluno.Turma = null;
            turma.Alunos.Remove(aluno);

            _context.Turma.Update(turma);
                
            await _context.SaveChangesAsync();

             return aluno;
        }


        private bool AlunoExists(int id)
        {
            return _context.Aluno.Any(e => e.IdAluno == id);
        }

       
    }
}
