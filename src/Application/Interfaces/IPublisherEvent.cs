using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces;

public interface IPublisherEvent
{
    Task ProduceEventAsync(string topic,
       string key,
       string value,
       CancellationToken cancellationToken = default);
}
