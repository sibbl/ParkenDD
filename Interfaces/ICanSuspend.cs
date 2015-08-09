using System.Threading.Tasks;

namespace ParkenDD.Interfaces
{
    public interface ICanSuspend
    {
        Task OnSuspend();
    }
}
