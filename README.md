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
- [x] Camera output rendering
- [ ] Self-contained drone control
  - [ ] Takeoff
  - [ ] Landing
  - [ ] Accelerate
  - [ ] Rotate
  - [ ] Elevate

# Screenshots

![Screenshot](https://www.indie-dev.at/wp-content/uploads/2021/05/Screenshot.png "Simulation replay and packet comparison")

![Screenshot](https://www.indie-dev.at/wp-content/uploads/2021/05/VideoPlayback.png "Camera Playback")

# Supported Drones

- [x] Mavic Pro 1
- [x] Phantom 3
- [x] Phantom 4

### Theoretical support

Those drones are theoretically supported. However, they do require a delimiter extension which won't be implemented anytime soon.

- [ ] Naza M
- [ ] Naza M V2
- [ ] Phantom 1
- [ ] Phantom 2

# Supported Platforms

The application runs on all [relevant platforms](https://github.com/dotnet/core/blob/main/release-notes/5.0/5.0-supported-os.md). It has been tested against Windows and Linux. System requirements x86/ARM:

- Windows (7 (Sp 1), 8.1, 10.1607+)
- Linux (Debian 9+, Mint, 18+, Ubuntu 18.04, RH 7+)
- Mac (10.13+)

# Prerequisites

1. [Install the .NET 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
2. Download [FFmpeg 4.2 binaries](https://ffbinaries.com/downloads) according to your OS/CPU and place it into the root folder.
3. _Optional_ for Linux (copy function): `apt install -y xsel`
4. _Optional_ for Linux (video playback): `apt install libvlc-dev`
5. _Optional_ for Linux (video playback): `apt install vlc`
6. _Optional_** for Linux (video playback): `apt install gtk-sharp2`
7. _Optional_** for Linux (video playback): `apt install libx11-dev`

** install when step 4-5 don't enable dji-camera output rendering

# Compile + Run Instructions

Switch to the root folder and execute

1. `dotnet restore`
2. `dotnet build`
3. `dotnet run --project Dji.Ui`

Project configurations for Visual Studio 2019 and Visual Studio Code are included.

# Release Instructions

For the most easy setup download the latest release and execute `dotnet Dji.UI.dll`.

# Usage Instructions

If you do require any usage instructions, maybe this tool isn't meant for you.

# Support

[Buy me a coffee ☕](https://www.buymeacoffee.com/yoghurt)

# Credits

Based on [OG's firmware-tools](https://github.com/o-gs/dji-firmware-tools)
