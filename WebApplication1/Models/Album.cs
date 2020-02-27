using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Album
    {
        //編號
        public int Alb_Id { get; set; }
        //檔名
        public string FileName { get; set; }
        //路徑
        public string Url { get; set; }
        //大小
        public int Size { get; set; }
        //檔案類型
        public string Type { get; set; }
        //會員帳號
        public string Account { get; set; }
        //上傳時間
        public DateTime CreateTime { get; set; }
        //Members資料表（外來鍵）
        //預設時就將Members物件建立完畢
        public Members Member { get; set; } = new Members();

    }
}