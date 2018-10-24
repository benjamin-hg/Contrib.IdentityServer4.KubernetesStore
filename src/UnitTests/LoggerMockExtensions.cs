using System;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Language.Flow;

namespace Contrib.IdentityServer4.KubernetesStore
{
    public static class LoggerMockExtensions
    {
        public static ISetup<ILogger<T>> SetupLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel logLevel)
        {
            return loggerMock.Setup(GetLogSetupMethodExpression<T>(logLevel));
        }

        public static ICallBaseResult Callback<T>(
            this ISetup<ILogger<T>> loggerMockSetup,
            Action<LogLevel, EventId, object, Exception, Func<object, Exception, string>> action
        ) => loggerMockSetup.Callback(action);

        public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel logLevel, Times times)
        {
            loggerMock.Verify(GetLogSetupMethodExpression<T>(logLevel), times);
        }

        private static Expression<Action<ILogger<T>>> GetLogSetupMethodExpression<T>(LogLevel logLevel)
        {
            return x => x.Log(
                logLevel,
                It.IsAny<EventId>(),
                It.IsAny<object>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception, string>>()
            );
        }
    }
}