using MusalaSoft.GatewayApi.Data;

namespace MusalaSoft.GatewayApi.Repository
{
    public class RepositoryWrapper: IRepositoryWrapper
    {
        private readonly ApplicationDbContext _repoContext;

        private IGatewayRepository _gateway;
        private IDeviceRepository _device;

        public RepositoryWrapper(ApplicationDbContext repositoryContext) {
            _repoContext = repositoryContext;
        }

        public IGatewayRepository Gateway
        {
            get { return _gateway ??= new GatewayRepository(_repoContext); }
        }

        public IDeviceRepository Device
        {
            get { return _device ??= new DeviceRepository(_repoContext); }
        }
    }
}