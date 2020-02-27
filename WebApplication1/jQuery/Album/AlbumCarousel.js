var _imgNum = $('#count').val(), //相片總數變數
    _currectNum = 0, //目前所在相片位置暫存變數
    _carouselBlock, //相片輪播區塊變數
    _carouselContenter, //相片輪播相片放置區塊變數
    _carouselControl, //相片輪播控制項區塊變數
    _carouseTimer, //相片輪播定時器
    _carouseInterval = 5000, //相片輪播定時間隔時間
    _carouseAnimateTime = 700, //相片切畫動畫時間
    _carouseWidth = 800, //相片輪播寬度
    _carouseIsAnimate = false; //相片輪播是否在動畫中變數，用於當作開關

$(function () {
    //建立_carouselBlock變數，取得相片輪播區塊並儲存
    _carouselBlock = $('.myCarousel');
    //建立_carouselContenter變數，透過find方法尋找物件中的物件並儲存
    _carouselContenter = _carouselBlock.find('.carousel_container');
    //建立_carouselControl變數，透過find方法尋找物件中的物件並儲存
    _carouselControl = _carouselBlock.find('.carousel_control');
    //設定_carouselContenter圖片區塊最大寬度與圖片起始位置
    _carouselContenter.width(_carouseWidth * _imgNum).css({ left: 0 });
    _carouselControl.find('li').eq(_currectNum).addClass('active');

    //建立計時器，在計時器中呼叫CarouselToNext動作，並設定間隔時間
    _carouseTimer = setTimeout(CarouselToNext, _carouseInterval);
    _carouseIsAnimate = false;

    //綁定控制項點擊事件
    $('li', _carouselControl).click(function () {
        //呼叫CarouselTo函式，並將目前物件的index當作參數
        CarouselTo($(this).index());
    });

    //綁定上一個按鈕控制項點擊事件
    $('.carousel_previous', _carouselBlock).click(function () {
        //呼叫CarouselTo函式，並將目前物件的所在位置-1作為參數
        CarouselTo(_currectNum - 1);
    });

    //綁定下一個按鈕控制項點擊事件
    $('.carousel_next', _carouselBlock).click(function () {
        //呼叫CarouselTo函式，並將目前物件的所在位置+1作為參數
        CarouselTo(_currectNum + 1);

    });

    //自訂義function，其命名為CarouselToNext，其用途為移動至下一張圖片
    function CarouselToNext() {
        //呼叫CarouselTo函式，並將目前物件的所在位置+1作為參數
        CarouselTo(_currectNum + 1);
    }

    //自訂義function，其命名為CarouselTo可接受一參數，其用途為移動至指定圖片
    function CarouselTo(ind) {
        //判斷是否同時其他地方有動畫，若其他地方在動畫則取消此次
        if (_carouseIsAnimate)
            return false;

        //設定開始動畫
        _carouseIsAnimate = true;
        //重置計時器
        clearTimeout(_carouseTimer);

        //定義初始位置
        _carouselContenter.css({ left: -_currectNum * _carouseWidth + 'px' });

        //設定要顯示第幾個圖片
        if (ind < 0) {
            ind = _imgNum - 1;
        }
        else if (ind >= _imgNum) {
            ind = 0;
        }
        _currectNum = ind;

        //呼叫物件的animate方法，可設定物件切換的動畫
        _carouselContenter.animate({
            left: -_currectNum * _carouseWidth + 'px'
        },
            _carouseAnimateTime,
            function () { //動畫結束後function
                _carouselControl.find('li.active').removeClass('active');
                _carouselControl.find('li').eq(_currectNum).addClass('active');

                _carouseIsAnimate = false;
                _carouseTimer = setTimeout(CarouselToNext, _carouseInterval);
            });
    }
});
