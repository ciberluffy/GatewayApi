using FluentAssertions;
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
    public class GatewayControllerTest
    {
        private GatewayController _unitUnderTesting = null;

        private DatabaseFixture _fixture;

        public GatewayControllerTest(DatabaseFixture fixture)
        {
            _fixture = fixture;
            var wrapper = new RepositoryWrapper(fixture.context);
            ILogger<GatewayController> logger = new Logger<GatewayController>(new LoggerFactory());
            if (_unitUnderTesting == null)
            {
                _unitUnderTesting = new GatewayController(wrapper, logger);
            }
        }

        [Fact(DisplayName = "GetAll Gateways Whit His Devices")]
        public async Task GetAllShouldReturn3Gateways()
        {
            var gateways = (await _unitUnderTesting.GetAll()).ToList();
            var contextGateways = await _fixture.context.gateways.Include(g => g.Devices).ToListAsync();

            gateways.Should().NotBeEmpty().And.HaveCount(contextGateways.Count);

            foreach (var contextGateway in contextGateways)
            {
                contextGateway.Devices.Should().HaveCount(gateways.FirstOrDefault(g => g.USN == contextGateway.USN).Devices.Count);
            }

            gateways.First().Devices.Should().NotBeEmpty().And.HaveCount(2);
        }

        [Theory]
        [InlineData("gateway-1")]
        [InlineData("gateway-2")]
        [InlineData("gateway-3")]
        public async Task GetGatewayWithHisDevices(string usn)
        {
            var gateway = await _unitUnderTesting.Get(usn);
            var contextDevices = await _fixture.context.devices.Include(d => d.Gateway).Where(d => d.Gateway.USN == usn).ToListAsync();

            gateway.Devices.Should().HaveCount(contextDevices.Count);
        }

        [Theory]
        [InlineData("gateway-1")]
        [InlineData("gateway-2")]
        [InlineData("gateway-3")]
        public async Task GetOneGatewayDetails(string usn)
        {
            var gateway = await _unitUnderTesting.Get(usn);
            var contextGateway = await _fixture.context.gateways.Include(d => d.Devices).FirstOrDefaultAsync(g => g.USN == usn);

            gateway.USN.Should().Be(contextGateway.USN);
            gateway.Name.Should().Be(contextGateway.Name);
            gateway.Address.Should().Be(contextGateway.Address);
            gateway.Devices.Should().HaveCount(contextGateway.Devices.Count);

            foreach (var device in contextGateway.Devices)
            {
                var controllerDevice = gateway.Devices.FirstOrDefault(d => d.UID == device.UID);
                device.UID.Should().Be(controllerDevice.UID);
                device.Vendor.Should().Be(controllerDevice.Vendor);
                device.Online.Should().Be(controllerDevice.Online);
                device.Created.Should().Be(controllerDevice.Created);
            }
        }

    }
}
