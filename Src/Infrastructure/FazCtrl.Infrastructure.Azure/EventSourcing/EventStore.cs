using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FazCtrl.Contract.Interfaces;
using Microsoft.Azure.Cosmos.Table;

namespace FazCtrl.Infrastructure.Azure.EventSourcing
{
    public class EventStore : IRepository
    {
        private const string UnpublishedRowKeyPrefix = "Unpublished_";
        private const string RowKeyVersionUpperLimit = "9999999999";

        private readonly IMapper _mapper;
        private readonly CloudTable _table;

        public EventStore(IMapper mapper)
        {
            _mapper = mapper;
            _table = CreateTableAsync("teste").GetAwaiter().GetResult();
        }

        public static async Task<CloudTable> CreateTableAsync(string tableName)
        {
            string storageConnectionString = "UseDevelopmentStorage=true";

            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(storageConnectionString);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            Console.WriteLine("Create a Table for the demo");

            // Create a table client for interacting with the table service 
            CloudTable table = tableClient.GetTableReference(tableName);
            if (await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Created Table named: {0}", tableName);
            }
            else
            {
                Console.WriteLine("Table {0} already exists", tableName);
            }

            Console.WriteLine();
            return table;
        }

        public static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }


        private IQueryable<EventTableServiceEntity> GetEntitiesQuery(string partitionKey, string minRowKey, string maxRowKey)
        {

            var query = _table.CreateQuery<EventTableServiceEntity>()
                .Where(x => x.PartitionKey == partitionKey && x.RowKey.CompareTo(minRowKey) >= 0 && x.RowKey.CompareTo(maxRowKey) <= 0);

            return query.AsQueryable();
        }

        public async Task<IEnumerable<IEventData>> GetAsync(Guid id, string type)
        {
            try
            {
                int version = 0;
                var minRowKey = version.ToString("D10");

                string partitionKey = $"{type}_{id}";
                TableQuery<EventTableServiceEntity> rangeQuery = new TableQuery<EventTableServiceEntity>()
                    .Where(
                        TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                            TableOperators.And,
                            TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, minRowKey),
                                TableOperators.And,
                                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, RowKeyVersionUpperLimit))));


                List<IEventData> list = new List<IEventData>();


                TableContinuationToken token = null;
                rangeQuery.TakeCount = 50;

                do
                {
                    TableQuerySegment<EventTableServiceEntity> segment = await _table.ExecuteQuerySegmentedAsync(rangeQuery, token);

                    token = segment.ContinuationToken;

                    foreach (EventTableServiceEntity entity in segment)
                    {
                        list.Add(_mapper.Map<IEventData>(entity));
                    }
                }
                while (token != null);

                return list;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task SaveAsync(Guid id, string type, IEnumerable<IDomainEvent> events)
        {
            try
            {
                var insertBatch = new TableBatchOperation();

                foreach (var eventData in events)
                {
                    string creationDate = DateTime.UtcNow.ToString("o");
                    var formattedVersion = eventData.Version.ToString("D10");

                    var entity = _mapper.Map(new EventData(eventData), new EventTableServiceEntity
                    {
                        PartitionKey = $"{type}_{id.ToString()}",
                        RowKey = formattedVersion,
                        CreationDate = creationDate,
                    });

                    insertBatch.Insert(entity);

                    // Add a duplicate of this event to the Unpublished "queue"
                    insertBatch.Insert(_mapper.Map(new EventData(eventData), new EventTableServiceEntity
                    {
                        PartitionKey = $"{type}_{id.ToString()}",
                        RowKey = UnpublishedRowKeyPrefix + formattedVersion,
                        CreationDate = creationDate,
                    }));
                }

                TableBatchResult result = _table.ExecuteBatch(insertBatch);

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }

                await Task.CompletedTask;
            }
            catch (Exception e)
            {
                throw;
            }
        }



    }
}
