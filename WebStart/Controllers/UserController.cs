using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebStart.Data;
using WebStart.Dto;
using WebStart.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebStart.Controllers
{
    public interface IUserInfoController
    {
        Task<IActionResult> Add(UserInfoDto userInfo);
        Task<IActionResult> Delete(int id);
        Task<IActionResult> GetAll();
        Task<IActionResult> GetById(int id);
        Task<IActionResult> Update(UserInfoDto userInfo);
       Task<ActionResult<UserInfoDto>> GetPolicyHolderByEmail(string email);
    }
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase, IUserInfoController
    {
        private readonly IUserService service;
        public UserController(IUserService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<UserInfoDto> policyHolderDtos = (List<UserInfoDto>)await service.GetAll();
            return Ok(policyHolderDtos);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await service.Delete(id);
                return Ok();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserInfoDto policyHolderDto)
        {
            await service.Add(policyHolderDto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(UserInfoDto policyHolderDto)
        {
            try
            {
                await service.Update(policyHolderDto);
                return Ok();
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var found = await service.GetById(id);
                return Ok(found);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
        }
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDto loginModel)
        {
            var user = await service.ValidateUser(loginModel.Email, loginModel.Password);

            if (user == null)
            {
                // Optional: Generate a token if you're using JWT
                return Unauthorized();
            }

            return Ok(user);
        }
        [HttpGet("getByEmail")]
        public async Task<ActionResult<UserInfoDto>> GetPolicyHolderByEmail(string email)
        {
            var policyHolder = await service.GetPolicyHolderByEmailAsync(email);

            if (policyHolder == null)
            {
                return NotFound("Policy holder not found with the given email.");
            }

            return Ok(policyHolder);
        }
    }
}
