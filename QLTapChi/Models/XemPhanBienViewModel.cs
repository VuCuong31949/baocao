using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLTapChi.Models
{
    public class XemPhanBienViewModel
    {
        public TapChiBaiViet BaiViet { get; set; }
        public List<PhanBien> DanhSachPhanBien { get; set; }
        public List<PhanCongViewModel> DanhSachPhanCong { get; set; }
    }
}