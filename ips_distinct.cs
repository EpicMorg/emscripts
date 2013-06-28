#!/usr/bin/csharp
using System.IO;
using System.Net;
File.WriteAllLines("localhost.ips",File.ReadAllLines("localhost.ips").Distinct().OrderBy(x=>BitConverter.ToUInt32(IPAddress.Parse(x).GetAddressBytes(),0)).ToArray());