using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class ItemController : Controller
    {
        //宣告Cart相關的Service物件
        private readonly CartService cartService = new CartService();
        //宣告Cart相關的Service物件
        private readonly ItemService itemService = new ItemService();

        // GET: Item
        public ActionResult Index(int Page = 1)
        {
            //宣告一個新頁面模型
            ItemViewModel Data = new ItemViewModel();
            //新增頁面模型中的分頁
            Data.Paging = new ForPaging(Page);
            //從Service中取得頁面所需陣列資料
            Data.IdList = itemService.GetIdList(Data.Paging);
            Data.ItemBlock = new List<ItemDetailViewModel>();
            foreach (var Id in Data.IdList)
            {
                //宣告一個新陣列內物件
                ItemDetailViewModel newBlock = new ItemDetailViewModel();
                //藉由Service取得商品資料
                newBlock.Data = itemService.GetDataById(Id);
                //取得Session內購物車資料
                string Cart = (HttpContext.Session["Cart"] != null)
                    ? HttpContext.Session["Cart"].ToString() : null;
                //藉由Service確認是否於購物車中
                newBlock.InCart = cartService.CheckInCart(Cart, Id);
                Data.ItemBlock.Add(newBlock);
            }
            //將頁面資料傳入View中
            return View(Data);
        }

        #region 商品頁面
        //商品頁面要根據傳入編號來決定要顯示的資料
        public ActionResult Item(int Id)
        {
            //宣告一個新頁面模型
            ItemDetailViewModel ViewData = new ItemDetailViewModel();
            //藉由Service取得商品資料
            ViewData.Data = itemService.GetDataById(Id);
            //取得Session內購物車資料
            string Cart = (HttpContext.Session["Cart"] != null)
               ? HttpContext.Session["Cart"].ToString() : null;
            //藉由Service確認是否於購物車中
            ViewData.InCart = cartService.CheckInCart(Cart, Id);
            //將資料傳入View中
            return View(ViewData);
        }
        #endregion

        #region 商品列表區塊
        //商品列表中每一個商品區塊Action
        public ActionResult ItemBlock(int Id)
        {
            //宣告一個新頁面模型
            ItemDetailViewModel ViewData = new ItemDetailViewModel();
            //藉由Service取得商品資料
            ViewData.Data = itemService.GetDataById(Id);
            //取得Session內購物車資料
            string Cart = (HttpContext.Session["Cart"] != null)
                ? HttpContext.Session["Cart"].ToString() : null;
            //藉由Service確認是否於購物車中
            ViewData.InCart = cartService.CheckInCart(Cart, Id);
            //將資料傳入View中
            return PartialView(ViewData);
        }
        #endregion

        #region 新增商品
        //新增商品一開始載入頁面
        [Authorize(Roles = "Admin")] //設定此Action只有Admin角色才可使用
        public ActionResult Create()
        {
            return View();
        }
        //新增商品傳入資料時的Action
        [Authorize(Roles = "Admin")] //設定此Action只有Admin角色才可使用
        [HttpPost] //設定此Action只接受頁面POST資料傳入
        public ActionResult Add(ItemCreateViewModel Data)
        {
            if (Data.ItemImage != null)
            {
                //取得檔名
                string filename = Path.GetFileName(Data.ItemImage.FileName);
                //將檔案和伺服器上路徑合併
                string Url = Path.Combine(Server.MapPath("~/Upload/"), filename);
                //將檔案儲存於伺服器上
                Data.ItemImage.SaveAs(Url);
                //設定路徑
                Data.NewData.Image = filename;
                //使用Service來新增一筆商品資料
                itemService.Insert(Data.NewData);
                //重新導向頁面至開始頁面
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("ItemImage", "請選擇上傳檔案");
                //返回頁面
                return View(Data);
            }
        }
        #endregion

        #region 刪除商品
        //刪除商品要根據傳入編號來刪除資料
        [Authorize(Roles = "Admin")] //設定此Action只有Admin角色才可使用
        public ActionResult Delete(int Id)
        {
            //使用Service來刪除資料
            itemService.Delete(Id);
            //重新導向頁面至開始頁面
            return RedirectToAction("Index", "Home");
        }
        #endregion

    }
}