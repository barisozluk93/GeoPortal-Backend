using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Authorization;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;

namespace UserManagement.Controllers
{
    [Route("/geoPortalApi/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("Paginate")]
        [Authorize]
        [HasPermission("UserScene.Paging.Permission")]

        public async Task<IActionResult> Paginate([FromQuery] PagingParameter pagingParameter, [FromQuery] bool? isDeleted, [FromQuery] string? nameSurname, [FromQuery] string? username, [FromQuery] string? email, [FromQuery] string? phone)
        {
            var result = await _userService.Paginate(pagingParameter, isDeleted, nameSurname, username, email, phone);
            return new OkObjectResult(result);
        }

        [HttpGet("GetSuperUserList")]
        [Authorize]
        [HasPermission("UserScene.SuperAll.Permission")]
        public async Task<IActionResult> GetSuperUserList()
        {
            var result = await _userService.GetSuperUserList();
            return new OkObjectResult(result);
        }

        [HttpPost("Save")]
        [Authorize]
        [HasPermission("UserScene.Save.Permission")]

        public async Task<IActionResult> Save([FromBody] User user)
        {
            var result = await _userService.Save(user);
            return new OkObjectResult(result);
        }

        [HttpPost("Update")]
        [Authorize]
        [HasPermission("UserScene.Edit.Permission")]

        public async Task<IActionResult> Update([FromBody] User user)
        {
            var result = await _userService.Update(user);
            return new OkObjectResult(result);
        }

        [HttpPost("UserProfileEdit")]
        [Authorize]
        [HasPermission("ProfileScene.Edit.Permission")]
        public async Task<IActionResult> UserProfileEdit([FromBody] User user)
        {
            var result = await _userService.Update(user);
            return new OkObjectResult(result);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize]
        [HasPermission("UserScene.Delete.Permission")]

        public async Task<IActionResult> Delete(long id)
        {
            var result = await _userService.Delete(id);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        [HasPermission("UserScene.Get.Permission")]
        public async Task<IActionResult> GetById(long id)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _userService.GetById(id, token);
            return new OkObjectResult(result);
        }

        [HttpGet("UserAvatarUpdate/{id}/{fileId}")]
        [Authorize]
        [HasPermission("ProfileScene.Edit.Permission")]
        public async Task<IActionResult> UserAvatarUpdate(long id, long fileId)
        {
            var result = await _userService.UserAvatarUpdate(id, fileId);
            return new OkObjectResult(result);
        }

        [HttpGet("GetUserPermissions")]
        [Authorize]
        //[HasPermission("UserScene.GetUserPermissionList.Permission")]
        public async Task<IActionResult> GetUserPermissions()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var result = await _userService.GetUserPermissions(token);
            return new OkObjectResult(result);
        }

        [HttpGet("UserAddressList/{userId}")]
        [Authorize]
        [HasPermission("ProfileScene.AddressList.Permission")]

        public async Task<IActionResult> GetUserAddresses(long userId)
        {
            var result = await _userService.GetUserAddresses(userId);
            return new OkObjectResult(result);
        }

        [HttpPost("UserAddressSave")]
        [Authorize]
        [HasPermission("ProfileScene.AddressSave.Permission")]
        public async Task<IActionResult> UserAddressSave([FromBody] UserAddress userAddress)
        {
            var result = await _userService.UserAddressSave(userAddress);
            return new OkObjectResult(result);
        }

        [HttpPost("UserAddressUpdate")]
        [Authorize]
        [HasPermission("ProfileScene.AddressEdit.Permission")]
        public async Task<IActionResult> UserAddressUpdate([FromBody] UserAddress userAddress)
        {
            var result = await _userService.UserAddressUpdate(userAddress);
            return new OkObjectResult(result);
        }

        [HttpDelete("UserAddressDelete/{id}")]
        [Authorize]
        [HasPermission("ProfileScene.AddressDelete.Permission")]
        public async Task<IActionResult> UserAddressDelete(long id)
        {
            var result = await _userService.UserAddressDelete(id);
            return new OkObjectResult(result);
        }

        [HttpGet("UserAddressById/{id}")]
        [Authorize]
        [HasPermission("ProfileScene.AddressGet.Permission")]
        public async Task<IActionResult> GetUserAddressById(long id)
        {
            var result = await _userService.GetUserAddressById(id);
            return new OkObjectResult(result);
        }
    }
}
