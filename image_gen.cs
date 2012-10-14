#!/usr/bin/csharp2
print("PWNing image gen by kasthack");

LoadAssembly("System.Drawing.dll");
using System.Drawing;
using System.Drawing.Drawing2D;
print("Enter Bitmap side. Ex:10000");
int size=int.Parse(Console.ReadLine());
print("Enter cell side. Ex:100");
int size_b=int.Parse(Console.ReadLine());
print("Processing...");

var bmp = new Bitmap(size,size);
Graphics gr = Graphics.FromImage(bmp);
SolidBrush brb = new SolidBrush(Color.Black);
SolidBrush brw = new SolidBrush(Color.White);
Rectangle rect= new Rectangle(new Point(0,0), new Size(size_b,size_b));
gr.Clear(Color.White);
for (int i=0;i<size/size_b+1; i++) 
{
    rect.Y=i*size_b;
    for (int j=i&1;j<size/size_b+1;j++) 
    { 
	rect.X=j*size_b;
	gr.FillRectangle((((j+(i&1))&1)==0)?brb:brw, rect);
    }
}
gr.Flush(FlushIntention.Sync);
gr.Dispose();

print("Enter filename");
var fn =Console.ReadLine();
print("Saving");
bmp.Save(fn);
bmp.Dispose();
GC.Collect();
print("Successfully finished.");