using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagedDesigns.RealWorldCqrs.Core.Infrastructure
{
    public interface ILogger
    {
        void LogDebug(string loggerName, string message, params object[] args);
        void LogInfo(string loggerName, string message, params object[] args);
        void LogWarning(string loggerName, string message, params object[] args);
        void LogError(string loggerName, string message, params object[] args);
        void LogException(string loggerName, Exception e);
    }

    public static class LoggerNames
    {
        public const string Denormalizer = "Denormalizer";
        public const string Aggregate = "Aggregate";
        public const string Saga = "Saga";
    }

    public class EmptyLogger
        : ILogger
    {
        public void LogDebug(string loggerName, string message, params object[] args)
        {
        }
        public void LogInfo(string loggerName, string message, params object[] args)
        {
        }
        public void LogWarning(string loggerName, string message, params object[] args)
        {
        }
        public void LogError(string loggerName, string message, params object[] args)
        {
        }
        public void LogException(string loggerName, Exception e)
        {
        }
    }
}
