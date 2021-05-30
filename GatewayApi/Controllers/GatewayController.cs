using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusalaSoft.GatewayApi.Model;
using MusalaSoft.GatewayApi.Repository;
using System.ComponentModel.DataAnnotations;

namespace MusalaSoft.GatewayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly ILogger<GatewayController> _logger;

        public GatewayController(IRepositoryWrapper repositoryWrapper,ILogger<GatewayController> logger)
        {
            _repositoryWrapper = repositoryWrapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<Gateway>> GetAll()
        {
            return await _repositoryWrapper.Gateway.GetAll();
        }

        [HttpGet("available")]
        public async Task<IEnumerable<Gateway>> GetAllAvailable()
        {
            return (await _repositoryWrapper.Gateway.GetAll()).Where(g => g.Devices.Count < 10).ToList();
        }

        [HttpGet("{usn}")]
        public async Task<Gateway> Get(string usn)
        {
            return (await _repositoryWrapper.Gateway.GetAll()).FirstOrDefault(g => g.USN == usn);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(Gateway gateway) {
            try {
                var toCreate = new Gateway(){
                    USN = Guid.NewGuid().ToString(),
                    Address = gateway.Address,
                    Name = gateway.Name,
                    Devices = new List<Device>()
                };
                ValidationContext context = new ValidationContext(gateway, null, null);
                List<ValidationResult> validationResults = new List<ValidationResult>();
                bool valid = Validator.TryValidateObject(gateway, context, validationResults, true);
                if (!valid)
                {
                    foreach (ValidationResult validationResult in validationResults)
                    {
                        _logger.LogError($"{validationResult.ErrorMessage}");
                    }
                    return BadRequest(validationResults.FirstOrDefault().ErrorMessage);
                }

                await _repositoryWrapper.Gateway.CreateGateway(toCreate);

                foreach(var device in gateway.Devices ?? new List<Device>()) {
                    device.Gateway = toCreate;
                    await _repositoryWrapper.Device.UpdateDevice(device);
                }

                return Ok();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(Gateway gateway) {
            try {
                ValidationContext context = new ValidationContext(gateway, null, null);
                List<ValidationResult> validationResults = new List<ValidationResult>();
                bool valid = Validator.TryValidateObject(gateway, context, validationResults, true);
                if (!valid)
                {
                    foreach (ValidationResult validationResult in validationResults)
                    {
                        _logger.LogError($"{validationResult.ErrorMessage}");
                    }
                    return BadRequest(validationResults.FirstOrDefault().ErrorMessage);
                }

                var storedGateway = await _repositoryWrapper.Gateway.GetForUpdate(gateway.USN);
                
                storedGateway.Address = gateway.Address;
                storedGateway.Name = gateway.Name;

                var toInsert = gateway.Devices.Where(d => 
                                        storedGateway.Devices.Where(d2 => d2.UID == d.UID).ToList().Count == 0
                                    ).ToList();

                var toDelete = storedGateway.Devices.Where(d => 
                                        gateway.Devices.Where(d2 => d2.UID == d.UID).ToList().Count == 0
                                    ).ToList();                               

                foreach(var device in toDelete) {
                    storedGateway.Devices.Remove(device);
                }
                foreach(var device in toInsert) {
                    device.Gateway = gateway;
                    storedGateway.Devices.Add(device);
                }
                await _repositoryWrapper.Gateway.UpdateGateway(storedGateway);

                return Ok();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
