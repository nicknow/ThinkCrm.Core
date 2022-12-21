using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core.CrmService
{
    public class CrmService : ICrmService
    {
        private const string ClassName = "CrmService";
        private readonly IOrganizationService _service;
        private readonly bool _throwErrors;
        private CrmVersion _crmVersion;
        private readonly ILogging _logger;

        public CrmService(IOrganizationService service, ILogging logger = null, bool throwErrors = true, bool doVersionCheck = false, CrmVersion crmVersion = CrmVersion.Unknown )
        {
            _service = service;
            _throwErrors = throwErrors;
            _crmVersion = crmVersion;
            if (service == null) throw new ArgumentNullException(nameof(service));

            _logger = logger ?? new NullLogger();
            
            if (doVersionCheck) CheckCrmVersion();
        }

        public ICrmService GetNewCrmService(ILogging logger = null, bool throwErrors = true, bool doVersionCheck = false,
            CrmVersion crmVersion = CrmVersion.Unknown)
        {
            return new CrmService(_service, logger, throwErrors, doVersionCheck, crmVersion);
        }

        public Guid Create(Entity entity)
        {
            _logger.WithCaller(ClassName).Write("Creating Entity");
            var result = Run<Guid>(() => _service.Create(entity));
            return _logger.WithCaller(ClassName).WriteAndReturn(result, $"Created Entity: {result}");
        }

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
        {
            _logger.WithCaller(ClassName).Write($"Retrieve Entity: {entityName}, {id}, {(columnSet.AllColumns ? "All Columns" : string.Join(", ", columnSet.Columns))} ");
            var result = Run<Entity>(() => _service.Retrieve(entityName, id, columnSet));
            return _logger.WithCaller(ClassName).WriteAndReturn(result, $"Retrieved Entity: {result}");
        }

        public void Update(Entity entity)
        {
            _logger.WithCaller(ClassName)
                .Write(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    $"Updating Entity: {entity.LogicalName}; {(entity?.Id.ToString() ?? "(No Id)")}; {(entity?.KeyAttributes?.Count < 1 ? "(No Keys)" : string.Join(", ", entity.KeyAttributes.ToList().Select(x => ($"Key={x.Key}/Value={x.Value.ToString()}")).ToList()))};");
            Run(() => _service.Update(entity));
            _logger.WithCaller(ClassName).Write("Updated Entity");
        }

        public void Delete(string entityName, Guid id)
        {
            _logger.WithCaller(ClassName).Write($"Deleting Entity: {entityName}, {id}");
            Run(() => _service.Delete(entityName, id));
            _logger.WithCaller(ClassName).Write("Deleted Entity");
        }

        public OrganizationResponse Execute(OrganizationRequest request)
        {
            _logger.WithCaller(ClassName).Write($"Executing Request: {request.RequestName}");
            var result = Run<OrganizationResponse>(() => _service.Execute(request));
            return _logger.WithCaller(ClassName).WriteAndReturn(result, "Execute Completed");
        }

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            _logger.WithCaller(ClassName).Write($"Associate Request: {entityName}/{entityId}/{relationship.SchemaName}/{relationship.PrimaryEntityRole?.ToString() ?? string.Empty}/{EntityReferenceCollectionToString(relatedEntities)}");
            Run(() => _service.Associate(entityName, entityId, relationship, relatedEntities));
            _logger.WithCaller(ClassName).Write("Associate Entity Completed");
        }

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
        {
            _logger.WithCaller(ClassName).Write($"Disassociate Request: {entityName}/{entityId}/{relationship.SchemaName}/{relationship.PrimaryEntityRole?.ToString() ?? string.Empty}/{EntityReferenceCollectionToString(relatedEntities)}");
            Run(() => _service.Disassociate(entityName, entityId, relationship, relatedEntities));
            _logger.WithCaller(ClassName).Write("Disassociate Entity Completed");
        }

        public EntityCollection RetrieveMultiple(QueryBase query)
        {
            if (query is QueryExpression) _logger.WithCaller(ClassName).Write($"Retrieve Multple, QueryExpression: {((QueryExpression)query).EntityName}");
            else if (query is FetchExpression) _logger.WithCaller(ClassName).Write($"Retrieve Multiple, FetchExpression.");
            try
            {
                var result = Run<EntityCollection>(() => _service.RetrieveMultiple(query));
                return _logger.WithCaller(ClassName)
                    .WriteAndReturn(result, $"RetrieveMultiple Success: Records returned={result.Entities.Count()}");
            }
            catch (Exception ex)
            {
                if (query is FetchExpression)
                {
                    var q = (FetchExpression) query;
                    _logger.WithCaller(ClassName).Write("Exception Executing RetrieveMultiple with FetchExpression");
                    _logger.WithCaller(ClassName).Write(ex);
                    _logger.WithCaller(ClassName).Write(q.Query);

                }
                else if (query is QueryExpression)
                {
                    var q = (QueryExpression) query;
                    _logger.WithCaller(ClassName).Write("Exception Executing RetrieveMultiple with QueryExpression");
                    _logger.WithCaller(ClassName).Write(ex);

                }
                if (_throwErrors) throw;
                return null;
            }

        }


        public List<Entity> RetrieveMultipleAll(QueryBase query, int batchSize = -1, int maxRecords = -1)
        {
            if (query == null)
            {
                if (_throwErrors) throw new ArgumentNullException(nameof(query));
                return null;
            }

            if (query is QueryExpression) _logger.WithCaller(ClassName).Write($"Retrieve Multple (Page All), QueryExpression: {((QueryExpression)query).EntityName}");
            else if (query is FetchExpression) _logger.WithCaller(ClassName).Write($"Retrieve Multiple (Page All), FetchExpression.");

            var results = new List<Entity>();

            if (maxRecords == 0) return results;

            var pageNumber = 1;

            while (true)
            {
                var entColl = RetrieveMultiple(query);

                if (entColl.Entities.Any())
                {
                    results.AddRange(entColl.Entities);
                }
                else return results;

                if (maxRecords > 0 && results.Count >= maxRecords) return results;

                if (!entColl.MoreRecords) return results;
                else
                {
                    pageNumber++;
                    query = SetPagingInfo(query, entColl.PagingCookie, pageNumber);
                }

                _logger.WithCaller(ClassName).Write($"Total Retrieved: {results.Count}");

            }

        }

        private QueryBase SetPagingInfo(QueryBase query, string pagingCookie, int pageNumberForFetchQuery)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (pagingCookie == null) throw new ArgumentNullException(nameof(pagingCookie));

            if (query is QueryExpression)
            {
                var q = (QueryExpression)query;
                q.PageInfo.PageNumber++;
                q.PageInfo.PagingCookie = pagingCookie;
                return q;
            }
            else if (query is FetchExpression)
            {
                var q = (FetchExpression)query;
                return new FetchExpression(CreateXml(q.Query, pagingCookie, pageNumberForFetchQuery));

            }
            else
            {
                throw new ArgumentException($"{nameof(query)} is not QueryExpression or FetchExpression.");
            }
        }

        public TEntity Retrieve<TEntity>(string entityName, Guid id, ColumnSet columnSet) where TEntity : Entity
        {
            return Retrieve(entityName, id, columnSet).ToEntity<TEntity>();
        }

        public Entity Retrieve(string entityName, Guid id)
        {
            return Retrieve(entityName, id, new ColumnSet(true));
        }

        public TEntity Retrieve<TEntity>(string entityName, Guid id) where TEntity : Entity
        {
            return Retrieve(entityName, id).ToEntity<TEntity>();
        }

        public Entity Retrieve(string entityName, Guid id, params string[] columns)
        {
            return Retrieve(entityName, id, columns.Any() ? new ColumnSet(columns) : new ColumnSet(true));
        }

        public TEntity Retrieve<TEntity>(string entityName, Guid id, params string[] columns) where TEntity : Entity
        {
            return Retrieve(entityName, id, columns).ToEntity<TEntity>();
        }

        public TResponse Execute<TResponse>(OrganizationRequest request) where TResponse : OrganizationResponse
        {
            return Execute(request) as TResponse;
        }

        #region Private Methods

        private void CheckCrmVersion()
        {
            var version = new Version(this.Execute<RetrieveVersionResponse>(new RetrieveVersionRequest()).Version);

            _crmVersion = version >= new Version("7.1") ? CrmVersion.v71 : CrmVersion.Pre_v71;
        }

        private void Run(Action method)
        {
            Run<object>(() =>
            {
                method();
                return null;
            });
        }

        private T Run<T>(Func<T> method)
        {
            if (method == null)
            {
                _logger.Write($"Null Argument: {nameof(method)}");
                if (_throwErrors) throw new ArgumentNullException(nameof(method));
                return default(T);
            }

            try
            {
                var result = method();
                return _logger.WriteAndReturn(result, "Completed Without Exception");
            }
            catch (Exception ex)
            {
                _logger.Write("Exception During Crm Service Operation");
                _logger.Write(ex);
                if (_throwErrors) throw;
                return default(T);
            }

        }

        private string EntityReferenceCollectionToString(EntityReferenceCollection entityReferenceCollection)
        {
            if (entityReferenceCollection.Any())
                return string.Join(", ",
                    entityReferenceCollection.ToList().Select(x => ($"LogicalName={x.LogicalName}/Id={x.Id};")).ToList());

            return string.Empty;
        }

        #endregion

        #region FetchXml Paging

        private string CreateXml(string xml, string cookie, int page)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return CreateXml(doc, cookie, page);
        }

        private string CreateXml(XmlDocument doc, string cookie, int page)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            StringBuilder sb = new StringBuilder();
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }

        #endregion

    }

    public enum CrmVersion
    {
        Unknown,
        Pre_v71,
        v71
    }

}
