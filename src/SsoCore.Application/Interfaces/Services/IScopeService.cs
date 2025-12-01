using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SsoCore.Application.DTOs;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Interfaces.Services
{
    public interface IScopeService
    {
        Task<Result<ScopeDto>> CreateScope(ScopeDto scopeDTO);
        Task<List<ScopeDto>> GetAllScopes();
        Task<ScopeDto?> GetByName(string name);
        Task<Result<ScopeDto>> UpdateScope(ScopeDto scopeDTO);
    }
}
