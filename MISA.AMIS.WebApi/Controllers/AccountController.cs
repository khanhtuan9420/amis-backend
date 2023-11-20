using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.AMIS.WebApi.BL;
using MISA.AMIS.WebApi.Common;
using MISA.AMIS.WebApi.Controllers;

namespace MISA.AMIS.WebApi.Controllers
{
    public class AccountController : BaseController<Account>
    {
        private readonly IAccountBL _accountBL;
        public AccountController(IAccountBL accountBL) : base(accountBL)
        {
            _accountBL = accountBL;
        }

        [HttpPut]
        [Route("updateActive")]
        public async Task<IActionResult> UpdateActiveStatus([FromBody] UpdateActiveStatus data)
        {
            var res = await _accountBL.UpdateActiveAsync(data.Id, data.Value, data.ForceChild);
            return Ok(res);
        }
    }
}
