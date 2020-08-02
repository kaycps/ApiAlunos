using System;
using System.Collections.Generic;

namespace ApiTeste
{
    public partial class Aluno
    {
        public int IdAluno { get; set; }
        public string Matricula { get; set; }
        public string Nome { get; set; }

        public int IdTurma { get; set; }
        public virtual Turma Turma { get; set; }
    }
}
