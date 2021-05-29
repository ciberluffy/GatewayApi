using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusalaSoft.GatewayApi.Model;
using MusalaSoft.GatewayApi.Repository;

namespace MusalaSoft.GatewayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(IRepositoryWrapper repositoryWrapper,ILogger<DeviceController> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<Device>> GetAll()
        {
            return await _repositoryWrapper.Device.GetAll();
        }

        [HttpGet("lonely")]
        public async Task<IEnumerable<Device>> GetAllLonely(){
            return await _repositoryWrapper.Device.GetAllLonely();
        }
    }
}