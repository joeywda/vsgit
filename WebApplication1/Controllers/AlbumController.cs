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
    public class AlbumController : Controller
    {
        //宣告Album資料表的Service物件
        private readonly AlbumDBService albumService = new AlbumDBService();

        #region 首頁
        [Authorize(Roles = "Admin")] //設定此Action須登入為Admin身分
        // GET: Album
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region 相片列表
        [Authorize(Roles = "Admin")] //設定此Action須登入為Admin身分
        //一開始載入Action，並設定初始頁數為1
        public ActionResult List(int Page = 1)
        {
            //宣告一個新頁面模型
            AlbumViewModel Data = new AlbumViewModel();
            //新增頁面模型中的分頁
            Data.Paging = new ForPaging(Page);
            //從Service中取得頁面所需陣列資料
            Data.FileList = albumService.GetDataList(Data.Paging);
            //將頁面資料傳入View中
            return PartialView(Data);
        }
        #endregion

        #region 上傳檔案
        [Authorize(Roles = "Admin")] //設定此Action須登入為Admin身分
        //上傳檔案表單內容用的Action
        public ActionResult Create()
        {
            return PartialView();
        }
        //上傳檔案用的Action
        [Authorize(Roles = "Admin")] //設定此Action須登入為Admin身分
        [HttpPost] //設定此Action只接受頁面POST資料傳入
                   //使用Bind的Inculde來定義只接受的欄位，用來避免傳入其他不相干值
        public ActionResult Upload([Bind(Include = "upload")]AlbumViewModel File)
        {
            //檢查是否有上傳檔案
            if (File.upload != null)
            {
                //取得最新一筆Alb_Id
                int Alb_Id = albumService.LastAlbumFinder();
                //將相片Id、檔案和伺服器上路徑合併
                //檔名名稱格式變成 Alb_Id_FileName
                string Url = Path.Combine(Server.MapPath("~/Upload/"), Alb_Id.ToString() + "_" + File.upload.FileName);
                //將檔案儲存於伺服器上
                File.upload.SaveAs(Url);
                //藉由Service將檔案資料存入資料庫
                albumService.UploadFile(Alb_Id, Alb_Id.ToString() + "_" + File.upload.FileName, Url, File.upload.ContentLength
                        , File.upload.ContentType, User.Identity.Name);
            }
            //重新導向頁面至開始頁面
            return RedirectToAction("Index");
        }
        #endregion

        #region 顯示圖片
        [Authorize(Roles = "Admin")] //設定此Action須登入為Admin身分
        //取得顯示圖片連結用Action
        public ActionResult Show(int Alb_Id)
        {
            //取得顯示圖片內容資料
            AlbumViewModel ToShow = new AlbumViewModel();
            ToShow.File = albumService.GetDataById(Alb_Id);
            //判斷是否有資料
            if (ToShow.File != null)
            {
                //使用UrlHelper產生圖片路徑
                UrlHelper urlHelper = new UrlHelper(Request.RequestContext);
                urlHelper.Content("~/Upload/" + ToShow.File.FileName);
                //回傳圖片路徑
                return Content(urlHelper.Content("~/Upload/" + ToShow.File.FileName));
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 下載檔案
        [Authorize(Roles = "Admin")] //設定此Action須登入為Admin身分
        //下載檔案用Action
        public ActionResult DownloadFile(int Alb_Id)
        {
            //取得下載檔案內容資料
            AlbumViewModel Download = new AlbumViewModel();
            Download.File = albumService.GetDataById(Alb_Id);
            //判斷是否有資料
            if (Download != null)
            {
                //將檔案讀成串流
                Stream iStream = new FileStream(Download.File.Url, FileMode.Open, FileAccess.Read, FileShare.Read);
                //回傳出檔案
                return File(iStream, Download.File.Type, Download.File.FileName);
            }
            else
            {
                //回傳JavaScript語法，呼叫alert函式顯示訊息
                return JavaScript("alert(\"無此檔案\")");
            }
        }
        #endregion

        #region 相片輪播
        public ActionResult Carousel()
        {
            //宣告一個新頁面模型
            AlbumViewModel Data = new AlbumViewModel();
            //新增頁面模型中的分頁(只顯示第一頁)
            Data.Paging = new ForPaging(1);
            //從Service中取得頁面所需陣列資料
            Data.FileList = albumService.GetDataList(Data.Paging);
            //將頁面資料傳入View中
            return View(Data);
        }
        #endregion

        #region 刪除檔案
        [Authorize(Roles = "Admin")] //設定此Action須登入為Admin身分
        //刪除檔案用Action
        public ActionResult DeleteFile(int Alb_Id)
        {
            //將檔案刪除
            albumService.Delete(Alb_Id);
            //重新導向頁面至相簿頁面
            return RedirectToAction("Index");
        }
        #endregion
    }
}