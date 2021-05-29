using System.Collections.Generic;
using System.Threading.Tasks;
using MusalaSoft.GatewayApi.Model;

namespace MusalaSoft.GatewayApi.Repository
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<Device>> GetAll();

        Task<IEnumerable<Device>> GetAllLonely();

        Task UpdateDevice(Device device);
    }
}