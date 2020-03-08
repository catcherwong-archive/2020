namespace DelayDemo2
{
    using System.Threading.Tasks;

    public interface ITaskServices
    {
        Task DoTaskAsync();

        Task SubscribeToDo();
    }
}
