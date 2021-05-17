# dji-wifi-tools

This repo contains a set of tools to facilitate the analysis of the Dji wifi communication protocol. Meant for drone enthusiasts, scientific or educational purposes only.

# Features

- [x] Automatic Drone and Operator detection
- [x] Sniffing relevant UDP network traffic
- [x] UDP packet enumeration; including filter predicates
- [x] UDP packet inspection (HEX/Binary) and comparison
- [x] Export as *.pcap
- [x] Import *.pcap
- [x] Export as [DUML](https://github.com/o-gs/dji-firmware-tools/tree/master/comm_dissector) for further in-depth analysis
- [ ] Camera output rendering
- [ ] Self-contained drone control
  - [ ] Takeoff
  - [ ] Landing
  - [ ] Accelerate
  - [ ] Rotate
  - [ ] Elevate

# Screenshot

![Screenshot](https://www.indie-dev.at/wp-content/uploads/2021/05/Screenshot.png "Simulation replay and packet comparison")

# Supported Drones

- [x] Mavic Pro 1
- [ ] Phantom 2
- [x] Phantom 3
- [x] Phantom 4
- [ ] Naza M
- [ ] Naza M V2
- [ ] Phanto

# Platforms

The application runs on all [relevant platforms](https://github.com/dotnet/core/blob/main/release-notes/5.0/5.0-supported-os.md). It has been tested against Windows and Linux. System requirements x86/ARM:

- Windows (7 (Sp 1), 8.1, 10.1607+)
- Linux (Debian 9+, Mint, 18+, Ubuntu 18.04, RH 7+)
- Mac (10.13+)

# Compile + Run instructions

[Install the .NET 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0) and switch to the root folder. Execute

1. `dotnet restore`
2. `dotnet build`
3. `dotnet run --project Dji.Ui`

# Support

[Buy me a coffee ☕](https://www.buymeacoffee.com/yoghurt)

# Credits

Based on [OG's firmware-tools](https://github.com/o-gs/dji-firmware-tools)
