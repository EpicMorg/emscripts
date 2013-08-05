#!/usr/bin/csharp 
//License: CC-BY-SA
//Author:kasthack, 2012-2013

using System.IO;
using System.Diagnostics;
 
//paths
var www_dir = "/var/www";																	//dir for servers
var htdocs_prefix ="htdocs";																//htdocs prefix
var apache_domain_cfg = "/etc/apache2/vhosts.d/00_default_vhost.conf"; //apache config for domains
var apache_dirs_cfg = "/etc/apache2/vhosts.d/vhost.dirs";						//apache config for dirs
var nginx_domain_cfg = "/etc/nginx/server_domains.conf";						//nginx domains config
var dir_owner = "apache:apache";														//web server files owner
var backend_address ="127.0.0.1:81";													//backend for nginx proxy
var itworks_file_name = "index.html";													//file to write itworks page
//Apache configuration: directory config
//{1} will be replaced with $www_did
//{0} - domain
//{2} - htdocs prefix

string apache_dir_string =@"
<Directory ""{1}/{0}/{2}"">
	Order allow,deny
	Allow from all
	#DirectoryIndex index.html
</Directory>";

//Apache configuration: main domain config
//{0} will he replaced with ghost
//{1} - domain
//{2} - domain dir + domain
//{3} - subdomain rewrite rules
//{4} - htdocs prefix

string apache_vhost_string =@"
<VirtualHost {0}>
	ServerName {1}
	ServerAlias {1} *.{1}
	DocumentRoot {2}/{4}
	Include /etc/apache2/vhosts.d/env.include{3}
</VirtualHost>\r\n";

//Apache configuration: rewrites for subdomains
//{USER_DOMAIN} will be replaced with ur domain

string subdomain_rewrite = @"
	RewriteCon %{HTTP_HOST} !^www.{USER_DOMAIN}$
	RewriteCond %{HTTP_HOST} ^((.*)\.{USER_DOMAIN}
	RewriteRule ^/(.*) /%2/$1";

//'It works' page template
//{0} will be replaced with domain

string default_html_tpl = @"<center><h1>{0} works.</h1></center>";

//#########################################################################
//Don't edit below this line
//#########################################################################

string domain, dir, rewrite_now = " ",ngc;
bool subdomains_support;
string[] f;
int index;
Func<string, string> _ri = x => {
	print(x);
	Console.Write('>');
	return Console.ReadLine().Trim();
};
domain = _ri("Enter domain(unicode domains must be converted to punycode)").ToLower();
print("Adding {0}", domain);

//create server root
dir=Path.Combine(www_dir,domain);
print("//create dir");
if (!Directory.Exists(dir)){
       Directory.CreateDirectory(dir);
       Directory.CreateDirectory(Path.Combine(dir,htdocs_prefix));
       var ifn = Path.Combine(
       		dir,
       		htdocs_prefix,
       		itworks_file_name
       	);
       if (!File.Exists(ifn))
       	File.WriteAllText(
       		ifn,
       		String.Format(
       			default_html_tpl,
       			domain
       		)
       	);
}

print("//fix privileges");
Process.Start("chown", "-R "+dir_owner+" "+Path.Combine(www_dir,domain));

//apache
print("//add domain to apache config");
subdomains_support = bool.Parse(_ri("Enable autosubdomain support(true/false)?"));
f = File.
ReadAllLines(apache_domain_cfg);
if (!String.Concat(f).Contains(domain)){
	index = f.
	       Select(a=>a.Trim(new char[]{' ','\t'})).
	       ToList().
	       LastIndexOf("</VirtualHost>");
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
         rewrite_now,
         htdocs_prefix
      )
    }
    ).
    Concat(
    	f.Skip(index+1).
    	Take(f.Length-index-1)
    ).
    ToArray()
	);
               
	//moar apache
	print("//dirs apache access");
	File.
	AppendAllText(
		apache_dirs_cfg,
		String.Format(
			apache_dir_string,
			domain,
			www_dir,
			htdocs_prefix)
		);
}
//nginx
ngc = File.ReadAllText(nginx_domain_cfg);
if (!ngc.Contains(domain)){
print("//add address to nginx");
File.WriteAllText(
       nginx_domain_cfg,
       ngc.
       TrimEnd(
               " \r\n\t;".
               ToCharArray()
               )+
       " ."+domain+";\r\n"
       );
}
//reload configs
print("//reload nginx config");
Process.Start("/etc/init.d/nginx","reload");
print("//reload apache config");
Process.Start("/etc/init.d/apache2","reload");
print("Finished");
