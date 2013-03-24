#!/usr/bin/csharp
//tool to add domain for nginx-apache server
//Vars:
//nginx_domain_cfg - file with string "server_name (domain_name)+;" included from main file
//apache_dirs_cfg - file with permissions for each dir included in global context
using  System.IO;
using System.Diagnostics;
var www_dir = "/var/www";
var htdocs_prefix ="htdocs";
var apache_domain_cfg = "/etc/apache2/vhosts.d/00_default_vhost.conf";
var apache_dirs_cfg = "/etc/apache2/vhosts.d/vhost.dirs";
var nginx_domain_cfg = "/etc/nginx/server_domains.conf";
var dir_owner = "apache:apache";
var backend_address ="127.0.0.1:81";
string apache_dir_string ="\r\n<Directory \"/var/www/{0}/htdocs\">\r\n\tOrder allow,deny\r\n\tAllow from all\r\n\tDirectoryIndex index.html\r\n</Directory>\r\n";
string apache_vhost_string ="\r\n<VirtualHost {0}>\r\n\tServerName {1}\r\n\tServerAlias {1} *.{1}\r\n\tDocumentRoot {2}\r\n\tInclude /etc/apache2/vhosts.d/env.include{3}\r\n</VirtualHost>\r\n";
string subdomain_rewrite = "\r\n\tRewriteCond %{HTTP_HOST} !^www.{USER_DOMAIN}$\r\n\tRewriteCond %{HTTP_HOST} ^((.*)\\.){USER_DOMAIN}\r\n\tRewriteRule ^/(.*) /%2/$1";
print("Enter domain");
string domain = Console.ReadLine().ToLower();
print("Adding {0}", domain);
string dir=Path.Combine(www_dir,domain);
print("//create dir");
if (!Directory.Exists(dir))
{
        Directory.CreateDirectory(dir);
        Directory.CreateDirectory(dir=Path.Combine(dir,htdocs_prefix));
}
print("//fix privileges");
Process.Start("chown", "-R "+dir_owner+" "+Path.Combine(www_dir,domain));
print("//add domain to apache config");
print("Enable autosubdomain support(true/false)?");
bool subdomains_support = bool.Parse(Console.ReadLine());
var f = File.
ReadAllLines(apache_domain_cfg);
int index = f.
        Select(a=>a.Trim(new char[]{' ','\t'})).
        ToList().
        LastIndexOf("</VirtualHost>");
string rewrite_now = " ";
if (subdomains_support)
    rewrite_now =  subdomain_rewrite.Replace( "{USER_DOMAIN}", domain );
print("//rewrite");
File.
        WriteAllLines(
                apache_domain_cfg,
                f.Take(index+1).
                        Concat(
                                new string[]{
                                        String.Format(
                                                apache_vhost_string,
                                                backend_address,
                                                domain,
                                                dir,
                                                rewrite_now
                                        )
                                        }
                                ).
                        Concat(
                                f.Skip(index+1).
                                Take(f.Length-index-1)
                                ).
                        ToArray()
                );
print("//dirs apache access");
File.AppendAllText(apache_dirs_cfg, String.Format(apache_dir_string, domain) );
print("//add address to nginx");
File.WriteAllText(
        nginx_domain_cfg,
        File.ReadAllText(nginx_domain_cfg).
        TrimEnd(
                " \r\n\t;".
                ToCharArray()
                )+
        " ."+domain+";\r\n"
        );
print("//reload nginx config");
Process.Start("/etc/init.d/nginx","reload");
print("//reload apache config");
Process.Start("/etc/init.d/apache2","reload");
Console.WriteLine("Finished");#                  
