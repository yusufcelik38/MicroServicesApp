using EventBusRabbitMQ.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBusRabbitMQ.Producer
{
    public class EventBusRabbitMQProducer
    {
        private readonly IRabbitMQConnection _connection;

        public EventBusRabbitMQProducer(IRabbitMQConnection connection)
        {
            _connection = connection;
        }
        public void PublishBasketCheckout(string queueName, BasketCheckoutEvent publishModel)
        {
            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queueName, false, false, false, null);
                var message = JsonConvert.SerializeObject(publishModel);
                var body = Encoding.UTF8.GetBytes(message);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                channel.ConfirmSelect();
                channel.BasicPublish("", queueName, true, properties, body);
                channel.WaitForConfirmsOrDie();

                channel.BasicAcks += (sender, eventArgs) =>
                {
                    Console.WriteLine("Sent RabbitMQ");
                    //implement ack handle
                };
                channel.ConfirmSelect();

            }
        }
    }
}
