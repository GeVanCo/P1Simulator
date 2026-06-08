using System.Text;

namespace P1Simulator.CRC
{
    public static class DsmrCrc16
    {
        // DSMR 5.0.2 CRC parameters:
        // Polynomial: x^16 + x^15 + x^2 + 1  → 0x8005 (MSB-first)
        // Reflected polynomial (LSB-first engine): 0xA001
        // Init: 0x0000
        // XOR in: none
        // XOR out: none
        // RefIn: false
        // RefOut: false
        // Bit order: LSB-first (shift right)

        public static ushort Compute(ReadOnlySpan<byte> data)
        {
            ushort crc = 0x0000;
            const ushort poly = 0xA001; // reflected 0x8005

            foreach (byte b in data)
            {
                crc ^= b;

                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x0001) != 0)
                        crc = (ushort)((crc >> 1) ^ poly);
                    else
                        crc >>= 1;
                }
            }

            return crc; // no reflection, no xorout
        }
        public static ushort Compute(byte[] buffer, int length)
        {
            return Compute(buffer.AsSpan(0, length));
        }

        public static string ComputeHex(string telegramWithoutCrc)
        {
            var bytes = Encoding.ASCII.GetBytes(telegramWithoutCrc);
            ushort crc = Compute(bytes);
            return crc.ToString("X4"); // MSB-first hex output
        }
    }
}
