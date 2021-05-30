using System.Collections.Generic;
using System.Threading.Tasks;
using MusalaSoft.GatewayApi.Model;

namespace MusalaSoft.GatewayApi.Repository
{
    public interface IGatewayRepository
    {
        Task<IEnumerable<Gateway>> GetAll();

        Task<Gateway> Get(string usn);

        Task<Gateway> GetForUpdate(string usn);

        Task CreateGateway(Gateway gateway);

        Task UpdateGateway(Gateway gateway);
    }
}