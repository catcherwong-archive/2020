namespace WSDemo.Handlers
{
    using Microsoft.AspNetCore.Http;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class DisHandler : IDisHandler, IDisposable
    {
        private readonly IConnection _conn;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer _consumer;
        private readonly ConcurrentDictionary<string, WebSocket> _sockets;

        public DisHandler()
        {
            _sockets = new ConcurrentDictionary<string, WebSocket>();

            _conn = InitConnection();

            _channel = InitChannel(_conn);

            _consumer = InitConsumer(_channel);

            _channel.BasicConsume("demo.queue.durable.task", false, _consumer);
        }

        private IConnection InitConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "127.0.0.1",
                UserName = "guest",
                Password = "guest",
                Port = 5672,
                VirtualHost = "/",
            };

            var conn = factory.CreateConnection();
            return conn;
        }

        private IModel InitChannel(IConnection conn)
        {
            var channel = conn.CreateModel();
            channel.ExchangeDeclare("demo.exchange.topic.task", ExchangeType.Topic);
            channel.QueueDeclare("demo.queue.durable.task", true, false, false, null);
            channel.QueueBind("demo.queue.durable.task", "demo.exchange.topic.task", "*.queue.durable.task", null);
            channel.BasicQos(0, 1, false);
            return channel;
        }

        private EventingBasicConsumer InitConsumer(IModel channel)
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine($"received content = {content}");

                // {"TaskName":"Demo", "TaskType":1}
                var msg = Newtonsoft.Json.JsonConvert.DeserializeObject<MqMsg>(content);

                var workIds = Worker.GetByTaskType(msg.TaskType);
                var onlineWorkerIds = _sockets.Keys.Intersect(workIds).ToList();                
                if (onlineWorkerIds == null || !onlineWorkerIds.Any())
                {
                    if (!ea.Redelivered)
                    {
                        Console.WriteLine("No online worker, reject the message and requeue");
                        // should requeue here
                        _channel.BasicReject(ea.DeliveryTag, true);
                    }
                    else
                    {     
                        // should not requeue here, but this message will be discarded
                        _channel.BasicReject(ea.DeliveryTag, false);
                    }
                }
                else
                {
                    // free or busy
                    var randomNumberBuffer = new byte[10];
                    new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
                    var rd = new Random(BitConverter.ToInt32(randomNumberBuffer, 0));                                        
                    var index = rd.Next(0, 9999) % onlineWorkerIds.Count;
                    var workerId = onlineWorkerIds[index];

                    if (_sockets.TryGetValue(workerId, out var ws) && ws.State == WebSocketState.Open)
                    {
                        // simulating handle the message an get the result.
                        // put your own logic here
                        var val = msg.TaskName;
                        if (msg.TaskType != 1) val = $"Special-{msg.TaskName}";

                        var task = Encoding.UTF8.GetBytes(val);

                        Console.WriteLine($"send to {workerId}-{val}");

                        // should ack here? or when to ack is better?
                        _channel.BasicAck(ea.DeliveryTag, false);

                        // sending message to specify client
                        await ws.SendAsync(new ArraySegment<byte>(task, 0, task.Length), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else
                    {
                        Console.WriteLine("Not found a worker");
                    }
                }
            };

            return consumer;
        }

        public async Task PushAsync(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string clientId = Encoding.UTF8.GetString(buffer, 0, result.Count);

            // record the client id and it's websocket instance
            if (_sockets.TryGetValue(clientId, out var wsi))
            {                
                if (wsi.State == WebSocketState.Open)
                {
                    Console.WriteLine($"abort the before clientId named {clientId}");
                    await wsi.CloseAsync(WebSocketCloseStatus.InternalServerError, "A new client with same id was connected!", CancellationToken.None);                    
                }

                _sockets.AddOrUpdate(clientId, webSocket, (x, y) => webSocket);
            }
            else
            {
                Console.WriteLine($"add or update {clientId}");
                _sockets.AddOrUpdate(clientId, webSocket, (x, y) => webSocket);
            }

            while (!result.CloseStatus.HasValue)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }            

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            Console.WriteLine("close=" + clientId);

            _sockets.TryRemove(clientId, out _);
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
        {
        }

        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
            Console.WriteLine("OnConsumerUnregistered");
        }

        private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
        {
            Console.WriteLine("OnConsumerRegistered");
        }

        private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("OnConsumerShutdown");
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public void Dispose()
        {
            Console.WriteLine("Dispose");       
            _sockets?.Clear();     
            _channel?.Dispose();
            _conn?.Dispose();
        }
    }
}
