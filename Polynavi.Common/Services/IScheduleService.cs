﻿using Polynavi.Common.Models;
using System;
using System.Threading.Tasks;

namespace Polynavi.Common.Services
{
    public interface IScheduleService
    {
        Task<WeekSchedule> GetScheduleAsync(DateTime date);
    }
}