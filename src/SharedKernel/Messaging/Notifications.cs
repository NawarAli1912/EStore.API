namespace SharedKernel.Messaging;

public interface INotification
{
}

public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}

public interface IPublisher
{
    Task Publish(object notification, CancellationToken cancellationToken = default);

    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}
