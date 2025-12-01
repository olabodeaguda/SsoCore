using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Handlers.Queries
{
    public class FilterUserRequest : IRequest<Pageable<UserDto>>
    {
        public string? Query { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }

    public class FilterUserHandler : IRequestHandler<FilterUserRequest, Pageable<UserDto>>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<FilterUserHandler> _logger;
        public FilterUserHandler(IUserService userService, IMapper mapper, ILogger<FilterUserHandler> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Pageable<UserDto>> Handle(FilterUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userService.FilterUsersAsync(request.Query, request.PageNumber, request.PageSize);

                var data = _mapper.Map<List<UserDto>>(result.Data);

                return Pageable<UserDto>.Create(data, result.TotalItems, request.PageNumber, result.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering users");

                return Pageable<UserDto>.Empty;
            }
        }
    }
}
