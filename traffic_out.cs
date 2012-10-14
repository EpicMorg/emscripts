#!/usr/bin/csharp
using System.IO;
print("Введите алиас для расчетов");
var alias = Console.ReadLine();
try
{
var vv=File.ReadAllLines("traffic_apache_"+alias).Select(a=>double.Parse(a)).ToArray();
double traf=vv.Sum();
string s="";
try{s=File.ReadAllText("traffic_stat");}catch{}
int sz=0;
char[] szs=new char[]{'B','K','M','G'};
while(traf>1024)
{
    sz++;
    traf/=1024;
}
File.WriteAllText("traffic_stat",s+"["+DateTime.Now.ToString()+"] - Server="+alias+"; Traffic="+traf.ToString("0.0000")+szs[sz]+", Requests="+vv.Length+Environment.NewLine);
File.WriteAllText("traffic_apache_"+alias,"");
}
catch
{
print("Fail");
}