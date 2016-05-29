using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotRas;
using System.Net;

namespace com.gmail.jjseen6.Ras
{
    /// <summary>
    /// 此类工作在用户空间下。
    /// </summary>
    public class RasDecorator : IDisposable
    {
        public static readonly IPAddress GoogleFirstDns;
        public static readonly IPAddress GoogleSecondDns;

        #region Attribute

        public string Entry { get; private set; }

        public RasDialer Dialer { get; private set; }

        public NetworkCredential Credential
        {
            get
            {
                return Dialer.Credentials;
            }
            set
            {
                Dialer.Credentials = value;
            }
        }
        #endregion

        #region Construct
        static RasDecorator()
        {
            GoogleFirstDns = IPAddress.Parse(@"8.8.8.8");
            GoogleSecondDns = IPAddress.Parse(@"8.8.4.4");
        }
        
        /// <summary>
        /// entryName is important. It is used to create new Vpn entry, to find connection.
        /// </summary>
        /// <param name="entry"></param>
        public RasDecorator(string entry)
        {
            this.Entry = entry;

        }
        #endregion

        #region function
        /// <summary>
        /// 使用/恢复 默认的DNS
        /// </summary>
        public bool UseDefualtDns()
        {
            using (var pbk = new RasPhoneBook())
            {
                pbk.Open(RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User));
                pbk.Entries[Entry].DnsAddress = IPAddress.Any;
                pbk.Entries[Entry].DnsAddressAlt = IPAddress.Any;
                return pbk.Entries[Entry].Update();
            }
        }

        /// <summary>
        /// 使用Google DNS
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public bool UseGoogleDns()
        {
            using (var pbk = new RasPhoneBook())
            {
                pbk.Open(RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User));
                pbk.Entries[Entry].DnsAddress = GoogleFirstDns;
                pbk.Entries[Entry].DnsAddressAlt = GoogleSecondDns;
                return pbk.Entries[Entry].Update();
            }
        }

        /// <summary>
        /// 使用自定义的DNS
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exception">DNS地址转换错误</exception>
        public bool UseUserDefinedDns(string first, string second)
        {
            var master = IPAddress.Parse(first);
            var alter = IPAddress.Parse(second);

            using (var pbk = new RasPhoneBook())
            {
                pbk.Open(RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User));
                pbk.Entries[Entry].DnsAddress = master;
                pbk.Entries[Entry].DnsAddressAlt = alter;
                return pbk.Entries[Entry].Update();
            }
        }


        /// <summary>
        /// 更新此RasDialer默认的目标主机。如果本次已经连接，在下次连接有效。
        /// </summary>
        /// <param name="host">域名地址或者IP地址</param>
        /// <exception cref="System.Exception"/>
        public bool UpdateDestination(string host)
        {
            using (var pbk = new RasPhoneBook())
            {
                pbk.Open(RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.User));
                var me = pbk.Entries[Entry];
                me.PhoneNumber = host;
                return me.Update();
            }
        }

        /// <summary>
        /// 获取当前连接。如果当前未连接，则返回null。
        /// </summary>
        /// <returns></returns>
        public RasConnection Connection()
        {
            var allConnections = RasConnection.GetActiveConnections();
            try
            {
                return allConnections.First(conn => conn.EntryName.Equals(Entry));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 断开连接。
        /// </summary>
        public void Hangup()
        {
            Connection()?.HangUp();
        }

        #endregion

        #region implement IDisposable
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Method

        public static RasDecorator GetPptpVpnDecoratorInstance(string entry)
        {
            throw new NotImplementedException();
        }

        public static RasDecorator GetL2tpVpnDecoratorInstance(string entry)
        {
            throw new NotImplementedException("暂不支持L2TP");
        }
        #endregion
    }
}
;