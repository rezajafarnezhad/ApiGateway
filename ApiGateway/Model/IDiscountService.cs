using DiscountService.Proto;
using Mapster;

namespace ApiGateway.Model;

public interface IDiscountService
{
    Task<OperationResult<DiscountInfo>> GetDiscountById(string id);
    Task<OperationResult<DiscountInfo>> GetDiscountByCode(string code);
    Task<ResponseStatus> UseDiscountBy(string code);

}
public class DiscountService : IDiscountService
{
    private readonly DiscountServiceProto.DiscountServiceProtoClient _grpcClient;
    public DiscountService(DiscountServiceProto.DiscountServiceProtoClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<OperationResult<DiscountInfo>> GetDiscountById(string id)
    {
        var data = await _grpcClient.GetDiscountByIdAsync(new RequestGetDiscountById() { Id = id });
        var response = new OperationResult<DiscountInfo>()
        {
            Data = data.Date.Adapt(new DiscountInfo())
        };

        return response;
    }

    public async Task<OperationResult<DiscountInfo>> GetDiscountByCode(string code)
    {
        var data = await _grpcClient.GetDiscountByAsync(new RequestGetDiscountBy() { Code = code });

        var response = new OperationResult<DiscountInfo>()
        {
            Data = data.Date.Adapt(new DiscountInfo())
        };

        return response;
    }

    public async Task<ResponseStatus> UseDiscountBy(string code)
    {
        var data = await _grpcClient.UseDiscountByAsync(new RequestGetDiscountBy() { Code = code });
        return data.Adapt(new ResponseStatus());
    }
}