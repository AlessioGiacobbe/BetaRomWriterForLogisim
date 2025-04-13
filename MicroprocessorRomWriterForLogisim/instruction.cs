using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroprocessorRomWriterForLogisim
{
    public class istruction
    {
        String nome;
      

        public istruction(String name)
        {
            this.nome = name;
        }

        public override string ToString()
        {
            return nome;
        }
    }
}
