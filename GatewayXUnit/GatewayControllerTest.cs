using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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

        [Fact(DisplayName = "IPv4 validation when create device")]
        public async Task ValidateIPv4AddressWhenCreateGateway()
        {
            var goodIPv4Address = "127.0.0.1";
            var badIPv4Address = "127.0.0.257";

            var gateways = await _fixture.context.gateways.ToListAsync();
            var gatewayInitialCount = gateways.Count;

            var badGateway = new Gateway()
            {
                USN = Guid.NewGuid().ToString(),
                Name = "badAddressGateway",
                Devices = null,
                Address = badIPv4Address
            };

            var createResult = await _unitUnderTesting.Create(badGateway) as BadRequestObjectResult;
            createResult.StatusCode.Should().Be(400);
            createResult.Value.ToString().Should().Be("The Address must be \"d.d.d.d\" with d in range 0-255");
            gateways = await _fixture.context.gateways.ToListAsync();
            gateways.Should().HaveCount(gatewayInitialCount);

            var goodGateway = new Gateway()
            {
                USN = Guid.NewGuid().ToString(),
                Name = "goodAddressGateway",
                Devices = null,
                Address = goodIPv4Address
            };

            await _unitUnderTesting.Create(goodGateway);
            var gatewaysAfterCreate = await _fixture.context.gateways.ToListAsync();
            gatewaysAfterCreate.Should().HaveCount(gatewayInitialCount + 1);

        }

        [Fact(DisplayName = "Gateway List Devices Validation Count <= 10")]
        public async Task ValidateGatewayListOfDeviceLenghtLessThan10()
        {
            var gateways = await _fixture.context.gateways.ToListAsync();
            var gatewayInitialCount = gateways.Count;

            var gatewayTooManyDevices = new Gateway()
            {
                USN = Guid.NewGuid().ToString(),
                Address = "1.2.3.8",
                Name = "gateway-8",
                Devices = new List<Device>() {
                        new Device() { UID = 11, Vendor = "device-11", Created = DateTime.Parse("30/5/2021"), Online = true },
                        new Device() { UID = 12, Vendor = "device-12", Created = DateTime.Parse("30/5/2021"), Online = true },
                        new Device() { UID = 13, Vendor = "device-13", Created = DateTime.Parse("30/5/2021"), Online = true },
                        new Device() { UID = 14, Vendor = "device-14", Created = DateTime.Parse("30/5/2021"), Online = true },
                        new Device() { UID = 15, Vendor = "device-15", Created = DateTime.Parse("30/5/2021"), Online = true },
                        new Device() { UID = 16, Vendor = "device-16", Created = DateTime.Parse("30/5/2021"), Online = true },
                        new Device() { UID = 17, Vendor = "device-17", Created = DateTime.Parse("30/5/2021"), Online = true },
                        new Device() { UID = 18, Vendor = "device-18", Created = DateTime.Parse("30/5/2021"), Online = true },
                        new Device() { UID = 19, Vendor = "device-19", Created = DateTime.Parse("30/5/2021"), Online = true },
                        new Device() { UID = 20, Vendor = "device-20", Created = DateTime.Parse("30/5/2021"), Online = true },
                        new Device() { UID = 21, Vendor = "device-21", Created = DateTime.Parse("30/5/2021"), Online = true },
                    }
            };

            var createResult = await _unitUnderTesting.Create(gatewayTooManyDevices) as BadRequestObjectResult;
            createResult.StatusCode.Should().Be(400);
            createResult.Value.ToString().Should().Be("The field Devices must be a string or array type with a maximum length of '10'.");
            gateways = await _fixture.context.gateways.ToListAsync();
            gateways.Should().HaveCount(gatewayInitialCount);
        }

        [Fact(DisplayName = "Store Gateway")]
        public async Task StoreGatewayTest()
        {
            var gateways = await _fixture.context.gateways.ToListAsync();
            var gatewayInitialCount = gateways.Count;

            var gatewayWithDevices = new Gateway()
            {
                USN = Guid.NewGuid().ToString(),
                Address = "1.2.3.8",
                Name = "gateway-8",
                Devices = new List<Device>() {
                        new Device() { UID = 11, Vendor = "device-11", Created = DateTime.Parse("30/5/2021"), Online = true },
                        new Device() { UID = 12, Vendor = "device-12", Created = DateTime.Parse("30/5/2021"), Online = true },
                        new Device() { UID = 13, Vendor = "device-13", Created = DateTime.Parse("30/5/2021"), Online = true },
                        new Device() { UID = 14, Vendor = "device-14", Created = DateTime.Parse("30/5/2021"), Online = true }
                    }
            };

            await _unitUnderTesting.Create(gatewayWithDevices);
            gateways = await _fixture.context.gateways.ToListAsync();
            gateways.Should().HaveCount(gatewayInitialCount + 1);
        }
    }
}
