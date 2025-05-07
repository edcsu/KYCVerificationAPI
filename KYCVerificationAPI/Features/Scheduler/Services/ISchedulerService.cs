namespace KYCVerificationAPI.Features.Scheduler.Services;

public interface ISchedulerService
{
    Task<bool> ScheduleVerificationAsync(Guid verificationId, CancellationToken cancellationToken = default);
}