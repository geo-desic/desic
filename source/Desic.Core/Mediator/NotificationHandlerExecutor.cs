namespace Desic.Core.Mediator;

public record NotificationHandlerExecutor(object HandlerInstance, Func<INotification, CancellationToken, Task> HandlerCallback);
