﻿using System;
using System.IO;
using System.Text;

namespace Helion.Util
{
    /// <summary>
    /// A convenience class for reading from bytes. It also has support for
    /// reading big endian if the computer is in little endian format.
    /// </summary>
    public class ByteReader : BinaryReader
    {
        // The docs explicitly say that memory streams do not need to have any
        // .dispose() call invoked because there are no resources to dispose.
        public ByteReader(byte[] data) : base(new MemoryStream(data))
        {
            Assert.Precondition(BitConverter.IsLittleEndian, "We only support little endian systems");
        }

        /// <summary>
        /// Reads the eight bytes of a string, unless it hits a null terminator
        /// to which it returns early. This is intended for raw lump strings
        /// that are always eight characters in length.
        /// </summary>
        /// <remarks>
        /// This will throw an exception like any other reading.
        /// </remarks>
        /// <returns>The string</returns>
        public string ReadEightByteString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < 8; i++)
            {
                char c = (char)ReadByte();
                if (c == 0)
                {
                    // We need to always consume eight characters. Since we
                    // have not incremented the loop iteration yet, we are 
                    // off by one and use 7 instead of 8.
                    Advance(7 - i);
                    break;
                }
                else
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Reads a 32-bit big endian integer. Throws if any errors result from
        /// reading the data (such as not enough data).
        /// </summary>
        /// <returns>The desired big endian integer.</returns>
        public int ReadInt32BE()
        {
            byte[] data = base.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        /// Moves the stream ahead (or backward if negative) by the provided
        /// number of bytes.
        /// </summary>
        /// <param name="amount">The amount of bytes to move.</param>
        public void Advance(int amount)
        {
            BaseStream.Seek(amount, SeekOrigin.Current);
        }

        /// <summary>
        /// Moves the internal pointer to the offset provided, relative to the
        /// beginning.
        /// </summary>
        /// <param name="offset">The offset to go to.</param>
        public void Offset(int offset)
        {
            BaseStream.Seek(offset, SeekOrigin.Begin);
        }
    }
}
