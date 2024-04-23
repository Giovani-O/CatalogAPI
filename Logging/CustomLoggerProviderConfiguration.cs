namespace CatalogAPI.Logging
{
    // Define a configuração do provedor de logs personalizados
    public class CustomLoggerProviderConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;

        public int EventId { get; set; } = 0;
    }
}
