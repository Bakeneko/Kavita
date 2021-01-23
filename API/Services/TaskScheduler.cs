﻿using API.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace API.Services
{
    public class TaskScheduler : ITaskScheduler
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<TaskScheduler> _logger;
        private readonly IDirectoryService _directoryService;
        public BackgroundJobServer Client => new BackgroundJobServer();

        public TaskScheduler(ICacheService cacheService, ILogger<TaskScheduler> logger, 
            IDirectoryService directoryService)
        {
            _cacheService = cacheService;
            _logger = logger;
            _directoryService = directoryService;

            _logger.LogInformation("Scheduling/Updating cache cleanup on a daily basis.");
            RecurringJob.AddOrUpdate(() => _cacheService.Cleanup(), Cron.Daily);
            RecurringJob.AddOrUpdate(() => directoryService.ScanLibraries(), Cron.Daily);
        }

        public void ScanLibrary(int libraryId, bool forceUpdate = false)
        {
            _logger.LogInformation($"Enqueuing library scan for: {libraryId}");
            BackgroundJob.Enqueue(() => _directoryService.ScanLibrary(libraryId, forceUpdate));
        }

        public void CleanupVolumes(int[] volumeIds)
        {
            BackgroundJob.Enqueue(() => _cacheService.CleanupVolumes(volumeIds));
            
        }

        public void ClearCache()
        {
            throw new System.NotImplementedException();
        }
    }
}