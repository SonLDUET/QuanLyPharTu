using CMS_Infrastructure.Context;
//using CMS_WebDesignCore.Entities.NguoiDung;
using CMS_WebDesignCore.Enums;
using CMS_WebDesignCore.IBusiness;
using dj_webdesigncore.AuthModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using CMS_WebDesignCore.Entities;
using SendGrid;
using SendGrid.Helpers.Mail;
using CMS_WebDesignCore.Entities.ConfirmationCodes;
using Microsoft.VisualBasic;

namespace CMS_Infrastructure.Business.AuthBusiness
{
    public class AppAuth : IAppAuth
    {

        private readonly AppDbContext db;
        private IConfiguration configuration;
        public AppAuth(AppDbContext _db, IConfiguration config) 
        {
            this.db = _db;
            this.configuration = config;
        }

        public async Task<Response<AuthDataRespon>> Login(RequestLogin request)
        {
            using (var trans = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    PhatTu phatTu = db.PhatTu.SingleOrDefault(x => x.Email == request.UserName && x.Password == request.Password);
                    if (phatTu == null)
                    {
                        return new Response<AuthDataRespon>
                        {
                            Success = (int)AuthStatusEnum.FAILED,
                            Message = "Email hoạc mật khẩu không hơp lệ"
                        };
                    }
                    
                    if (!phatTu.EmailConfimed)
                    {
                        return new Response<AuthDataRespon>
                        {
                            Message = "Please confirm email first",
                            Success = (int)AuthStatusEnum.UNACTIVATE,
                            Data = null
                        };
                    }

                    TokenModel token = await GenToken(phatTu);
                    phatTu.RefreshToken = token.RefreshToken;
                    phatTu.refreshTokenExpiredTime = DateTime.UtcNow.AddHours(1);
                    phatTu.IsTokenRevoked = false;
                    db.Update(phatTu);
                    await db.SaveChangesAsync();
                    await trans.CommitAsync();
                    //user.IssuedTime = DateTime.UtcNow;
                    return new Response<AuthDataRespon>
                    {
                        Success = (int)AuthStatusEnum.SUCCESS,
                        Message = "Authenticate success",
                        Data = new AuthDataRespon
                        {
                            id = phatTu.Id,
                            // avatar = user.UserAvatarData40x40,
                            nickName = "hehehehe",
                            email = phatTu.Email,
                            name = phatTu.Ho + " " + phatTu.Ten,
                            Token = await GenToken(phatTu),
                            role = (int)phatTu.KieuThanhVienId
                        }
                    };
                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                }
            }
            return new Response<AuthDataRespon>
            {
                Message = "Something went wrong, check again please",
                Success = (int) AuthStatusEnum.FAILED,
                Data = null
            };
        }

