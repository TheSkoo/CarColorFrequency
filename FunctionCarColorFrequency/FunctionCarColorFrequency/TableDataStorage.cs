using Azure;
using Azure.Data.Tables;
using CarColorFrequencyApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FunctionCarColorFrequency
{
    internal class TableDataStorage
    {
        public static readonly string STORAGE_ACCOUNT_NAME = "carcolorstorageaccount";
        public static readonly string PARTITION_KEY = "CarColorData";
        public static readonly string TABLE_NAME = "CarColorDictionary";
        public static readonly string ACCOUNT_KEY = "bs7Wlk0cjcttlacrUo8X7vHAZjan4HmTPqXhciYjmNE75L2pWefWS8AKNwihK6HVqnw1b0xIA+yN+ASt5jdsdg==";
        public static readonly string CONNECTION_STRING = 
            string.Format(
                    "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net",
                    STORAGE_ACCOUNT_NAME,
                    ACCOUNT_KEY);

        #region default Color Table Enties
        private static List<ColorDataEntity> DefaultTableEntries = new List<ColorDataEntity>()
        {
            new ColorDataEntity()
            {
                PartitionKey = PARTITION_KEY,
                RowKey = Guid.NewGuid().ToString(),
                ColorDictId = 1,
                Color = "Black",
                BackgroundColorRGB = 0,
                ForegroundColorRGB = 16777215,
                Count = 0
            },
            new ColorDataEntity()
            {
                PartitionKey = PARTITION_KEY,
                RowKey = Guid.NewGuid().ToString(),
                ColorDictId = 2,
                Color = "White",
                BackgroundColorRGB = 16777215,
                ForegroundColorRGB = 0,
                Count = 0
            },
            new ColorDataEntity()
            {
                PartitionKey = PARTITION_KEY,
                RowKey = Guid.NewGuid().ToString(),
                ColorDictId = 3,
                Color = "Silver",
                BackgroundColorRGB = 12632256,
                ForegroundColorRGB = 0,
                Count = 0
            },
            new ColorDataEntity()
            {
                PartitionKey = PARTITION_KEY,
                RowKey = Guid.NewGuid().ToString(),
                ColorDictId = 4,
                Color = "Gray",
                BackgroundColorRGB = 9013641,
                ForegroundColorRGB = 0,
                Count = 0
            },
            new ColorDataEntity()
            {
                PartitionKey = PARTITION_KEY,
                RowKey = Guid.NewGuid().ToString(),
                ColorDictId = 5,
                Color = "Yellow",
                BackgroundColorRGB = 16768545,
                ForegroundColorRGB = 0,
                Count = 0
            },
            new ColorDataEntity()
            {
                PartitionKey = PARTITION_KEY,
                RowKey = Guid.NewGuid().ToString(),
                ColorDictId = 6,
                Color = "Black",
                BackgroundColorRGB = 16753920,
                ForegroundColorRGB = 0,
                Count = 0
            },
            new ColorDataEntity()
            {
                PartitionKey = PARTITION_KEY,
                RowKey = Guid.NewGuid().ToString(),
                ColorDictId = 7,
                Color = "Red",
                BackgroundColorRGB = 16722988,
                ForegroundColorRGB = 0,
                Count = 0
            },
            new ColorDataEntity()
            {
                PartitionKey = PARTITION_KEY,
                RowKey = Guid.NewGuid().ToString(),
                ColorDictId = 8,
                Color = "Green",
                BackgroundColorRGB = 32768,
                ForegroundColorRGB = 16777215,
                Count = 0
            },
            new ColorDataEntity()
            {
                PartitionKey = PARTITION_KEY,
                RowKey = Guid.NewGuid().ToString(),
                ColorDictId = 9,
                Color = "Blue",
                BackgroundColorRGB = 255,
                ForegroundColorRGB = 16777215,
                Count = 0
            },
        };
        #endregion

        public static async Task CheckIfStorageExists()
        {
            TableClient tableClient = new TableClient(CONNECTION_STRING, TABLE_NAME);

            // Query entities using LINQ expressions
            AsyncPageable<TableEntity> queryResults = null;
            try
            {
                queryResults = tableClient.QueryAsync<TableEntity>(ent => ent.PartitionKey == PARTITION_KEY);
                var firstPage = await queryResults.AsPages().FirstOrDefaultAsync();
            }
            catch (Azure.RequestFailedException e)
            {
                if (e.ErrorCode == "TableNotFound")
                {
                    Console.WriteLine("Table not found. Creating table...");
                    await tableClient.CreateIfNotExistsAsync();

                    foreach (var cdEntity in DefaultTableEntries)
                    {
                        tableClient.AddEntity(cdEntity);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static async Task<List<ColorData>> GetColorData()
        {
            List<ColorData> result = new List<ColorData>();

            TableClient tableClient = new TableClient(CONNECTION_STRING, TABLE_NAME);

            // Query entities using LINQ expressions
            Pageable<ColorDataEntity> queryResults = tableClient.Query<ColorDataEntity>();
            foreach (ColorDataEntity qEntity in queryResults)
            {
                result.Add(new ColorData()
                {
                    ColorDictId = qEntity.ColorDictId,
                    Color = qEntity.Color,
                    BackgroundColorRGB = qEntity.BackgroundColorRGB,
                    ForegroundColorRGB = qEntity.ForegroundColorRGB,
                    Count = qEntity.Count
                });
            }

            return result;
        }

        public static async Task UpdateColorData(List<ColorData> colorData)
        {
            TableClient tableClient = new TableClient(CONNECTION_STRING, TABLE_NAME);

            // Get all entities in the partition
            Pageable<ColorDataEntity> entities = tableClient.Query<ColorDataEntity>(ent => ent.PartitionKey == PARTITION_KEY);

            foreach (var colorDatum in colorData)
            {
                var entityToUpdate = entities.FirstOrDefault(e => e.ColorDictId == colorDatum.ColorDictId);
                if (entityToUpdate != null)
                {
                    entityToUpdate.Count = colorDatum.Count;
                    await tableClient.UpdateEntityAsync(entityToUpdate, entityToUpdate.ETag);
                }
            }

        }
    }
}
