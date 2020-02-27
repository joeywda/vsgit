using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        //宣告Members資料表的Service物件
        private readonly MembersDBService membersService = new MembersDBService();
        public ActionResult Index()
        {
            return View();
        }

        //取得資料陣列用Action，將Page(頁數)預設為1

        public ActionResult GetDataList(string Search, int Page = 1)
        {
            //宣告一個新頁面模型
            HomeViewModel Data = new HomeViewModel();
            //將傳入值Search(搜尋)放入頁面模型中
            Data.Search = Search;
            //新增頁面模型中的分頁
            Data.Paging = new ForPaging(Page);
            //從Service中取得頁面所需陣列資料
            Data.DataList = membersService.GetDataList(Data.Paging, Data.Search);
            //將頁面資料傳入View中
            return PartialView(Data);
        }
        [HttpPost]
        //設定搜尋為接受頁面POST傳入
        //使用Bind的Inculde來定義只接受的欄位，用來避免傳入其他不相干值
        public ActionResult GetDataList([Bind(Include = "Search")]HomeViewModel Data)
        {
            //重新導向頁面至開始頁面，並傳入搜尋值
            return RedirectToAction("GetDataList", new { Search = Data.Search });
        }

    }
}