using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    //新增商品頁面用ViewModel
    public class ItemCreateViewModel
    {
        [DisplayName("商品圖片")]
        [FileExtensions(ErrorMessage = "所上傳檔案不是圖片")]
        //HttpPostedFileBase 為檔案上傳的操作類別
        public HttpPostedFileBase ItemImage { get; set; }
        //新增商品內容
        public Item NewData { get; set; }
    }

}