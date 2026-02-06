namespace Desic.Core.Mediator;

public interface INotificationPublisher
{
    Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification,
        CancellationToken cancellationToken);
}
