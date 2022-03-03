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
    operationNameOutbound: "new_ApplicationOutboundQueue",
    operationNameAgreementCalc: "new_AgreementCalculateRequest",

    onload: function (executionContext) {
        debugger;
        application.formContext = executionContext.getFormContext();

        const submitted = 100000000;
        const underReview = 100000001;
        const reviewCompleted = 100000002;
        const approved = 100000003;
        const rejected = 100000004;

        var applicationStatusFld = application.formContext.getAttribute("new_applicationstatus");
        var applicationStatusVal = null;

        if (applicationStatusFld !== null && applicationStatusFld !== undefined) {
            applicationStatusVal = applicationStatusFld.getValue();
        }
        else {
            return;
        }

        if (applicationStatusVal !== null && (applicationStatusVal === reviewCompleted || applicationStatusVal === approved || applicationStatusVal === rejected)) {

            var applicationId = application.formContext.data.entity.getId().replace("{", "").replace("}", "");
            var qry = "?$select=activityid,statecode&$filter=_regardingobjectid_value eq " + applicationId + " and statecode eq 0";

            Xrm.WebApi.online.retrieveMultipleRecords("task", qry).then(
                function success(results) {
                    debugger;
                    console.log(results);

                    if (results.entities.length) {
                        alert("Cannot set to the selected Application Status as there are open Tasks associated");
                    }
                },
                function (error) {
                    console.log(error.message);
                }
            );
        }
    },
    CallCustomActionFromJavaScript: function (context, route) {

        application.formContext = context;
        application.routing = route;

        application.timestampValue = application.formContext.getAttribute("new_timestamp").getValue();

        // begin polling for returned value
        application.refreshField(application.formContext);

        if (application.routing === application.outboundSpinner || application.routing === application.agreementCalcSpinner)
            Xrm.Utility.showProgressIndicator("Please wait a moment...");

        var appId = context.data.entity.getId();
        appId = application.cleanIdField(appId);

        var req = {};
        var target = { entityType: "new_application", id: appId };
        req.entity = target;

        //var parameters = {
        //    ApplicationID: appId
        //};
        //req.ApplicationID = parameters.ApplicationID;

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
                        "typeName": "mscrm.new_application",
                        "structuralProperty": 5
                    }//,
                    //parameterTypes: {
                    //    "ApplicationID": {
                    //        "typeName": "Edm.Guid",
                    //        "structuralProperty": 1
                    //    }
                    //}
                }
            };
        }

        Xrm.WebApi.online.execute(req)
            .then(function (response) { console.log("Success!")/*alert("Success!")*/ }, function (e) { alert(e.message); });
    },

    polling: function (callback) {

        var applicationId = application.formContext.data.entity.getId().replace("{", "").replace("}", "");

        Xrm.WebApi.online.retrieveRecord("new_application", applicationId,
            "?$select=new_timestamp,new_singlebusinessidentifier,new_uniqueorganisationidentifier,new_organisationname,new_organisationaddress").then(
                function success(result) {
                    console.log(result);
                    debugger;

                    if (application.timestampValue !== result.new_timestamp) {
                        application.timestampValue = result.new_timestamp;
                        application.sbiValue = result.new_singlebusinessidentifier;
                        application.orgIdValue = result.new_uniqueorganisationidentifier;
                        application.nameValue = result.new_organisationname;
                        application.addressValue = result.new_organisationaddress;
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
                application.formContext.getAttribute("new_timestamp").setValue(application.timestampValue);
                application.formContext.getAttribute("new_singlebusinessidentifier").setValue(application.sbiValue);
                application.formContext.getAttribute("new_uniqueorganisationidentifier").setValue(application.orgIdValue);
                application.formContext.getAttribute("new_organisationname").setValue(application.nameValue);
                application.formContext.getAttribute("new_organisationaddress").setValue(application.addressValue);
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


