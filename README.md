# GatewayApi
This solution has 2 projects. 
A Web Api project for management the Gateways and Devices as required in the specifications. 
The another one is a Test XUnit project for provide the solution with meaningful tests. 

### Running Web Api
Go to GatewayApi folder. Open a terminal. The first time has to run `dotnet ef database update`. Run `dotnet run` for a Web Api server. <br />
If you want to see a Swagger you can run `dotnet watch run`, this command must open a new tab in your predetermined browser with a Swagger showing all functionalities.

### Running XUnit Test
Go to GatewayXUnit folder. Open a terminal. Run `dotnet test` for run all tests. This must show you all test passed.

## Dependencies
Install [dotnet 5](https://dotnet.microsoft.com/download/dotnet/thank-you/sdk-5.0.300-windows-x64-installer) or have an updated version of Visual Studio 2019. <br />
Install the Entity Framework Core tools with the command `dotnet tool install --global dotnet-ef`. <br />
Install any SqlServer.
