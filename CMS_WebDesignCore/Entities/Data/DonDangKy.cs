using CMS_Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_WebDesignCore.Entities
{
    public class DonDangKy : BaseEntity
    {
        public int PhatTuId { get; set; }
        public PhatTu? PhatTu { get; set; }
        public TrangThaiDon TrangThaiDon { get; set; }
        public DateTime NgayGuiDon { get; set; }
        public DateTime? NgayXuLy { get; set; }
        public int? NguoiXuLyId { get; set; }
       // public PhatTu? NguoiXuLy { get; set; }
        public int DaoTrangId { get; set; }
        public DaoTrang? daoTrang { get; set; }    
    }
}
