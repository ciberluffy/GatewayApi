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

                var gateway = await _repositoryWrapper.Gateway.GetForUpdate(device.Gateway?.USN ?? "");
                if (gateway != null) {
                    if(gateway.Devices == null) 
                        gateway.Devices = new List<Device>();

                    var toInsert = device.Gateway.Devices.Where(d =>
                                        gateway.Devices.Where(d2 => d2.UID == d.UID).ToList().Count == 0
                                    ).ToList();

                    foreach (var item in toInsert)
                    {
                        gateway.Devices.Add(item);
                    }
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

        [HttpPost("update")]
        public async Task<IActionResult> Update(Device device) {
            try {
                var storeDevice = await _repositoryWrapper.Device.GetForUpdate(device.UID);
                storeDevice.Created = device.Created;
                storeDevice.Online = device.Online;
                storeDevice.Vendor = device.Vendor;
                
                if(storeDevice?.Gateway?.USN != device?.Gateway?.USN || storeDevice?.Gateway?.USN == null) {
                    if(storeDevice.Gateway != null) {
                        var gatewayToDetach = await _repositoryWrapper.Gateway.GetForUpdate(storeDevice.Gateway.USN);
                        gatewayToDetach.Devices.Remove(storeDevice);
                        await _repositoryWrapper.Gateway.UpdateGateway(gatewayToDetach);
                    }
                    if(device.Gateway != null) {
                        var gatewayToAttach = await _repositoryWrapper.Gateway.GetForUpdate(device.Gateway.USN);
                        gatewayToAttach.Devices.Add(storeDevice);
                        await _repositoryWrapper.Gateway.UpdateGateway(gatewayToAttach);
                    }
                }
                await _repositoryWrapper.Device.UpdateDevice(storeDevice);

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