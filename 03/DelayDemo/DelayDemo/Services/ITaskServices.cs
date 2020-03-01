namespace DelayDemo
{
    using System.Threading.Tasks;

    public interface ITaskServices
    {
        void SubscribeToDo(string keyPrefix);

        Task DoTaskAsync();
    }
}
