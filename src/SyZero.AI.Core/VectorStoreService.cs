using Grpc.Core;
using Milvus.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SyZero.AI.Core
{
    public class VectorStoreService
    {
        private readonly MilvusClient milvusClient;

        public VectorStoreService()
        {
            milvusClient = new MilvusClient("192.168.2.130", 19530);
        }
        public async Task Test(long id, ReadOnlyMemory<float> vectors)
        {
            string collectionName = "book";

            var hasCollection = await milvusClient.HasCollectionAsync(collectionName);
            if (!hasCollection)
            {
                var fields = new List<FieldSchema>
                {
                    FieldSchema.Create("id", MilvusDataType.Int64, isPrimaryKey: true),
                    FieldSchema.CreateFloatVector("vector_field", 768)
                };
                // 创建集合
                var createStatus = await milvusClient.CreateCollectionAsync(collectionName, fields);
            }

            var collection2 = milvusClient.GetCollection(collectionName);

            // 假设我们要插入的向量和ID
            long[] ids = { id };
            ReadOnlyMemory<float>[] vectors2 =
            {
             vectors
            };

            // 创建字段数据
            var idFieldData = FieldData.Create("id", ids);
            var vectorFieldData = FieldData.CreateFloatVector("vector_field", vectors2);

            // 将字段数据组合到列表中
            var fieldDataList = new List<FieldData>
            {
                idFieldData,
                vectorFieldData
            };


            var insertResult = await collection2.InsertAsync(fieldDataList);


        }


        public async Task SearchAsync(ReadOnlyMemory<float> vectors)
        {
            string collectionName = "book";

            
            var collection2 = milvusClient.GetCollection(collectionName);
            await collection2.CreateIndexAsync("vector_field", metricType: SimilarityMetricType.L2);
            await collection2.LoadAsync();

            ReadOnlyMemory<float>[] vectors2 =
           {
             vectors
            };
            var searchResult = await collection2.SearchAsync("vector_field", vectors2, SimilarityMetricType.L2, 2);

            var pp = searchResult.Ids;
        }
    }
}
