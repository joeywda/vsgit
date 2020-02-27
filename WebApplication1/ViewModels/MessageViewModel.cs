using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.ViewModels
{
    //留言用ViewModel
    public class MessageViewModel
    {
        //顯示資料陣列
        public List<Message> DataList { get; set; }
        //分頁內容
        public ForPaging Paging { get; set; }
        //文章編號
        public int A_Id { get; set; }
    }

}