using System.Diagnostics;
using Mediator;
using Microsoft.Extensions.Logging;

namespace PetitesVictoires.UseCases.Behaviors;

public class LoggingBehavior<TMessage, TResponse>(ILogger<LoggingBehavior<TMessage, TResponse>> logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Handling {RequestName} with {@Request}", typeof(TMessage).Name, message);

        var sw = Stopwatch.StartNew();

        var response = await next(message, cancellationToken);

        sw.Stop();

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Handled {RequestName} with {Response} in {ElapsedMilliseconds} ms",
                typeof(TMessage).Name, response, sw.ElapsedMilliseconds);

        return response;
    }
}
