using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.AMIS.WebApi.BL;
using MISA.AMIS.WebApi.Common;

namespace MISA.AMIS.WebApi.Controllers
{
    [ValidateModel]
    public class PaymentController : BaseController<Payment>
    {
        private readonly IPaymentDetailBL _paymentDetailBL;
        private readonly IPaymentBL _paymentBL;
        public PaymentController(IPaymentBL paymentBL, IPaymentDetailBL paymentDetailBL) : base(paymentBL)
        {
            _paymentDetailBL = paymentDetailBL;
            _paymentBL = paymentBL;
        }

        public override async Task<IActionResult> GetRecordsFilterAsync([FromQuery] bool? expand, [FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? searchString, [FromQuery] Guid? id)
        {
            object result = await _paymentBL.GetPaymentsAsync(searchString, pageSize, page);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}/details")]
        public async Task<IActionResult> GetDetailsById(Guid id, [FromQuery] string? filter, [FromQuery] int? pageSize, [FromQuery] int? pageNum)
        {
            var result = await _paymentDetailBL.GetDetailsByMasterId(id, filter, pageSize, pageNum);
            return Ok(result);
        }

        [HttpGet]
        [Route("NewCode")]
        public async Task<IActionResult> GetNewCode()
        {
            var result = await _paymentBL.GetNewCode();
            return Ok(result);
        }

        [HttpPost]
        [Route("CreateMasterDetail")]
        public new async Task<IActionResult> CreateRecordAsync([FromBody] PaymentCreateDto paymentCreateDto)
        {
            var result = await _paymentBL.CreateRecordAsync(paymentCreateDto.Payment, paymentCreateDto.Details);
            return StatusCode(201, result);
        }

        [HttpGet]
        [Route("export")]
        public async Task<IActionResult> ExportExcelAsync([FromQuery] string? searchString)
        {
            var memoryStreamCopy = await _paymentBL.ExportAsync(searchString);

            return new FileStreamResult(memoryStreamCopy, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "exported_data.xlsx"
            };
        }

        [HttpPut]
        [Route("UpdateMasterDetail")]
        public new async Task<IActionResult> UpdateRecordAsync([FromBody] PaymentUpdateDto paymentUpdateDto)
        {
            await _paymentBL.UpdateRecordAsync(paymentUpdateDto.Payment, paymentUpdateDto.DetailsUpdate,paymentUpdateDto.DetailsCreate, paymentUpdateDto.DetailDeleteIds);
            return StatusCode(201);
        }

        [HttpDelete]
        [Route("DeleteMasterDetail")]
        public async Task<IActionResult> DeleteRecordAsync(List<Guid> ids)
        {
            var result = await _paymentBL.DeleteRecordsAsync(ids);
            return Ok(result);
        }
    }
}
