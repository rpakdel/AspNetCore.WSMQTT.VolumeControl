# Windows Volume Control via MQTT

Asp.NET Core Worker Service running as a Windows Service to subscribe to MQTT message and change Windows volume.

* Update Worker.cs and set the IP address of the MQTT broker.
* Ensure the subscribe to the correct MQTT topic.
* Follow [this guide](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-3.0&tabs=visual-studio) to configure the Windows Service
* Update install_service.ps1 accordingly and run it as Administrator

https://github.com/rpakdel/esp8266_rotaryencoder_mqtt is an example of MQTT client that publishes messages when a rotary encoder is turned.


