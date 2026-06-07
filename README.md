# P1 Smart Meter Simulator

A Windows‑based DSMR/SMR **P1 smart meter telegram generator** written in C#.

It sends fully valid DSMR 5.0.2 telegrams over a virtual COM port (USB‑UART) to an ESP32 or any other device that expects real smart‑meter data.

This simulator is designed as a **development and testing tool** for embedded systems, home‑automation gateways, and DSMR data parsers.

---

## Features

- Generates **valid DSMR 5.0.2 telegrams**
- Fully correct **DSMR CRC16** (LSB‑first, polynomial 0xA001)
- Sends telegrams over a **serial COM port**
- Auto‑detects USB‑UART adapters
- Supports multiple simulation modes
- Adjustable telegram interval
- Clean shutdown on **Ctrl+C**
- Interactive **Restart / Quit** menu (single key, no Enter needed)
- Logging system with file output

---

## Requirements

- Windows 10 or later
- .NET 8 SDK or runtime
- USB‑UART adapter (CH340, CP2102, FTDI, …)
- COM port available

---

## How It Works

The simulator:

1. Auto‑detects the first available USB‑UART COM port  
2. Opens the port at **115200 baud**  
3. Generates a DSMR telegram using `TelegramGenerator`  
4. Computes the correct DSMR CRC16  
5. Sends the telegram over serial  
6. Repeats at the configured interval  

---

## Controls

During simulation:

- Press **q** → Stop simulator and show menu  
- Press **Ctrl+C** → Graceful stop and show menu  

Menu options (single key, no Enter required):

- Press **R** → Restart simulator  
- Press **Q** → Quit application  

---

## Project Structure

```
.
├── CRC
│   └── DsmrCrc16.cs
├── Logging
│   └── Logger.cs
├── Program.cs
├── Serial
│   ├── ComPortDetectorHybrid.cs
│   └── SerialSender.cs
├── Simulation
│   ├── SimulationMode.cs
│   └── SimulationProfile.cs
└── Telegrams
    ├── TelegramGenerator.cs
    └── Templates
        ├── DsmrTemplates.cs
        └── TemplateType.cs
```