using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Services;

namespace WebApplication1.ViewModels
{
    //商品用ViewModel
    public class ItemViewModel
    {
        //顯示資料陣列
        public List<int> IdList { get; set; }
        //商品區塊
        public List<ItemDetailViewModel> ItemBlock { get; set; }
        //分頁內容
        public ForPaging Paging { get; set; }
    }

}