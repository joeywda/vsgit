using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.ViewModels
{
    //購物網站首頁用ViewModel
    public class HomeViewModel
    {
        //搜尋欄位
        [DisplayName("搜尋：")]
        public string Search { get; set; }
        //部落格列表資料
        public List<Members> DataList { get; set; }
        //分頁內容
        public ForPaging Paging { get; set; }
    }
}