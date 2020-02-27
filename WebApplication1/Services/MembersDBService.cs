using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class MembersDBService
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 註冊
        //註冊新會員方法
        public void Register(Members newMember)
        {
            //將密碼Hash過
            newMember.Password = HashPassword(newMember.Password);
            //sql新增語法
            //IsAdmin 預設為0
            string sql = $@" INSERT INTO Members (Account,Password,Name,Image,Email,AuthCode,IsAdmin) VALUES 
('{newMember.Account}','{newMember.Password}','{newMember.Name}','{newMember.Image}','{newMember.Email}','{newMember.AuthCode}','0') ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
        }
        #endregion

        #region 檢查圖片類型
        public bool CheckImage(string ContentType)
        {
            //使用HTML的ContentType進行檢查
            switch (ContentType)
            {
                case "image/jpg":
                case "image/jpeg":
                case "image/png":
                    return true;
            }
            return false;
        }
        #endregion

        #region Hash密碼
        //Hash密碼用的方法
        public string HashPassword(string Password)
        {
            //宣告Hash時所添加的無意義亂數值
            string saltkey = "1q2w3e4r5t6y7u8ui9o0po7tyy";
            //將剛剛宣告的字串與密碼結合
            string saltAndPassword = String.Concat(Password, saltkey);
            //定義SHA256的HASH物件
            SHA256CryptoServiceProvider sha256Hasher = new SHA256CryptoServiceProvider();
            //取得密碼轉換成byte資料
            byte[] PasswordData = Encoding.Default.GetBytes(saltAndPassword);
            //取得Hash後byte資料
            byte[] HashDate = sha256Hasher.ComputeHash(PasswordData);
            //將Hash後byte資料轉換成string
            string Hashresult = Convert.ToBase64String(HashDate);
            //回傳Hash後結果
            return Hashresult;
        }
        #endregion

        #region 查詢一筆資料
        //藉由帳號取得單筆資料的方法
        private Members GetDataByAccount(string Account)
        {
            Members Data = new Members();
            //Sql語法
            string sql = $@" select * from Members where Account = '{Account}' ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Password = dr["Password"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Email = dr["Email"].ToString();
                Data.AuthCode = dr["AuthCode"].ToString();
                Data.IsAdmin = Convert.ToBoolean(dr["IsAdmin"]);
            }
            catch (Exception e)
            {
                //查無資料
                Data = null;
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳根據編號所取得的資料
            return Data;
        }
        #endregion

        #region 查詢一筆公開性資料
        //取得會員資料(取得非私密性資料(名字&照片&帳號))
        public Members GetDatabyAccount(string Account)
        {
            //回傳根據帳號所取得的資料
            Members Data = new Members();
            //Sql語法Item
            string sql = $@" select * from Members where Account = '{Account}' ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read(); //獲得下一筆資料直到沒有資料
                Data.Image = dr["Image"].ToString();
                Data.Name = dr["Name"].ToString();
                Data.Account = dr["Account"].ToString();
            }
            catch (Exception e)
            {
                //沒有資料傳回null
                Data = null;
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            return Data;
        }
        #endregion

        #region 查詢陣列資料
        //根據搜尋來取得資料陣列的方法
        public List<Members> GetDataList(ForPaging Paging, string Search)
        {
            //宣告要接受全部搜尋資料的物件
            List<Members> DataList = new List<Members>();
            //Sql語法
            if (!string.IsNullOrWhiteSpace(Search))
            {
                //有搜尋條件時
                SetMaxPaging(Paging, Search);
                DataList = GetAllDataList(Paging, Search);
            }
            else
            {
                //無搜尋條件時
                SetMaxPaging(Paging);
                DataList = GetAllDataList(Paging);
            }
            return DataList;
        }

        //無搜尋值的搜尋資料方法
        public List<Members> GetAllDataList(ForPaging paging)
        {
            //宣告要回傳的搜尋資料為資料庫中的Members資料表
            List<Members> DataList = new List<Members>();
            //Sql語法
            string sql = $@" select * from (select row_number() over(order by Account) as sort,* from Members ) m Where m.sort Between {(paging.NowPage - 1) * paging.ItemNum + 1} and {paging.NowPage * paging.ItemNum} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Members Data = new Members();
                    Data.Account = dr["Account"].ToString();
                    Data.Name = dr["Name"].ToString();
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳搜尋資料
            return DataList;
        }

        //有搜尋值的搜尋資料方法
        public List<Members> GetAllDataList(ForPaging paging, string Search)
        {
            //宣告要回傳的搜尋資料為資料庫中的Members資料表
            List<Members> DataList = new List<Members>();
            //Sql語法
            string sql = $@" select * from (select row_number() over(order by Account) as sort,* from Members where Account like '%{Search}%' or Name like '%{Search}%' ) m 
Where m.sort Between {(paging.NowPage - 1) * paging.ItemNum + 1} and {paging.NowPage * paging.ItemNum} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Members Data = new Members();
                    Data.Account = dr["Account"].ToString();
                    Data.Name = dr["Name"].ToString();
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //回傳搜尋資料
            return DataList;
        }

        #region 設定最大頁數方法
        //無搜尋值的設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            string sql = $@" select * from Members ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Row++;
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }

        //有搜尋值的設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging, string Search)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            string sql = $@" select * from Members Where Account like '%{Search}%' or Name like '%{Search}%' ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                //取得Sql資料
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) //獲得下一筆資料直到沒有資料
                {
                    Row++;
                }
            }
            catch (Exception e)
            {
                //丟出錯誤
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                //關閉資料庫連線
                conn.Close();
            }
            //計算所需的總頁數
            Paging.MaxPage = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Row) / Paging.ItemNum));
            //重新設定正確的頁數，避免有不正確值傳入
            Paging.SetRightPage();
        }
        #endregion
        #endregion

        #region 帳號註冊重複確認
        //確認要註冊帳號是否有被註冊過的方法
        public bool AccountCheck(string Account)
        {
            //藉由傳入帳號取得會員資料
            Members Data = GetDataByAccount(Account);
            //判斷是否有查詢到會員
            bool result = (Data == null);
            //回傳結果
            return result;
        }
        #endregion

        #region 信箱驗證
        //信箱驗證碼驗證方法
        public string EmailValidate(string Account, string AuthCode)
        {
            //取得傳入帳號的會員資料
            Members ValidateMember = GetDataByAccount(Account);
            //宣告驗證後訊息字串
            string ValidateStr = string.Empty;
            if (ValidateMember != null)
            {
                //判斷傳入驗證碼與資料庫中是否相同
                if (ValidateMember.AuthCode == AuthCode)
                {
                    //將資料庫中的驗證碼設為空
                    //sql更新語法
                    string sql = $@" update Members set AuthCode = '{string.Empty}' where Account = '{Account}' ";
                    try
                    {
                        //開啟資料庫連線
                        conn.Open();
                        //執行Sql指令
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        //丟出錯誤
                        throw new Exception(e.Message.ToString());
                    }
                    finally
                    {
                        //關閉資料庫連線
                        conn.Close();
                    }
                    ValidateStr = "帳號信箱驗證成功，現在可以登入了";
                }
                else
                {
                    ValidateStr = "驗證碼錯誤，請重新確認或再註冊";
                }
            }
            else
            {
                ValidateStr = "傳送資料錯誤，請重新確認或再註冊";
            }
            //回傳驗證訊息
            return ValidateStr;
        }
        #endregion

        #region 登入確認
        //登入帳密確認方法，並回傳驗證後訊息
        public string LoginCheck(string Account, string Password)
        {
            //取得傳入帳號的會員資料
            Members LoginMember = GetDataByAccount(Account);
            //判斷是否有此會員
            if (LoginMember != null)
            {
                //判斷是否有經過信箱驗證，有經驗證驗證碼欄位會被清空
                if (String.IsNullOrWhiteSpace(LoginMember.AuthCode))
                {
                    //進行帳號密碼確認
                    if (PasswordCheck(LoginMember, Password))
                    {
                        return "";
                    }
                    else
                    {
                        return "密碼輸入錯誤";
                    }
                }
                else
                {
                    return "此帳號尚未經過Email驗證，請去收信";
                }
            }
            else
            {
                return "無此會員帳號，請去註冊";
            }
        }
        #endregion

        #region 取得角色
        //取得會員的權限角色資料
        public string GetRole(string Account)
        {
            //宣告初始角色字串
            string Role = "User";
            //取得傳入帳號的會員資料
            Members LoginMember = GetDataByAccount(Account);
            //判斷資料庫欄位，用以確認是否為Admon
            if (LoginMember.IsAdmin)
            {
                Role += ",Admin"; //添加Admin
            }
            //回傳最後結果
            return Role;
        }
        #endregion


        #region 密碼確認
        //進行密碼確認方法
        public bool PasswordCheck(Members CheckMember, string Password)
        {
            //判斷資料庫裡的密碼資料與傳入密碼資料Hash後是否一樣
            bool result = CheckMember.Password.Equals(HashPassword(Password));
            //回傳結果
            return result;
        }
        #endregion

        #region 變更密碼
        //變更會員密碼方法，並回傳最後訊息
        public string ChangePassword(string Account, string Password, string newPassword)
        {
            //取得傳入帳號的會員資料
            Members LoginMember = GetDataByAccount(Account);
            //確認舊密碼正確性
            if (PasswordCheck(LoginMember, Password))
            {
                //將新密碼Hash後寫入資料庫中
                LoginMember.Password = HashPassword(newPassword);
                //sql修改語法
                string sql = $@" update Members set Password = '{LoginMember.Password}' where Account = '{Account}' ";
                try
                {
                    //開啟資料庫連線
                    conn.Open();
                    //執行Sql指令
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    //丟出錯誤
                    throw new Exception(e.Message.ToString());
                }
                finally
                {
                    //關閉資料庫連線
                    conn.Close();
                }
                return "密碼修改成功";
            }
            else
            {
                return "舊密碼輸入錯誤";
            }
        }
        #endregion


    }
}