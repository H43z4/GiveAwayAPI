using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMQPSvc
{
    public interface IMessageProducer
    {
        Task SendMessage<T>(T message, string channelName, string queueName);
    }
}
