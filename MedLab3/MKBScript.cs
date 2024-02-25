/*
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;

class Program
{
    static void Main()
    {
        string filePath = "MKB.json";
        string jsonContent = File.ReadAllText(filePath);
        var mkbData = JsonConvert.DeserializeObject<MkbData>(jsonContent);
        List<Tuple<string, string>> lst = new List<Tuple<string, string>>();
        foreach (var item in mkbData.Records)
        {
            Tuple<string, string> newTuple = new Tuple<string, string>(item.ID, Guid.NewGuid().ToString());
            lst.Add(newTuple);
        }
        int i = 1;
        foreach (var item in mkbData.Records)
        {
            item.ID_PARENT = lst.FirstOrDefault(pair => pair.Item1 == item.ID_PARENT)?.Item2;
            item.ID = lst.FirstOrDefault(pair => pair.Item1 == item.ID)?.Item2;
            Console.WriteLine("Заменено строк " + i);
            i++;
        }
        string updatedJson = JsonConvert.SerializeObject(mkbData, Formatting.Indented);
        File.WriteAllText(filePath, updatedJson);
        Console.ReadKey();
    }
}

class MkbRecord
{
    public string ID { get; set; }
    public int ACTUAL { get; set; }
    public string MKB_CODE { get; set; }
    public string MKB_NAME { get; set; }
    public string REC_CODE { get; set; }
    public string ID_PARENT { get; set; }
}

class MkbData
{
    public List<MkbRecord> Records { get; set; }
}
*/