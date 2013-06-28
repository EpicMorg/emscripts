#!/usr/bin/csharp
using System.IO;
var s = File.ReadLines("distinct").
		Select(
			a=>
				new string(a.ToCharArray().
					Select(c=>(int)c).
						Where(
							b=>
								(
								 b<1300 && b>47 && 
									( b>1000 || 
										( b>64 && b<123 && 
											( b>96||b<91)
										)
										||b<58)
								)
		).Where(a=>a.Length>0).Select(a=>a.ToLower).ToArray();							).Select(d=>(char)d).ToArray())
Array.Sort(s);
s=s.Distinct().ToArray();
File.WriteAllLines("d2",s);