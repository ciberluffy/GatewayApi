using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusalaSoft.GatewayApi.Model;
using MusalaSoft.GatewayApi.Repository;
using Microsoft.Data.SqlClient;

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

        [HttpGet("{uid}")]
        public async Task<Device> Get(int uid) {
            return (await _repositoryWrapper.Device.GetAll()).FirstOrDefault(d => d.UID == uid);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(Device device) {
            try {
                var exist = _repositoryWrapper.Device.Exist(device.UID);
                if(exist) {
                    return BadRequest("The device UID is already taken");
                }

                var toCreate = new Device(){
                    Created = device.Created,
                    Online = device.Online,
                    UID = device.UID,
                    Vendor = device.Vendor
                };
                await _repositoryWrapper.Device.CreateDevice(toCreate);
                var gateway = await _repositoryWrapper.Gateway.Get(device.Gateway?.USN ?? "");
                if(gateway != null) {
                    if(gateway.Devices == null) 
                        gateway.Devices = new List<Device>();
                    gateway.Devices.Add(toCreate);
                    await _repositoryWrapper.Gateway.UpdateGateway(gateway);
                }

                return Ok();
            }
            catch(SqlException ex) {
                return BadRequest(ex.Message);
            }
            catch(Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}