using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ETModel
{
    [ObjectSystem]
    public class DbProxyComponentSystem: AwakeSystem<DBProxyComponent>
    {
        public override void Awake(DBProxyComponent self)
        {
            self.Awake();
        }
    }

    /// <summary>
    /// 用来与数据库操作代理
    /// </summary>
    public static class DBProxyComponentEx
    {
        public static void Awake(this DBProxyComponent self)
        {
            StartConfig dbStartConfig = StartConfigComponent.Instance.DBConfig;
            self.dbAddress = dbStartConfig.GetComponent<InnerConfig>().IPEndPoint;
        }

        public static async ETTask Save(this DBProxyComponent self, ComponentWithId component)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
            await session.Call(new DBSaveRequest { Component = component });
        }

        public static async ETTask SaveBatch(this DBProxyComponent self, List<ComponentWithId> components)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
            await session.Call(new DBSaveBatchRequest { Components = components });
        }

        public static async ETTask Save(this DBProxyComponent self, ComponentWithId component, CancellationToken cancellationToken)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
            await session.Call(new DBSaveRequest { Component = component }, cancellationToken);
        }

        public static async ETVoid SaveLog(this DBProxyComponent self, ComponentWithId component)
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
            await session.Call(new DBSaveRequest { Component = component, CollectionName = "Log" });
        }

        public static async ETTask<T> Query<T>(this DBProxyComponent self, long id) where T : ComponentWithId
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
            DBQueryResponse dbQueryResponse = (DBQueryResponse) await session.Call(new DBQueryRequest { CollectionName = typeof(T).Name, Id = id });
            return (T) dbQueryResponse.Component;
        }

        /// <summary>
        /// 根据查询表达式查询
        /// </summary>
        /// <param name="self"></param>
        /// <param name="exp"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async ETTask<List<ComponentWithId>> Query<T>(this DBProxyComponent self, Expression<Func<T, bool>> exp)
                where T : ComponentWithId
        {
            ExpressionFilterDefinition<T> filter = new ExpressionFilterDefinition<T>(exp);
            IBsonSerializerRegistry serializerRegistry = BsonSerializer.SerializerRegistry;
            IBsonSerializer<T> documentSerializer = serializerRegistry.GetSerializer<T>();
            string json = filter.Render(documentSerializer, serializerRegistry).ToJson();
            return await self.Query<T>(json);
        }

        public static async ETTask<List<ComponentWithId>> Query<T>(this DBProxyComponent self, List<long> ids) where T : ComponentWithId
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
            DBQueryBatchResponse dbQueryBatchResponse =
                    (DBQueryBatchResponse) await session.Call(new DBQueryBatchRequest { CollectionName = typeof(T).Name, IdList = ids });
            return dbQueryBatchResponse.Components;
        }

        /// <summary>
        /// 根据json查询条件查询
        /// </summary>
        /// <param name="self"></param>
        /// <param name="json"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async ETTask<List<ComponentWithId>> Query<T>(this DBProxyComponent self, string json) where T : ComponentWithId
        {
            Session session = Game.Scene.GetComponent<NetInnerComponent>().Get(self.dbAddress);
            DBQueryJsonResponse dbQueryJsonResponse =
                    (DBQueryJsonResponse) await session.Call(new DBQueryJsonRequest { CollectionName = typeof(T).Name, Json = json });
            return dbQueryJsonResponse.Components;
        }

        /// <summary>
        /// 根据ID删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async ETTask Delete<T>(this DBProxyComponent self, long id)
        {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            await dbComponent.GetCollection(typeof(T).Name).DeleteOneAsync(i => i.Id == id);
        }

        /// <summary>
        /// 清空表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static async ETTask DeleteAll<T>(this DBProxyComponent self)
        {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            var filter = Builders<ComponentWithId>.Filter.Empty;
            await dbComponent.GetCollection(typeof(T).Name).DeleteManyAsync(filter);
        }

        /// <summary>
        /// 根据表达式删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static async ETTask DeleteAll<T>(this DBProxyComponent self, Expression<Func<T, bool>> exp)
        {
            DBComponent dbComponent = Game.Scene.GetComponent<DBComponent>();
            ExpressionFilterDefinition<T> filter = new ExpressionFilterDefinition<T>(exp);
            IBsonSerializerRegistry serializerRegistry = BsonSerializer.SerializerRegistry;
            IBsonSerializer<T> documentSerializer = serializerRegistry.GetSerializer<T>();
            string json = filter.Render(documentSerializer, serializerRegistry).ToJson();
            await dbComponent.GetCollection(typeof(T).Name).FindOneAndDeleteAsync(json);

        }
    }
}