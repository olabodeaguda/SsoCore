using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Handlers.Queries
{
    public class GetScopesByClientIdRequest : IRequest<Result<List<string>>>
    {
        public string? ClientId { get; set;  }
    }
    public class GetScopesByClientIdHandler : IRequestHandler<GetScopesByClientIdRequest, Result<List<string>>>
    {
        public Task<Result<List<string>>> Handle(GetScopesByClientIdRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
