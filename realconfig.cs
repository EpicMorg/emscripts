using System.IO;
using System;
using System.Linq;
namespace EpicMorg.Tools.Config
{
	enum OPMode
	{
		File,
		Dir
	}
	class RealConfig
	{
		static string datestring=
			DateTime.Now.Day.ToString ()+ 
				DateTime.Now.Month.ToString ()+
				DateTime.Now.Year.ToString ();
		static void Main (string[] files)
		{
			var recursive = false;
			var mode = OPMode.File;
			var backup = true;
			var backup_name = "#file#.sample";
			if (files.Length == 0) {
				Console.WriteLine ("No input specified");
				print_help ();
				return;
			}
			var L = files.ToList ();
			backup = !L.Contains ("--no-backup");
			if (L.Contains ("--help") || L.Contains ("-h")) {
				print_help ();
				return;
			}
			if (L.Contains ("--recursive"))
				throw new NotImplementedException ("Not supported yet");
			var v2 = files.Where (a => a.StartsWith ("--mode")).ToArray ();
			if (v2.Length > 0) {
				switch (v2.Last ()) {
				case "--mode=f":
					mode = OPMode.File;
					break;
				case "--mode=d":
					mode = OPMode.Dir;
					throw new NotImplementedException ("Not supported yet");
				default:
					Console.WriteLine ("Wrong mode");
					print_help ();
					return;
				}
			}
			bool remove_empty=L.Contains("--remove-empty-lines");
			//bool trim_spaces=
			var v1 = files.Where (a => a.StartsWith ("--backup-file")).ToArray ();
			if (v1.Length > 0) {
				var v3 = v1.Last ();
				try {
					backup_name = v3.Substring (v3.IndexOf ('=') + 1);
				} catch {
					Console.WriteLine ("Wrong backup filename");
					print_help ();
					return;
				}
			}
			files = files.
				Where (a => !a.StartsWith ("--")).
					ToArray ();
			foreach (var f in files) 
			{
				Console.WriteLine("Processing "+f);
				try
				{
					if (backup)
						File.Copy (f, get_backup (f,backup_name));
				}
				catch
				{
					Console.WriteLine("Failed to backup "+f+" Skipping");
					continue;
				}
				try
				{
					var raw =File.ReadAllText(f).
					    Replace("\\\r\n","").
					    Replace("\\\r","").
					    Replace("\\\n","").
					    Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					raw=remove_empty?
					    raw.
						Where(a=>!String.IsNullOrEmpty(a.Trim("\t ".ToCharArray())))
						    .ToArray()
					    :raw;
					raw=raw.Where(a=>!a.Trim("\t ".ToCharArray()).StartsWith("#")).ToArray();
					File.WriteAllLines(f,raw);
				}
				catch
				{
					Console.WriteLine("Failed to process "+f+" Skipping");
					continue;
				}
			}
			Console.WriteLine("Finished");
		}
		static string get_backup (string file, string format)
		{
			string f = Path.GetFileName (file);
			return Path.Combine (
				Path.GetDirectoryName (file),
				format.Replace ("#date#", datestring).Replace ("#file#", f));
		}
		static void print_help()
		{
			Console.WriteLine("Real Config by kastack © 2012");
			Console.WriteLine(@"usage:
realconfig [...PARAMS...] DIRECTORIES or FILE(S)
Params:
--remove-empty-lines		:i believe u'll guess
--trim-spaces			:remove leading and trailing spaces in lines
--backup-file=(#file#.sample)	: rule for backuping, 
				    #file# will be replaced with filename
				    #date# will be replaced with DDMMYYYY date
--recursive 			:search in subdirs(false by default)
--mask=(*.cfg|*.conf)		:search mask
--no-backup			:don't create backups
--mode=f			:process [d] irecories or [f] iles
");
		}
	}
}
