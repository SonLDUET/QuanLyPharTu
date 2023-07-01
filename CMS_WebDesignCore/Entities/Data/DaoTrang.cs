using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_WebDesignCore.Entities
{
    public class DaoTrang : BaseEntity
    {
        public string NoiToChuc { get; set; }
        public int SoThanhVienThamGia { get; set; }
        public int NguoiChuTriId { get; set; }
        //public PhatTu? NguoiChuTri { get; set; }
        public DateTime? ThoiGianToChuc { get; set; }
        public string? NoiDung { get; set; }
        public bool DaKetThuc { get; set; }
    }
}
