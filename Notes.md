# To run as a systemd in Linux
Create a unit file */etc/systemd/system/huesundowner.service*
```
[Unit]
Description=HueSundowner

[Service]
Type=notify
ExecStart=/home/tim/development/HueSundowner/HueSundownDaemon/bin/Release/net5.0/HueSundownDaemon

[Install]
WantedBy=multi-user.target
```

Reload the service files:
```
sudo systemctl daemon-reload
```

Start the service
```
sudo systemctl start huesundowner
```

Enable the service to start at system start
```
sudo systemctl enable huesundowner
```
Can also stop and restart the service using the *systenctl* command.