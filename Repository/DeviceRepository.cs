using System.Threading.Tasks;
using System.Collections.Generic;
using MusalaSoft.GatewayApi.Model;
using MusalaSoft.GatewayApi.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MusalaSoft.GatewayApi.Repository
{
    public class DeviceRepository: RepositoryBase<Device>, IDeviceRepository
    {
        private readonly ApplicationDbContext _repositoryContext;

        public DeviceRepository(ApplicationDbContext repositoryContext): base(repositoryContext) {
            _repositoryContext = repositoryContext;
        }

        public async Task<IEnumerable<Device>> GetAll() {
            var devices = await _repositoryContext.devices.AsNoTracking<Device>().Include(d => d.Gateway).ToListAsync();
            return devices.OrderBy(x => x.UID);
        }

        public async Task<IEnumerable<Device>> GetAllLonely() {
            var devices = await _repositoryContext.devices.AsNoTracking<Device>().Where(x => x.Gateway == null).ToListAsync();
            return devices.OrderBy(x => x.UID);
        }

        public async Task CreateDevice(Device device) {
            Create(device);
            await SaveAsync();
        }

        public async Task UpdateDevice(Device device) {
            Update(device);
            await SaveAsync();
        }

        public bool Exist(int uid) {
            return _repositoryContext.devices.FirstOrDefault(d => d.UID == uid) != null;
        }
    }
}