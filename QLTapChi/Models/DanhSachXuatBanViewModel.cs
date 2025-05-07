using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLTapChi.Models
{
    public class DanhSachXuatBanViewModel
    {
        public int IDTapChiBaiViet { get; set; }
        public string TieuDe { get; set; }
        public string TacGia { get; set; }
        public string TenLinhVuc { get; set; }
        public int? TrangThaiPhanBien { get; set; }
        public string TrangThaiPhanBienText { get; set; }
        public int VongPhanBien { get; set; }
        public string KetQuaPhanBien { get; set; }
        public DateTime? NgayPhanBienMoiNhat { get; set; }
    }
}