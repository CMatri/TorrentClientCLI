﻿/*
Copyright (c) 2013, Darren Horrocks
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this
  list of conditions and the following disclaimer in the documentation and/or
  other materials provided with the distribution.

* Neither the name of Darren Horrocks nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/

using System;

namespace TorrentClientCLI.Tracker.Udp
{
    public static class Unpack
    {
        public enum Endianness
        {
            Machine,
            Big,
            Little
        }

        public static Int16 Int16(byte[] bytes, Int32 start, Endianness e = Endianness.Machine)
        {
            byte[] intBytes = Utils.GetBytes(bytes, start, 2);

            if (NeedsFlipping(e)) Array.Reverse(intBytes);

            return BitConverter.ToInt16(intBytes, 0);
        }

        public static Int32 Int32(byte[] bytes, Int32 start, Endianness e = Endianness.Machine)
        {
            byte[] intBytes = Utils.GetBytes(bytes, start, 4);

            if (NeedsFlipping(e)) Array.Reverse(intBytes);

            return BitConverter.ToInt32(intBytes, 0);
        }

        public static Int64 Int64(byte[] bytes, Int32 start, Endianness e = Endianness.Machine)
        {
            byte[] intBytes = Utils.GetBytes(bytes, start, 8);

            if (NeedsFlipping(e)) Array.Reverse(intBytes);

            return BitConverter.ToInt64(intBytes, 0);
        }

        public static UInt16 UInt16(byte[] bytes, Int32 start, Endianness e = Endianness.Machine)
        {
            byte[] intBytes = Utils.GetBytes(bytes, start, 2);

            if (NeedsFlipping(e)) Array.Reverse(intBytes);

            return BitConverter.ToUInt16(intBytes, 0);
        }

        public static UInt32 UInt32(byte[] bytes, Int32 start, Endianness e = Endianness.Machine)
        {
            byte[] intBytes = Utils.GetBytes(bytes, start, 4);

            if (NeedsFlipping(e)) Array.Reverse(intBytes);

            return BitConverter.ToUInt32(intBytes, 0);
        }

        public static UInt64 UInt64(byte[] bytes, Int32 start, Endianness e = Endianness.Machine)
        {
            byte[] intBytes = Utils.GetBytes(bytes, start, 8);

            if (NeedsFlipping(e)) Array.Reverse(intBytes);

            return BitConverter.ToUInt64(intBytes, 0);
        }

        private static bool NeedsFlipping(Endianness e)
        {
            switch (e)
            {
                case Endianness.Big:
                    return BitConverter.IsLittleEndian;
                case Endianness.Little:
                    return !BitConverter.IsLittleEndian;
            }

            return false;
        }
        public static String Hex(byte[] bytes, Endianness e = Endianness.Machine)
        {
            String str = "";

            foreach (byte b in bytes)
            {
                str += String.Format("{0:X2}", b);
            }

            return str;
        }
    }

    public static class Utils
    {
        public static bool GetBit(this byte t, UInt16 n)
        {
            return (t & (1 << n)) != 0;
        }

        public static byte SetBit(this byte t, UInt16 n)
        {
            return (byte)(t | (1 << n));
        }

        public static byte[] GetBytes(this byte[] bytes, Int32 start, Int32 length = -1)
        {
            int l = length;
            if (l == -1) l = bytes.Length - start;

            byte[] intBytes = new byte[l];

            for (int i = 0; i < l; i++) intBytes[i] = bytes[start + i];

            return intBytes;
        }

        public static byte[] Cat(this byte[] first, byte[] second)
        {
            byte[] returnBytes = new byte[first.Length + second.Length];

            first.CopyTo(returnBytes, 0);
            second.CopyTo(returnBytes, first.Length);

            return returnBytes;
        }

        public static bool Contains<T>(this T[] ar, T o)
        {
            foreach (T t in ar)
            {
                if (Equals(t, o)) return true;
            }

            return false;
        }

        public static bool Contains<T>(this T[] ar, Func<T, bool> expr)
        {
            foreach (T t in ar)
            {
                if (expr != null && expr(t)) return true;
            }

            return false;
        }
    }
}
