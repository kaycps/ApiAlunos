using System;
using System.Collections.Generic;

namespace ApiTeste
{
    public partial class Turma
    {
        public Turma()
        {
            Alunos = new HashSet<Aluno>();
        }

        public int IdTurma { get; set; }
        public string Descricao { get; set; }

        public virtual ICollection<Aluno> Alunos { get; set; }
    }
}
