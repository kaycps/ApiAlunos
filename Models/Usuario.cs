using System;
using System.Collections.Generic;

namespace ApiTeste
{
    public partial class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public byte[] Senha { get; set; }
        public string Email { get;set; }
        
    }
}
