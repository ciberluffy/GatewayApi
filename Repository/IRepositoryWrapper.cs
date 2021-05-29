namespace MusalaSoft.GatewayApi.Repository
{
    public interface IRepositoryWrapper
    {
        IGatewayRepository Gateway { get; }
        IDeviceRepository Device { get; }
    }
}