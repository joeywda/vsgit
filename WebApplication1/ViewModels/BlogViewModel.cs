using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.ViewModels
{
    //部落格畫面用ViewModel
    public class BlogViewModel
    {
        //顯示會員資料
        public Members Member { get; set; }
    }
}