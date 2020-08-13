namespace MbtaApiAdapter
{
    using NetMQ;
    using NetMQ.Sockets;
    using System;

    public class Publisher
    {
        IOutgoingSocket _publisherSocket;
        public Publisher(RootConfiguration configuration)
        {
            _publisherSocket = GetPublisherSocket(configuration);

        }

        public void Publish(string topic, string payload)
        {
            Console.WriteLine($"Publishing to Topic {topic} \n Payload: {payload}");
            _publisherSocket.SendMoreFrame(topic).SendFrame(payload);
        }

        PublisherSocket GetPublisherSocket(RootConfiguration configuration)
        {
            var publisherSocket = new PublisherSocket();
            publisherSocket.Bind(configuration.PublisherAddress);
            return publisherSocket;
        }
    }
}
