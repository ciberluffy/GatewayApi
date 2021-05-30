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
    public class DeviceControllerTest
    {
        private DeviceController _unitUnderTesting = null;

        private DatabaseFixture _fixture;

        public DeviceControllerTest(DatabaseFixture fixture)
        {
            _fixture = fixture;
            var wrapper = new RepositoryWrapper(fixture.context);
            ILogger<DeviceController> logger = new Logger<DeviceController>(new LoggerFactory());
            if (_unitUnderTesting == null)
            {
                _unitUnderTesting = new DeviceController(wrapper, logger);
            }
        }

        [Fact(DisplayName = "Add Device to Gateway")]
        public async Task AddDeviceToGateway()
        {
            var gatewayAvailable = await _fixture.context.gateways.Include(g => g.Devices).FirstOrDefaultAsync(g => g.Devices.Count < 10);
            var deviceCount = gatewayAvailable.Devices.Count;
            var devicelonely = await _fixture.context.devices.Include(g => g.Gateway).FirstOrDefaultAsync(d => d.Gateway == null);

            devicelonely.Gateway = gatewayAvailable;
            await _unitUnderTesting.Update(devicelonely);

            var deviceAfterInsert = await _fixture.context.devices.Include(g => g.Gateway).FirstOrDefaultAsync(d => d.UID == devicelonely.UID);

            gatewayAvailable.Devices.Should().HaveCount(deviceCount + 1);
            deviceAfterInsert.Gateway.USN.Should().Be(gatewayAvailable.USN);
        }

        [Fact(DisplayName = "Remove Device from Gateway")]
        public async Task RemoveDeviceFromGateway()
        {
            var gatewayWithDevice = await _fixture.context.gateways.Include(g => g.Devices).FirstOrDefaultAsync(g => g.Devices.Count > 0);
            var deviceCount = gatewayWithDevice.Devices.Count;
            var firstDevice = gatewayWithDevice.Devices.FirstOrDefault();

            firstDevice.Gateway = null;
            await _unitUnderTesting.Update(firstDevice);

            var deviceAfterRemove = await _fixture.context.devices.Include(g => g.Gateway).FirstOrDefaultAsync(d => d.UID == firstDevice.UID);

            gatewayWithDevice.Devices.Should().HaveCount(deviceCount - 1);
            deviceAfterRemove.Gateway.Should().Be(null);
        }


        [Fact(DisplayName = "Store Device")]
        public async Task StoreDeviceTest()
        {
            var devices = _fixture.context.Set<Device>().Local.ToList();
            var devicesInitialCount = devices.Count;

            var gatewayAvailable = _fixture.context.Set<Gateway>().Local.FirstOrDefault(g => g.Devices.Count < 10);
            var gatewayAvailableDevicesInitialCount = gatewayAvailable.Devices.Count;
            
            if (gatewayAvailable != null)
            {
                _fixture.context.Entry(gatewayAvailable).State = EntityState.Detached;
            }
            _fixture.context.SaveChanges();

            var deviceWithGateway = new Device()
            {
                UID = 30,
                Vendor = "vendor-30",
                Online = true,
                Created = DateTime.Now,
                Gateway = gatewayAvailable
            };

            await _unitUnderTesting.Create(deviceWithGateway);

            devices = await _fixture.context.devices.ToListAsync();
            devices.Should().HaveCount(devicesInitialCount + 1);
            gatewayAvailable = _fixture.context.Set<Gateway>().Local.FirstOrDefault(g => g.USN == gatewayAvailable.USN);
            gatewayAvailable.Devices.Should().HaveCount(gatewayAvailableDevicesInitialCount + 1);
        }
    }
}
