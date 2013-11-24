//requires github.com/EpicMorg/AdvancedWebClient


var init=20070806;//first pic
var d = @"<save_dir>";

var hasNext=true;
var r = new Regex("[0-9]{8}");
var ir = new Regex(@"http:\/\/s\.bash\.im\/img\/[a-zA-Z0-9]+\.jpg");
var root = "http://bash.im/comics/";
var cur = init;
while(hasNext){
	var dmp = AWC.DownloadString(root+cur);
	
	var img = ir.Match(dmp).Value;
	if (img!=""){
		AWC.DownloadFile(img, Path.Combine(d, Path.GetFileName(img)));
		img.Dump();
	}
	//next
	var rm = r
		.Matches(dmp)
		.OfType<Match>()
		.Select(a=>int.Parse(a.Value))
		.Distinct()
		.OrderBy(a=>a)
		.Where(a=>a>cur)
		.Take(1)
		.ToArray();
	if (rm.Length==0)
		break;
	cur=rm[0];
}
