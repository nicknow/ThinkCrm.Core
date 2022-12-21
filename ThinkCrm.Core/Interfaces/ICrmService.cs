using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace ThinkCrm.Core.Interfaces
{
    public interface ICrmService : IOrganizationService
    {
        T Retrieve<T>(string entityName, Guid id, ColumnSet columnSet) where T : Entity;
        Entity Retrieve(string entityName, Guid id);
        T Retrieve<T>(string entityName, Guid id) where T : Entity;
        Entity Retrieve(string entityName, Guid id, params string[] columns);
        T Retrieve<T>(string entityName, Guid id, params string[] columns) where T : Entity;
        TResponse Execute<TResponse>(OrganizationRequest request) where TResponse : OrganizationResponse;
        List<Entity> RetrieveMultipleAll(QueryBase query, int batchSize = -1, int maxRecords = -1);


    }
}