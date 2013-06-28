#!/usr/bin/csharp
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
print("Введите имя файла со списком урлов");
var filename=Console.ReadLine();
Regex rghost = new Regex(@"http://rghost.ru/download/[0-9]+/[0-9a-z]+/[A-Za-z0-9\.%]+");
var wc = new WebClient();

foreach (var s in File.ReadLines(filename).Select(a=>"http://"+a))
 {
 try
 {
 var u =r.Match(wc.DownloadString(s)).Value;
	print(u);
	if (u.Length>0)
		wc.DownloadFile(u,s.Substring(s.LastIndexOf('/')+1)+u.Substring(u.LastIndexOf('.')));
		}catch{}
 }
