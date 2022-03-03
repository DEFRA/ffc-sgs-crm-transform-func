var application = {
    timestampValue: null,
    sbiValue: null,
    orgIdValue: null,
    nameValue: null,
    addressValue: null,
    formContext: null,
    routing: 1,
    outbound: 1,
    outboundSpinner: 2,
    agreementCalc: 3,
    agreementCalcSpinner: 4,
    operationNameOutbound: "dff_ApplicationOutboundQueue",
    operationNameAgreementCalc: "dff_AgreementCalculateRequest",

    CallCustomActionFromJavaScript: function (context, route) {

        application.formContext = context;
        application.routing = route;

        application.timestampValue = application.formContext.getAttribute("dff_timestamp").getValue();

        // begin polling for returned value
        application.refreshField(application.formContext);

        if (application.routing === application.outboundSpinner || application.routing === application.agreementCalcSpinner)
            Xrm.Utility.showProgressIndicator("Please wait a moment...");

        var appId = context.data.entity.getId();
        appId = application.cleanIdField(appId);

        var req = {};
        var target = { entityType: "dff_grantapplication", id: appId };
        req.entity = target;

        var operationName = application.operationNameOutbound;
        debugger;
        if (application.routing === application.agreementCalc || application.routing === application.agreementCalcSpinner)
            operationName = application.operationNameAgreementCalc;

        req.getMetadata = function () {
            return {
                boundParameter: "entity",
                operationType: 0,
                operationName: operationName,
                parameterTypes: {
                    "entity": {
                        "typeName": "mscrm.dff_grantapplication",
                        "structuralProperty": 5
                    }
                }
            };
        }

        Xrm.WebApi.online.execute(req)
            .then(function (response) { console.log("Success!") }, function (e) { alert(e.message); });
    },

    polling: function (callback) {

        var applicationId = application.formContext.data.entity.getId().replace("{", "").replace("}", "");

        Xrm.WebApi.online.retrieveRecord("dff_grantapplication", applicationId,
            "?$select=dff_timestamp,dff_singlebusinessidentifier,dff_uniqueorganisationidentifier,dff_organisationname,dff_organisationaddress").then(
                function success(result) {
                    console.log(result);
                    debugger;

                    if (application.timestampValue !== result.dff_timestamp) {
                        application.timestampValue = result.dff_timestamp;
                        application.sbiValue = result.dff_singlebusinessidentifier;
                        application.orgIdValue = result.dff_uniqueorganisationidentifier;
                        application.nameValue = result.dff_organisationname;
                        application.addressValue = result.dff_organisationaddress;
                        return callback();
                    } else {
                        application.polling(callback);
                        return null;
                    }
                },
                function (error) {
                    console.log(error.message);
                }
            );
    },
    refreshField: function () {
        application.polling(
            function () {
                console.log("value received");
                application.formContext.getAttribute("dff_timestamp").setValue(application.timestampValue);
                application.formContext.getAttribute("dff_singlebusinessidentifier").setValue(application.sbiValue);
                application.formContext.getAttribute("dff_uniqueorganisationidentifier").setValue(application.orgIdValue);
                application.formContext.getAttribute("dff_organisationname").setValue(application.nameValue);
                application.formContext.getAttribute("dff_organisationaddress").setValue(application.addressValue);
                if (application.routing === application.outboundSpinner || application.routing === application.agreementCalcSpinner)
                    Xrm.Utility.closeProgressIndicator();
            }
        );
    },

    cleanIdField: function (id) {
        id = id.replace("{", "");
        id = id.replace("}", "");
        return id;
    }
}


