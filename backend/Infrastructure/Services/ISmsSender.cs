using System.Threading.Tasks;

namespace Hive.Backend.Infrastructure.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}

