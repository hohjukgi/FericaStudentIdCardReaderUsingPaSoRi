using System;
using System.Collections.Generic;
using FelicaLib;

    public static class FericaFunc
    {
    public static string readStudentId(Felica f)
    {
        f.Polling((int)SystemCode.Any);

        Felica.felicat felicaf2 = new Felica.felicat();
        Felica.felicat felicaf1 = f.felica_enum_systemcode();

        int syscode = ((felicaf1.system_code[0]) >> 8) & 0xff | ((felicaf1.system_code[0]) << 8) & 0xff00;
        felicaf2 = f.felica_enum_service(syscode);

        byte[] data = f.ReadWithoutEncryption((int)felicaf2.service_code[1], 0);
        if (data == null)
        {
            throw new Exception("カード読み取り失敗");
        }

        List<Byte> vs = new List<Byte>();

        for (int i = 0; i < data.Length; i++)
        {
            vs.Add(data[i]);
        }

        vs.RemoveRange(0, 2);
        vs.RemoveRange(8, 6);

        string studentId = string.Empty;

        for (int i = 0; i < vs.Count; i++)
        {
            studentId += vs[i] & 0x0f;
        }

        return studentId;
    }
}