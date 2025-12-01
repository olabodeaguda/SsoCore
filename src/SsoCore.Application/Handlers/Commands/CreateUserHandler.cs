using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SsoCore.Application.DTOs;
using SsoCore.Application.Interfaces.Services;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Handlers.Commands
{
    public class CreateUserRequest: IRequest<Result<UserDto>>
    {
        public string? LastName { get; set;  }
        public string? FirstName { get; set;  }
        public string? MiddleNames { get; set;  }
        public string? Email { get; set;  }
        public bool IsDisabled { get; set;  }
        public string? CreatedBy { get; set; }
    }

    public class CreateUserHandler : IRequestHandler<CreateUserRequest, Result<UserDto>>
    {
        private IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateUserHandler> _logger;
        public CreateUserHandler(IUserService userService, IMapper mapper, ILogger<CreateUserHandler> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<UserDto>> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userService.CreateUser(_mapper.Map<UserDto>(request));
            if (!user.IsSuccess)
            {
                _logger.LogError("@error {Error} - Error creating user", user.Error);
                return user;
            }

            return user;
        }
    }
}
