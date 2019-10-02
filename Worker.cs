using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi.CoreAudio;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore.WSMQTT
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        MQTTnet.Client.IMqttClient mClient;
        MQTTnet.MqttFactory mMQTTFactory = new MQTTnet.MqttFactory();
        bool mSubscribed = false;
        bool mSubscribing = false;
        CancellationToken mSubscribeAndConnectCancellationToken = new CancellationToken();
        CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            mClient = mMQTTFactory.CreateMqttClient();
            mClient.ApplicationMessageReceivedHandler = new MQTTnet.Client.Receiving.MqttApplicationMessageReceivedHandlerDelegate(MessageReceived);
            
        }

        class ESP8266NRFPayload
        {
            public int pipe { get; set; }
            public int value { get; set; }
        }

        class VolumePayload
        {
            public int value { get; set; }
        }

        void MessageReceived(MQTTnet.MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            var str = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload);
            var payload = JsonSerializer.Deserialize<VolumePayload>(str);
            _logger.LogInformation("Payload value {value}", payload.value);

            if (payload.value <= 100)
            {
                defaultPlaybackDevice.Volume = payload.value;
            }
            
            //Debug.WriteLine("Current Volume:" + defaultPlaybackDevice.Volume);
            //defaultPlaybackDevice.Volume = 80;
        }

        async Task ConnectAndSubscribe()
        {
            _logger.LogInformation("Connecting to MQTT at: {time}", DateTimeOffset.Now);
            var clientOptions = new MQTTnet.Client.Options.MqttClientOptions()
            {
                ChannelOptions = new MQTTnet.Client.Options.MqttClientTcpOptions() { Server = "192.168.1.29" } 
            };
            await mClient.ConnectAsync(clientOptions, mSubscribeAndConnectCancellationToken);
            _logger.LogInformation("Subscribing to MQTT at: {time}", DateTimeOffset.Now);
            var subOptions = new MQTTnet.Client.Subscribing.MqttClientSubscribeOptions();
            subOptions.TopicFilters.Add(new MQTTnet.TopicFilter() { Topic = "Volume" });
            var result = await mClient.SubscribeAsync(subOptions, mSubscribeAndConnectCancellationToken);
            _logger.LogInformation("Subscribed to MQTT at: {time} with code {code}", DateTimeOffset.Now, result.Items[0].ResultCode);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                if (!mSubscribed && !mSubscribing)
                {
                    mSubscribing = true;
                    await ConnectAndSubscribe();
                    mSubscribing = false;
                    mSubscribed = true;
                }

                
            }
        }
    }
}
