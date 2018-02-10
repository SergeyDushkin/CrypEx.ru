using System.Threading.Tasks;

namespace blockchain.rate.service
{
    public interface IRateService
    {
        Task<float> GetRateAsync();
    }
}
