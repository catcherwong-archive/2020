namespace DelayDemo
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
            var taskId = new Random().Next(1, 10000);
            int sec = new Random().Next(1, 5);

            await RedisHelper.SetAsync($"task:{taskId}", "1", sec);
            await RedisHelper.SetAsync($"other:{taskId + 10000}", "1", sec);
        }

        public void SubscribeToDo(string keyPrefix)
        {
            RedisHelper.Subscribe(
                ("__keyevent@0__:expired", arg =>
                    {
                        var msg = arg.Body;
                        Console.WriteLine($"recive {msg}");
                        if (msg.StartsWith(keyPrefix))
                        {
                            // read the task id from expired key
                            var val = msg.Substring(keyPrefix.Length);
                            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} begin to do task {val}");
                        }
                    }
            )
            );
        }
    }
}
