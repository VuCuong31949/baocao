using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLTapChi.Models
{
    public class ChuyenNganhViewModel
    {
        public string TenChuyenNganh { get; set; }
        public System.Collections.Generic.List<BienTapVien> ThanhViens { get; set; }
    }
}