        public async Task<TokenModel> GenToken(PhatTu phatTu)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.UTF8.GetBytes(configuration["SecretKey:key"]);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Email, phatTu.Email),
                    new Claim(ClaimTypes.Role, db.KieuThanhVien.Find(phatTu.KieuThanhVienId).TenKieu),
                    //new Claim("LoginTime", DateTime.Now.ToString()),
                    //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                // Thời gian token có hiệu lực
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = await GenerateRefreshToken();

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<string> GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        public async Task<Response<PhatTu>> Register (PhatTu newUser)
        {
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {

                    if (newUser == null)
                    {
                        return new Response<PhatTu>
                        {
                            Message = "Invalid request",
                            Success = (int)AuthStatusEnum.FAILED,
                            Data = null
                        };
                    }
                    if (newUser.Email == null)
                    {
                        return new Response<PhatTu>
                        {
                            Message = "Email field is required",
                            Success = (int)AuthStatusEnum.INVALID_EMAIL,
                            Data = null 
                        };
                    }
                    if (newUser.Ho == null || newUser.Ten == null)
                    {
                        return new Response<PhatTu>
                        {
                            Message = "First name and last name field is required",
                            Success = (int)AuthStatusEnum.INVALID_NAME,
                            Data = null
                        };
                    }
                    if (await db.PhatTu.FirstOrDefaultAsync(x => x.Email == newUser.Email) != null)
                    {
                        return new Response<PhatTu>
                        {
                            Message = "Email Existed, please try another Email",
                            Success = (int)AuthStatusEnum.EMAIL_EXISTED,
                            Data = null
                        };
                    }

                    PhatTu newPhatTu = new PhatTu
                    {
                        Ho = newUser.Ho,
                        Ten = newUser.Ten,
                        Email = newUser.Email,
                        SoDienThoai = newUser.SoDienThoai,
                        GioiTinh = newUser.GioiTinh,
                        NgaySinh = newUser.NgaySinh,
                        Password = newUser.Password,
                        RefreshToken = null,
                        KieuThanhVienId = 3,
                        EmailConfimed = false
                    };
                    await db.AddAsync(newPhatTu);
                    await db.SaveChangesAsync();
                    await trans.CommitAsync();
                    var code = await GenerateVerificationCode(newPhatTu.Email);
                    await SendCodeToEmail(newPhatTu.Email, code);

                    return new Response<PhatTu>
                    {
                        Message = "Register Successfully",
                        Success = (int)AuthStatusEnum.SUCCESS,
                        Data = newPhatTu
                    };
                } catch (Exception ex)
                {
                    await trans.RollbackAsync();
                }
            }
            return new Response<PhatTu>
            {
                Message = "Register unseccessfully",
                Success = (int)AuthStatusEnum.SUCCESS,
                Data = null
            };
        } 

        public async Task<bool> SendMessage(string from, string to, string code)
        {
            MailMessage message = new MailMessage(from, to);
            string mailBody = $"Please use this code to verify your Email.\nYour code is: \n{code}\nThank for using!";
            message.Subject = "Email comfirmation";
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp@gmail.com", 587);
            NetworkCredential basicCredential = new NetworkCredential("Son Le Danh", "12345678");
            client.EnableSsl = false;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential;
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            return true;
        }

        public async Task<string> getEmailFromToken(string accessToken)
        {
            //request.Headers.TryGetValue("Authorization", out StringValues headerAuth);
            //var accessToken = headerAuth.First().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
            ClaimsPrincipal claimPrincipal = await GetPrincipalFromToken(accessToken);
            IEnumerable<Claim> claims = claimPrincipal.Claims;
            var Email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;

            return Email ?? "Invalid";
        }

        private async Task<ClaimsPrincipal?> GetPrincipalFromToken(string? token)
        {
            var tokenValidationParameter = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SecretKey:key"])),
                ValidateLifetime = false
            };

            var tokenhandler = new JwtSecurityTokenHandler();
            var principal = tokenhandler.ValidateToken(token, tokenValidationParameter, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token !");
            }
            return principal;
        }

        public async Task<PhatTu> GetUserInfo(string Email)
        {
            var user = await db.PhatTu.FirstOrDefaultAsync(x => x.Email == Email);
            return user; 
        }

        public async Task<Response<string>> Logout(string Email)
        {
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var phatTu = await db.PhatTu.FirstOrDefaultAsync(x => x.Email == Email);
                    if (phatTu.IsTokenRevoked == true)
                    {
                        return new Response<string>
                        {
                            Message = "You need to login first",
                            Success = (int)AuthStatusEnum.FAILED,
                            Data = ""
                        };
                    }
                    phatTu.IsTokenRevoked = true;
                    phatTu.RefreshToken = null;
                    phatTu.refreshTokenExpiredTime = DateTime.Now.AddMinutes(-10);
                    db.Update(phatTu);
                    await db.SaveChangesAsync();
                    await trans.CommitAsync();
                    return new Response<string>
                    {
                        Message = "Logout Successfully",
                        Success = (int)AuthStatusEnum.SUCCESS,
                        Data = null
                    };
                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                }
            }
            return null;
        } 

        public async Task<Response<TokenModel>> RefreshToken (TokenModel model)
        {
            var response = await ValidateIncomingToken(model.AccessToken);
            if (response.Success != (int) AuthStatusEnum.TOKEN_EXPIRED)
            {
                return new Response<TokenModel>
                {
                    Message = "Invalid operation",
                    Success = (int)AuthStatusEnum.FAILED
                };
            }
            PhatTu phatTu = response.Data;
            try { 
                var token = await GenToken(phatTu);

                return new Response<TokenModel>
                {
                    Success = (int) AuthStatusEnum.SUCCESS,
                    Message = "Renew token success",
                    Data = token
                };
            }
            catch (Exception ex)
            {
                return new Response<TokenModel>
                {
                    Success = (int) AuthStatusEnum.FAILED,
                    Message = "Something went wrong"
                };
            }

        }

        public async Task<Response<PhatTu>> ValidateIncomingToken(string accessToken)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(configuration["SecretKey:key"]);
            var tokenValidateParam = new TokenValidationParameters
            {
                //tự cấp token
                ValidateIssuer = false,
                ValidateAudience = false,

                //ký vào token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                ClockSkew = TimeSpan.Zero,

                ValidateLifetime = false //ko kiểm tra token hết hạn
            };
            
            //check 1: AccessToken valid format
            var tokenInVerification = jwtTokenHandler.ValidateToken(accessToken, tokenValidateParam, out var validatedToken);

            //check 2: Check alg
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                if (!result)//false
                {
                    return new Response<PhatTu>
                    {
                        Success = (int)AuthStatusEnum.INVALID_TOKEN,
                        Message = "Invalid token"
                    };
                }
            }

            // Check if token is revoked 
            var Email = tokenInVerification.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            var phatTu = await db.PhatTu.FirstOrDefaultAsync(x => x.Email == Email);
            if (phatTu == null)
            {
                return new Response<PhatTu>
                {
                    Message = "Email doesn't exist",
                    Success = (int)AuthStatusEnum.INVALID_TOKEN,
                    Data = null,
                };
            }
            if (phatTu.IsTokenRevoked == true)
            {
                return new Response<PhatTu>
                {
                    Message = "Please sign in first, your token has been revoked",
                    Success = (int)AuthStatusEnum.TOKEN_REVOKED,
                    Data = null
                };
            }

            //check 3: Check accessToken expire?
            var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expireDate = await ConvertUnixTimeToDateTime(utcExpireDate);
            if (expireDate < DateTime.UtcNow)
            {
                // Check if refreshToken expired?
                if (phatTu.refreshTokenExpiredTime < DateTime.UtcNow)
                {
                    return new Response<PhatTu>
                    {
                        Message = "Refresh token expired, please sign in again !",
                        Success = (int)AuthStatusEnum.REFRESH_TOKEN_EXPIRED,
                        Data = null
                    };
                }
                return new Response<PhatTu>
                {
                    Success = (int)AuthStatusEnum.TOKEN_EXPIRED,
                    Message = "Access token has expired, please use your refresh token to sign in",
                    Data = phatTu
                };
            }

            return new Response<PhatTu>
            {
                Message = "Token oke !",
                Success = (int)AuthStatusEnum.SUCCESS,
                Data = phatTu
            };
    }

        private async Task<DateTime> ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0);
            return date.AddSeconds(utcExpireDate);
        }

        public async Task<Response<PhatTu>> SendCodeToEmail(string Email,string code)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("sonld.uet@gmail.com", "Quan Ly Phat Tu");
            var subject = "Account Verification";
            if (code == null)
            {
                return new Response<PhatTu>
                {
                    Message = "Some thing went wrong when generate your verification code, please try again",
                    Success = (int)AuthStatusEnum.FAILED
                };
            }
            var to = new EmailAddress(Email, "son");
            var plainTextContent = $"Please use this code to verify your account\nYour code here: \n{code} \nYour code will expire in 10 minutes";
            var htmlContent = $"<strong>Please use this code to verify your account\nYour code here: \n{code} \nYour code will expire in 10 minutes</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return new Response<PhatTu>
            {
                Message = response.ToString(),
                Success = (int)response.StatusCode,
                Data = null
            };
        }

        public async Task<string> GenerateVerificationCode(string Email)
        {
            using (var trans = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    PhatTu phatTu = db.PhatTu.FirstOrDefault(x => x.Email == Email);
                    if (phatTu == null)
                    {
                        return null;
                    }
                    ConfirmationCode newCode = new ConfirmationCode
                    {
                        Code = new Random().Next(100000, 999999).ToString(),
                        CodeExpireTime = DateTime.UtcNow.AddMinutes(10),
                        PhatTuId = phatTu.Id
                    };
                    await db.AddAsync(newCode);
                    await db.SaveChangesAsync();
                    await trans.CommitAsync();
                    return newCode.Code;
                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                }
                return null;
            }

        }

        public async Task<Response<string>> VerifyCode(string inputCode, string Email)
        {
            var phatTu = db.PhatTu.Include(x => x.code).FirstOrDefault(x => x.Email == Email);
            if (phatTu == null)
            {
                return new Response<string>
                {
                    Message = "No code has been created",
                    Success = (int) AuthStatusEnum.FAILED,
                };
            } 
            if (phatTu.code.Code != inputCode)
            {
                return new Response<string>
                {
                    Message = "Wrong code",
                    Success = (int)AuthStatusEnum.FAILED
                };
            }
            if (phatTu.refreshTokenExpiredTime < DateTime.UtcNow)
            {
                return new Response<string>
                {
                    Message = "Your Code has expired",
                    Success = (int)AuthStatusEnum.FAILED
                };
            }
            // Success from now 
            if (!phatTu.EmailConfimed)
            {
                phatTu.EmailConfimed = true;
                phatTu.code.PhatTu.code = null;
                db.Update(phatTu);
                db.SaveChanges();
            }
            return new Response<string>
            {
                Message = "Verify successfully",
                Success = (int) AuthStatusEnum.SUCCESS
            }; 
        }

        
    }
}
