using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Store_API.Contract
{
    public interface ILoggerService
    {
        void LogInfo(string message);
        void LogError(string message);
        void LogDebug(string message);
        void LogWarn(string message);
    }
}
