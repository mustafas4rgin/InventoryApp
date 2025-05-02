using AutoMapper;
using FluentValidation;
using InventoryApp.Application.DTOs;
using InventoryApp.Domain.Contracts;
using InventoryApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin")]
    [ApiController]
    public class TokenController : GenericController<AccessToken,CreateTokenDTO,UpdateTokenDTO,TokenDTO>
    {
        public TokenController(
            IGenericService<AccessToken> service,
            IValidator<CreateTokenDTO> createValidator,
            IValidator<UpdateTokenDTO> updateValidator,
            IMapper mapper
        ) : base(service,createValidator,updateValidator,mapper) {}
    }
}
