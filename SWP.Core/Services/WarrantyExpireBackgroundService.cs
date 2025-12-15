using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SWP.Core.Interfaces.Repositories;
using SWP.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWP.Core.Services
{
    public class WarrantyExpireBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WarrantyExpireBackgroundService> _logger;

        public WarrantyExpireBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<WarrantyExpireBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Delay để đảm bảo app + DB đã sẵn sàng
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var warrantyRepo = scope.ServiceProvider.GetRequiredService<IWarrantyRepo>();

                    await warrantyRepo.AutoUpdateExpiredAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Tự động cập nhật quá hạn của bảo hành thất bại");
                   
                }

                // chạy 1 lần / ngày
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}

