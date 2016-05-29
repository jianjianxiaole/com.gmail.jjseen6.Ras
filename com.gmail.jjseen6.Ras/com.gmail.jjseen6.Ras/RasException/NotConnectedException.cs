using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace com.gmail.jjseen6.Ras.RasException
{
    class NotConnectedException : Exception
    {
        private const string err = "连接不是处理已连接状态";

        public NotConnectedException() : base(err)
        {
        }

        public NotConnectedException(Exception innerException) : base(err, innerException)
        {
        }

    }
}
