using System;
using System.Collections.Generic;

namespace DEFRA.SGS.Listener.Models
{
    public class InputParameter
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class OwningExtension
    {
        public string Id { get; set; }
        public List<object> KeyAttributes { get; set; }
        public string LogicalName { get; set; }
        public object Name { get; set; }
        public object RowVersion { get; set; }
    }

    public class SharedVariable
    {
        public string key { get; set; }
        public object value { get; set; }
    }

    public class RemoteExecutionContext
    {
        public string BusinessUnitId { get; set; }
        public string CorrelationId { get; set; }
        public int Depth { get; set; }
        public string InitiatingUserAzureActiveDirectoryObjectId { get; set; }
        public string InitiatingUserId { get; set; }
        public List<InputParameter> InputParameters { get; set; }
        public bool IsExecutingOffline { get; set; }
        public bool IsInTransaction { get; set; }
        public bool IsOfflinePlayback { get; set; }
        public int IsolationMode { get; set; }
        public string MessageName { get; set; }
        public int Mode { get; set; }
        public string OperationCreatedOn { get; set; }
        public string OperationId { get; set; }
        public string OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public List<object> OutputParameters { get; set; }
        public OwningExtension OwningExtension { get; set; }
        public object ParentContext { get; set; }
        public List<object> PostEntityImages { get; set; }
        public List<object> PreEntityImages { get; set; }
        public string PrimaryEntityId { get; set; }
        public string PrimaryEntityName { get; set; }
        public string RequestId { get; set; }
        public string SecondaryEntityName { get; set; }
        public List<SharedVariable> SharedVariables { get; set; }
        public int Stage { get; set; }
        public string UserAzureActiveDirectoryObjectId { get; set; }
        public string UserId { get; set; }
    }
}