﻿namespace Stocks.API.Configurations
{
    public class MongoDbSettings
    {
        public string ConnectionUri { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;
    }
}
