using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.ViewModels
{
    //文章頁面用ViewModel
    public class ArticleViewModel
    {
        //文章本體
        public Article article { get; set; }
        //搜尋欄位
        [DisplayName("搜尋：")]
        public string Search { get; set; }
        //顯示留言
        public List<Message> DataList { get; set; }

    }

}