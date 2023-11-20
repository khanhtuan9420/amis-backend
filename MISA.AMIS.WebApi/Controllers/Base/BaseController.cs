using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.AMIS.WebApi.BL;
using MISA.AMIS.WebApi.Common;
using MISA.AMIS.WebApi.DL;

namespace MISA.AMIS.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ValidateModel]
    [ApiController]
    public class BaseController<T> : ControllerBase where T : IBaseEntity
    {
        protected readonly IBaseBL<T> _baseBL;

        public BaseController(IBaseBL<T> baseBL)
        {
            _baseBL = baseBL;
        }

        /// <summary>
        /// Lấy một bản ghi bằng id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetRecordByIdAsync(Guid id)
        {
            var result = await _baseBL.GetRecordByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Lấy nhiều bản ghi
        /// </summary>
        /// <param name="expand">nếu expand bằng true thì lấy hết, false thì chỉ lấy bản ghi cấp 1</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        [HttpGet]
        public virtual async Task<IActionResult> GetRecordsFilterAsync([FromQuery] bool? expand,[FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? searchString, [FromQuery] Guid? id)
        {
            object result = null;
            if(expand!=null && expand == true)
            {
                result = await _baseBL.FilterRecordExpandAsync(searchString, pageSize, page, id);
            }
            else result = await _baseBL.FilterRecordAsync(searchString, pageSize, page, id);
            return Ok(result);
        }

        /// <summary>
        /// Lấy các bản ghi con theo id của bản ghi cha và keyword
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        [HttpGet]
        [Route("{id}/children")]
        public async Task<IActionResult> GetChildrenRecordsAsync([FromQuery] string? searchString, Guid id)
        {
            IEnumerable<T> result = await _baseBL.ExpandRowAsync(searchString, id);
            return Ok(result);
        }

        /// <summary>
        /// Tạo 1 bản ghi mới
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        /// Created by: dktuan (01/11/2023)
        [HttpPost]
        public async Task<IActionResult> CreateRecordAsync([FromBody] T record)
        {
            var result = await _baseBL.CreateRecordAsync(record);
            return StatusCode(201, record.GetId());
        }

        /// <summary>
        /// Sửa thông tin một bản ghi
        /// </summary>
        /// <param name="id"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        /// Created by: dktuan (07/11/2023)
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateRecordAsync(Guid id, [FromBody] T record)
        {
            var result = await _baseBL.UpdateRecordAsync(id, record);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteRecordAsync(Guid id)
        {
            var result = await _baseBL.DeleteRecordAsync(id);
            return Ok(result);
        }
    }
}
