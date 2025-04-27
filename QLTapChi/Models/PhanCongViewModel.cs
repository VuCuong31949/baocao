using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLTapChi.Models
{
    public class PhanCongViewModel
    {
        public int IDPhanCong { get; set; }
        public int IDNguoiPhanBien { get; set; }
        public string TieuDeBaiViet { get; set; }
        public string TacGia { get; set; }
        public string TenLinhVuc { get; set; }
        public string NguoiPhanBien { get; set; }
        public string EmailNguoiPhanBien { get; set; }
        public int? TrangThaiPhanBien { get; set; }
        public string TrangThaiPhanBienText { get; set; }
        public DateTime NgayPhanCong { get; set; }
        public int? VongPhanBien { get; set; }
    }
}