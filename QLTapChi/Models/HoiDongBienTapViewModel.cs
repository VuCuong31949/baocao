using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static QLTapChi.Controllers.HomeController;

namespace QLTapChi.Models
{
    public class HoiDongBienTapViewModel
    {
        public BienTapVien TongBienTap { get; set; }
        public BienTapVien PhoTongBienTap { get; set; }
        public System.Collections.Generic.List<ChuyenNganhViewModel> ChuyenNganhs { get; set; }
    }
}