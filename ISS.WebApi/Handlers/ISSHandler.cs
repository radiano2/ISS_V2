using Microsoft.Extensions.Logging;

namespace ISS.WebApi.Handlers
{
    public class ISSHandler
    {
        private readonly ILogger<ISSHandler> _logger;

        public ISSHandler(ILogger<ISSHandler> logger)
        {
            _logger = logger;
        }
    }
}