using AutoMapper;
using FluentValidation;
using InventoryApp.API.Controllers;
using InventoryApp.Application.DTOs.Create;
using InventoryApp.Application.DTOs.List;
using InventoryApp.Application.DTOs.Update;
using InventoryApp.Domain.Contracts;
using InventoryApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : GenericController<Notification,CreateNotificationDTO,UpdateNotificationDTO,NotificationDTO>
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        public NotificationController(
            INotificationService service,
            IValidator<CreateNotificationDTO> createValidator,
            IValidator<UpdateNotificationDTO> updateValidator,
            IMapper mapper
        ) : base(service,createValidator,updateValidator,mapper) 
        {
            _mapper = mapper;
            _notificationService = service;
        }
        public override async Task<IActionResult> GetAll([FromQuery]string? include,[FromQuery]string? search)
        {
            var result = await _notificationService.GetNotificationsWithInclude(include,search);

            if (!result.Success)
                return NotFound(result.Message);

            var notifications = result.Data;

            var dto = _mapper.Map<List<NotificationDTO>>(notifications);

            return Ok(dto);
        }
        public override async Task<IActionResult> GetById(int id, string? include)
        {
            var result = await _notificationService.GetNotificationByIdWithInclude(include,id);

            if (!result.Success)
                return NotFound(result.Message);

            var notification = result.Data;

            var dto = _mapper.Map<NotificationDTO>(notification);

            return Ok(dto);
        }
        [HttpPut("{notificationId}")]
        public async Task<IActionResult> MarkAsRead([FromRoute]int notificationId)
        {
            var result = await _notificationService.MarkAsReadAsync(notificationId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}
