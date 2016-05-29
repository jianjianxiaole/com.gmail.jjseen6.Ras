using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.gmail.jjseen6.Ras
{
    public class RasVpnFactory
    {
        public RasDecorator GetPptpVpnRasDecorator(string entry)
        {

        }

        public RasDecorator GetL2tpVpnRasDecorator(string entry)
        {
            throw new NotImplementedException();
        }
    }
}
