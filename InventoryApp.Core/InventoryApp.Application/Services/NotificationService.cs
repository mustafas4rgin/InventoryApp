using FluentValidation;
using InventoryApp.Application.Results;
using InventoryApp.Application.Results.Data;
using InventoryApp.Application.Results.Raw;
using InventoryApp.Core.Results.Data;
using InventoryApp.Domain.Contracts;
using InventoryApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Application.Services;

public class NotificationService : GenericService<Notification>, INotificationService
{
    private readonly IGenericRepository _genericRepository;
    private readonly IValidator<Notification> _validator;
    public NotificationService(IGenericRepository genericRepository,
    IValidator<Notification> validator
    ) : base(genericRepository, validator)
    {
        _validator = validator;
        _genericRepository = genericRepository;
    }
    public async Task<IServiceResult> MarkAsReadAsync(int notificationId)
    {
        try
        {
            var notification = await _genericRepository.GetByIdAsync<Notification>(notificationId);

            if (notification is null)
                return new ErrorResult($"There is no notification with ID : {notificationId}");

            if (notification.Status == NotificationStatus.Read)
                return new ErrorResult("Notification already marked as read.");

            notification.Status = NotificationStatus.Read;

            await _genericRepository.UpdateAsync(notification);
            await _genericRepository.SaveChangesAsync();

            return new SuccessResult("Notification marked as read.");
        }
        catch (Exception ex)
        {
            return new ErrorResult(ex.Message);
        }


    }
    public async Task<IServiceResultWithData<Notification>> GetNotificationByIdWithInclude(string? include, int id)
    {
        try
        {
            var query = _genericRepository.GetAll<Notification>();

            if (!string.IsNullOrWhiteSpace(include))
            {
                var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var inc in includes.Select(i => i.Trim().ToLower()))
                {
                    if (inc == "user")
                        query = query.Include(n => n.User);
                }
            }

            var notification = await query.FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);

            if (notification is null)
                return new ErrorResultWithData<Notification>($"There is no notification with ID : {id}");

            return new SuccessResultWithData<Notification>("Notification found.", notification);
        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<Notification>(ex.Message);
        }
    }
    public async Task<IServiceResultWithData<IEnumerable<Notification>>> GetNotificationsWithInclude(string? include, string? search)
    {
        try
        {
            var query = _genericRepository.GetAll<Notification>();

            if (!string.IsNullOrWhiteSpace(include))
            {
                var includes = include.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var inc in includes.Select(i => i.Trim().ToLower()))
                {
                    if (inc == "user")
                        query = query.Include(n => n.User);
                }
            }

            var notifications = await query.Where(s => !s.IsDeleted)
                                .Where(s => string.IsNullOrEmpty(search) || s.Title.ToLower().Contains(search.ToLower()))
                                .ToListAsync();

            if (!notifications.Any())
                return new ErrorResultWithData<IEnumerable<Notification>>("There is no notification.");

            return new SuccessResultWithData<IEnumerable<Notification>>("Notifications found.", notifications);
        }
        catch (Exception ex)
        {
            return new ErrorResultWithData<IEnumerable<Notification>>(ex.Message);
        }
    }
    public override Task<IServiceResultWithData<IEnumerable<Notification>>> GetAllAsync()
    {
        throw new NotSupportedException("Use GetNotificationsWithInclude instead of GetAllAsync.");
    }

}