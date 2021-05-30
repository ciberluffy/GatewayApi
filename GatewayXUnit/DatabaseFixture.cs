using Microsoft.EntityFrameworkCore;
using MusalaSoft.GatewayApi.Data;
using MusalaSoft.GatewayApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GatewayXUnit
{
    public class DatabaseFixture: IDisposable
    {
        private DbContextOptions<ApplicationDbContext> dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "Gateways")
            .Options;

        public ApplicationDbContext context;

        public DatabaseFixture()
        {
            context = new ApplicationDbContext(dbContextOptions);
            SeedDb();
        }

        public void Dispose()
        {
            context.devices.RemoveRange(context.devices.ToList());
            context.gateways.RemoveRange(context.gateways.ToList());
        }

        #region Private Methods

        private void SeedDb()
        {
            var gateways = new List<Gateway>()
            {
                new Gateway()
                {
                    USN = "gateway-1",
                    Address = "1.2.3.4",
                    Name = "gateway-1",
                    Devices = new List<Device> ()
                    {
                        new Device()
                        {
                            UID = 1,
                            Vendor = "device-1",
                            Created = DateTime.Parse("30/5/2021"),
                            Online = true
                        },
                        new Device()
                        {
                            UID = 2,
                            Vendor = "device-2",
                            Created = DateTime.Parse("30/5/2021"),
                            Online = false
                        }
                    }
                },
                new Gateway()
                {
                    USN = "gateway-2",
                    Address = "1.2.3.5",
                    Name = "gateway-2",
                    Devices = new List<Device> ()
                    {
                        new Device()
                        {
                            UID = 3,
                            Vendor = "device-3",
                            Created = DateTime.Parse("30/5/2021"),
                            Online = true
                        }
                    }
                },
                new Gateway()
                {
                    USN = "gateway-3",
                    Address = "1.2.3.6",
                    Name = "gateway-3",
                    Devices = new List<Device> () { }
                }
            };

            var devices = new List<Device>()
            {
                new Device() {
                    UID = 4,
                    Vendor = "device-4",
                    Created = DateTime.Parse("30/5/2021"),
                    Online = false
                },
                new Device() {
                    UID = 5,
                    Vendor = "device-5",
                    Created = DateTime.Parse("30/5/2021"),
                    Online = false
                }
            };

            context.AddRange(gateways);
            context.AddRange(devices);
            context.SaveChanges();
        }

        #endregion
    }

    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
