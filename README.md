# 学生番号読み取りプログラム
Aichi Univercity of Technology(AUT) student id reader/CUI sample
## 概要
Felicaから学生情報を取り出します。<br>
[愛知工科大学](https://www.aut.ac.jp/)の学生証のみ対応しています

>**注意:**
>このプログラムを動作させるためには別売のICリーダ<br>
>PaSoRi [RC-S300](https://www.sony.co.jp/Products/felica/consumer/) が必要です

## メソッド
**FericaFunc.cs**
```cs
string readStudentId(Felica f)
```
引数: Ferica情報<br>
戻り値: 学籍番号(string)

## 要求要件
- windows 10
- Visual Studio 2019
- .NET Core 3.1
- PaSoRi RC-S300
- Felicalib [[元配布ページ]](http://felicalib.tmurakam.org/)
[[ダウンロード]](https://github.com/hohjukgi/Test/files/9956930/felicalib-0.4.2.zip)
- [NFCポートソフトウェア](https://www.sony.co.jp/Products/felica/consumer/support/download/nfcportsoftware.html?j-short=fsc_dl)

## 使用方法
**事前準備**
1. [NFCポートソフトウェア](https://www.sony.co.jp/Products/felica/consumer/support/download/nfcportsoftware.html?j-short=fsc_dl)のダウンロード・インストール<br>
2. [Felicalib](https://github.com/hohjukgi/Test/files/9956930/felicalib-0.4.2.zip)のダウンロード
3. このリポジトリをzip形式でダウンロード, 解凍

**導入手順**
1. 導入したいソリューションに**事前準備3**で解凍したフォルダ内にある**Felicalib.cs,FelicaFunc.cs**を追加
2. 導入したいソリューションの実行ファイルがある場所に**事前準備2**でダウンロードしたフォルダ内にある**Felicalib.dll**を入れる
3. 学籍番号を取得したい部分に以下のコードを挿入する
```cs
using (Felica f = new Felica())
{
    Console.WriteLine(FericaFunc.readStudentId(f));
}
```
このコードでは学籍番号を取得してコンソールに出力している

>**注意:**
>プラットフォームはx64ではなくx86を選択してください

## 別のFericaカードに対応させる
- 愛知工科大学以外の大学学生証、ICカードには未対応
- 別のFericaカードに対応させたい場合は以下のコードでFericaカードのダンプを行ってください
```cs
static void printserviceinfo(ushort s)
{
    string ident;
 
    switch ((s >> 1) & 0xf)
    {
        case 0: ident = "Area Code"; break;
        case 4: ident = "Random Access R/W"; break;
        case 5: ident = "Random Access Read only"; break;
        case 6: ident = "Cyclic Access R/W"; break;
        case 7: ident = "Cyclic Access Read only"; break;
        case 8: ident = "Purse (Direct)"; break;
        case 9: ident = "Purse (Cashback/decrement)"; break;
        case 10: ident = "Purse (Decrement)"; break;
        case 11: ident = "Purse (Read only)"; break;
        default: ident = "INVALID or UNKOWN"; break;
    }
 
    Console.Write("# Serivce code ={0:x4} : {1}", s, ident);
    if ((s & 0x1) == 0)
    {
        Console.Write(" (Protected)");
    }
    Console.Write("\n");
}
```
```cs
private void readCard(Felica f)
{
    int i, j, k;
    f.Polling((int)SystemCode.Any);
 
    Console.Write("# IDm: ");
    hexdump(f.IDm(), 8);
    Console.Write("\n");
    Console.Write("# PMm: ");
    hexdump(f.PMm(), 8);
    Console.Write("\n\n");
 
    felicat felicaf2 = new felicat();
    felicat felicaf1 = f.felica_enum_systemcode();
 
    for (i = 0; i < felicaf1.num_system_code; i++) {
        int syscode = ((felicaf1.system_code[i]) >> 8) & 0xff | ((felicaf1.system_code[i]) << 8) & 0xff00;
        Console.Write("# System code: {0:x4}\n", syscode);
        felicaf2 = f.felica_enum_service(syscode);
 
        Console.Write("# Number of area = {0}\n", felicaf2.num_area_code);
        for (j = 0; j < felicaf2.num_area_code; j++)
        {
            Console.Write("# Area: {0:x4} - {1:x4}\n", felicaf2.area_code[j], felicaf2.end_service_code[j]);
        }
 
        Console.Write("# Number of service code = {0}\n",  felicaf2.num_service_code);
        for (j = 0; j < felicaf2.num_service_code; j++)
        {
            ushort service = felicaf2.service_code[j];
            printserviceinfo(service);
 
            for (k = 0; k < 255; k++) {
                data = f.ReadWithoutEncryption((int)felicaf2.service_code[j], k);
                if (data == null) break;
 
                Console.Write("{0:x4}:{1:x4} ", (int)felicaf2.service_code[j], k);
                hexdump(data, 16);
                Console.Write("\n");
            }
        }
        Console.Write("\n");
    }
}
```
上記readCardメソッドでFelicaカードの情報をダンプしています<br>
<br>
この際
```cs
hexdump()
```
メソッドは定義されていないためエラーが発生します
- 第一引数: 変換するデータ(Byte[])
- 第二引数: 変換後の進数表記(int?)
- メソッド内容: 変換するデータを第二引数の進数表記に直したものを出力する
<br>上記の内容でメソッドを作成するか、代替メソッドを用意してください
<br>
## Contact
その他分からないことや提案、バグの報告がある場合はこのGitHubページのissue
引用: https://tomosoft.jp/design/?p=4737
