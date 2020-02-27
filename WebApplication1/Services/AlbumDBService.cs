using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class AlbumDBService
    {
        //建立與資料庫的連線字串
        private readonly static string cnstr = ConfigurationManager.ConnectionStrings["ASP.NET MVC"].ConnectionString;
        //建立與資料庫的連線
        private readonly SqlConnection conn = new SqlConnection(cnstr);

        #region 查詢一筆相片
        //藉由編號取的單筆資料的方法
        public Album GetDataById(int Alb_Id)
        {
            //回傳根據編號所取得的資料
            Album Data = new Album();
            //Sql語法Item
            string sql = $@" select m.*,d.Name from Album m inner join Members d on m.Account = d.Account where m.Alb_Id = {Alb_Id} ";
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
                Data.Alb_Id = Convert.ToInt32(dr["Alb_Id"]);
                Data.FileName = dr["FileName"].ToString();
                Data.Size = Convert.ToInt32(dr["Size"]);
                Data.Url = dr["Url"].ToString();
                Data.Type = dr["Type"].ToString();
                Data.Account = dr["Account"].ToString();
                Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                Data.Member.Name = dr["Name"].ToString();
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

        #region 查詢相片陣列資料
        //根據分頁以及搜尋來取得資料陣列的方法
        public List<Album> GetDataList(ForPaging Paging)
        {
            //計算所需的總頁面
            SetMaxPaging(Paging);
            //設定要接受全部搜尋資料的物件
            List<Album> DataList = new List<Album>();
            //Sql語法
            //desc為反向排序 越晚新增的越前面
            string sql = $@" select m.*,d.Name from (select row_number() over(order by CreateTime desc) as sort,* from Album ) m inner join Members d on m.Account = d.Account 
Where m.sort Between {(Paging.NowPage - 1) * Paging.ItemNum + 1} and {Paging.NowPage * Paging.ItemNum} ";
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
                    Album Data = new Album();
                    Data.Alb_Id = Convert.ToInt32(dr["Alb_Id"]);
                    Data.FileName = dr["FileName"].ToString();
                    Data.Size = Convert.ToInt32(dr["Size"]);
                    Data.Account = dr["Account"].ToString();
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    Data.Member.Name = dr["Name"].ToString();
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
            //先排序再根據分頁來回傳所需的部分資料陣列
            return DataList;
        }

        #region 設定最大頁數方法
        public void SetMaxPaging(ForPaging Paging)
        {
            //計算列數
            int Row = 0;
            //Sql語法
            string sql = $@" select * from Album ";
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

        #region 上傳檔案
        public void UploadFile(int Alb_Id, string FileName, string Url, int Size, string Type, string Account)
        {
            //Sql新增語法Album
            string sql = $@" INSERT INTO Album(Alb_Id,FileName,Url,Size,Type,Account,CreateTime) VALUES ( {Alb_Id},'{FileName}','{Url}',{Size},'{Type}','{Account}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ) ";
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

        #region 計算目前相片最新一筆的Alb_Id
        public int LastAlbumFinder()
        {
            // 宣告要回傳的值
            int Id;

            //Sql查詢語法
            string sql = $@" select top 1 * from Album order by Alb_Id desc";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                //開啟資料庫連線
                conn.Open();
                //執行Sql指令
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                // 取得SQL資料
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Id = Convert.ToInt32(dr["Alb_Id"]);
            }
            catch (Exception e)
            {
                //沒資料時Id為0
                Id = 0;
            }
            finally
            {
                conn.Close();
            }
            return Id + 1;
        }
        #endregion

        #region 刪除檔案
        //刪除資料方法
        public void Delete(int Alb_Id)
        {
            //Sql語法
            string sql = $@" Delete from Album where Alb_Id = {Alb_Id} ";
            //確保程式不會因執行錯誤而整個中斷
            try
            {
                Album Data = GetDataById(Alb_Id);
                //將檔案於Upload刪除
                File.Delete(Data.Url);
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
    }
}