using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CMS_WebDesignCore.Entities.ConfirmationCodes
{

    public class ConfirmationCode
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime CodeExpireTime { get; set; }
        public int? PhatTuId { get; set; }
        public PhatTu? PhatTu { get; set; }
    }
}
