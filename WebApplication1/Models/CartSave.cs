using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class CartSave
    {
        //會員編號
        public string Account { get; set; }
        //編號
        public string Cart_Id { get; set; }
        //Members資料表（外來鍵）
        //預設時就將Members物件建立完畢
        public Members Member { get; set; } = new Members();
    }
}