using Application.Integration.Pushwoosh;

namespace Application.Interfaces.Repositories;

public interface INotificationRepository
{
    /// <summary>
    /// Envia uma notificação pelo Pushwoosh (chamada HTTP POST para /messaging/v2/notify).
    /// </summary>
    /// <param name="request">Dados da requisição contendo o token e o body conforme a API do Pushwoosh.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    Task<PushwooshNotifyResponse?> NotifyAsync(PushwooshNotifyRequest request, CancellationToken cancellationToken = default);
}
