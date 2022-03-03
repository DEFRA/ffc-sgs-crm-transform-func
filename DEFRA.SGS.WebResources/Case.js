var GrantCase = {
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
    operationNameAgreementCalc: "dff_CaseAgreementCalculateRequest",
    notificationTime: 180000,
    timeOutVal: null,

    CallCustomActionFromJavaScript: function (context, route) {

        GrantCase.formContext = context;
        GrantCase.routing = route;

        GrantCase.timestampValue = GrantCase.formContext.getAttribute("dff_timestamp").getValue();

        // begin polling for returned value
        GrantCase.refreshField(GrantCase.formContext);

        if (GrantCase.routing === GrantCase.outboundSpinner || GrantCase.routing === GrantCase.agreementCalcSpinner)
            Xrm.Utility.showProgressIndicator("Please wait a moment...");

        var appId = context.data.entity.getId();
        appId = GrantCase.cleanIdField(appId);

        var req = {};
        var target = { entityType: "dff_case", id: appId };
        req.entity = target;

        var operationName = GrantCase.operationNameOutbound;
        debugger;
        if (GrantCase.routing === GrantCase.agreementCalc || GrantCase.routing === GrantCase.agreementCalcSpinner)
            operationName = GrantCase.operationNameAgreementCalc;

        req.getMetadata = function () {
            return {
                boundParameter: "entity",
                operationType: 0,
                operationName: operationName,
                parameterTypes: {
                    "entity": {
                        "typeName": "mscrm.dff_case",
                        "structuralProperty": 5
                    }
                }
            };
        }

        Xrm.WebApi.online.execute(req)
            .then(function (response) { console.log("Success!") }, function (e) { alert(e.message); });
    },

    CallNotifyActionFromJavaScript: function (context) {

        GrantCase.formContext = context;

        var caseId = context.data.entity.getId();
        caseId = GrantCase.cleanIdField(caseId);

        var req = {};
        req.entity = { entityType: "dff_case", id: caseId };
        req.TemplateID = "8a27798d-b2d6-4a58-b719-78937fe81024";  // TODO: hard-coded for now
        req.CalledFrom = "JavaScript";

        var operationName = "dff_NotifyCaseRequest";
        debugger;

        req.getMetadata = function () {
            return {
                boundParameter: "entity",
                operationType: 0,
                operationName: operationName,
                parameterTypes: {
                    "entity": {
                        "typeName": "mscrm.dff_case",
                        "structuralProperty": 5
                    },
                    "TemplateID": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    },
                    "CalledFrom": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    }
                }
            };
        }

        Xrm.WebApi.online.execute(req)
            .then(function (response) { console.log("Success!") }, function (e) { alert(e.message); });
    },

    polling: function (callback) {

        var caseId = GrantCase.formContext.data.entity.getId().replace("{", "").replace("}", "");

        Xrm.WebApi.online.retrieveRecord("dff_case", caseId,
            "?$select=dff_timestamp,dff_singlebusinessidentifier,dff_uniqueorganisationidentifier,dff_organisationname,dff_organisationaddress").then(
                function success(result) {
                    console.log(result);
                    debugger;

                    if (GrantCase.timestampValue !== result.dff_timestamp) {
                        GrantCase.timestampValue = result.dff_timestamp;
                        GrantCase.sbiValue = result.dff_singlebusinessidentifier;
                        GrantCase.orgIdValue = result.dff_uniqueorganisationidentifier;
                        GrantCase.nameValue = result.dff_organisationname;
                        GrantCase.addressValue = result.dff_organisationaddress;

                        clearTimeout(GrantCase.timeOutVal);

                        return callback();
                    } else {
                        GrantCase.polling(callback);
                        return null;
                    }
                },
                function (error) {
                    console.log(error.message);
                }
            );
    },
    refreshField: function () {
        GrantCase.timeOutVal = setTimeout(
            function () {
                //alert("Timed Out");
                if (GrantCase.routing === GrantCase.outboundSpinner || GrantCase.routing === GrantCase.agreementCalcSpinner)
                    Xrm.Utility.closeProgressIndicator();
            },
            GrantCase.notificationTime
        );

        GrantCase.polling(
            function () {
                console.log("value received");
                GrantCase.formContext.getAttribute("dff_timestamp").setValue(GrantCase.timestampValue);
                GrantCase.formContext.getAttribute("dff_singlebusinessidentifier").setValue(GrantCase.sbiValue);
                GrantCase.formContext.getAttribute("dff_uniqueorganisationidentifier").setValue(GrantCase.orgIdValue);
                GrantCase.formContext.getAttribute("dff_organisationname").setValue(GrantCase.nameValue);
                GrantCase.formContext.getAttribute("dff_organisationaddress").setValue(GrantCase.addressValue);
                if (GrantCase.routing === GrantCase.outboundSpinner || GrantCase.routing === GrantCase.agreementCalcSpinner)
                    Xrm.Utility.closeProgressIndicator();
            }
        );
    },
    clearFields: function (context) {
        if (GrantCase.formContext === null)
            GrantCase.formContext = context;

        GrantCase.formContext.getAttribute("dff_timestamp").setValue(null);
        GrantCase.formContext.getAttribute("dff_singlebusinessidentifier").setValue(null);
        GrantCase.formContext.getAttribute("dff_uniqueorganisationidentifier").setValue(null);
        GrantCase.formContext.getAttribute("dff_organisationname").setValue(null);
        GrantCase.formContext.getAttribute("dff_organisationaddress").setValue(null);

        GrantCase.formContext.data.save().then(function () { console.log("Successfully Saved "); }, function () { console.log("Something Went Wrong"); });
    },
    cleanIdField: function (id) {
        id = id.replace("{", "");
        id = id.replace("}", "");
        return id;
    }
}


