<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pos_check_out.aspx.cs" Inherits="OBShopWeb.pos_check_out" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>門市POS系統_收銀介面</title>
    <link href="css/POS_style.css" rel="stylesheet" />
    <link href="css/POS_Consumer.css" rel="stylesheet" />
    <link href="css/Vip_style.css" rel="stylesheet" />
    <script src="js/jquery-1.9.1.js"></script>
    <script src="js/jquery.paulund_modal_box.js"></script>
     <link href="css/jquery-ui.css" rel="stylesheet" />
    <link href="css/jquery-ui.theme.min.css" rel="stylesheet" />
    <script src="js/jquery-ui.min.js"></script>

    <script src="js/jquery.signalR-2.0.3.js"></script>
    <script src="http://localhost:8080/signalr/hubs"></script>

    <script type="text/javascript">
        var OpenCardRemind = false;
        var ReturnOrderID = "";
        var RemainderNotEnough = false;
        var CardNo = "";
        var ApprovalNo = "";
        var SerialNo = "";
        var CreditCardData = "";
        var IsAllowances = false;
        var AllowanceID = 0;
        var UniformSerialNumber = "";
        var TempBarcodeID = "";
        var TempQuantity = 0;
        var condition = "buy";
        var act = "quantity";
        //付款方式
        var PayType = 1;
        var wordList = [];
        var InvoiceStartNumber = '<%=InvoiceStartNumber %>';
        var PosNo = '<%=PosNo %>';
        var HostName = '<%=HostName %>';
        var ClerkName = '<%=ck.Name %>';
        var ClerkID = '<%=ck.ID %>';
        var MachineNo = '<%=MachineNo%>';
        var InvoiceQty = 1;
        //撿貨狀態   0：出貨 1:出貨重出 2:寄倉調出 3:建議與刪單  4:瑕疵退倉  5:出貨含展售
        var pickType = 0;
        var ReturnCreditAmount = 0;
        var PosManager;
        var DiscountType= '<%=DiscountType %>';
        var DiscountLimit = '<%=DiscountLimit %>';
        var VIPDiscountType = '<%=VIPDiscountType %>';
        var VIPDiscountLimit = '<%=VIPDiscountLimit %>';
        var VIPDiscount = '<%=VIPDiscount %>';
        var DiscountClotheNum = 0;
        var TouristsDiscountAmount = parseInt('<%=TouristsDiscountAmount%>');
        var TouristsDiscountLimit = parseInt('<%=TouristsDiscountLimit%>');
        var IsVip = false;
        var UseBonus = false;
        var UseBirthdayDiscount = false;
        var last_bonus = 0;
        var bonus_multiplication = 1;
        var GetApplyVipDiscount = '<%=GetApplyVipDiscount %>'
        var PickOrderID = 0;
        var PaperReturnID = "";


        for (var i = 65; i <= 90; i++) {
            wordList.push(String.fromCharCode(i));
        }

        var ReturnType = 0;

        $(function () {

            if (GetApplyVipDiscount == "False") {
                $(".VIP").html("");
                $("#NewVIP").hide();
            }

            SignalRInitialization();
            GetClerkName();
            SetCheckOutInfo();
            $(".InputBox").focus();

            $("body").click(function () {
                $(".InputBox").focus();
                $("#VIPInputBox").focus();
            })

            //清除占量
            ClearTempStorage("");

            var Height = 560;
            var Width = 650;

            $("#SerialItemList").dialog({
                closeOnEscape: false,
                open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                resizable: false,
                width: Width,
                height: Height,
                autoOpen: false,
                modal: true,
                //position: [150,100],
                buttons: {
                    "取消": function () {
                        $(this).dialog("close");
                        $("#SerialItemList").find(".pro_infoDT").remove();

                    },
                    "確定": function () {
                        $(this).dialog("close");

                        if (TempBarcodeID.length > 0) {
                            ScanAct(TempBarcodeID, 1);
                        }
                        $("#SerialItemList").find(".pro_infoDT").remove();
                    }
                }
            });

            $("#VIPdialog").dialog({
                closeOnEscape: false,
                title:"VIP卡號資料",
                open: function (event, ui) { $(".ui-dialog-titlebar-close").hide(); },
                resizable: false,
                width: 600,
                height: 500,
                autoOpen: false,
                modal: true,
                buttons: {
                    "關閉": function () {
                        $(this).dialog("close");
                        $(".InputBox").focus();
                    }
                }
            });

            var ContentMsg = "";
            ContentMsg += '硬體偵測中，請耐心等候.....<br/><br/>';
            ContentMsg += ' <br/>';

            $('.OrderFinish').paulund_modal_box({
                title: '交易確認',
                description: ContentMsg
            });

            for (var i = 0; i < wordList.length; i++) {
                $(".EnglishWord").append($("<option></option>").attr("value", wordList[i]).text(wordList[i]));
            }
            account();

            //number keybroad
            $(".number").click(function () {

                $(".InputBox").focus();
                var num = $(this).html();


                if (act == "quantity") {
                    var quantity = $(".quantity_msg").html();
                    if (quantity.indexOf("_") > -1)
                        quantity = "";

                    quantity += num;
                    $(".quantity_msg").html(quantity);
                }

                if (act == "barcode") {
                    if ($(".InputBox").val().length >= 8)
                        return false;

                    $(".InputBox").val($(".InputBox").val() + num);
                }

                if (act == "cash") {
                    var received_msg = $(".received_msg").html();
                    if (received_msg.indexOf("_") > -1) {
                        received_msg = "";
                    }
                    $(".received_msg").html(received_msg + num);
                }

                if (act == "changeInvoice") {
                    var StartNum = $(".StartNum").html();
                    var EndNum = $(".EndNum").html();

                    if (StartNum.indexOf("_") > -1)
                        StartNum = ""

                    if (EndNum.indexOf("_") > -1)
                        EndNum = ""

                    if (StartNum.length == 8 && EndNum.length == 8)
                        return false;

                    if (StartNum.length < 8) {
                        StartNum += num;
                        $(".StartNum").html(StartNum);
                        if (StartNum.length == 8 && EndNum.indexOf("_") == -1) {
                            EndNum = parseInt(StartNum) + 249;
                            $(".EndNum").html(EndNum);
                            //fillZero
                            var ZeroLength = 8 - $(".EndNum").html().length;
                            for (var i = 0; i < ZeroLength; i++) {
                                var EndNumber = "0" + $(".EndNum").html();
                                $(".EndNum").html(EndNumber);
                            }
                        }
                    } else {
                        EndNum += num;
                        $(".EndNum").html(EndNum);
                    }
                }

                if (act == "voidedInvoice") {

                    var voidedInvoiceNum = $(".voidedInvoiceNum").html();

                    if (voidedInvoiceNum.indexOf("_") > -1)
                        voidedInvoiceNum = ""

                    if (voidedInvoiceNum.length == 8)
                        return false;

                    voidedInvoiceNum += num;
                    $(".voidedInvoiceNum").html(voidedInvoiceNum);

                }

                if (act == "settingInvoice") {
                    var ResetInvoiceNum = $(".ResetInvoiceNum").html();

                    if (ResetInvoiceNum.indexOf("_") > -1)
                        ResetInvoiceNum = ""

                    if (ResetInvoiceNum.length == 8)
                        return false;

                    ResetInvoiceNum += num;
                    $(".ResetInvoiceNum").html(ResetInvoiceNum);
                }

                if (act == "ReturnPurchase" || act == "ReCheckOut") {
                    var OrderID = $(".ReturnOrderID_msg").html();
                    OrderID += num;
                    $(".ReturnOrderID_msg").html(OrderID);
                }

                if (act == "UniformNo") {

                    var UniformNo = $(".UniformNo_msg").html();
                    if (UniformNo.length >= 8)
                        return false;

                    UniformNo += num;
                    $(".UniformNo_msg").html(UniformNo);
                }

            });

            //ESC
            $(".ESC").click(function () {

                if (condition == "return") {
                    $(".cash").html("現金交易");
                    $(".creditcard").html("信用卡交易");

                    CancelTransaction();
                }

                HideAllScreen();
                $(".Scan_barcode_msg").show();
                act = "quantity";
                $(".InputBox").val("");

                SetCheckOutInfo();
            })

            //backspace button
            $(".backspace").click(function () {
                $(".InputBox").focus();
                if (act == "quantity") {
                    var quantity_msg = $(".quantity_msg").html();
                    if (quantity_msg.indexOf("_") > -1)
                        return false;

                    var new_quantity_msg = quantity_msg.substring(0, quantity_msg.length - 1);
                    if (new_quantity_msg.length == 0) {
                        new_quantity_msg = "____"
                    }
                    $(".quantity_msg").html(new_quantity_msg);
                }

                if (act == "barcode") {
                    var num = $(".InputBox").val();
                    var NewNum = num.substring(0, num.length - 1);
                    $(".InputBox").val(NewNum);
                }

                if (act == "cash") {
                    var received_msg = $(".received_msg").html();
                    if (received_msg.indexOf("_") == -1) {
                        var new_received_msg = received_msg.substring(0, received_msg.length - 1);
                        if (new_received_msg == 0) {
                            new_received_msg = "____";
                        }
                        $(".received_msg").html(new_received_msg);
                    }
                }

                if (act == "changeInvoice") {
                    var StartNum = $(".StartNum").html();
                    var EndNum = $(".EndNum").html();

                    if (StartNum.indexOf("_") > -1 && EndNum.indexOf("_") > -1)
                        return false;

                    if (StartNum.length == 8 && EndNum.indexOf("_") == -1) {
                        var new_EndNum = EndNum.substring(0, EndNum.length - 1);
                        if (new_EndNum.length == 0) {
                            new_EndNum = "__________";
                        }
                        $(".EndNum").html(new_EndNum);

                    } else {
                        var new_StartNum = StartNum.substring(0, StartNum.length - 1);
                        if (new_StartNum.length == 0) {
                            new_StartNum = "__________";
                        }
                        $(".StartNum").html(new_StartNum);
                    }
                }

                if (act == "voidedInvoice") {
                    var voidedInvoiceNum = $(".voidedInvoiceNum").html();
                    if (voidedInvoiceNum.indexOf("_") > -1)
                        return false;

                    var new_voidedInvoiceNum = voidedInvoiceNum.substring(0, voidedInvoiceNum.length - 1);
                    if (new_voidedInvoiceNum.length == 0) {
                        new_voidedInvoiceNum = "__________";
                    }
                    $(".voidedInvoiceNum").html(new_voidedInvoiceNum);
                }

                if (act == "settingInvoice") {
                    var ResetInvoiceNum = $(".ResetInvoiceNum").html();
                    if (ResetInvoiceNum.indexOf("_") > -1)
                        return false;

                    var new_ResetInvoiceNum = ResetInvoiceNum.substring(0, ResetInvoiceNum.length - 1);
                    if (new_ResetInvoiceNum.length == 0) {
                        new_ResetInvoiceNum = "__________";
                    }
                    $(".ResetInvoiceNum").html(new_ResetInvoiceNum);
                }

                if (act == "ReturnPurchase" || act == "ReCheckOut") {
                    var OrderID = $(".ReturnOrderID_msg").html();
                    OrderID = OrderID.substring(0, OrderID.length - 1);
                    $(".ReturnOrderID_msg").html(OrderID);
                }

                if (act == "UniformNo") {
                    var UniformNo = $(".UniformNo_msg").html();
                    UniformNo = UniformNo.substring(0, UniformNo.length - 1);
                    $(".UniformNo_msg").html(UniformNo);

                }

            })

            //統一編號
            $(".uniformNumber").click(function () {
                HideAllScreen();
                $(".Uniform_No_msg").show();
                act = "UniformNo"
            })

            $(document).on("click", ".item td", function () {

                $(".item").each(function () {
                    $(this).css("background-color", "#FFFFFF")
                })

                $(this).parent("tr").css("background-color", "#FCEE55")

                if ($(this).hasClass("Delete_img")) {

                    if (condition == "return") {
                        var itemID = $(this).parent("tr").find(".itemNo").html();
                        var Qty = parseInt($("#" + itemID + "_Qty").html());
                        if (Qty == 1) {
                            $(this).parent("tr").remove();
                        } else {
                            Qty -= 1
                            var price = parseInt($(this).parent("tr").find(".price").html());
                            var amount = price * Qty;
                            console.log(amount);
                            $("#" + itemID + "_Qty").html(Qty);
                            $("#" + itemID + "_Amount").html(amount);
                        }
                          
                        account();
                        var TotalAmount = $(".TotalAmount").html();
                        var received = $(".received").html();
                        var change = parseInt(received) - parseInt(TotalAmount);
                        $(".change").html(change);
                        SetCheckOutInfo();
                    } else {
                        var ProductID = $(this).parent("tr").find(".itemNo").html();
                        var TrName = $(this).parent("tr");
                        ClearTempStorage(ProductID, TrName);
                        
                    }

                } else {

                    if (condition == "return") {
                        return false;
                    }
                    //開啟選單
                    var ProductID = $(this).parent("tr").find(".itemNo").html();
                    TempQuantity = parseInt($("#" + ProductID + "_Qty").html());
                    TempBarcodeID = "";
                    GetSerailProductByProductID(ProductID);
                }

            });

            $(".CancelTransaction").click(function () {
                CancelTransaction();

            })

            $(".changeInvoice").click(function () {
                HideAllScreen();
                $(".Change_Invoice_msg").show();
                act = "changeInvoice";
            })

            $(".cash").click(function () {
                if (condition == "buy") {
                    HideAllScreen();
                    $(".cash_msg").show();
                    act = "cash";
                    PayType = 1;
                    $("#cash_info").show();
                    $("#credit_info").hide();
                }

            })

            $(".creditcard").click(function () {
                if (condition == "buy") {
                    act = "quantity";
                    HideAllScreen();
                    $(".Scan_barcode_msg").show();
                    PayType = 2;
                    $("#cash_info").hide();
                    $("#credit_info").show();
                }
            })

            //Enter Button
            $(".enter").click(function () {
                $(".InputBox").focus();

                if (act == "quantity") {
                    scan();
                }
                if (act == "barcode") {
                    scan();
                }

                if (act == "cash") {
                    var received_msg = $(".received_msg").html();
                    if (received_msg.indexOf("_") > -1) {
                        alert("請輸入金額!"); return false;
                    }
                    $(".received").html(received_msg);
                    $(".received_msg").html("____");
                    CountingChange();
                }

                if (act == "changeInvoice") {
                    var StartNum = parseInt($(".StartNum").html());
                    var EndNum = parseInt($(".EndNum").html());
                    if ($(".wordFirst").val() == "請選擇") {
                        alert("請選擇第一個英文字")
                    } else if ($(".wordSecond").val() == "請選擇") {
                        alert("請選擇第二個英文字")
                    } else if ($(".StartNum").html().length < 8 || $(".EndNum").html().length < 8 || $(".StartNum").html().indexOf("_") > -1 || $(".EndNum").html().indexOf("_") > -1) {
                        alert("發票號碼數字長度不足")
                    } else if (EndNum < StartNum) {
                        alert("發票號碼結束小於起始")
                    } else {
                        var r = confirm("請確認是否要更換一整捲新發票    \n如果是修正跳號請選擇[設定發票號碼]");
                        if (r) {
                            SettingInvoiceStartNumAndEndNum($(".StartNum").html(), $(".EndNum").html());
                        }
                    }

                }

                if (act == "voidedInvoice") {

                    if ($(".voidedInvoiceNum").html().length < 8 || $(".voidedInvoiceNum").html().indexOf("_") > -1) {
                        alert("發票號碼數字長度不足"); return false;
                    }

                    var NumberNow = parseInt($(".InvoiceNumberNow").html().substring(2, 10));
                    var Remainder = parseInt($(".InvoiceRemainder").html());
                    var EndNumber = NumberNow + Remainder;
                    var VoidedNo = parseInt($(".voidedInvoiceNum").html());
                    var StartNumber = parseInt(InvoiceStartNumber.substring(2, 10));
                    if (VoidedNo > EndNumber) {
                        alert("發票號碼超過結束號碼：" + EndNumber); return false;
                    }
                    if (VoidedNo >= NumberNow) {
                        alert("發票號碼必須小於當前號碼：" + NumberNow); return false;
                    }
                    if (VoidedNo < StartNumber) {
                        alert("發票號碼低於起始號碼：" + StartNumber); return false;
                    }

                    var InvoiceNo = $(".wordFirstNow").val() + $(".wordSecondNow").val() + $(".voidedInvoiceNum").html();
                    voidedInvoice(InvoiceNo);
                }

                if (act == "settingInvoice") {

                    if ($(".ResetInvoiceNum").html().length < 8 || $(".ResetInvoiceNum").html().indexOf("_") > -1) {
                        alert("發票號碼數字長度不足"); return false;
                    }
                    var NumberNow = parseInt($(".InvoiceNumberNow").html().substring(2, 10));
                    var Remainder = parseInt($(".InvoiceRemainder").html());
                    var EndNumber = NumberNow + Remainder;
                    var NewNumber = parseInt($(".ResetInvoiceNum").html());
                    var StartNumber = parseInt(InvoiceStartNumber.substring(2, 10));
                    //console.log(EndNumber)
                    if (NewNumber > EndNumber) {
                        alert("發票號碼超過結束號碼：" + EndNumber); return false;
                    }
                    if (NewNumber < StartNumber) {
                        alert("發票號碼低於起始號碼：" + StartNumber); return false;
                    }
                    var ResetInvoiceNum = $(".wordFirstNow").val() + $(".wordSecondNow").val() + $(".ResetInvoiceNum").html();
                    ResettingInvoiceNum(ResetInvoiceNum);
                }


                if (act == "ReturnPurchase" || act == "ReCheckOut") {
                    var OrderID = $(".ReturnOrderID_msg").html();
                    if (OrderID.length == 0) {
                        alert("請輸入訂單編號!");
                        return false;
                    }

                    ReturnOrderID = OrderID;
                    GetOrderItemByOrderID(ReturnOrderID);
                }

                if (act == "UniformNo") {
                    var UniformNo = $(".UniformNo_msg").html();
                    if (UniformNo.length < 8) {
                        alert("統一編號數字長度不足"); return false;
                    }
                    $("#UniformNo").html(UniformNo)
                    $(".UniformNo_msg").html("");
                }
            })


            //輸入條碼
            $(".InputBarcode").click(function () {
                if (condition == "buy") {
                    HideAllScreen();
                    $(".Scan_barcode_msg").show();
                    act = "barcode"
                }
            })

            $(".settingInvoice").click(function () {

                if ($(".InvoiceNumberNow").html().length == 0) {
                    alert("請先更換發票才可以更改發票號碼")
                } else {
                    act = "settingInvoice"
                    var wordFirstNow = $(".InvoiceNumberNow").html().substring(0, 1);
                    var wordSecondNow = $(".InvoiceNumberNow").html().substring(1, 2);
                    var InvoiceNum = $(".InvoiceNumberNow").html().substring(2, 10);
                    $(".wordFirstNow").html($("<option></option>").attr("value", wordFirstNow).text(wordFirstNow));
                    $(".wordSecondNow").html($("<option></option>").attr("value", wordSecondNow).text(wordSecondNow));
                    $(".ResetInvoiceNum").html(InvoiceNum);
                    HideAllScreen();
                    $(".Setting_Invoice_msg").show();
                }


            })

            //交易完成
            $(".OrderFinish").click(function () {
                CountingChange();
                $(".InputBox").focus();
                //check Invoice
                var InvoiceRemainder = $(".InvoiceRemainder").html();
                
                if (parseInt(InvoiceRemainder) == 0 || InvoiceRemainder.length == 0) {
                    $(".paulund_inner_modal_box").html('<h2>請先更換發票!!</h2><br/><br/><br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">確定</li>');
                    return false;
                }

                //檢查發票機能不能印
                GetCheckMachineStatus();
            });

            $(".ActivitiesSetting").click(function () {
                var url = "ActivitiesSetting.aspx?ClerkID=" + ClerkID;
                window.location.href = url;
                //window.open(url ,"",' width=1024,height=768,top=0, left=0');
            })

            //LogOut
            $(".logout").click(function () {
                var clerk = {};
                localStorage.setItem('clerk', JSON.stringify(clerk));
                window.location.href = "logout.aspx";
            });

            //Return Order
            $(document).on("click", ".ReturnOrder", function () {
                $('.paulund_block_page').fadeOut().remove();
            })

            //Reload web
            $(document).on("click", ".WebReload", function () {
                CancelTransaction();
                location.reload();
            })

            //Order Submit
            $(document).on("click", ".OrderSubmit", function () {
                CheckOut();
                $(this).hide();
            })


            $(document).on("click", ".VipOrderSubmit", function () {
                if (PayType == 1) {
                    OrderFinishCash()
                }
                if (PayType == 2) {
                    OrderFinishCredit()
                }
            })

            //Return Purchase
            $(".ReturnPurchase").click(function () {
                $("#ReturnePurchaseTitle").html("退貨處理");
                ReturnPurchaseFunction();
                ReturnType = 0;
            })

            //Return Purchase No Product
            $(".ReturnPurchaseNoProduct").click(function () {
                $("#ReturnePurchaseTitle").html("無貨處理");
                ReturnPurchaseFunction();
                ReturnType = 1;
            })

            //Return Purchase submit
            $(document).on("click", ".ReturnePurchaseSubmit", function () {
                var OrderID = $(".ReturnOrderID_msg").html();
                //console.log(OrderID)
                CheckOut(OrderID);
            })

            //Return Purchase Credit
            $(document).on("click", ".ReturnePurchaseCreditCard", function () {
                    var content = '<h2>刷卡結帳</h2>刷卡金額：' + $(".TotalAmount").html() + '<br/><br/>等候回應中.....<br/><br/>';
                    $(".paulund_inner_modal_box").html(content);
                    CreditCardCheckOut(ReturnOrderID);
            });


            //CreditCard Submit
            $(document).on("click", ".CreditCardSubmit", function () {
                var content = '<h2>刷卡結帳</h2>刷卡金額：' + $(".TotalAmount").html() + '<br/><br/>等候回應中.....<br/><br/>';
                $(".paulund_inner_modal_box").html(content);
                CreditCardCheckOut()
            });


            //voided Invoice
            $(".voidedInvoice").click(function () {

                if ($(".InvoiceNumberNow").html().length == 0) {
                    alert("請先更換發票才可以更改發票號碼");
                } else {
                    act = "voidedInvoice"
                    var wordFirstNow = $(".InvoiceNumberNow").html().substring(0, 1);
                    var wordSecondNow = $(".InvoiceNumberNow").html().substring(1, 2);
                    var InvoiceNum = $(".InvoiceNumberNow").html().substring(2, 10);
                    $(".wordFirstNow").html($("<option></option>").attr("value", wordFirstNow).text(wordFirstNow));
                    $(".wordSecondNow").html($("<option></option>").attr("value", wordSecondNow).text(wordSecondNow));
                    $(".voidedInvoiceNum").html(InvoiceNum);
                    HideAllScreen();
                    $(".Voided_Invoice_msg").show();
                }

            })


            //開啟同系列商品
            $(document).on('click', ".SerialItem", function () {
                $(".SerialItem").each(function () {
                    $(this).css("background-color", "#FFFFFF")
                })
                $(this).css("background-color", "#FCEE55")

                TempBarcodeID = $(this).attr("id").replace("_barcode", "");
            });


            //折讓單填寫完畢後
            $(document).on("click", ".AllowancesFinish", function () {

                if (PayType == 1) {
                    $(".paulund_inner_modal_box").html('<h2>退貨資料</h2><br/>退款：<span style="color:red;font-size:20px">' + $(".change").html() + '</span>元<br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li><li class="icon_PrcEND ReturnePurchaseSubmit">確定</li>');
                }

                if (PayType == 2) {
                    ReturnCreditCardOrder();
                }

            })

            //避免onFocus 搶走造成無法下拉
            $(".wordFirst").click(function () {
                return false;
            })

            $(".wordSecond").click(function () {
                return false;
            })

            $(window).bind("beforeunload", function () {
                ClearTempStorage("");
            })

            //開錢箱
            $(".OpenCashBox").click(function () {
                PosManager.server.openCashDrawer();
            })

            //日結表
            $(".DayEndReport").click(function () {
                window.open("pos_day_end_report.aspx?ClerkName=" + ClerkName, "_blank", "toolbar=yes, scrollbars=yes, resizable=yes, top=0, left=0, width=500, height=768");
            })

            $(".OpenSecondWindow").click(function () {
                window.open("pos_transaction.aspx", "_blank", "resizable=yes, top=0, left=0, width=1024, height=768");

            })

            //重印撿貨單
            $(document).on("click", ".RePrintPickList", function () {
                var OrderStep = 2;
                PrintPickList(OrderStep);
            })


            //觀光客折扣
            $(".Tourists").click(function () {
                TouristsDiscountCounting();
            })

            $(".VIP").click(function () {
                if (condition == "return") {
                    return;
                }

                if (GetApplyVipDiscount == "False") {
                    return;
                }

                $("#VIPdialog").dialog('open');
            })

            $(".BirthdayDiscount").click(function () {

                if (UseBirthdayDiscount) {
                    $("#BirthdayDiscount").prop('checked', false);
                    UseBirthdayDiscount = false;
                    $("#vip_discount").text(VIPDiscount);
                    $("#vip_discount").css("color", "#000000")
                } else {
                    $("#BirthdayDiscount").prop('checked', true);
                    UseBirthdayDiscount = true;
                    $("#vip_discount").text(0.9);
                    $("#vip_discount").css("color", "red");
                }

                account();
                $(this).toggleClass("switchOn");

            });

            $('.UseBonus').click(function () {
                $(this).toggleClass("switchOn");
                if (UseBonus) {
                    $("#UseBonus").prop('checked', false);
                    UseBonus = false;
                    $(".SaleItemList").find(".BonusUse").remove();
                } else {
                    $("#UseBonus").prop('checked', true);
                    UseBonus = true;

                    var origin_bonus = parseInt($("#vip_bonus").text());
                    discount = -Math.floor(origin_bonus / 150);
                    used_bonus = discount * 150;
                    console.log(discount)
                    console.log(used_bonus)
                    var tr="";
                    tr+=' <tr class="BonusUse"> <td style="width:20px;"></td>';
                    tr+=' <td style="width:60px;" class="itemNo"></td>';
                    tr+='<td style="width:142px;">紅利折扣</td>';
                    tr += '<td style="width:48px;" class="color"></td>';
                    tr += '<td style="width:48px;"></td>';
                    tr += '<td style="width:48px;">0</td>'
                    tr += '<td style="width:48px;">0</td>'
                    tr += '<td style="width:48px;"><span class="amount">' + discount + '</span></td>'
                    tr+='<td style="width:38px;" ></td>'
                    tr += '<td style="width:38px;"></td></tr>'
                    $(".SaleItemList").append(tr);
                }
                account();
            });

            //編輯VIP
            $("#VipQuery").click(function () {
                var url = "pos_vip.aspx?vip_id=" + $("#vip_id").text();
                window.open(url, "_blank", "toolbar=yes, scrollbars=yes, resizable=yes, top=0, left=0, width=1024, height=768");
            })


            //紙本刷退
            $(document).on("click", ".PaperReturnOrder", function () {
                var htmlTag = [];
                htmlTag.push('<h2>請填寫單號</h2><br/>');
                htmlTag.push('<span style="color:red" id="PaperReturnID"></span><br/><br/><br/>');
                htmlTag.push('<li class="icon_PrcEND ExcutePaperReturn" style="float: left;">確定</li>');
                htmlTag.push('<li class="icon_PrcEND ReturnOrder">取消</li>');
                $(".paulund_inner_modal_box").html(htmlTag.join(''));

                $("#PaperReturnID").html(PaperReturnID);

            })

            //送出紙本刷退
            $(document).on("click", ".ExcutePaperReturn", function () {
                //有商品走信用卡刷退待結，無商品改為一般退貨
                if ($(".itemNo").length > 0) {
                    ChangeStatusToWaitingForCheckOut(ReturnOrderID);
                } else {
                    CheckOut(ReturnOrderID);
                }
            })

        });

        //other function start//

        //更新VIP紅利
        function UpdateVIPBonus() {
            var vip_id=$("#vip_id").text();
            var url = "?act=UpdateVIPBonus&HasUseBirthdayDiscount=" + UseBirthdayDiscount + "&vip_id=" + vip_id + "&LastBonus=" + last_bonus;
            $.get(url, function (data) {
                var json = JSON.parse(data);
                if (json.result) {
                   console.log("紅利更新成功")
                }
                else {
                   console.log("紅利更新失敗")
                }
            })

        }

        //印撿貨單
        function PrintPickList(OrderStep) {
            //如果沒有寫單成功無法扣數
            if (PickOrderID == 0) {
                return;
            }

            var ItemNoList = [];
            var ColorList = [];
            var PriceList = [];
            var QuantityList = [];
            var OldInvoiceNo = $(".InvoiceNumberNow").html();

            $(".itemNo").each(function () {
                ItemNoList.push($(this).html());
            });

            $(".color").each(function () {
                ColorList.push($(this).html());
            });

            $(".price").each(function () {
                PriceList.push(parseInt($(this).html()));
            });

            $(".quantity").each(function () {
                QuantityList.push(parseInt($(this).html()))
            });

            var ItemNoListString = JSON.stringify(ItemNoList);
            var ColorListString = JSON.stringify(ColorList);
            var PriceListString = JSON.stringify(PriceList);
            var QuantityListString = JSON.stringify(QuantityList);

            Postdata = {
                act: "RePrintPickList",
                PickOrderID:PickOrderID,
                pickType: pickType,
                ItemNoList: ItemNoListString,
                PriceList: PriceListString,
                QuantityList: QuantityListString,
                ColorList: ColorListString,
                InvoiceNumberNow: OldInvoiceNo,
                ClerkID: ClerkID,
                ClerkName:ClerkName,
                OrderStep: OrderStep,
                ReturnOrderID: ReturnOrderID
            }

            $.ajax({
                type: "POST",
                url: "?",
                async: false,
                dataType: "json",
                data: Postdata,
                success: function (data) {
                    console.log(data);
                    if (data.result) {
                        $(".paulund_inner_modal_box").html('<h2>交易完成</h2><br/>請確認發票號碼為&nbsp;<span>' + OldInvoiceNo.substring(0, 6) + '</span><span style="color: red;">' + OldInvoiceNo.substring(6, 10) + '</span><br/><br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">確定</li>');
                        CancelTransaction();
                        $(".InvoiceNumberNow").html(data.NumberNow);
                        $(".InvoiceRemainder").html(data.Remainder);
                    } else {
                        $(".paulund_inner_modal_box").html('<h2>' + data.ErrorMsg + '</h2>請點重印撿貨單<br/>如果重印失敗請點離開，並手動修正庫存<br/><li class="icon_PrcEND RePrintPickList" style="float: left;">重印撿貨單</li><li class="icon_PrcEND WebReload">離開</li>');
                    }
                },
                timeout: 60000,
                error: function (data) {
                    $(".paulund_inner_modal_box").html('<h2>撿貨失敗</h2>請點重印撿貨單<br/>如果重印失敗請點離開，並手動修正庫存<br/><li class="icon_PrcEND RePrintPickList" style="float: left;">重印撿貨單</li><li class="icon_PrcEND WebReload">離開</li>');
                }
            });
        }

        function CountingChange() {
            var TotalAmount = parseInt($(".TotalAmount").html());
            var received = parseInt($(".received").html());
            var chanege = received - TotalAmount;
            $(".change").html(chanege);
            SetCheckOutInfo();
        }

        //結帳
        function CheckOut(OrderID) {
            var ItemNoList = [];
            var ColorList = [];
            var PriceList = [];
            var QuantityList = [];
            var OriginalAmount = 0;
            var Amount = 0;
            var UniformNo = "";
            var InvoiceRemainder = parseInt($(".InvoiceRemainder").html());

            if (parseInt($("#UniformNo").html()) != 0) {
                UniformNo = $("#UniformNo").html();
            }
            $(".itemNo").each(function () {
                ItemNoList.push($(this).html());
            });

            $(".color").each(function () {
                ColorList.push($(this).html());
            });

            $(".price").each(function () {
                PriceList.push(parseInt($(this).html()));
            });
            $(".quantity").each(function () {
                QuantityList.push(parseInt($(this).html()))
            });
            $(".amount").each(function () {
                Amount += parseInt($(this).html());
            });

            $(".OriginalAmount").each(function () {
                OriginalAmount += parseInt($(this).html());
            })

            if (Amount == 0) {
                //退貨沒有商品還是可以進入結帳
                if (OrderID == undefined) {
                    $(".paulund_inner_modal_box").html('<h2>沒有結帳商品!!</h2><br/><br/><br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li>');
                    return false;
                }
            }
            if (RemainderNotEnough == true) {
                $(".paulund_inner_modal_box").html('<h2>商品數超過剩餘發票!!</h2><br/><br/><br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li>');
                return false;
            }

            if (InvoiceRemainder < 1) {
                $(".paulund_inner_modal_box").html('<h2>請先更換發票!!</h2><br/><br/><br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li>');
                return false;
            }

            if (Amount > parseInt($(".received").html())) {
                if (PayType == 1) {
                    $(".paulund_inner_modal_box").html('<h2>收現金額小於商品總額!!</h2><br/><br/><br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li>');
                    return false;
                }
            }
            var ItemNoListString = JSON.stringify(ItemNoList);
            var ColorListString = JSON.stringify(ColorList);
            var PriceListString = JSON.stringify(PriceList);
            var QuantityListString = JSON.stringify(QuantityList);
            //取得店員名稱和ID
            var clerk = JSON.parse(localStorage.getItem('clerk'));
            ClerkName = clerk.name;
            ClerkID = clerk.id;

            var received = $(".received").html();
            var change = $(".change").html();
            //開錢箱
            if (PayType == 1) {
                PosManager.server.openCashDrawer();
            }

            if (OrderID == undefined) {
                POSTCheckOut(ItemNoListString, ColorListString, PriceListString, QuantityListString, Amount, UniformNo, received, change, OriginalAmount)
            } else {
                POSTReturnePurchase(ItemNoListString, ColorListString, PriceListString, QuantityListString, Amount, OrderID, received, change, UniformNo)
            }

        }

        function POSTCheckOut(ItemNoListString, ColorListString, PriceListString, QuantityListString, Amount, UniformNo, received, change, OriginalAmount) {
            var BonusUseAmount = 0;
            var vip_id = $("#vip_id").text();
            if (UseBonus) {
                BonusUseAmount =- parseInt($(".BonusUse").find(".amount").text());
            }
            console.log("本次使用紅利：" + BonusUseAmount);

            $.ajax({
                type: "POST",
                url: "?",
                async: false,
                dataType: "json",
                data: {
                    act: "AddOrder",
                    ItemNoList: ItemNoListString,
                    ColorList: ColorListString,
                    PriceList: PriceListString,
                    QuantityList: QuantityListString,
                    OriginalAmount: OriginalAmount,
                    Amount: Amount,
                    PayType: PayType,
                    pickType: pickType,
                    PosNo: PosNo,
                    UniformNo: UniformNo,
                    ClerkID: ClerkID,
                    ClerkName: ClerkName,
                    received: received,
                    change: change,
                    CardNo: CardNo,
                    ApprovalNo: ApprovalNo,
                    CreditCardData: CreditCardData,
                    HasUseBonus:UseBonus,
                    BonusUseAmount: BonusUseAmount,
                    vip_id: vip_id
                },
                success: function (data) {
                    if (data.result) {
                        var jsonText = JSON.stringify(data.InvoiceContent);
                        PickOrderID = data.OrderID;
                        PosManager.server.printInvoice(jsonText);
                    } else {
                        $(".paulund_inner_modal_box").html('<h2>訂單寫入失敗!</h2><br/><br/><br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">確定</li>');
                    }

                },
                timeout: 60000,
                error: function (data) {
                    $(".paulund_inner_modal_box").html('<h2>交易逾時</h2><br/>若發票未印出<br/>請重新結帳及取消訂單<br/>並檢查發票機內的紙張目前是否為<br/><span>' + OldInvoiceNo.substring(0, 6) + '</span><span style="color: red;">' + OldInvoiceNo.substring(6, 10) + '</span><br/><li class="icon_PrcEND WebReload" style="float: left;">確定</li>');
                    CancelTransaction();
                }
            });

        }

        function POSTReturnePurchase(ItemNoListString, ColorListString, PriceListString, QuantityListString, Amount, OrderID, received, change, UniformNo) {
            $.ajax({
                type: "POST",
                url: "?",
                async: false,
                dataType: "json",
                data: {
                    act: "ReturnPurchase",
                    ItemNoList: ItemNoListString,
                    ColorList: ColorListString,
                    PriceList: PriceListString,
                    QuantityList: QuantityListString,
                    Amount: Amount,
                    OriginalAmount: Amount,
                    PayType: PayType,
                    OrderID: OrderID,
                    pickType: pickType,
                    PosNo: PosNo, ClerkID: ClerkID,
                    ClerkName: ClerkName,
                    received: received,
                    change: change,
                    ReturnType: ReturnType,
                    CardNo: CardNo,
                    ApprovalNo: ApprovalNo,
                    CreditCardData: CreditCardData,
                    AllowanceID: AllowanceID,
                    UniformNo: UniformNo,
                    PaperReturnID: PaperReturnID
                },
                success: function (data) {
                    console.log(data);
                    if (data.result) {
                        if (data.OrderStep == 5) {
                            //退貨完成，不需結帳
                            $(".InvoiceNumberNow").html(data.NumberNow)
                            $(".InvoiceRemainder").html(data.Remainder)
                            $(".paulund_inner_modal_box").html('<h2>退貨完成!!</h2><br/><br/><br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">確定</li>');
                            CancelTransaction();
                        }

                        if (data.OrderStep == 1){
                            //退貨完成重新結帳的回傳，故要呼叫發票機去印發票
                            var jsonText = JSON.stringify(data.InvoiceContent);
                            PosManager.server.printInvoice(jsonText);
                        }
                        
                    } else {
                        $(".paulund_inner_modal_box").html('<h2>退貨失敗!!</h2><br/><br/>請重新執行退貨流程<br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">確定</li>');
                        CancelTransaction();
                    }
                },
                timeout: 60000,
                error: function (data) {
                    if (ItemNoListString.length > 2) {
                        $(".paulund_inner_modal_box").html('<h2>交易逾時</h2><br/>請重新結帳<br/><br/><br/><br/><li class="icon_PrcEND WebReload" style="float: left;">確定</li>');
                        CancelTransaction();
                    }
                }
            });

        }

        //刷退後回寫訂單
        function ReturnCreditCardOrder() {

            var content = '<h2>信用卡刷退資料</h2>刷退金額：<span class="allprc">' + ReturnCreditAmount + '</span>';
            content += '<br/>授權碼：<span class="allprc">' + ApprovalNo + '</span>';
            content += '<br/>序號：<span class="allprc">' + SerialNo + '</span>';
            content += '<br/><br/>請耐心等候系統回應....</span>';

            $(".paulund_inner_modal_box").html(content);
            PosManager.server.creditcardReturn(ReturnCreditAmount, ApprovalNo);
        }

        //設定發票起始和結束
        function SettingInvoiceStartNumAndEndNum(StartNum, EndNum) {
            InvoiceStartNumber = $(".wordFirst").val() + $(".wordSecond").val() + StartNum;
            var InvoiceEndNumber = $(".wordFirst").val() + $(".wordSecond").val() + EndNum;
            var InvoiceRemainder = parseInt(EndNum) - parseInt(StartNum) + 1;
            var url = "?act=SettingInvoiceNumber&PosNo=" + PosNo + "&InvoiceStartNumber=" + InvoiceStartNumber + "&InvoiceEndNumber=" + InvoiceEndNumber + "&ClerkID=" + ClerkID;

            $.get(url, function (data) {
                var json = JSON.parse(data);
                if (json.result) {
                    console.log(json.data)
                    $(".InvoiceNumberNow").html(InvoiceStartNumber);
                    $(".InvoiceRemainder").html(InvoiceRemainder);
                    alert("更換成功");
                }
                else {
                    alert("更新失敗");
                }
            })
        }

        //作廢跳號發票
        function voidedInvoice(InvoiceNo) {
            var url = "?act=VoidedInvoice&PosNo=" + PosNo + "&InvoiceNo=" + InvoiceNo + "&ClerkID=" + ClerkID;
            $.get(url, function (data) {
                var json = JSON.parse(data);
                if (json.result) {
                    $(".InvoiceNumberNow").html(json.NumberNow);
                    $(".InvoiceRemainder").html(json.Remainder);
                    alert(json.Message);
                }
                else {
                    alert(json.Message);
                }
            })
        }

        //發票跳號修正
        function ResettingInvoiceNum(InvoiceNo) {
            var url = "?act=ReSettingInvoiceNumber&PosNo=" + PosNo + "&InvoiceNo=" + InvoiceNo + "&ClerkID=" + ClerkID;
            $.get(url, function (data) {
                var json = JSON.parse(data);
                if (json.result) {
                    $(".InvoiceNumberNow").html(json.NumberNow);
                    $(".InvoiceRemainder").html(json.Remainder);
                    alert(json.Message);
                }
                else {
                    alert(json.Message);
                }
            })
        }

        //輸入產品編號
        function scan() {
            var barcode = $(".InputBox").val()

            if (barcode.match(/^\d{8}$/) || barcode.match(/^[a-zA-Z]\d+/)) {

                var quantityMsg = $(".quantity_msg").html();
                var quantity = 1;
                if (quantityMsg.indexOf("_") == -1) {
                    quantity = parseInt(quantityMsg);
                    if (quantity < 1) {
                        alert("請至少輸入一件商品");
                        return false;
                    }
                }
                ScanAct(barcode, quantity)
               
            } else {
                alert("商品條碼格式錯誤!");
                $(".InputBox").val("");
            }
          
        }

        //VIP輸入卡號或是手機號碼
        function VipScan() {
            var barcode = $("#VIPInputBox").val();
            if (barcode.match(/^VIP[A-Z0-9]{9}$/) || barcode.match(/^09\d{8}$/)) {
                var url;

                if (barcode.match(/^VIP[A-Z0-9]{9}$/))
                    url = "?act=vip&vip_id=" + barcode;
                else
                    url = "?act=vip&mobile=" + barcode;

                $.get(url, function (data) {
                    var json = JSON.parse(data);
                    console.log(json)
                    if (json.result) {
                        if (json.data.valid) {
                            IsVip = true;
                            UseBirthdayDiscount = false;
                            $("#VipQuery").show();
                            console.log(json.data);
                            $("#vip_id").text(json.data.vip_id);
                            $("#vip_name").text(json.data.name);
                            $("#vip_mobile").text(json.data.mobile);
                            $("#vip_birthday").text(json.data.birthday);
                            $("#vip_last_birthday").text(json.data.last_birthday);
                            $("#vip_valid_date").text(json.data.valid_date);
                            $("#vip_bonus").text(json.data.bonus);
                            $("#vip_discount").text(VIPDiscount);
                            $(".VipTable").show();
                            $(".VIP").css("color", "red");
                            if (json.data.bonus>150) {
                                $("#UseBonusDIV").show();
                            } else {
                                $("#UseBonusDIV").hide();
                            }
                           
                            if (json.data.birthday_discount) {
                                $("#BirthdayDiscountDIV").show();
                            } else {
                                $("#BirthdayDiscountDIV").hide();
                            }

                            account();
                            //CheckPromotion(json.data.birthday_discount);

                        }
                        else {
                            alert("VIP過期!");
                        }
                    }
                    else {
                        alert("錯誤的『VIP卡號』或『行動電話』!");
                    }
                });

                $("#VIPInputBox").val("");
            } else {
                alert("手機號碼或是VIP卡號格式錯誤!");
            }

            $("#VIPInputBox").val("");
        }

        function ScanAct(barcode, quantity) {
            var url = "?act=Scan&barcode=" + barcode + "&quantity=" + quantity + "&PosNo=" + PosNo;
            if (barcode.match(/^\d{8}$/)) {
                $.get(url, function (data) {
                    var json = JSON.parse(data);
                    console.log(json);
                    if (json.result) {

                        var itemNo = json.itemNo;
                        var QtyID = itemNo + "_Qty";
                        var AmountID = itemNo + "_Amount";
                        var SkuQtyID = itemNo + "_SkuQty";
                        var ShowQtyID = itemNo + "_ShowQty";

                        if (json.DivitionStatus == 0) {

                            $("#" + SkuQtyID).html(json.SkuQty);
                            $("#" + ShowQtyID).html(json.ShowQtyID);

                            alert("目前沒有庫存!");
                        }
                        if (json.DivitionStatus == 1 || json.DivitionStatus == 2) {
                            if (json.DivitionStatus == 2) {
                                pickType = 5;
                            }

                            if ($("#" + QtyID).html() != undefined) {

                                if (json.DivitionStatus == 2) {
                                    //tr 要變色
                                    var TrID = itemNo + "_Item";
                                    $("#" + TrID).find("td").css("color", "red");
                                }

                                var Qty = parseInt($("#" + QtyID).html());
                                var Original_Amount = parseInt($("#" + AmountID).find(".OriginalAmount").html());
                                var Amount = parseInt($("#" + AmountID).find(".amount").html());

                                Qty += json.quantity;
                                Amount += json.amount;
                                Original_Amount += json.original_amount;
                                var Amount_content = '<strike class="OriginalAmount" style="display:none">' + Original_Amount + '<br/></strike><span class=\"amount\">' + Amount + '</span></td>';

                                $("#" + QtyID).html(Qty);
                                $("#" + AmountID).html(Amount_content);
                                $("#" + SkuQtyID).html(json.SkuQty);
                                $("#" + ShowQtyID).html(json.ShowQty);
                            } else {
                                $(".SaleItemList").append(json.data);
                                $(".pro_infoDT").scrollTop($(".pro_infoDT")[0].scrollHeight);
                            }
                            account();
                            //CheckPromotion(false); 
                        }
                        console.log("pickType:" + pickType);
                    }
                    else {
                        alert("錯誤的『產品條碼』或『產品代碼』!");
                    }
                })
            } else {
                alert("『產品條碼』長度錯誤!");
            }
            $(".InputBox").val("");
            $(".quantity_msg").html("____");
        }

        // 計算價錢
        function account() {
            var Price = 0;
            var Amount = 0;
            var AllQuantity = 0;
            var AllOriginalAmount = 0;

            $(".price").each(function () {
                Price += parseInt($(this).html());
            });
            $(".quantity").each(function () {
                AllQuantity += parseInt($(this).html());
            });

            $(".OriginalAmount").each(function () {
                AllOriginalAmount += parseInt($(this).html());
            })

            $(".TotalAmount").html(AllOriginalAmount.toString());
            $(".OriginalTotalAmount").html(AllOriginalAmount.toString());
            $(".AllQuantity").html(AllQuantity.toString());

            //計算發票數
            var rowNum = $(".item").length;
            var Remainder = parseInt($(".InvoiceRemainder").html());
            var pageSize = 7;
            InvoiceQty = Math.floor((rowNum - 1) / pageSize) + 1;
            console.log("使用發票數量：" + InvoiceQty)
            console.log("剩餘發票數：" + Remainder)

            if (InvoiceQty > Remainder) {
                alert("商品數超過剩餘發票!!")
                RemainderNotEnough = true;
            } else {
                RemainderNotEnough = false;
            }
           
            //如果有使用生日折扣就不走一般則扣，但是在8/12被推翻了
            //if (UseBirthdayDiscount) {
            //    VipBirthdayDiscount();
            //} else {
            //    //計算折扣價
            //    GeneralDiscount();
            //}

            GeneralDiscount();
           
            //重新計算總價
            $(".amount").each(function () {
                Amount += parseInt($(this).html());
            });
            $(".TotalAmount").html(Amount.toString());
            SetCheckOutInfo();

            if (Amount >= 3000 && IsVip==false) {
                if (OpenCardRemind == false) {
                    alert("消費滿3千，已達開卡資格")
                }
                OpenCardRemind = true;
            } else {
                OpenCardRemind = false;
            }

        }

        //取得退款訂單的商品名細
        function GetOrderItemByOrderID(ReturnOrderID) {
            var url = "?act=GetOrderItemByOrderID&OrderID=" + ReturnOrderID;
            $.get(url, function (data) {
                var json = JSON.parse(data);
                //clear all tr
                $(".SaleItemList").find(".item").remove();
                if (json.result) {
                    if (json.status == 2 || json.status == 6) {
                        alert("此訂單已作廢!!");
                        $(".ReturnOrderID_msg").html("");
                    } else {
                        console.log(json);
                        act = "ReturnPurchase";
                        $(".SaleItemList").append(json.data);
                        PayType = json.payType;
                        ReturnCreditAmount = json.Amount;
                        ApprovalNo = json.ApprovalNo;
                        SerialNo = json.SerialNo;
                        IsAllowances = json.IsAllowances;
                        AllowanceID = json.AllowanceID;
                        PaperReturnID = json.PaperReturnID;
                        UniformSerialNumber = json.UniformSerialNumber;
                        if (UniformSerialNumber.length > 0) {
                            $("#UniformNo").html(UniformSerialNumber);
                        }
                        $(".received").html(json.Amount);
                        $(".change").html("0");
                        //判斷是否為重新結帳
                        if (json.status == 3) {
                            act = "ReCheckOut";
                            $(".Delete_img").removeClass();
                        }

                        account();
                    }
                }
                else {
                    $(".ReturnOrderID_msg").html("");
                    alert("查無訂單")
                }
                               
            })
        }

        //取消交易
        function CancelTransaction() {

            $(".item,.BonusUse").each(function () {
                $(this).remove();
            });

            ClearTempStorage("");
            ReturnOrderID = "";
            $(".TotalAmount").html("0");
            $(".OriginalTotalAmount").html("0");
            $(".AllQuantity").html("0");
            $(".received").html("0");
            $(".change").html("0");
            HideAllScreen();
            $(".Scan_barcode_msg").show();
            act = "quantity";
            condition = "buy";
            $(".ReturnOrderID_msg").html("");
            $(".quantity_msg").html("____");
            $(".cash").html("現金交易");
            $(".creditcard").html("信用卡交易");
            $(".change_msg").html("找零");
            $(".OriginalTotalAmountLi").show();
            $(".TotalAmountMsg").html("折扣後金額");
            PayType = 1;
            $("#cash_info").show();
            $("#credit_info").hide();
            pickType = 0;
            $("#UniformNo").html("00000000")
            $(".InputBox").val("");
            $(".VIP").html("VIP");
            CardNo = "";
            ApprovalNo = "";
            UniformSerialNumber = "";
            CreditCardData = "";
            RemainderNotEnough = false;
            IsAllowances = false;
            AllowanceID = 0;
            OpenCardRemind = false;
            PickOrderID = 0;
            PaperReturnID = "";
            VipReset();
            SetCheckOutInfo();
        }

        function VipReset() {
            last_bonus = 0;
            IsVip = false;
            UseBonus = false;
            UseBirthdayDiscount = false;
            var bonus_multiplication = 1;


            $("#vip_id").text("");
            $("#vip_name").text("");
            $("#vip_mobile").text("");
            $("#vip_birthday").text("");
            $("#vip_last_birthday").text("");
            $("#vip_valid_date").text("");
            $("#vip_bonus").text("");
            $("#vip_discount").text("");
            $(".VipTable").hide();
            $(".VIP").css("color", "#666666");
            $("#VIPInputBox").val("");
            $("#UseBonusDIV").hide();
            $("#BirthdayDiscountDIV").hide();
            $(".BirthdayDiscount").removeClass("switchOn")
            $(".UseBonus").removeClass("switchOn")
            $("#vip_discount").css("color", "#000000");

            //$("#NewVIP").hide();
        }

        //隱藏黃色銀幕
        function HideAllScreen() {
            $(".Scan_barcode_msg").hide();
            $(".cash_msg").hide();
            $(".Change_Invoice_msg").hide();
            $(".Voided_Invoice_msg").hide();
            $(".Setting_Invoice_msg").hide();
            $(".ReturnePurchase_msg").hide();
            $(".Uniform_No_msg").hide();
            if (condition == "return") {
                $(".change_msg").html("退款");
            }

        }

        //清暫量庫存
        function ClearTempStorage(ProductID, TrName) {
            var url = "?act=ClearTempStorage&PosNo=" + PosNo + "&ProductID=" + ProductID;
            $.get(url, function (data) {
                var json = JSON.parse(data);
                if (json.result) {
                    console.log("Clear TempStorage Success");
                    
                }
                else {
                    console.log("No TempStorage");
                }

                if (TrName != undefined) {
                    TrName.remove();
                    account();
                }
                
                //CheckPromotion(false);
                return json.result;
            })
        }

        function GetSerailProductByProductID(ProductID) {
            var url = "GetSerailProductByProductID.aspx?ProductID=" + ProductID;
            $.get(url, function (data) {
                $("#SerialItemList").html(data);
                $("#SerialItemList").dialog('open');
            })
        }

        function ReturnPurchaseFunction() {
            HideAllScreen();
            $(".ReturnePurchase_msg").show();
            $(".cash").html("");
            $(".creditcard").html("");
            act = "ReturnPurchase";
            condition = "return";
            $(".change_msg").html("退款");
            $(".OriginalTotalAmountLi").hide();
            $(".TotalAmountMsg").html("應收金額");
            $(".VIP").html("");
            pickType = 1;
        }

        //取得登入的員工姓名和編號
        function GetClerkName() {
            var clerk = {};
            clerk.id = ClerkID;
            clerk.name = ClerkName;
            localStorage.setItem('clerk', JSON.stringify(clerk));
        }

        function SetCheckOutInfo() {
            var CheckOutInfo = {};
            var ItemList = '';
            CheckOutInfo.AllQuantity = $(".AllQuantity").html();
            CheckOutInfo.TotalAmount = $(".TotalAmount").html();
            CheckOutInfo.OriginalTotalAmount = $(".OriginalTotalAmount").html();
            CheckOutInfo.received = $(".received").html();
            CheckOutInfo.change = $(".change").html();
            CheckOutInfo.condition = condition;

            $(".item").each(function () {
                name = $(this).find("td:eq(2)").html()
                color = $(this).find("td:eq(3)").html()
                size = $(this).find("td:eq(4)").html()
                quantity = $(this).find("td:eq(5)").html()
                amount = $(this).find(".amount").html()
                OriginalAmount = $(this).find(".OriginalAmount").html();
                discount_type = $(this).find(".discount_type").val();

                ProductItem = '<tr>';
                ProductItem += '<td class="tit_list" style="width: 280px;"><div class="TruncateLongText colProductName">' + name + '</div></td>';
                ProductItem += '<td class="tit_list" style="width: 50px;" ><div class="TruncateLongText colColor">' + color + '</div></td>';
                ProductItem += '<td class="tit_list" style="width: 50px;">' + size + '</td>';
                ProductItem += '<td class="tit_list" style="width: 50px;">' + quantity + '</td>';
                if (discount_type != undefined) {
                    ProductItem += '<td class="tit_list" style="width: 100px;"><strike style="display:none">$' + OriginalAmount + '</strike><span class="amount">$' + amount + '</span></td></tr>';
                } else {
                    ProductItem += '<td class="tit_list" style="width: 100px;"><span class="amount">$' + amount + '</span></td></tr>';
                }

                ItemList += ProductItem;
            })

            CheckOutInfo.ItemList = ItemList;

            $.connection.hub.start().done(function () {
                PosManager.server.setCheckOutInfo(JSON.stringify(CheckOutInfo));
            })
        }

        //呼叫信用卡機，執行刷卡
        function CreditCardCheckOut(OrderID) {
            PosManager.server.creditCardSale($(".TotalAmount").html());
        }

        //刷退完成，作廢發票，改為待結帳狀態
        function ChangeStatusToWaitingForCheckOut(OrderID) {
            console.log("刷退ID:" + OrderID)
            var ItemNoList = [];
            var PriceList = [];
            var QuantityList = [];

            $(".itemNo").each(function () {
                ItemNoList.push($(this).html());
            });

            $(".price").each(function () {
                PriceList.push(parseInt($(this).html()));
            });
            $(".quantity").each(function () {
                QuantityList.push(parseInt($(this).html()))
            });

            var ItemNoListString = JSON.stringify(ItemNoList);
            var PriceListString = JSON.stringify(PriceList);
            var QuantityListString = JSON.stringify(QuantityList);

            var Postdata = {
                act: "ChangeStatusToWaitingForCheckOut",
                OrderID: OrderID,
                ItemNoList: ItemNoListString,
                PriceList: PriceListString,
                QuantityList: QuantityListString,
                PayType: PayType,
                pickType: pickType,
                PosNo: PosNo,
                ClerkID: ClerkID,
                ClerkName: ClerkName,
                PaperReturnID: PaperReturnID
            }

            $.ajax({
                type: "POST",
                url: "?",
                async: false,
                dataType: "json",
                data: Postdata,
                success: function (data) {
                    console.log(data);
                    if (data.result) {
                        //退款完成，將act改為重新結帳
                        act = "ReCheckOut";
                        content = '<h2>退款完成</h2><br/>請重新刷卡結帳<br/><br/><br/><li class="icon_PrcEND ReturnePurchaseCreditCard">確定</li>';
                        $(".paulund_inner_modal_box").html(content);
                    } else {
                        $(".paulund_inner_modal_box").html('<h2>退貨失敗!!</h2><br/><br/>信用卡退款完成，訂單狀態更改失敗<br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">確定</li>');
                    }
                },
                timeout: 60000,
                error: function (data) {
                    $(".paulund_inner_modal_box").html('<h2>退貨失敗!!</h2><br/><br/>信用卡退款完成，訂單狀態更改失敗<br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">確定</li>');
                }
            });
        }

        //將刷退過的訂單重新結帳
        function ReCheckOut(OrderID) {
            console.log("重結ID:" + OrderID);

            var ItemNoList = [];
            var ColorList = [];
            var PriceList = [];
            var QuantityList = [];
            var Amount = 0;
            var UniformNo = "";
            var InvoiceRemainder = parseInt($(".InvoiceRemainder").html());

            if (parseInt($("#UniformNo").html()) != 0) {
                UniformNo = $("#UniformNo").html();
            }
            $(".itemNo").each(function () {
                ItemNoList.push($(this).html());
            });

            $(".color").each(function () {
                ColorList.push($(this).html());
            });

            $(".price").each(function () {
                PriceList.push(parseInt($(this).html()));
            });
            $(".quantity").each(function () {
                QuantityList.push(parseInt($(this).html()))
            });
            $(".amount").each(function () {
                Amount += parseInt($(this).html());
            });

            if (RemainderNotEnough == true) {
                $(".paulund_inner_modal_box").html('<h2>商品數超過剩餘發票!!</h2><br/><br/><br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li>');
                return false;
            }

            if (InvoiceRemainder < 1) {
                $(".paulund_inner_modal_box").html('<h2>請先更換發票!!</h2><br/><br/><br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li>');
                return false;
            }

            var ItemNoListString = JSON.stringify(ItemNoList);
            var ColorListString = JSON.stringify(ColorList);
            var PriceListString = JSON.stringify(PriceList);
            var QuantityListString = JSON.stringify(QuantityList);
            //取得店員名稱和ID
            var clerk = JSON.parse(localStorage.getItem('clerk'));
            ClerkName = clerk.name;
            ClerkID = clerk.id;

            var Postdata = {
                act: "ReCheckOut",
                OrderID: OrderID,
                ItemNoList: ItemNoListString,
                PriceList: PriceListString,
                QuantityList: QuantityListString,
                ColorList: ColorListString,
                Amount: Amount,
                PayType: PayType,
                pickType: pickType,
                PosNo: PosNo,
                ClerkID: ClerkID,
                ClerkName: ClerkName,
                CardNo: CardNo,
                ApprovalNo: ApprovalNo,
                CreditCardData: CreditCardData,
                OriginalAmount: Amount,
                AllowanceID: AllowanceID,
                UniformNo:UniformNo
            }

            var OldInvoiceNo = $(".InvoiceNumberNow").html();

            $.ajax({
                type: "POST",
                url: "?",
                async: false,
                dataType: "json",
                data: Postdata,
                success: function (data) {
                    console.log(data);
                    if (data.result) {
                        var jsonText = JSON.stringify(data.InvoiceContent);
                        PosManager.server.printInvoice(jsonText);
                    } else {
                        $(".paulund_inner_modal_box").html('<h2>交易失敗</h2><br/>訂單寫入失敗<br/>請重新結帳<br/><br/><li class="icon_PrcEND WebReload" style="float: left;">確定</li>');
                        CancelTransaction();
                    }
                },
                timeout: 60000,
                error: function (data) {
                    $(".paulund_inner_modal_box").html('<h2>交易逾時</h2><br/>若發票未印出<br/>請重新結帳及取消訂單<br/>並檢查發票機內的紙張目前是否為<br/><span>' + OldInvoiceNo.substring(0, 6) + '</span><span style="color: red;">' + OldInvoiceNo.substring(6, 10) + '</span><br/><li class="icon_PrcEND WebReload" style="float: left;">確定</li>');
                    CancelTransaction();
                }
            });

        }

        //一般折扣計算
        function GeneralDiscount() {
            var birthDiscount = 1;
            var AllOriginalAmount = 0;

            $(".price").css("color", "#000000");
            $(".amount").css("color", "#000000");
            $("strike").hide();

            if (IsVip == true && UseBirthdayDiscount==true) {
                birthDiscount=0.9;
            }

            $(".OriginalAmount").each(function () {
                AllOriginalAmount += parseInt($(this).html());
            })

            if (condition == "buy") {
                DiscountClotheNum = 0;

                $(".item").each(function () {

                    var IsStoreFloat = $(this).attr("data-storefloat");
                    console.log("IsStoreFloat:" + IsStoreFloat);
                    if (IsStoreFloat == "True") {
                        $(this).find(".price").css("color", "blue");
                        $(this).find(".amount").css("color", "blue");
                    }

                    var OriginalPrice = parseInt($(this).find(".OriginalPrice").html());
                    var quantity = parseInt($(this).find("td:eq(5)").html())
                    $(this).find(".price").html(OriginalPrice)
                    var OriginalAmount = OriginalPrice * quantity;
                    $(this).find(".amount").html(OriginalAmount);

                    //折扣件數加總
                    if ($(this).find(".discount_type").val().length > 0) {
                        DiscountClotheNum += quantity;
                    }
                })

                //如果此期間有折扣活動(一般或VIP)
                if (DiscountType.length > 0 || VIPDiscountType.length > 0) {
                    var HasDiscount = false;

                    if (IsVip) {
                        VIPDiscountLimitList = jQuery.parseJSON(VIPDiscountLimit);
                        var LimitIndex = 0;

                        for (var i = 0; i < VIPDiscountLimitList.length; i++) {
                            if (VIPDiscountType == "滿X元X%元") {
                                if (AllOriginalAmount >= VIPDiscountLimitList[i]) {
                                    HasDiscount = true;
                                    LimitIndex = i;
                                }
                            }

                            if (VIPDiscountType == "滿X件X%元") {
                                if (DiscountClotheNum >= VIPDiscountLimitList[i]) {
                                    HasDiscount = true;
                                    LimitIndex = i;
                                }
                            }
                        }
                        
                    } else {

                        DiscountLimitList = jQuery.parseJSON(DiscountLimit);
                        var LimitIndex = 0;

                        for (var i = 0; i < DiscountLimitList.length; i++) {
                            if (DiscountType == "滿X元X%元") {
                                if (AllOriginalAmount >= DiscountLimitList[i]) {
                                    HasDiscount = true;
                                    LimitIndex = i;
                                }
                            }

                            if (DiscountType == "滿X件X%元") {
                                if (DiscountClotheNum >= DiscountLimitList[i]) {
                                    HasDiscount = true;
                                    LimitIndex = i;
                                }
                            }

                            if (DiscountType == "新品折X元") {
                                HasDiscount = true;
                                LimitIndex=0
                            }
                        }

                    }

                    if (HasDiscount == false) {
                        SetCheckOutInfo();
                        return;
                    }
                }

                $(".item").each(function () {

                    if ($(this).find(".discount_type").val().length > 0 || $(this).find(".VIP_discount_type").val().length>0) {
                        var PriceListString = "";
                        if (IsVip) {
                            PriceListString = $(this).find(".VIP_discount_price").val()
                        } else {
                            PriceListString = $(this).find(".discount_price").val()
                        }

                        PriceList = jQuery.parseJSON(PriceListString)
                        //多乘了生日折扣
                        discount_price = Math.floor(PriceList[LimitIndex] * birthDiscount);

                        //贈品不需要再去打折
                        if (discount_price > 0) {
                            quantity = parseInt($(this).find("td:eq(5)").html());
                            $(this).find(".price").html(discount_price);
                            $(this).find(".price").css("color", "red");
                            $(this).find(".amount").css("color", "red");
                            discount_amount = discount_price * quantity;
                            $(this).find(".amount").html(discount_amount);
                            $(this).find("strike").show();
                        }
                    }

                })
            }

        }

        //觀光客折扣運算
        function TouristsDiscountCounting() {
            if (TouristsDiscountAmount == 0) {
                alert("本活動已結束，請通知官網人員");
                return;
            }

            if (condition == "return") {
                return;
            }

            var OriginalTotalAmount = parseInt($(".OriginalTotalAmount").html());
            var TotalAmount = parseInt($(".TotalAmount").html());
            var amount = OriginalTotalAmount - TouristsDiscountAmount;

            if (TotalAmount == 0) {
                alert("尚未購買任何商品");
                return;
            }

            if (OriginalTotalAmount < TouristsDiscountLimit) {
                alert("金額尚未達到1000元");
                return;
            }

            if (OriginalTotalAmount - TotalAmount > TouristsDiscountAmount) {
                msg = '使用觀光客折扣後金額為' + amount + '元，\n較不划算，建議使用目前促銷方案';
                alert(msg);
            } else {

                var Amount = 0;
                var AllOriginalAmount = 0;
                var ItemNo = 1;
                var TouristsDiscount = TouristsDiscountAmount;
                var TotalItemNum = 0;
                var StoreFloatAmount = 0;

                //排除贈品和花車商品，算出實際上需要算觀光客折扣的品項數，並且要排除花車的總金額來攤折價
                $(".item").each(function () {
                    var oPrice = parseInt($(this).find("strike").html())
                    var amount = parseInt($(this).find(".amount").html())

                    var IsStoreFloat = $(this).attr("data-storefloat");
                    console.log("IsStoreFloat=>" + IsStoreFloat)
                    if (oPrice > 0 && IsStoreFloat == "False") {
                        TotalItemNum += 1;
                    }

                    if (IsStoreFloat == "True") {
                        StoreFloatAmount += amount;
                    }
                })

                if (TotalItemNum == 0) {
                    alert("目前的商品不適用觀光客折扣");
                    return;
                }

                $(".item").each(function () {
                    var oPrice = parseInt($(this).find("strike").html())
                    var IsStoreFloat = $(this).attr("data-storefloat");
                    if (oPrice > 0 && IsStoreFloat == "False") {
                        quantity = parseInt($(this).find("td:eq(5)").html());
                        //console.log(amount - StoreFloatAmount)
                        //console.log(OriginalTotalAmount - StoreFloatAmount)
                        var discount_price = Math.floor(oPrice * (amount - StoreFloatAmount) / (OriginalTotalAmount - StoreFloatAmount));
                        $(this).find(".price").html(discount_price);
                        $(this).find(".price").css("color", "red");
                        $(this).find(".amount").css("color", "red");

                        if (ItemNo == TotalItemNum) {
                            discount_amount = (oPrice * quantity) - TouristsDiscount
                        } else {
                            discount_amount = discount_price * quantity;
                        }

                        $(this).find(".amount").html(discount_amount);
                        $(this).find("strike").show();
                        TouristsDiscount -= ((oPrice * quantity) - discount_amount);
                        //console.log(TouristsDiscount)

                        ItemNo += 1;
                    }

                })

                $(".OriginalAmount").each(function () {
                    AllOriginalAmount += parseInt($(this).html());
                })

                $(".amount").each(function () {
                    Amount += parseInt($(this).html());
                });

                $(".TotalAmount").html(Amount.toString());
                $(".OriginalTotalAmount").html(AllOriginalAmount.toString());
                SetCheckOutInfo();
            }
        }

        //VIP生日折扣
        function VipBirthdayDiscount() {

            if (IsVip==false) {
                return;
            }
            
            var Amount = 0;
            var AllOriginalAmount = 0;
            var discount = 0.9;
            $(".item").each(function () {
                var oPrice = parseInt($(this).find("strike").html())
                if (oPrice > 0) {
                    quantity = parseInt($(this).find("td:eq(5)").html());
                    var discount_price = Math.floor(oPrice * discount);
                    discount_amount = discount_price * quantity;

                    $(this).find(".price").html(discount_price);
                    $(this).find(".price").css("color", "red");
                    $(this).find(".amount").css("color", "red");
                    $(this).find(".amount").html(discount_amount);
                    $(this).find("strike").show();
                }
            })

            $(".OriginalAmount").each(function () {
                AllOriginalAmount += parseInt($(this).html());
            })

            $(".amount").each(function () {
                Amount += parseInt($(this).html());
            });

            $(".TotalAmount").html(Amount.toString());
            $(".OriginalTotalAmount").html(AllOriginalAmount.toString());
            SetCheckOutInfo();

        }

        function GetCheckMachineStatus() {
            PosManager.server.checkInvoicePrinter();
        }

        $.connection.hub.stateChanged(function (state) {
            var stateConversion = { 0: 'connecting', 1: 'connected', 2: 'reconnecting', 4: 'disconnected' };

            if (state.newState == 4) {
                CancelTransaction();
                alert("POS服務中斷\n系統將重新整理頁面");
                location.reload();
            }

            console.log('SignalR state changed from: ' + stateConversion[state.oldState]
             + ' to: ' + stateConversion[state.newState]);

        });


        function SignalRInitialization() {

            $.connection.hub.url = "http://localhost:8080/signalr";
            PosManager = $.connection.myHub;
            $.connection.hub.start();

            if (PosManager != undefined) {

                //列印發票結束回傳
                PosManager.client.printInvoiceEnd = function (message) {
                    console.log(message)
                    var OldInvoiceNo = $(".InvoiceNumberNow").html();
                    var PrintResult = JSON.parse(message);

                    //要記得註解掉
                    PrintResult.Success = true;
                    //要記得註解掉

                    if (PrintResult.Success) {
                        var OrderStep = 2;

                        if (PickOrderID == 0) {
                            return;
                        }

                        if (IsVip) {
                            UpdateVIPBonus();
                        }
                        PrintPickList(OrderStep);
                    } else {
                        $(".paulund_inner_modal_box").html('<h2>發票列印失敗</h2>' + PrintResult.ExceptionMessage + '<br/><br/>請重新結帳及取消訂單<br/>並檢查發票機內的紙張目前是否為<br/><span>' + OldInvoiceNo.substring(0, 6) + '</span><span style="color: red;">' + OldInvoiceNo.substring(6, 10) + '</span><br/><li class="icon_PrcEND WebReload" style="float: left;">確定</li>');
                    }
                };

                //刷卡結束回傳
                PosManager.client.creditCardSaleEnd = function (message) {
                    var json = JSON.parse(message);
                    if (json.Success) {
                        CreditCardData = message;
                        CardNo = json.CardNo;
                        ApprovalNo = json.ApprovalNo;
                        if (ReturnOrderID == "") {
                            CheckOut();
                        } else {
                            ReCheckOut(ReturnOrderID)
                        }
                    } else {

                        if (condition == "buy") {
                            error_msg = "<h2>刷卡結帳</h2>刷卡失敗<br/><br/>請重新嘗試一次<br/><br/><li class='icon_PrcEND ReturnOrder' style='float: left;'>確定</li>";
                        }

                        if (condition == "return") {
                            error_msg = "<h2>刷卡結帳</h2>刷退失敗<br/><br/>請重新嘗試一次<br/><br/><li class='icon_PrcEND ReturnePurchaseCreditCard' style='float: left;'>確定</li>";
                        }
                        $(".paulund_inner_modal_box").html(error_msg);
                    }
                };


                //刷退結束回傳
                PosManager.client.creditcardReturnEnd = function (message) {
                    var json = JSON.parse(message);
                    if (json.Success) {
                        CreditCardData = message;
                        CardNo = json.CardNo;
                        ApprovalNo = json.ApprovalNo;

                        if ($(".itemNo").length > 0) {
                            ChangeStatusToWaitingForCheckOut(ReturnOrderID);
                        } else {
                            CheckOut(ReturnOrderID);
                        }

                    } else {
                        $(".paulund_inner_modal_box").html('<h2>刷退失敗</h2><br/><br/>請重新執行<br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">確定</li><li class="icon_PrcEND PaperReturnOrder">填紙本單刷退</li>');
                    }
                };


                //撿查發票機回傳   
                PosManager.client.checkInvoicePrinterEnd = function (message) {

                    var AllowancesType = "二聯式";

                    if (UniformSerialNumber != "") {
                            AllowancesType = "三聯式";
                     }

                    var json = JSON.parse(message);
                    console.log(json);
                    //要記得註解掉
                    json.Success = true;
                    //要記得註解掉
                    if (!json.Success) {
                        error_msg = "<h2>發票機異常</h2>請檢查發票機<br/><br/>" + json.ExceptionMessage + "<br/><br/><li class='icon_PrcEND ReturnOrder' style='float: left;'>確定</li>";
                        $(".paulund_inner_modal_box").html(error_msg);
                    } else {


                        if (condition == "buy") {
                            if (IsVip) {
                                OrderFinishVIP()
                                return;
                            }

                            if (PayType == 1) {
                                OrderFinishCash()
                            }
                            if (PayType == 2) {
                                OrderFinishCredit()
                            }
                        }


                        if (condition == "return") {
                            //信用卡重新結帳
                            if (act == "ReCheckOut") {

                                $(".paulund_inner_modal_box").html('<h2>信用卡結帳</h2>總件數：<span style="color:red;font-size:20px">' + $(".AllQuantity").html() + '</span><br/> 總金額：<span style="color:red;font-size:20px">' + $(".TotalAmount").html() + '</span>元<br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li><li class="icon_PrcEND CreditCardSubmit">確定</li>');

                            } else {
                                //其他退貨流程
                                //沒有刪除商品不能按退貨
                                var ReturnMoney = parseInt($(".change").html());

                                if (ReturnMoney == 0) {
                                    $(".paulund_inner_modal_box").html('<h2>退貨資料</h2><br/>請至少選擇一件退貨商品!<br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li>');
                                } else {
                                    if (PayType == 1) {
                                       
                                        if (IsAllowances == true) {
                                            $(".paulund_inner_modal_box").html('<h2>請先填寫' + AllowancesType + '折讓單</h2><br/>折讓編號：<span style="color:red">' + AllowanceID + '</span><br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li><li class="icon_PrcEND AllowancesFinish">填寫完畢</li>');
                                        } else {
                                            $(".paulund_inner_modal_box").html('<h2>退貨資料</h2><br/>退款：<span style="color:red;font-size:20px">' + $(".change").html() + '</span>元<br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li><li class="icon_PrcEND ReturnePurchaseSubmit">確定</li>');
                                        }
                                    }

                                    if (PayType == 2) {
                                        //信用卡正常刷退
                                        if (act == "ReturnPurchase") {
                                            if (IsAllowances == true) {
                                                $(".paulund_inner_modal_box").html('<h2>請先填寫' + AllowancesType + '折讓單</h2><br/>折讓編號：<span style="color:red">' + AllowanceID + '</span><br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li><li class="icon_PrcEND AllowancesFinish">填寫完畢</li>');
                                            } else {
                                                ReturnCreditCardOrder();
                                            }
                                        }

                                    }
                                }
                            }

                        }

                    }
                };

            } else {
                alert("目前無法連接發票機\n 請按F5重新整理")
            }


        }

        function OrderFinishCash() {
            ContentMsg = "<h2>交易確認</h2>";
            ContentMsg += '總件數：<span class="AllQuantityMsg">0</span><br/>應收金額：<span class="AmountMsg">0</span><br/>';
            ContentMsg += '已收金額：<span class="receivedMsg">0</span><br/>找零：<span class="changeMsg">0</span><br/>';
            ContentMsg += ' <br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li><li class="icon_PrcEND OrderSubmit">確定</li>';
            $(".paulund_inner_modal_box").html(ContentMsg);

            $(".AmountMsg").html($(".TotalAmount").html());
            $(".receivedMsg").html($(".received").html());
            $(".AllQuantityMsg").html($(".AllQuantity").html());
            $(".changeMsg").html($(".change").html());
        }

        function OrderFinishCredit() {
            $(".paulund_inner_modal_box").html('<h2>信用卡結帳</h2>總件數：<span style="color:red;font-size:20px">' + $(".AllQuantity").html() + '</span><br/> 總金額：<span style="color:red;font-size:20px">' + $(".TotalAmount").html() + '</span>元<br/><br/><br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li><li class="icon_PrcEND CreditCardSubmit">確定</li>');
        }

        function OrderFinishVIP() {
            vip_discount = VIPDiscount * 100;
            var bonus_discount = 0;
            var vip_bonus = parseInt($("#vip_bonus").text());
            var TotalAmount = parseInt($(".TotalAmount").html());
            if (UseBonus) {
                bonus_discount = -parseInt($(".BonusUse").find(".amount").text());
            }
            var used_bonus = bonus_discount * 150;
            var new_bonus = (TotalAmount + bonus_discount) * bonus_multiplication;
            last_bonus = vip_bonus - used_bonus + new_bonus;

            ContentMsg = "<h2>VIP交易確認</h2>";
            ContentMsg += '<div class="vip">非特價商品 '
            ContentMsg += '<span  class="red">' + $(".OriginalTotalAmount").text() + '</span> x '
            ContentMsg += '<span  class="red">' + vip_discount + '</span>% + 特價商品 '
            ContentMsg += '<span  class="red">0</span> - 紅利折抵 '
            ContentMsg += '<span  class="red">' + bonus_discount + '</span> = '
            ContentMsg += '<span  class="red">' + TotalAmount + '</span></div><br/>';

            ContentMsg += '<div class="vip">原本紅利 <span id="origin_bonus" class="red">' + vip_bonus + '</span>'
            ContentMsg += '- 本次使用 <span id="used_bonus" class="red">' + used_bonus + '</span>+ 本次新增'
            ContentMsg += '<span id="new_bonus" class="red">' + new_bonus + '</span>'
            ContentMsg += ' = 最新紅利 <span id="last_bonus" class="red">' + last_bonus + '</span></div>'
            ContentMsg += '<br/><li class="icon_PrcEND ReturnOrder" style="float: left;">返回</li>'
            ContentMsg += '<li class="icon_PrcEND VipOrderSubmit">確定</li>';
            $(".paulund_inner_modal_box").html(ContentMsg)
        }

        function CheckPromotion(IsBirthday) {
          
            if (IsBirthday == true && $(".BirthdayPresent").html() == undefined) {
                GetPromotion(IsBirthday);
            }

            if (IsBirthday == false) {
                bonus_multiplication = 1;
                GetPromotion(false);
            }
            
        }



    </script>
