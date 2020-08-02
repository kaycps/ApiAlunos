using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTeste.Utils
{
    public class UsuarioToken
    {
        public bool Autenticado { get; set; }
        public DateTime Expiracao { get; set; }
        public string Token { get; set; }
        public string Mensagem { get; set; }
    }
}
