using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace WebApplication1.Services
{
    public class MailService
    {
        private string gmail_account = "joey.yujoey.yu @gmail.com"; //Gmail帳號
        private string gmail_password = "Qweasd@12"; //Gmail密碼
        private string gmail_mail = "joey.yujoey.yu@gmail.com"; //Gmail信箱

        #region 寄會員驗證信
        //產生驗證碼方法
        public string GetValidateCode()
        {
            //設定驗證碼字元的陣列
            string[] Code ={ "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K"
        , "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y"
            , "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b"
                , "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n"
                    , "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            //宣告初始為空的驗證碼字串
            string ValidateCode = string.Empty;
            //宣告可產生隨機數值的物件
            Random rd = new Random();
            //使用迴圈產生出驗證碼
            for (int i = 0; i < 10; i++)
            {
                ValidateCode += Code[rd.Next(Code.Count())];
            }
            //回傳驗證碼
            return ValidateCode;
        }

        //將使用者資料填入驗證信範本中
        public string GetRegisterMailBody(string TempString, string UserName, string ValidateUrl)
        {
            //將使用者資料填入
            TempString = TempString.Replace("{{UserName}}", UserName);
            TempString = TempString.Replace("{{ValidateUrl}}", ValidateUrl);
            //回傳最後結果
            return TempString;
        }

        //寄驗證信的方法
        public void SendRegisterMail(string MailBody, string ToEmail)
        {
            //建立寄信用Smtp物件，這裡使用Gmail為例
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            //設定使用的Port，這裡設定Gmail所使用的
            SmtpServer.Port = 587;
            //建立使用者憑據，這裡要設定您的Gmail帳戶
            SmtpServer.Credentials = new System.Net.NetworkCredential(gmail_account, gmail_password);
            //開啟SSL
            SmtpServer.EnableSsl = true;
            //宣告信件內容物件
            MailMessage mail = new MailMessage();
            //設定來源信箱
            mail.From = new MailAddress(gmail_mail);
            //設定收信者信箱
            mail.To.Add(ToEmail);
            //設定信件主旨
            mail.Subject = "會員註冊確認信";
            //設定信件內容
            mail.Body = MailBody;
            //設定信件內容為HTML格式
            mail.IsBodyHtml = true;
            //送出信件
            SmtpServer.Send(mail);
        }
        #endregion
    }
}