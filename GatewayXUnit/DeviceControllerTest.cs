using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusalaSoft.GatewayApi.Controllers;
using MusalaSoft.GatewayApi.Data;
using MusalaSoft.GatewayApi.Model;
using MusalaSoft.GatewayApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GatewayXUnit
{
    [Collection("Database collection")]
    public class DeviceControllerTest
    {
        private GatewayController _unitUnderTesting = null;

        private DatabaseFixture _fixture;

        public DeviceControllerTest(DatabaseFixture fixture)
        {
            _fixture = fixture;
            var wrapper = new RepositoryWrapper(fixture.context);
            ILogger<GatewayController> logger = new Logger<GatewayController>(new LoggerFactory());
            if (_unitUnderTesting == null)
            {
                _unitUnderTesting = new GatewayController(wrapper, logger);
            }
        }

        [Fact(DisplayName = "Add Device to Gateway")]
        public async Task AddDeviceToGateway()
        {
            //var gatewayAvailable = await _fixture.context.gateways.Include(g => g.Devices).FirstOrDefaultAsync(g => g.Devices.Count < 10);

        }
    }
}
