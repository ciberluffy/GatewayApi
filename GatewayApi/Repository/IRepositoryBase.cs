using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusalaSoft.GatewayApi.Repository
{
    public interface IRepositoryBase<T>
    {
         Task<IEnumerable<T>> FindAllAsync();
    }
}