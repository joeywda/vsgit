using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using WebApplication1.Security;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class MembersController : Controller
    {
        //宣告Members資料表的Service物件
        private readonly MembersDBService membersService = new MembersDBService();
        //宣告寄信用的Service物件
        private readonly MailService mailService = new MailService();
        //宣告Cart相關的Service物件
        private readonly CartService cartService = new CartService();

        // GET: Members
        public ActionResult Index()
        {
            return View();
        }

        #region 註冊
        //註冊一開始顯示頁面
        public ActionResult Register()
        {
            //判斷使用者是否已經過登入驗證
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            //已登入則重新導向
            //否則進入註冊畫面
            return View();
        }
        //傳入註冊資料的Action
        [HttpPost]
        //設定此Action只接受頁面POST資料傳入
        public ActionResult Register(MembersRegisterViewModel RegisterMember)
        {
            //判斷頁面資料是否都經過驗證
            if (ModelState.IsValid)
            {
                if (RegisterMember.MembersImage != null)
                {
                    //使用HTML的ContentType進行檢查
                    if (membersService.CheckImage(RegisterMember.MembersImage.ContentType))
                    {
                        //取得檔名
                        string filename = Path.GetFileName(RegisterMember.MembersImage.FileName);
                        //將檔案和伺服器上路徑合併
                        string url = Path.Combine(Server.MapPath($@"~/Upload/Members/"), filename);
                        //將檔案儲存於伺服器上
                        RegisterMember.MembersImage.SaveAs(url);
                        //設定路徑
                        RegisterMember.newMember.Image = filename;
                        //將頁面資料中的密碼欄位填入
                        RegisterMember.newMember.Password = RegisterMember.Password;
                        //取得信箱驗證碼
                        string AuthCode = mailService.GetValidateCode();
                        //將信箱驗證碼填入
                        RegisterMember.newMember.AuthCode = AuthCode;
                        //呼叫Serrvice註冊新會員
                        membersService.Register(RegisterMember.newMember);
                        //取得寫好的驗證信範本內容
                        string TempMail = System.IO.File.ReadAllText(
                            Server.MapPath("~/Views/Shared/RegisterEmailTemplate.html"));
                        //宣告Email驗證用的Url
                        UriBuilder ValidateUrl = new UriBuilder(Request.Url)
                        {
                            Path = Url.Action("EmailValidate", "Members"
                            , new
                            {
                                Account = RegisterMember.newMember.Account,
                                AuthCode = AuthCode
                            })
                        };

                        //藉由Service將使用者資料填入驗證信範本中
                        string MailBody = mailService.GetRegisterMailBody(TempMail
                            , RegisterMember.newMember.Name
                            , ValidateUrl.ToString().Replace("%3F", "?"));
                        //呼叫Service寄出驗證信
                        mailService.SendRegisterMail(MailBody, RegisterMember.newMember.Email);
                        //用TempData儲存註冊訊息
                        TempData["RegisterState"] = "註冊成功，請去收信以驗證Email";
                        //重新導向頁面
                        return RedirectToAction("RegisterResult");
                    }
                    else
                    {
                        ModelState.AddModelError("MembersImage", "所上傳檔案不是圖片");
                    }
                }
                else
                {
                    ModelState.AddModelError("MembersImage", "請選擇上傳檔案");
                    //返回頁面
                    return View(RegisterMember);
                }               
            }
            //未經驗證清空密碼相關欄位
            RegisterMember.Password = null;
            RegisterMember.PasswordCheck = null;
            //將資料回填至View中
            return View(RegisterMember);
        }
        #endregion

        #region 註冊結果
        //註冊結果顯示頁面
        public ActionResult RegisterResult()
        {
            return View();
        }

        //判斷註冊帳號是否已被註冊過Action
        public JsonResult AccountCheck(MembersRegisterViewModel RegisterMember)
        {
            //呼叫Service來判斷，並回傳結果
            return Json(membersService.AccountCheck(RegisterMember.newMember.Account),
                JsonRequestBehavior.AllowGet);
        }

        //接收驗證信連結傳進來的Action
        public ActionResult EmailValidate(string Account, string AuthCode)
        {
            //用ViewData儲存，使用Service進行信箱驗證後的結果訊息
            ViewData["EmailValidate"] = membersService.EmailValidate(Account, AuthCode);
            return View();
        }
        #endregion

        #region 登入
        //登入一開始載入畫面
        public ActionResult Login()
        {
            //判斷使用者是否已經過登入驗證
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home"); //已登入則重新導向
            return View();//否則進入登入畫面
        }
        //傳入登入資料的Action
        [HttpPost] //設定此Action只接受頁面POST資料傳入
        public ActionResult Login(MembersLoginViewModel LoginMember)
        {
            //使用Service裡的方法來驗證登入的帳號密碼
            string ValidateStr = membersService.LoginCheck(LoginMember.Account, LoginMember.Password);
            //判斷驗證後結果是否有錯誤訊息
            if (String.IsNullOrEmpty(ValidateStr))
            {
                //無錯誤訊息，則登入
                //先清空Session
                HttpContext.Session.Clear();

                //取得購物車保存
                string Cart = cartService.GetCartSave(LoginMember.Account);
                //判斷是否有保存，若有則存入Session
                if (Cart != null)
                {
                    HttpContext.Session["Cart"] = Cart;
                }

                //先藉由Service取得登入者角色資料
                string RoleData = membersService.GetRole(LoginMember.Account);
                //設定JWT
                JwtService jwtService = new JwtService();
                //從Web.Config撈出資料
                //Cookie名稱
                string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
                string Token = jwtService.GenerateToken(LoginMember.Account, RoleData);
                ////產生一個Cookie
                HttpCookie cookie = new HttpCookie(cookieName);
                //設定單值
                cookie.Value = Server.UrlEncode(Token);
                //寫到用戶端
                Response.Cookies.Add(cookie);
                //設定Cookie期限
                Response.Cookies[cookieName].Expires = DateTime.Now.AddMinutes(Convert.ToInt32(WebConfigurationManager.AppSettings["ExpireMinutes"]));
                //重新導向頁面
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //有驗證錯誤訊息，加入頁面模型中
                ModelState.AddModelError("", ValidateStr);
                //將資料回填至View中
                return View(LoginMember);
            }
        }
        #endregion

        #region 登出
        //登出Action
        [Authorize] //設定此Action須登入
        public ActionResult Logout()
        {
            //使用者登出
            //Cookie名稱
            string cookieName = WebConfigurationManager.AppSettings["CookieName"].ToString();
            //清除Cookie
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Expires = DateTime.Now.AddDays(-1);
            cookie.Values.Clear();
            Response.Cookies.Set(cookie);
            //重新導向至登入Action
            return RedirectToAction("Login");
        }
        #endregion

        #region 修改密碼
        //修改密碼一開始載入頁面
        [Authorize] //設定此Action須登入
        public ActionResult ChangePassword()
        {
            return View();
        }
        //修改密碼傳入資料Action
        [Authorize] //設定此Action須登入
        [HttpPost] //設定此Action接受頁面POST資料傳入
        public ActionResult ChangePassword(MembersChangePasswordViewModel ChangeData)
        {
            //判斷頁面資料是否都經過驗證
            if (ModelState.IsValid)
            {
                ViewData["ChangeState"] = membersService.ChangePassword(User.Identity.Name, ChangeData.Password, ChangeData.NewPassword);
            }
            return View();
        }
        #endregion

    }
}