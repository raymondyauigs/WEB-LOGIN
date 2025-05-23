﻿
function setupLoginBtn(loginbtn) {
    var listener = new window.keypress.Listener();
    listener.register_many([
        {
            keys: "enter",
            on_release: function (e) {
                Login();


                return true;
            },
        },
    ]);
    $(loginbtn).click(function (e) {
        Login();
    });
}


function Login() {
    var paras =
    {
        account: $("#account").val(),
        password: $("#password").val(),
        returnUrl: $('#returnUrl').val(),
        __RequestVerificationToken: $('input[name="__RequestVerificationToken').val(),
    };

    $('body').mLoading({ text: 'try login ...' });


    JWTbox.sendAjaxRequest({
        type: "post",
        url: $('form').attr('action'),
        param: paras,
        dataType: "json",
        callBack: function (result) {
            $('body').mLoading('hide');
            if (typeof result === "string" ) {
                console.log("Login fail");
                
                alertLib.Core.alert('Login Result', result =='n' ? 'Login Failed': result, 'Close', { mode: 'danger' });

            } else {

                window.location.href = result.returnUrl;

            }
        }
    });
}

$(document).ready(function () {
    themeLib.Core.setupTheme(null);
    setupLoginBtn('.btn-login');
});