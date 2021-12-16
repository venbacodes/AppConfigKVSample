namespace AppConfigKVSample
{
    public static class LoggerExtension
    {
        public static ILogger<T> GetConsoleLogger<T>()
        {
            var serviceProvider = new ServiceCollection().AddLogging(config => config.AddConsole())
                .Configure<LoggerFilterOptions>(config => config.MinLevel = LogLevel.Trace)
                .BuildServiceProvider();

            return serviceProvider.GetService<ILogger<T>>();
        }
    }
}
