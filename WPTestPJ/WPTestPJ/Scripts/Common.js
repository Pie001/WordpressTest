'use strict';

$(function() {
    _wpInfoBlock.init();
});

var _wpInfoBlock = {
    $me: null,
    init: function() {
        this.$me = $('.wpInfoBlock');
        this.btnWPUserAddSubmit.init();
        this.wpUserListBlock.init();
        this.wpUserAddModal.init();
    },
    btnWPUserAddSubmit:{
        $me: null,
        init: function() {
            this.$me = $('#btnWPUserAddSubmit');
            this.click();
        },
        click: function() {
            this.$me.click(function(e) {
                _wpInfoBlock.wpUserAddModal.show();
            });
        }
    },
    wpUserListBlock:{
        $me: null,
        init: function() {
            this.$me = $('.wpUserListBlock');
        },
        reset: function(type) {
            $.ajax({
                url: '/Home/ViewWPUserList',
                cache: false
            }).done(function(data) {
                _wpInfoBlock.wpUserListBlock.$me.replaceWith(data);
                _wpInfoBlock.wpUserListBlock.init();
            });
        }
    },
    wpUserAddModal:{
        $me: null,
        init: function() {
            this.$me = $('#wpUserAddModal');
            this.form.init();
            this.btnAddSubmit.init();
        },
        form: {
            $me: null,
            init: function() {
                this.$me = $('#formWPUserAddModal');
            },
            getSerializeArray: function() {
                return this.$me.serializeArray();
            },
            isValid: function() {
                return this.$me.validate().form();
            }
        },
        show: function() {
            this.$me.modal('show');
        },
        hide: function() {
            this.$me.modal('hide');
        },
        btnAddSubmit:{
            $me: null,
            init: function() {
                this.$me = $('#btnAddSubmit');
                this.click();
            },
            click: function() {
                this.$me.click(function(e) {
                    if (_wpInfoBlock.wpUserAddModal.form.isValid()) {
                        _ajax.call('POST', '/Home/AddWPUser', _wpInfoBlock.wpUserAddModal.form.getSerializeArray(), function(response) {
                            if (response != null && response.Result) {
                                _wpInfoBlock.wpUserAddModal.hide();
                                _wpInfoBlock.wpUserListBlock.reset();
                            }
                            else {
                                //_commonUtil.addValidationSummary(response.ErrorList);
                                alert(response.ErrorList);
                            }
                        }, false);
                    }
                });
            }
        }
    }

}


var _ajax = {
    call: function(type, url, data, callback, isAlert) {
        $.ajax({
            type: type,
            timeout: 30000,
            url: url,
            data: data,
            cache: false,
            async: true,
            dataType: 'json',
            success: function(response) {
                if (response.Result) {
                    if (typeof callback === 'function') {
                        callback(response);
                    }
                } else {
                    this.error(response);
                }
            },
            error: function(response) {
                if (response.responseText != null) {
                    response = $.common.parseJson(response.responseText);
                }

                $.each(response.ErrorList, function(index, element) {
                    if (element.Type == 'errorMessage') {
                        if (isAlert) {
                            alert('エラー！ ' + element.Message);
                        }
                        callback(response);
                    }
                });
            },
            complete: function() {
            }
        });
    }
}