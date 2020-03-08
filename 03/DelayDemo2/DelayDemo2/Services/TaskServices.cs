namespace DelayDemo2
{
    using System;
    using System.Threading.Tasks;

    public class TaskServices : ITaskServices
    {       
        public async Task DoTaskAsync()
        {
            // do something here
            // ...

            // this operation should be done after some min or sec

            var cacheKey = "task:delay";
            int sec = new Random().Next(1, 5);
            var time = DateTimeOffset.Now.AddSeconds(sec).ToUnixTimeSeconds();
            var taskId = new Random().Next(1, 10000);
            await RedisHelper.ZAddAsync(cacheKey, (time, taskId));
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} done {taskId} here - {sec}");
        }

        public async Task SubscribeToDo()
        {
            var cacheKey = "task:delay";
            while (true)
            {
                var vals = RedisHelper.ZRangeByScore(cacheKey, -1, DateTimeOffset.Now.ToUnixTimeSeconds(), 1, 0);

                if (vals != null && vals.Length > 0)
                {
                    var val = vals[0];

                    // add a lock here may be more better
                    var rmCount = RedisHelper.ZRem(cacheKey, vals);

                    if (rmCount > 0)
                    {
                        Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} begin to do task {val}");
                    }
                }
                else
                {
                    await Task.Delay(500);
                }
            }
        }
    }
}
