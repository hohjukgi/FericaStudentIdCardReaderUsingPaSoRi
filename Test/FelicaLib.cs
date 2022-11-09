/*
 felicalib - FeliCa access wrapper library

 Copyright (c) 2007, Takuya Murakami, All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions are
 met:

 1. Redistributions of source code must retain the above copyright notice,
    this list of conditions and the following disclaimer. 

 2. Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution. 

 3. Neither the name of the project nor the names of its contributors
    may be used to endorse or promote products derived from this software
    without specific prior written permission. 

 THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace FelicaLib
{
    // システムコード
    enum SystemCode : int
    {
        Any = 0xffff,           // ANY
        Common = 0xfe00,        // 共通領域
        Cyberne = 0x0003,       // サイバネ領域

        Edy = 0xfe00,           // Edy (=共通領域)
        Suica = 0x0003,         // Suica (=サイバネ領域)
    }

    public class Felica : IDisposable
    {
        [DllImport("felicalib.dll")]
        private extern static IntPtr pasori_open(String dummy);
        [DllImport("felicalib.dll")]
        private extern static void pasori_close(IntPtr p);
        [DllImport("felicalib.dll")]
        private extern static int pasori_init(IntPtr p);
        [DllImport("felicalib.dll")]
        private extern static IntPtr felica_polling(IntPtr p, ushort systemcode, byte rfu, byte time_slot);
        [DllImport("felicalib.dll")]
        private extern static void felica_free(IntPtr f);
        [DllImport("felicalib.dll")]
        private extern static void felica_getidm(IntPtr f, byte[] data);
        [DllImport("felicalib.dll")]
        private extern static void felica_getpmm(IntPtr f, byte[] data);
        [DllImport("felicalib.dll")]
        private extern static int felica_read_without_encryption02(IntPtr f, int servicecode, int mode, byte addr, byte[] data);

        private IntPtr pasorip = IntPtr.Zero;
        private IntPtr felicap = IntPtr.Zero;

        public Felica()
        {
            pasorip = pasori_open(null);
            if (pasorip == IntPtr.Zero)
            {
                throw new Exception("felicalib.dll を開けません");
            }
            if (pasori_init(pasorip) != 0)
            {
                throw new Exception("PaSoRi に接続できません");
            }
        }

        public void Dispose()
        {
            if (pasorip != IntPtr.Zero)
            {
                pasori_close(pasorip);
                pasorip = IntPtr.Zero;
            }
        }

        ~Felica()
        {
            Dispose();
        }

        public void Polling(int systemcode)
        {
            felica_free(felicap);

            felicap = felica_polling(pasorip, (ushort)systemcode, 0, 0);
            if (felicap == IntPtr.Zero)
            {
                throw new Exception("カード読み取り失敗");
            }
        }

        public byte[] IDm()
        {
            if (felicap == IntPtr.Zero)
            {
                throw new Exception("no polling executed.");
            }

            byte[] buf = new byte[8];
            felica_getidm(felicap, buf);
            return buf;
        }    

        public byte[] PMm()
        {
            if (felicap == IntPtr.Zero)
            {
                throw new Exception("no polling executed.");
            }

            byte[] buf = new byte[8];
            felica_getpmm(felicap, buf);
            return buf;
        }    

        public byte[] ReadWithoutEncryption(int servicecode, int addr)
        {
            if (felicap == IntPtr.Zero)
            {
                throw new Exception("no polling executed.");
            }

            byte[] data = new byte[16];
            int ret = felica_read_without_encryption02(felicap, servicecode, 0, (byte)addr, data);
            if (ret != 0)
            {
                return null;
            }
            return data;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct felicat
        {
            public IntPtr p;          /**< PaSoRi ハンドル */
            public ushort systemcode;  /**< システムコード */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] IDm;       /**< IDm */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] PMm;       /**< PMm */

            /* systemcode */
            public byte num_system_code;       /**< 列挙システムコード数 */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public ushort[] system_code;       /**< 列挙システムコード */
            /* area/service codes */
            public byte num_area_code;         /**< エリアコード数 */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public ushort[] area_code;         /**< エリアコード */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public ushort[] end_service_code;  /**< エンドサービスコード */
            public byte num_service_code;      /**< サービスコード数 */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public ushort[] service_code;      /**< サービスコード */
        };

        [DllImport("felicalib.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr felica_enum_systemcode(IntPtr p);
        [DllImport("felicalib.dll", CallingConvention = CallingConvention.Cdecl)]
        private extern static IntPtr felica_enum_service(IntPtr p, ushort systemcode);

        public felicat felica_enum_systemcode()
        {
            felica_free(felicap);

            felicap = felica_enum_systemcode(pasorip);
            if (felicap == IntPtr.Zero)
            {
                throw new Exception("カード読み取り失敗");
            }
            felicai = (felicat)Marshal.PtrToStructure(felicap, typeof(felicat));
            return felicai;
        }

        public felicat felica_enum_service(int systemcode)
        {
            felica_free(felicap);

            felicap = felica_enum_service(pasorip, (ushort)systemcode);
            if (felicap == IntPtr.Zero)
            {
                throw new Exception("カード読み取り失敗");
            }
            felicai = (felicat)Marshal.PtrToStructure(felicap, typeof(felicat));
            return felicai;
        }

        private felicat felicai;

    }
}
