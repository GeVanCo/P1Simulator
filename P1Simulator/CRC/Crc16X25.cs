using System.Text;

namespace P1Simulator.CRC
{
    public static class Crc16X25
    {
        // CRC-16/X25 parameters:
        // Poly: 0x1021
        // Init: 0xFFFF
        // RefIn: true
        // RefOut: true
        // XorOut: 0xFFFF

        public static ushort Compute(ReadOnlySpan<byte> data)
        {
            const ushort poly = 0x1021;
            ushort crc = 0xFFFF;

            foreach (byte b in data)
            {
                byte current = Reflect8(b);
                crc ^= (ushort)(current << 8);

                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ poly);
                    else
                        crc <<= 1;
                }
            }

            crc = Reflect16(crc);
            crc ^= 0xFFFF;

            return crc;
        }

        public static string ComputeHex(string telegramWithoutCrc)
        {
            var bytes = Encoding.ASCII.GetBytes(telegramWithoutCrc);
            ushort crc = Compute(bytes);
            return crc.ToString("X4"); // 4 hex digits, uppercase
        }

        private static byte Reflect8(byte value)
        {
            byte result = 0;
            for (int i = 0; i < 8; i++)
            {
                if ((value & (1 << i)) != 0)
                    result |= (byte)(1 << (7 - i));
            }
            return result;
        }

        private static ushort Reflect16(ushort value)
        {
            ushort result = 0;
            for (int i = 0; i < 16; i++)
            {
                if ((value & (1 << i)) != 0)
                    result |= (ushort)(1 << (15 - i));
            }
            return result;
        }
    }
}

