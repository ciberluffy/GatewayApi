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

                foreach(var device in gateway.Devices) {
                    device.Gateway = toCreate;
                    await _repositoryWrapper.Device.UpdateDevice(device);
                }

                return Ok();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
