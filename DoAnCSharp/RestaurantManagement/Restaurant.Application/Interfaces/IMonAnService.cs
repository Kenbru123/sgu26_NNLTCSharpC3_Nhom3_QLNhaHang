using System;
using System.Collections.Generic;
using System.Text;
using Restaurant.Application.DTOs;

namespace Restaurant.Application.Interfaces
{
    public interface IMonAnService
    {
        Task<List<MonAnDto>> GetAllAsync();
    }
}