</head>
<body>

    <div class="All">

        <!--最上面的頁面連結按鈕-->
        <div class="Top_Menu">
            <ul>
                <li><a href="pos_check_out.aspx">交易結帳</a></li>
                <li><a href="pos_stock_query.aspx" >庫存查詢</a></li>
                 <li><a href="pos_order_query.aspx" onclick="window.open(this.href, '', 'width=1024,height=768,top=0, left=0'); return false;">交易查詢</a></li>
                <li><a href="javascript:void(0)" class="ActivitiesSetting">販促設定</a></li>
                <li><a href="index.aspx">管理頁面</a></li>
                <li><a href="http://www.obdesign.com.tw/" onclick="window.open(this.href, '', 'width=1024,height=768,top=0, left=0'); return false;">官網首頁</a></li>
                <li id="NewVIP"><a href="pos_vip.aspx" onclick="window.open(this.href, '', 'width=1024,height=768,top=112, left=84'); return false;"><span style="color:red">新增VIP</span></a></li>
                </ul>
        </div>

        <!--左邊結帳細目-->
        <div class="L_list">
            <div>
                <ul style="width: 568px; height: 50px;">
                    <li class="icon_proDetail ReturnPurchaseNoProduct">無貨處理</li>
                    <li class="icon_proDetail ReturnPurchase">退貨處理</li>
                    <li class="icon_proDetail DayEndReport">日結報表</li>
                </ul>
            </div>

            <div style="clear: both;"></div>
            <!--間容ie7的div浮動-->

            <div class="pro_infoTIT">訂單明細</div>

            <div class="pro_infoDT">
                <table class="SaleItemList" width="560" border="0" cellspacing="0" cellpadding="0">
                    <tbody>
                        <tr>
                            <td style="width: 20px;line-height: 35px;"></td>
                            <td style="width: 60px;line-height: 35px;">款號</td>
                            <td style="width: 142px;line-height: 35px;">款名</td>
                            <td style="width: 48px;line-height: 35px;">顏色</td>
                            <td style="width: 48px;line-height: 35px;">尺寸</td>
                            <td style="width: 48px;line-height: 35px;">數量</td>
                            <td style="width: 48px;line-height: 35px;">單價</td>
                            <td style="width: 48px;line-height: 35px;">總價</td>
                            <td style="width: 38px;line-height: 35px;">B1</td>
                             <td style="width: 38px;line-height: 35px;">展售</td>
                        </tr>
                    </tbody>
                </table>

            </div>


            <div class="pro_infoPRC">

                <ul>
                    <li>發票號碼：<span class="allprc InvoiceNumberNow"><%=InvoiceNumberNow %></span></li>
                    <li>剩餘發票張數：<span class="allprc InvoiceRemainder"><%=InvoiceRemainder %></span>張</li>
                    <li>統一編號：<span class="allprc" id="UniformNo">00000000</span></li>
                </ul>

                <ul>
                    <li class="OriginalTotalAmountLi">商品總金額：<span class="allprc OriginalTotalAmount">0</span>元</li>
                    <li><span class="TotalAmountMsg">折扣後金額</span>：<span class="allprc TotalAmount">0</span>元</li>
                    <li id="credit_info" style="display:none;color:#ff0000"><b>信用卡交易</b></li>
                </ul>
                <ul>
                    <li >總數：<span class="AllQuantity">0</span>件</li>
                    <li>已收金額：<span class="received">0</span>元</li>
                    <li><span class="change_msg">找零</span>：<span class="change">0</span>元</li>
                </ul>

            </div>
            <div style="height: 50px;">
                <ul >
                    <li class="icon_PrcEND settingInvoice">設定發票號碼</li>
                    <li class="icon_PrcEND voidedInvoice">發票作廢處理</li>
                    <li class="icon_PrcEND changeInvoice">更換發票</li>
                </ul>

            </div>

            <div style="clear: both;"></div>
            <!--間容ie7的div浮動-->

            <div class="bottom_info">
                <ul>
                    <li>機台號碼：POS<%=PosNo %>號</li>
                    <li>登入者：<%=ck.Name %> <span class="logout">登出</span></li>

                </ul>
            </div>

            <div>
            </div>

        </div>

        <!--右邊鍵盤與計算畫面-->
        <div class="R_list">

            <div>
                <ul style="height: 50px;">
                    <li class="icon_dealDetail Tourists">觀光客</li>
                    <li class="icon_dealDetail CancelTransaction">取消交易</li>
                    <li class="icon_dealDetail OpenSecondWindow">交班結帳</li>
                </ul>
            </div>

            <div style="clear: both;"></div>
            <!--間容ie7的div浮動-->


            <div class="Screen">
                <div class="Scan_barcode_msg">
                     <ul class="Mode">
                        <li>商品輸入</li>
                    </ul>
                     <ul>
                         <li>請輸入商品數量，<span class="quantity_msg">____</span>件</li>
                         </ul>

                </div>
                <div class="Change_Invoice_msg" style="display: none">
                    <ul class="Mode">
                        <li>更換發票</li>
                    </ul>
                    <ul>
                        <li>
                            <select class="EnglishWord wordFirst">
                                <option>請選擇</option>
                            </select>&nbsp;
                  <select class="EnglishWord wordSecond">
                      <option>請選擇</option>
                  </select>&nbsp;
                        </li>
                    </ul>
                    <ul>
                        <li>請輸入發票起始號碼
                  <span class="StartNum">__________</span></li>
                        <li>請輸入發票截止號碼
                  <span class="EndNum">__________</span></li>
                    </ul>
                    <ul class="pointWord">
                        <li>完成後請按<img src="./Image/enter_bg.png" />確認，或按<span style="color: #666666">ESC</span>略過</li>
                    </ul>
                </div>
                <div class="Voided_Invoice_msg" style="display:none">
                    <ul class="Mode">
                        <li>作廢發票號碼</li>
                    </ul>
                    <ul>
                        <li>
                            <select class="wordFirstNow">
                                <option>請選擇</option>
                            </select>&nbsp;
                  <select class="wordSecondNow">
                      <option>請選擇</option>
                  </select>&nbsp;
                        </li>
                    </ul>
                    <ul>
                        <li>請輸入發票
                  <span class="voidedInvoiceNum">__________</span></li>
                    </ul>
                    <ul class="pointWord">
                        <li>完成後請按<img src="./Image/enter_bg.png" />確認，或按<span style="color: #666666">ESC</span>略過</li>
                    </ul>
                </div>
                <div class="Setting_Invoice_msg" style="display: none">
                    <ul class="Mode">
                        <li>設定發票號碼</li>
                    </ul>
                    <ul>
                        <li>
                            <select class="wordFirstNow">
                                <option>請選擇</option>
                            </select>&nbsp;
                  <select class="wordSecondNow">
                      <option>請選擇</option>
                  </select>&nbsp;
                        </li>
                    </ul>
                    <ul>
                        <li>請輸入發票
                  <span class="ResetInvoiceNum">__________</span></li>
                    </ul>
                    <ul class="pointWord">
                        <li>完成後請按<img src="./Image/enter_bg.png" />確認，或按<span style="color: #666666">ESC</span>略過</li>
                    </ul>
                </div>
                <div class="cash_msg" style="display: none">
                    <ul class="Mode">
                        <li>現金交易</li>
                    </ul>
                    <ul>
                        <li>應收<span class="TotalAmount"></span>元，已收<span class="received_msg">____</span>元</li>
                    </ul>
                    <ul class="pointWord">
                        <li>完成後請按<img src="./Image/enter_bg.png" />確認，或按<span style="color: #666666">ESC</span>略過</li>
                    </ul>
                </div>
                <div class="ReturnePurchase_msg" style="display: none">
                    <ul class="Mode">
                        <li id="ReturnePurchaseTitle">商品退貨</li>
                    </ul>
                    <ul>
                        <li>訂單編號：<span class="ReturnOrderID_msg"></span></li>
                    </ul>
                    <ul class="pointWord">
                        <li>完成後請按<img src="./Image/enter_bg.png" />確認，或按<span style="color: #666666">ESC</span>略過</li>
                    </ul>
                </div>
                 <div class="Uniform_No_msg" style="display: none">
                    <ul class="Mode">
                        <li>統一編號</li>
                    </ul>
                    <ul>
                        <li>統一編號：<span class="UniformNo_msg"></span></li>
                    </ul>
                    <ul class="pointWord">
                        <li>完成後請按<img src="./Image/enter_bg.png" />確認，或按<span style="color: #666666">ESC</span>略過</li>
                    </ul>
                </div>
            
            </div>

            <!--右邊鍵入區-->
             <form onsubmit="scan();return false;">
            <div class="Keyin">
                <ul>
                    <li class="li-1">
                       <input class="li-1 InputBox" type="text" style="font-size: 24pt" />
                        </li>
                   <li style="width:150px;" class="ESC">ESC</li>
                    <li class="li-0 OpenCashBox">開錢箱</li>
                    <li class="uniformNumber">統一編號</li>
                    <li class="InputBarcode">輸入型號</li>
                    <li class="backspace"></li>
                    <li class="li-0 number">7</li>
                    <li class="number">8</li>
                    <li class="number">9</li>
                    <li style="width: 150px;" class="cash">現金交易</li>
                    <li class="li-0 number" >4</li>
                    <li class="number">5</li>
                    <li class="number">6</li>
                    <li style="width: 150px;" class="creditcard">信用卡交易</li>
                    <li class="li-0 number">1</li>
                    <li class="number">2</li>
                    <li class="number">3</li>
                     <li class="enter"></li>
                    <li class="li-0 number" style="width: 174px;">0</li>
                    <li class="VIP">VIP</li>
                    <li style="width: 150px;" class="OrderFinish">交易完成</li>
                </ul>

            </div>

                 </form>
        </div>


        </div>

     <div id="SerialItemList" style="display:none"></div>
    <div id="VIPdialog" style="display:none;overflow: hidden;">
        <form onsubmit="VipScan();return false;">
            <input id="VIPInputBox" type="text" />
            <input type="button" onclick="VipScan();" value="送出" />&nbsp;&nbsp;
            <span id="UseBonusDIV" style="display: none;color:red">
            使用紅利
                <span class="switch UseBonus"></span>
                </span>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <span id="BirthdayDiscountDIV" style="display: none;color:red">生日折扣
                 <span class="switch BirthdayDiscount"></span>
            </span>
        </form><br />
        <br /> 
        <div class="VipTable" style="width:100%;height:350px;display:none;overflow-y:hidden">
        <table>
              <tbody>
               <tr><td>VIP卡號</td><td><span id="vip_id"></span>&nbsp;&nbsp;<a id="VipQuery" href="#">修改資料</a></td></tr>
              <tr><td>姓名</td><td id="vip_name"></td></tr>
              <tr><td>行動電話</td><td id="vip_mobile"></td></tr>
              <tr><td >生日</td><td id="vip_birthday"></td></tr>
              <tr><td>上一次使用壽星折扣</td><td id="vip_last_birthday"></td></tr>
              <tr><td>有效日期</td><td id="vip_valid_date"></td></tr>
              <tr><td>折扣</td><td id="vip_discount"></td></tr>
              <tr><td>累積紅利</td><td id="vip_bonus"></td></tr>
        </tbody>
        </table>
            </div>
    </div>
</body>
</html>
