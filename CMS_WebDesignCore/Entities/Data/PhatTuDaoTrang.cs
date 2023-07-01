using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_WebDesignCore.Entities
{
    public class PhatTuDaoTrang : BaseEntity
    {

        [Required(ErrorMessage = "Không được bỏ trống phật tử đăng ký")]
        public int PhatTuId { get; set; }
        public PhatTu? PhatTu { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống đạo tràng")]
        public int DaoTrangId { get; set; }
        public DaoTrang? DaoTrang { get; set; }
        [Required(ErrorMessage = "Trạng thái đơn không được bỏ trống")]
        public bool DaThamGia { get; set; }
        public string? LyDoKhongThamGia { get; set; }
    }
}
