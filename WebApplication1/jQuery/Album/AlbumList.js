//畫面載入後事件
$(function () {
    //綁定上傳圖片按鈕點擊事件
    //因為上傳圖片按鈕是後來用AJAX取回產生，故採用$(document).delegate
    $(document).delegate('#UploadModal #uploadBtn', 'click', function () {
        $('#UploadModal form').submit();
    });

    //綁定顯示圖片超連結點擊事件
    //因為顯示圖片超連結是後來用AJAX取回產生，故採用$(document).delegate
    $(document).delegate('#AlbumListBlock a.showImgLink', 'click', function (e) {
        //設定超連結變數
        var $this = $(this);
        //使用AJAX取得圖片連結
        $.ajax({
            //連結網址，使用原超連結之連結路徑
            url: $this.attr('href'),
            success: function (data) {
                //判斷回傳資料，字串長度是否大於0(代表有值)
                if (data.length > 0) {
                    //顯示圖片區塊
                    $('#showImgBlock').removeClass('hidden');
                    //更改圖片連結路徑位置
                    $('#showImg').attr('src', data);
                }
                else {
                    //隱藏圖片區塊
                    $('#showImgBlock').addClass('hidden');
                    //清空圖片連結路徑位置
                    $('#showImg').attr('src', '');
                    //提示訊息
                    alert('找無此圖片');
                }
            },
            error: function (jqXHR) {
                //隱藏圖片區塊
                $('#showImgBlock').addClass('hidden');
                //清空圖片連結路徑位置
                $('#showImg').attr('src', '');
                //提示訊息
                alert('找無此圖片');
            }
        });
        //取消原先超連結作用
        e.preventDefault();
        return false;
    });
});
