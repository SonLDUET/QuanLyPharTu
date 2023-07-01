//using CMS_WebDesignCore.Entities.NguoiDung;
using CMS_WebDesignCore.Enums;
using dj_webdesigncore.AuthModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CMS_WebDesignCore.Entities;

namespace CMS_WebDesignCore.IBusiness
{
    public interface IAppAuth
    {
        public Task<Response<AuthDataRespon>> Login(RequestLogin request);
        public Task<TokenModel> GenToken(PhatTu user);
        public Task<string> GenerateRefreshToken();
        public Task<Response<PhatTu>> Register(PhatTu newPhatTu);
        public Task<bool> SendMessage(string from, string to, string code);
        public Task<string> getEmailFromToken(string accessToken);
        public Task<PhatTu> GetUserInfo(string Email);
        public Task<Response<string>> Logout(string Email);
        public Task<Response<TokenModel>> RefreshToken(TokenModel model);
        public Task<Response<PhatTu>> ValidateIncomingToken(string accessToken);
        public Task<string> GenerateVerificationCode(string Email);
        public Task<Response<string>> VerifyCode(string inputCode, string Email);
    }
}
