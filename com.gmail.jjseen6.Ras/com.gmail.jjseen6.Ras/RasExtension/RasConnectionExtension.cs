using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotRas;
using com.gmail.jjseen6.Ras.RasException;

namespace com.gmail.jjseen6.Ras.RasExtension
{
    public static class RasConnectionExtension
    {
        /// <summary>
        /// 异步计算当前连接的上传速度（KB/s）和下载速度(KB/s)
        /// </summary>
        /// <param name="conn"></param>
        /// <returns>Tuple(上传速度，下载速度)</returns>
        /// <remarks>连续获取两次间隔为一秒的统计数据，计算速度</remarks>
        /// <exception cref="NotConnectedException">如果连接并未成功连接，则抛出该异常</exception>
        public static Task<Tuple<long,long>> GetRateAsync(this RasConnection conn)
        {
            if (conn.GetConnectionStatus().ConnectionState != RasConnectionState.Connected)
                throw new NotConnectedException();
            return Task.Run<Tuple<long, long>>(() =>
             {
                 var point1 = conn.GetConnectionStatistics();
                 System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                 var point2 = conn.GetConnectionStatistics();

                 var transmitted = point2.BytesTransmitted - point1.BytesTransmitted;
                 var received = point2.BytesReceived - point1.BytesReceived;
                 if (transmitted < 0)
                     transmitted = 0;
                 if (received < 0)
                     received = 0;
                 return new Tuple<long, long>(transmitted, received);
             });
        }

        /// <summary>
        /// 计算连接的平均速度，而不在乎当前连接是否已经连接上。
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static Tuple<long, long> AverageRate(this RasConnection conn)
        {           
            var statistics = conn.GetConnectionStatistics();
            if (statistics.ConnectionDuration > TimeSpan.FromSeconds(0))
                return new Tuple<long, long>(statistics.BytesTransmitted / statistics.ConnectionDuration.Seconds,
                                             statistics.BytesReceived / statistics.ConnectionDuration.Seconds);
            else
                return new Tuple<long, long>(0, 0);
        }
    }
}
