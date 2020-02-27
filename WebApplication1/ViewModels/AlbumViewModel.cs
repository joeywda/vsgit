using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.ViewModels
{
    public class AlbumViewModel
    {
        [DisplayName("上傳圖片")]
        [FileExtensions(ErrorMessage = "所上傳檔案不是圖片")]
        public HttpPostedFileBase upload { get; set; }
        //儲存得檔案陣列
        public List<Album> FileList { get; set; }
        //分頁內容
        public ForPaging Paging { get; set; }
        //單一筆檔案
        public Album File { get; set; }


    }
}