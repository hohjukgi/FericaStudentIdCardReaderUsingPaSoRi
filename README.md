# name
Aichi Univercity of Technology(AUT) student id reader/CUI sample
## Overview
Fericaから学生情報を取り出します。<br>
[愛知工科大学](https://www.aut.ac.jp/ "愛知工科大学ホームページ")の学生証のみ対応しています

>**注意:**
>このプログラムを動作させるためには別売のICリーダ<br>
>PaSoRi [RC-S300](https://www.sony.co.jp/Products/felica/consumer/) が必要です

## Method
**FericaFunc.cs**
```cs
string readStudentId(Felica f)
```
引数: Ferica情報<br>
戻り値: 学籍番号(string)

## Requirement
- windows 10
- Visual Studio 2019
- .NET Core 3.1
- Pasori
- Fericalib [[元配布ページ]](http://felicalib.tmurakam.org/)
[[ファイル]](https://github.com/hohjukgi/Test/files/9956930/felicalib-0.4.2.zip)
- [NFCポートソフトウェア](https://www.sony.co.jp/Products/felica/consumer/support/download/nfcportsoftware.html?j-short=fsc_dl)
- NuGetパッケージ: [PCSC 6.0.0](https://www.nuget.org/packages/PCSC/6.0.0?_src=template)
- NuGetパッケージ: [PCSC.Iso7816 6.0.0](https://www.nuget.org/packages/PCSC.Iso7816/6.0.0?_src=template)

## How to use
