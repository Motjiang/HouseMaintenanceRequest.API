using HouseMaintenanceRequest.API.Features.Property.Command;
using HouseMaintenanceRequest.API.Features.Property.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HouseMaintenanceRequest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMemoryCache _cache;
        private readonly ILogger<PropertyController> _logger;

        public PropertyController(IMediator mediator, IMemoryCache cache, ILogger<PropertyController> logger)
        {
            _mediator = mediator;
            _cache = cache;
            _logger = logger;
        }

        // ✅ CREATE PROPERTY
        [HttpPost("create-property")]
        public async Task<IActionResult> Create(CreatePropertyCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);

                // ✅ Clear cache
                _cache.Remove("properties_cache");

                return Ok(new
                {
                    success = true,
                    message = "Property created successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating property");

                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while creating property",
                    error = ex.Message
                });
            }
        }

        // ✅ GET ALL PROPERTIES (PAGED + CACHE)
        [HttpGet("all-properties")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // ✅ Build cache key
                string cacheKey = $"properties_cache_{pageNumber}_{pageSize}";

                if (!_cache.TryGetValue(cacheKey, out object cacheData))
                {
                    var query = new GetPropertiesQuery(pageNumber, pageSize);
                    var result = await _mediator.Send(query);

                    cacheData = result;

                    // Cache for 2 minutes
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                    };

                    _cache.Set(cacheKey, cacheData, cacheOptions);
                }

                return Ok(cacheData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving properties");

                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving properties",
                    error = ex.Message
                });
            }
        }
    }
}
