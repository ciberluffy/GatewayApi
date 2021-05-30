using System.Threading.Tasks;
using System.Collections.Generic;
using MusalaSoft.GatewayApi.Model;
using MusalaSoft.GatewayApi.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MusalaSoft.GatewayApi.Repository
{
    public class GatewayRepository: RepositoryBase<Gateway>, IGatewayRepository
    {
        private readonly ApplicationDbContext _repositoryContext;

        public GatewayRepository(ApplicationDbContext repositoryContext): base(repositoryContext) {
            _repositoryContext = repositoryContext;
        }

        public async Task<IEnumerable<Gateway>> GetAll() {
            var gateways = await _repositoryContext.gateways.AsNoTracking<Gateway>().Include(g => g.Devices).ToListAsync();
            return gateways.OrderBy(x => x.USN);
        }

        public async Task<Gateway> Get(string usn) {
            return (await GetAll()).FirstOrDefault(g => g.USN == usn);
        }

        public async Task<Gateway> GetForUpdate(string usn) {
            return (await _repositoryContext.gateways
                            .AsTracking<Gateway>()
                            .Include(g => g.Devices)
                            .ThenInclude(x => x.Gateway)
                            .ToListAsync())
                    .FirstOrDefault(g => g.USN == usn);
        }

        public async Task CreateGateway(Gateway gateway) {
            Create(gateway);
            await SaveAsync();
        }

        public async Task UpdateGateway(Gateway gateway) {
            Update(gateway);
            await SaveAsync();
        }
    }
}