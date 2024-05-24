using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P_Futebol
{
    internal class Clube
    {
        public string nome { get; set; }
        public string apelido { get; set; }
        public DateOnly dtcriacao { get; set; }

        public Clube(string nome, string apelido, DateOnly dtcriacao)
        {
            this.nome = nome;
            this.apelido = apelido;
            this.dtcriacao = dtcriacao;
        }
    }
}
