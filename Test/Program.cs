using System;
using PCSC;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IntPtr hContext = IntPtr.Zero;

            // ##################################################
            // 1. SCardEstablishContext
            // ##################################################
            Console.WriteLine("***** 1. SCardEstablishContext *****");
            uint ret = Api.SCardEstablishContext(Constant.SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, out hContext);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                string message;
                switch (ret)
                {
                    case Constant.SCARD_E_NO_SERVICE:
                        message = "サービスが起動されていません。";
                        break;
                    default:
                        message = "サービスに接続できません。code = " + ret;
                        break;
                }
                throw new ApplicationException(message);
            }

            if (hContext == IntPtr.Zero)
            {
                throw new ApplicationException("コンテキストの取得に失敗しました。");
            }
            Console.WriteLine("　サービスに接続しました。");


            // ##################################################
            // 2. SCardListReaders
            // ##################################################
            Console.WriteLine("***** 2. SCardListReaders *****");
            uint pcchReaders = 0;

            // NFCリーダの文字列バッファのサイズを取得
            ret = Api.SCardListReaders(hContext, null, null, ref pcchReaders);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                // 検出失敗
                throw new ApplicationException("NFCリーダを確認できません。");
            }

            // NFCリーダの文字列を取得
            byte[] mszReaders = new byte[pcchReaders * 2]; // 1文字2byte
            ret = Api.SCardListReaders(hContext, null, mszReaders, ref pcchReaders);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                // 検出失敗
                throw new ApplicationException("NFCリーダの取得に失敗しました。");
            }


            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            string readerNameMultiString = unicodeEncoding.GetString(mszReaders);

            // 認識したNDCリーダの最初の1台を使用
            int nullindex = readerNameMultiString.IndexOf((char)0);
            var readerName = readerNameMultiString.Substring(0, nullindex);
            Console.WriteLine("　NFCリーダを検出しました。 " + readerName);



            // ##################################################
            // 3. SCardConnect
            // ##################################################
            Console.WriteLine("***** 3. SCardConnect *****");
            IntPtr hCard = IntPtr.Zero;
            IntPtr activeProtocol = IntPtr.Zero;
            ret = Api.SCardConnect(hContext, readerName, Constant.SCARD_SHARE_SHARED, Constant.SCARD_PROTOCOL_T1, ref hCard, ref activeProtocol);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("カードに接続できません。code = " + ret);
            }
            Console.WriteLine("　カードに接続しました。");



            // ##################################################
            // 4. SCardTransmit
            // ##################################################
            Console.WriteLine("***** 4. SCardTransmit *****");
            uint maxRecvDataLen = 256;
            var recvBuffer = new byte[maxRecvDataLen + 2];
            var sendBuffer = new byte[] { 0xff, 0xca, 0x00, 0x00, 0x00 };  // ← IDmを取得するコマンド

            Api.SCARD_IO_REQUEST ioRecv = new Api.SCARD_IO_REQUEST();
            ioRecv.cbPciLength = 255;

            int pcbRecvLength = recvBuffer.Length;
            int cbSendLength = sendBuffer.Length;

            IntPtr handle = Api.LoadLibrary("Winscard.dll");
            IntPtr pci = Api.GetProcAddress(handle, "g_rgSCardT1Pci");
            Api.FreeLibrary(handle);

            ret = Api.SCardTransmit(hCard, pci, sendBuffer, cbSendLength, ioRecv, recvBuffer, ref pcbRecvLength);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("NFCカードへの送信に失敗しました。code = " + ret);
            }

            // 受信データからIDmを抽出する
            // recvBuffer = IDm + SW1 + SW2 (SW = StatusWord)
            // SW1 = 0x90 (144) SW1 = 0x00 (0) で正常だが、ここでは見ていない
            string cardId = BitConverter.ToString(recvBuffer, 0, pcbRecvLength - 2);
            Console.WriteLine("　カードからデータを取得しました。");
            Console.WriteLine("　【IDm】：" + cardId);


            // ##################################################
            // 5. SCardDisconnect
            // ##################################################
            Console.WriteLine("***** 5. SCardDisconnect *****");
            ret = Api.SCardDisconnect(hCard, Constant.SCARD_LEAVE_CARD);
            if (ret != Constant.SCARD_S_SUCCESS)
            {
                throw new ApplicationException("NFCカードとの切断に失敗しました。code = " + ret);
            }
            Console.WriteLine("　カードを切断しました。");
        }
    }
}
