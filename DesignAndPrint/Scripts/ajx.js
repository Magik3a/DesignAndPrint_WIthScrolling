var ajx = {
    config: {
        url: ''
    },
    invoke: function (method, data, successCB, failureCB) {
        successCB = successCB || "";
        failureCB = failureCB || "";

        $.ajax({
            type: "POST",
            url: this.config.url + method,
            data: data,
            async: true,
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                successCB(response);
            },
            error: failureCB
        });
    }

};