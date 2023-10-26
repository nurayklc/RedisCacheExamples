using StackExchange.Redis;
using System.Net;

namespace Redis.Sentinel.Services
{
    public class RedisService
    {
        static ConfigurationOptions sentinelOptions => new()
        {
            EndPoints =
            {
                { "localhost", 6383 },
                { "localhost", 6384 },
                { "localhost", 6385 },
            },
            AbortOnConnectFail = false,
            CommandMap = CommandMap.Sentinel
        };

        static ConfigurationOptions masterOptions => new()
        {
            AbortOnConnectFail = false
        };

        public static async Task<IDatabase> RedisMasterDatabase()
        {
            ConnectionMultiplexer sentinelConnection = await ConnectionMultiplexer.SentinelConnectAsync(sentinelOptions);

            EndPoint masterEndPoint = null;
            foreach (EndPoint endpoint in sentinelConnection.GetEndPoints())
            {
                IServer server = sentinelConnection.GetServer(endpoint);
                if (!server.IsConnected)
                    continue;
                masterEndPoint = await server.SentinelGetMasterAddressByNameAsync("mymaster");
                break; 
            }

            var localMasterIP = masterEndPoint.ToString() switch
            {
                "172.17.0.3:6379" => "localhost:6379",
                "172.17.0.4:6379" => "localhost:6379"
            };

            ConnectionMultiplexer masterConnection = await ConnectionMultiplexer.ConnectAsync(localMasterIP);
            IDatabase database = masterConnection.GetDatabase();
            return database;
        }
    }
}
