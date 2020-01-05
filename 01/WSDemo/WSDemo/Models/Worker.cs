using System.Collections.Generic;

namespace WSDemo.Handlers
{
    public class Worker
    {
        public string WorkerId { get; set; }

        public static List<string> GetAllWorkerId()
        {
            var all = new List<string> { "1", "2", "3", "4" };
            return all;
        }

        public static List<string> GetByTaskType(int type)
        {
            var list = new List<string>();

            if (type == 1)
            {
                list.Add("1");
                list.Add("3");
            }
            else
            {
                list.Add("2");
                list.Add("4");
            }

            return list;
        }
    }
}
