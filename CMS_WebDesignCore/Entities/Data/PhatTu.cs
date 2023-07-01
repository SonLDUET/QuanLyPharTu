using CMS_Core.Enums;
using CMS_WebDesignCore.Entities.ConfirmationCodes;
//using CMS_WebDesignCore.Entities.NguoiDung;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_WebDesignCore.Entities
{
    public class PhatTu : BaseEntity
    {
        [Required(ErrorMessage = "Không được bỏ trống trường này")]
        public string Ho { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống trường này")]
        public string Ten { get; set; }
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Required(ErrorMessage = "Không được bỏ trống trường này")]
        public string Email {get; set; }

        [Phone (ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string? SoDienThoai { get; set; }
        public string? PhapDanh { get; set; }
        public byte[]? AnhChup { get; set; } 
        public DateTime NgaySinh { get; set; }
        public DateTime? NgayXuatGia { get; set; }
        public bool DaHoanTuc { get; set; }
        public DateTime? NgayHoanTuc { get; set; }
        public GioiTinh GioiTinh { get; set; }
        public int KieuThanhVienId { get; set; }
        public KieuThanhVien? KieuThanhVien { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public int? ChuaId { get; set; }
        public Chua? Chua { get; set; }
        public string Password { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? refreshTokenExpiredTime { get; set; }
        public bool? IsTokenRevoked { get; set; }
        public bool EmailConfimed { get; set; }
        public int? CodeId { get; set; }
        public ConfirmationCode? code { get; set; }
        public IEnumerable<DonDangKy>? DonDangKys { get; set; }
        public IEnumerable<PhatTuDaoTrang>? listPhatTuDaoTrang { get; set; }
    }
}
