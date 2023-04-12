using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SharedLib.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMQPSvc.RabbitMQ
{
    public class RabbitMQProducer : IMessageProducer
    {
        readonly RabbitMQServerConfig rabbitMQServerConfig;

        public RabbitMQProducer(IOptions<RabbitMQServerConfig> rabbitMQServerConfig)
        {
            this.rabbitMQServerConfig = rabbitMQServerConfig.Value;
        }

        public async Task SendMessage<T>(T message, string channelName, string queueName)
        {
            var queue = this.rabbitMQServerConfig.Channels.FirstOrDefault(x => x.Name == channelName).Queues.SingleOrDefault(x => x.Key == queueName).Value;

            var factory = new ConnectionFactory
            {
                HostName = this.rabbitMQServerConfig.HostName,
                UserName = this.rabbitMQServerConfig.UserName,
                Password = this.rabbitMQServerConfig.Password,
                Port = this.rabbitMQServerConfig.Port
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                await Task.Factory.StartNew(() =>
                {
                    channel.BasicPublish(exchange: this.rabbitMQServerConfig.Exchange,
                        routingKey: queue,
                        basicProperties: null,
                        body: body);
                });

    //            channel.BasicPublish(exchange: this.rabbitMQServerConfig.Exchange,
                    //routingKey: queue,
                    //basicProperties: null,
                    //body: body);
            }
        }
    }
}