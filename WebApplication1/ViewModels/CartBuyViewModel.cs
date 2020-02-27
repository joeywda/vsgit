using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    //購物車頁面用ViewModel
    public class CartBuyViewModel
    {
        //購物車內商品陣列
        public List<CartBuy> DataList { get; set; }
        //購物車是否已保存
        public bool isCartsave { get; set; }
    }
}