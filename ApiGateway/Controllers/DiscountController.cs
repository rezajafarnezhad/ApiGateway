using ApiGateway.Model;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DiscountController : ControllerBase
{

    private readonly IDiscountService _discountService;
    public DiscountController(IDiscountService discountService)
    {
        _discountService = discountService;
    }

    [HttpGet("GetByCode")]
    public async Task<IActionResult> Get(string code)
    {
        var result = await _discountService.GetDiscountByCode(code);
        return Ok(result);
    }

    [HttpGet("GetById")]

    public async Task<IActionResult> GetById(string Id)
    {
        var result = await _discountService.GetDiscountById(Id);
        return Ok(result);
    }

    [HttpPut("UseDiscount")]
    public async Task<IActionResult> Put(string code)
    {
        var result = await _discountService.UseDiscountBy(code);
        return Ok(result);
    }
 [HttpGet("get-podcast-file/{identifier:guid}")]
 public async Task<IActionResult> GetPodcastFile(Guid identifier, CancellationToken cancellationToken)
 {
     var fileMetaData = await _academyFileQueryFacade.GetFileMetaData(identifier);
     if (fileMetaData == null)
         throw new PhoenixGeneralException("شناسه فایل یافت نشد");

     var fileModel = new GetFileDto { StorageFilePath = fileMetaData.Path };
     var totalSize = await _storage.GetFileSizeAsync(fileModel, cancellationToken);

     if (totalSize == 0)
         throw new PhoenixGeneralException("فایلی یافت نشد");


     long start = 0;
     long end = totalSize - 1;

     /*long start = 0;
     long end = 1200;*/

     var rangeHeader = Request.Headers["Range"].ToString();
     var isRangeRequest = !string.IsNullOrEmpty(rangeHeader) && rangeHeader.StartsWith("bytes=");

     if (isRangeRequest)
     {
         var range = rangeHeader.Replace("bytes=", "").Split('-');

         if (!string.IsNullOrWhiteSpace(range[0]))
             start = long.Parse(range[0]);

         if (range.Length > 1 && !string.IsNullOrWhiteSpace(range[1]))
             end = long.Parse(range[1]);

         end = Math.Min(end, totalSize - 1);

         Response.StatusCode = StatusCodes.Status206PartialContent;
         Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{totalSize}");
     }

     var length = end - start + 1;

     var stream = await _storage.GetPartialFileAsync(fileModel, start, length, cancellationToken);

     Response.Headers.Add("Accept-Ranges", "bytes");
     Response.ContentLength = length;

     return File(stream, fileMetaData.ContentType, enableRangeProcessing: false);
 }

    
}

 public async Task<Stream> GetPartialFileAsync(GetFileDto getFileModel, long start, long length, CancellationToken cancellationToken)
 {
     var memoryStream = new MemoryStream();

     var args = new GetObjectArgs()
         .WithBucket(GetBucketName)
         .WithObject(getFileModel.StorageFilePath)
         .WithOffsetAndLength(start, length)
         .WithCallbackStream(stream =>
         {
             stream.CopyTo(memoryStream);
         });

     await _minio.GetObjectAsync(args, cancellationToken);

     memoryStream.Seek(0, SeekOrigin.Begin);
     return memoryStream;
 }
