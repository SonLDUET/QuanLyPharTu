using CMS_Core.Enums;
using CMS_Infrastructure.Business;
using CMS_WebDesignCore.Entities;
//using CMS_WebDesignCore.Entities.NguoiDung;
using CMS_WebDesignCore.Enums;
using CMS_WebDesignCore.IBusiness;
using dj_webdesigncore.AuthModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Diagnostics.Contracts;

namespace CMS_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IAppAuth authServices;
        private IPhatTuServices phatTuServices;

        public AuthController (IAppAuth appAuth , IPhatTuServices _phatTuServices)
        {
            authServices = appAuth;
            phatTuServices = _phatTuServices;

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(PhatTu newUser)
        {
            var res =await authServices.Register(newUser);
            if (res.Success == (int)AuthStatusEnum.SUCCESS)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(RequestLogin request)
        {
            var res = await authServices.Login(request);    
            if (res.Success == (int) AuthStatusEnum.SUCCESS)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        [HttpGet("GetUserInfo")]
        [Authorize]
        public async Task<IActionResult> getUserInfo()
        {
            var accessToken = await GetTokenFromRequest(this.Request);
            var validateResponse = await authServices.ValidateIncomingToken(accessToken);
            if (validateResponse.Success != (int)AuthStatusEnum.SUCCESS)
            {
                return Unauthorized(validateResponse);
            }

            if (accessToken == null)
            {
                return Unauthorized("Invalid Token");
            }
            var Email = await authServices.getEmailFromToken(accessToken);
            if (Email == null)
            {
                return Unauthorized("Invalid Email in Token");
            }
            var res = await authServices.GetUserInfo(Email);
            if (res == null)
            {
                return BadRequest("Invalid user");
            }
            if (res.IsTokenRevoked == true)
            {
                return Unauthorized("Please sign in first");
            }
            if (!res.EmailConfimed)
            {
                return BadRequest("Please verify your Email first and then sign in again");
            }
            return Ok(res);
        }

        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenModel model)
        {
            var res = await authServices.RefreshToken(model);
            if (res.Success == (int) AuthStatusEnum.SUCCESS)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        [HttpPut("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var accessToken = await GetTokenFromRequest(this.Request);
            var res = await authServices.ValidateIncomingToken(accessToken);

            if (res.Success != (int)AuthStatusEnum.SUCCESS)
            {
                return Unauthorized(res);
            }
            var logoutResponse = await authServices.Logout(await authServices.getEmailFromToken(accessToken));
            return Ok(logoutResponse);
        }

        [HttpPost("VerifyEmail")]
        //[Authorize]
        public async Task<IActionResult> VerifyEmail(string Email ,string code)
        {
            var res = await authServices.VerifyCode(code, Email);
            if (res.Success != (int)AuthStatusEnum.SUCCESS)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpPut("ModifyUserInfo")]
        [Authorize]
        public async Task<IActionResult> ModifyUserInfo(PhatTu newPhatTu)
        {
            var accessToken = await GetTokenFromRequest(this.Request);
            var validateResponse = await authServices.ValidateIncomingToken(accessToken);
            if (validateResponse.Success != (int)AuthStatusEnum.SUCCESS)
            {
                return Unauthorized(validateResponse);
            }
            if (accessToken == null)
            {
                return Unauthorized("Invalid Token");
            }
            var Email = await authServices.getEmailFromToken(accessToken);
            if (Email == null)
            {
                return Unauthorized("Invalid Email in Token");
            }
            if (Email != newPhatTu.Email)
            {
                return BadRequest("Email in the form doesn't match email in token");
            }
            var res = await phatTuServices.SuaPhatTu(newPhatTu);
            if (res != PhatTuState.SUCCESS)
            {
                return BadRequest("Some thing went wrong, please try again !");
            }
            return Ok(newPhatTu);
        }

        [HttpPost("ThemDaoTrang")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ThemDaoTrang(DaoTrang newDaoTrang)
        {
            var res = await phatTuServices.MoDaoTrang(newDaoTrang);
            if (res)
            {
                return Ok("Add Dao Trang successfully");
            }
            return BadRequest("Somethings wents wrong");
        }

        [HttpPut("SuaDaoTrang")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SuaDaoTrang(DaoTrang newDaoTrang)
        {
            var res = await phatTuServices.SuaDaoTrang(newDaoTrang);
            if (res.Success == (int) AuthStatusEnum.SUCCESS)
            {
                return Ok(res);

            }
            return BadRequest(res);
        }

        [HttpPut("KetThucDaoTrang")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> KetThucDaoTrang(int DaoTrangId)
        {
            var res = await phatTuServices.KetThucDaoTrang(DaoTrangId);
            if (res)
            {
                return Ok("Đạo tràng đã được đánh dấu kết thúc !");
            }
            return BadRequest("Thao tác không thành công, vui lòng thử lại");
        }

        [HttpPost("DangKyDaoTrang")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> DangKyThamGiaDaoTrang(int DaoTrangId)
        {
            var accessToken = await GetTokenFromRequest(this.Request);
            var validateResponse = await authServices.ValidateIncomingToken(accessToken);
            if (validateResponse.Success != (int)AuthStatusEnum.SUCCESS)
            {
                return Unauthorized(validateResponse);
            }
            if (accessToken == null)
            {
                return Unauthorized("Invalid Token");
            }
            var Email = await authServices.getEmailFromToken(accessToken);
            if (Email == null)
            {
                return Unauthorized("Invalid Email in Token");
            }
            var res = await phatTuServices.DangKyThamGiaDaoTrang(Email, DaoTrangId);
            if (res)
            {
                return Ok("Dăng ký thành công");
            }
            return BadRequest("Đăng ký đạo tràng không thành công");
        }

        [HttpPut("XuLyDon")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> XuLyDon(int DonId, TrangThaiDon state)
        {
            var accessToken = await GetTokenFromRequest(this.Request);
            var validateResponse = await authServices.ValidateIncomingToken(accessToken);
            if (validateResponse.Success != (int)AuthStatusEnum.SUCCESS)
            {
                return Unauthorized(validateResponse);
            }
            if (accessToken == null)
            {
                return Unauthorized("Invalid Token");
            }
            var Email = await authServices.getEmailFromToken(accessToken);
            if (Email == null)
            {
                return Unauthorized("Invalid Email in Token");
            }
            var res = await phatTuServices.XuLyDon(Email, DonId, state);
            if (res)
            {
                return Ok("Xử lý đơn thành công");
            }
            return BadRequest("Thao tác không thành công");
        }

        [HttpPut("ChangeUserRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangUserRole(string roleName)
        {
            var accessToken = await GetTokenFromRequest(this.Request);
            var validateResponse = await authServices.ValidateIncomingToken(accessToken);
            if (validateResponse.Success != (int)AuthStatusEnum.SUCCESS)
            {
                return Unauthorized(validateResponse);
            }
            if (accessToken == null)
            {
                return Unauthorized("Invalid Token");
            }
            var Email = await authServices.getEmailFromToken(accessToken);
            if (Email == null)
            {
                return Unauthorized("Invalid Email in Token");
            }
            var res = await phatTuServices.SuaKieuThanhVienChoPhatTu(Email, roleName);
            if (res)
            {
                return Ok("Thay đổi thành công");
            }
            return BadRequest("Thay đổi không thành công");
        }

        [HttpPut("DiemDanh")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DiemDanh(int PhatTuDaoTrangId, bool isAttended, string? LyDo)
        {
            var res = await phatTuServices.DiemDanh(PhatTuDaoTrangId, isAttended, LyDo);
            if (res)
            {
                return Ok("Điểm danh thành công");
            }
            return BadRequest("Điểm danh không thành công");
        }

        [HttpGet("LocDaoTrang")]
        public async Task<IActionResult> LocDaoTrang(int KetThuc, int pageSize, int pageNumber)
        {
            var res = await phatTuServices.LocDaoTrang( KetThuc,  pageSize,  pageNumber);
            return Ok(res);
        }

        [HttpGet("LocDonDangKy")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LocDonDangKy(string? Ten, string? Email, TrangThaiDon? trangThaiDon, int pageSize, int pageNumber)
        {
            return Ok( await phatTuServices.LocDonDangKy(Ten, Email, trangThaiDon, pageSize, pageNumber));
        }


        private static async Task<string> GetTokenFromRequest(HttpRequest request)
        {
            request.Headers.TryGetValue("Authorization", out StringValues headerAuth);
            var accessToken = headerAuth.First().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
            return accessToken;
        }
    }
}
