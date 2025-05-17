using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLTapChi.Models
{
    public class TapChiBaiVietDTO
    {
        public int IDTapChiBaiViet { get; set; }
        public string TieuDe { get; set; }
        public string TacGia { get; set; }
        public string DongTacGia { get; set; }
        public string TomTat { get; set; }
        public string TuKhoa { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayGui { get; set; }
        public DateTime? NgayXuatBan { get; set; } 
    }
}