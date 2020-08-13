namespace MbtaWebInfoBoard
{
    using Microsoft.Extensions.Configuration;
    using NetMQ;
    using NetMQ.Sockets;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class Subscriber
    {
        IConfiguration _configuration;
        SubscriberSocket _subscriberSocket;
        bool _isInitialized;

        readonly IDictionary<string, Action<string>> _handlers = new Dictionary<string, Action<string>>();

        public Subscriber(IConfiguration configuration)
        {
            _configuration = configuration;
            _isInitialized = false;
        }

        public void Intialize(CancellationToken token)
        {
            if (!_isInitialized)
            {
                _subscriberSocket = new SubscriberSocket();
                _subscriberSocket.Connect(_configuration.GetValue<string>("SubscriuberAddress"));
                _isInitialized = true;
                var taskFactory = new TaskFactory(token);
                taskFactory.StartNew(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        var messageTopicRecieved = _subscriberSocket.ReceiveFrameString(System.Text.Encoding.UTF8);

                        var messagePayload = _subscriberSocket.ReceiveFrameString();

                        Console.WriteLine($"Recieved topic {messageTopicRecieved} \n Payload: {messagePayload}");

                        _handlers[messageTopicRecieved].Invoke(messagePayload);
                    }
                });
                Console.WriteLine("Subscriber Initialized");
            }
        }

        public bool IsInitialized => _isInitialized;

        public void Subscribe(string topic, Action<string> messageHandler)
        {
            _subscriberSocket.Subscribe(topic);
            _handlers.Add(topic, messageHandler);
            
        }
    }
}
