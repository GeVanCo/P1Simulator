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
├── CRC                             // CRC16 calculation for DSMR telegrams
│   └── DsmrCrc16.cs
├── Logging                         // Logging system for console and file output
│   └── Logger.cs
├── Program.cs                      // Main application entry point and control flow
├── Serial                          // Serial communication and COM port detection
│   ├── ComPortDetectorHybrid.cs
│   └── SerialSender.cs
├── Simulation                      // Simulation modes and profiles
│   ├── SimulationMode.cs
│   └── SimulationProfile.cs
└── Telegrams                       // Telegram generation and DSMR templates
    ├── TelegramGenerator.cs
    └── Templates                   // DSMR telegram templates and types
        ├── DsmrTemplates.cs
        └── TemplateType.cs
```

---

## DSMR CRC16

The simulator uses the **official DSMR 5.0.2 CRC algorithm**:

- Polynomial: `0x8005` (reflected: `0xA001`)
- Init: `0x0000`
- No XOR in/out
- No reflection
- LSB‑first bit order

This ensures compatibility with real Belgian (Fluvius) and Dutch DSMR meters.

- DSMR 5.0.2 specs can be found here:  
https://www.netbeheernederland.nl/publicatie/dsmr-502-p1-companion-standard

- Direct link to PDF file:  
https://www.netbeheernederland.nl/sites/default/files/2024-02/dsmr_5.0.2_p1_companion_standard.pdf

---

## Understanding the DSMR CRC16 Algorithm (Visual Explanation)

The DSMR 5.0.2 CRC is a **CRC‑16/IBM‑style** checksum, but with a few important constraints:

- Polynomial: **0x8005** (reflected: **0xA001**)  
- Initial value: **0x0000**  
- No XOR in  
- No XOR out  
- No reflection of input bytes  
- **LSB‑first bit order** (shift right)

This section explains visually how the CRC is computed.

---

### 1. The Polynomial

The DSMR polynomial is:

```x¹⁶ + x¹⁵ + x² + 1```


If we drop the implicit `x¹⁶` term (always present in CRC‑16), the remaining bits form:

```1000 0000 0000 0101  →  0x8005```  (MSB‑first)


For LSB‑first CRC engines, the polynomial must be **reflected**:

```0x8005  →  0xA001```  (LSB‑first)


This is the value used in the simulator and ESP32.

---

### 2. CRC Bit‑Flow (LSB‑first)

DSMR uses **LSB‑first shifting**, meaning:

- The **least significant bit** is processed first  
- The CRC register is shifted **right**  
- If the outgoing bit is `1`, the polynomial is XORed in  

Visual flow:

```
+-------------------------------+
|   CRC ^= current_byte         |
+-------------------------------+
|
v
+-------------------+
| Process 8 bits    |
+-------------------+
|
v
For each bit:

if (CRC & 0x0001) == 1:
CRC = (CRC >> 1) XOR 0xA001
else:
CRC = CRC >> 1
```

---

### 3. Visual Example (Byte Processing)

Suppose the next byte is:

```0x5A  →  01011010```


CRC starts at:

```0000 0000 0000 0000```


Step 1 — XOR the byte:

```
CRC ^= 0x5A
CRC = 0000 0000 0101 1010
```

Step 2 — Process each bit (LSB first):

```
Bit 0 (LSB): 0 → Shift right
Bit 1: 1 → shift right + XOR poly
Bit 2: 0 → shift right
Bit 3: 1 → shift right + XOR poly
...
Bit 7: ...
```


This continues for every byte in the telegram.

---

### 4. CRC Range in DSMR

The CRC is computed over:

```/  ...  !```

**Including the exclamation mark**,  
but **excluding** the 4 hex digits after it.

Example:

```
/FLU5\253770123_A
...
!ABCD

Code
```

CRC is computed over:

```/FLU5\253770123_A\n...\n!```

Not over:

```ABCD```

---

### 5. Final Output

The resulting 16‑bit CRC is formatted as:

- **4 uppercase hex characters**
- **MSB‑first order**

Example:
```0x3F2A  →  "3F2A"```

This is appended directly after the `!`.

---

### 6. Why DSMR Uses LSB‑First

Most CRC‑16 variants (like X25) use MSB‑first shifting.  
DSMR meters, however, use a **hardware UART‑friendly LSB‑first CRC**, which matches how bits are transmitted over the P1 port.

This makes DSMR CRC:

- Simple to implement  
- Efficient on embedded hardware  
- Consistent across all Belgian/Dutch meters  

---

### 7. Summary Diagram

```
+-------------------------------+
|  Start CRC = 0x0000           |
+-------------------------------+
|
v
+-------------------------------+
|  For each byte in telegram    |
|  (from '/' to '!'):           |
+-------------------------------+
|
v
+-------------------------------+
|  CRC ^= byte                  |
+-------------------------------+
|
v
+-------------------------------+
|  Repeat 8 times:              |
|    if (CRC & 1):              |
|        CRC = (CRC >> 1) ^ A001|
|    else                       |
|        CRC = CRC >> 1         |
+-------------------------------+
|
v
+-------------------------------+
|  Final CRC → 4 hex chars      |
+-------------------------------+
```

---

## Example Telegram

```
/FLU5\253769484_A

1-3:0.2.8(50)
0-0:1.0.0(260607092641)
0-0:96.1.1(4530303435303030303030303030303136)
1-0:1.8.1(248.800*kWh)
1-0:1.8.2(391.813*kWh)
1-0:2.8.1(93.582*kWh)
1-0:2.8.2(522.765*kWh)
1-0:1.7.0(1.450*kW)
1-0:2.7.0(0.461*kW)
1-0:32.7.0(232.3*V)
1-0:31.7.0(6.2*A)
!C30B
```

---

## Logging

Logs are written to: ```logs/p1simulator.log```

and have the following format:

```
2024-06-07 12:34:56.345 [INFO] Starting P1 Smart Meter Simulator...
2024-06-07 12:34:57.456 [INFO] Detected COM port: COM3
2024-06-07 12:34:57.479 [INFO] Simulation mode: Normal
2024-06-07 12:34:57.486 [INFO] Telegram interval: 1 second
2024-06-07 12:34:57.493 [INFO] Press 'q' to stop simulation and show menu.
2024-06-07 12:35:07.501 [INFO] Telegram sent successfully.
2026-06-07 12:35:07.872 [TELEGRAM] /FLU5\253769484_A\n\n1-3:0.2.8(50)\n0-0:1.0.0(260607094309)\n0-0:96.1.1(4530303435303030303030303030303136)\n1-0:1.8.1(480.562*kWh)\n1-0:1.8.2(662.367*kWh)\n1-0:2.8.1(958.929*kWh)\n1-0:2.8.2(685.280*kWh)\n1-0:1.7.0(3.954*kW)\n1-0:2.7.0(0.326*kW)\n1-0:32.7.0(231.4*V)\n1-0:31.7.0(17.1*A)\n!9EBF\n
2024-06-07 12:35:17.987 [INFO] Telegram sent successfully.
```

The file is automatically created if it doesn't exist, and new logs are created on each run. 
The logging system also outputs to the console for real‑time feedback.

The log file format is as follows: ```p1sim_YYYYMMDD_HHMMSS.log```

Example: ```p1sim_20260604_163439.log```

---

## License

MIT License

---

## Author

Geert Vancompernolle — P1 Simulator Project
