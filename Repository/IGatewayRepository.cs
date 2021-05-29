using System.Collections.Generic;
using System.Threading.Tasks;
using MusalaSoft.GatewayApi.Model;

namespace MusalaSoft.GatewayApi.Repository
{
    public interface IGatewayRepository
    {
        Task<IEnumerable<Gateway>> GetAll();

        Task CreateGateway(Gateway gateway);
    }
}