namespace SnowflakeDemo
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;

    public class IdGenerator
    {
        // 基准时间
        public const long Twepoch = 1288834974657L;

        // 机器标识位数
        private const int WorkerIdBits = 5;

        // 数据标志位数
        private const int DatacenterIdBits = 5;

        // 序列号识位数
        private const int SequenceBits = 12;

        // 机器ID最大值
        private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);

        // 数据标志ID最大值
        private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

        // 序列号ID最大值
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);

        // 机器ID偏左移12位
        private const int WorkerIdShift = SequenceBits;

        // 数据ID偏左移17位
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;

        // 时间毫秒左移22位
        public const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

        private long _sequence = 0L;
        private long _lastTimestamp = -1L;

        public long WorkerId { get; protected set; }

        public long DatacenterId { get; protected set; }

        public long Sequence
        {
            get { return _sequence; }
            internal set { _sequence = value; }
        }

        public IdGenerator(long datacenterId = -1)
        {
            if (datacenterId == -1)
            {
                // default
                datacenterId = GetDatacenterId();
            }

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException("非法数据标志ID", nameof(datacenterId));
            }

            // 先检验再赋值
            WorkerId = GetWorkerId();
            DatacenterId = datacenterId;

            Console.WriteLine($"w = {WorkerId}");
            Console.WriteLine($"d = {DatacenterId}");
        }

        private readonly object _lock = new object();

        /// <summary>
        /// 获取下一个Id
        /// </summary>
        /// <returns></returns>
        public long NextId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();
                if (timestamp < _lastTimestamp)
                {
                    throw new Exception(string.Format("时间戳必须大于上一次生成ID的时间戳.  拒绝为{0}毫秒生成id", _lastTimestamp - timestamp));
                }

                // 如果上次生成时间和当前时间相同,在同一毫秒内
                if (_lastTimestamp == timestamp)
                {
                    // sequence自增，和sequenceMask相与一下，去掉高位
                    _sequence = (_sequence + 1) & SequenceMask;

                    // 判断是否溢出,也就是每毫秒内超过1024，当为1024时，与sequenceMask相与，sequence就等于0
                    if (_sequence == 0)
                    {
                        // 等待到下一毫秒
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    // 如果和上次生成时间不同,重置sequence，就是下一毫秒开始，sequence计数重新从0开始累加,
                    // 为了保证尾数随机性更大一些,最后一位可以设置一个随机数
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;
                return ((timestamp - Twepoch) << TimestampLeftShift) | (DatacenterId << DatacenterIdShift) | (WorkerId << WorkerIdShift) | _sequence;
            }
        }

        /// <summary>
        /// 防止产生的时间比之前的时间还要小（由于NTP回拨等问题）,保持增量的趋势.
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }

            return timestamp;
        }

        /// <summary>
        /// 获取当前的时间戳
        /// </summary>
        /// <returns></returns>
        private long TimeGen()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 获取机器Id
        /// </summary>
        /// <returns></returns>
        private int GetWorkerId()
        {
            var workerId = 0;

            try
            {
                IPAddress ipaddress = IPAddress.Parse("0.0.0.0");
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface ni in interfaces)
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (UnicastIPAddressInformation ip in
                            ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                ipaddress = ip.Address;
                                break;
                            }
                        }
                    }
                }
                Console.WriteLine($"ip = {ipaddress.ToString()}");
                var val = ipaddress.GetAddressBytes().Sum(x => x);

                workerId = val % (int)MaxWorkerId;
            }
            catch
            {
                workerId = new Random().Next((int)MaxWorkerId - 1);
            }

            return workerId;
        }

        /// <summary>
        /// 获取数据中心Id
        /// </summary>
        /// <returns></returns>
        private int GetDatacenterId()
        {
            var hostName = Dns.GetHostName();
            Console.WriteLine($"hostname = {hostName}");
            var val = System.Text.Encoding.UTF8.GetBytes(hostName).Sum(x => x);

            return val % (int)MaxDatacenterId;
        }
    }
}
