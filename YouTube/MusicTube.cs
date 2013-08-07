/* This program is using google-gdata */

using System;
using System.Windows.Forms;
using System.Net;
using System.Web;
using System.Collections.Generic;
using System.Drawing;

using Google.GData.YouTube;
using Google.GData.Extensions;
using Google.GData.Extensions.MediaRss;
using System.Collections;

class YouTube1 : Form
{
    private const String Html = "<html>" +
                                  "<body>" +
                                    "<object width='{0}' height='{1}'>" +
                                    "<param name='movie' value='{2}&hl=ja=ja&fs=1'>" +
                                    "</param>" +
                                    "<param name=\"wmode\" value=\"transparent\"></param>" +
                                    "<embed src='{2}&fmt=22&hl=ja&fs=1&loop=1&autoplay=1' type='application/x-shockwave-flash' allowscriptaccess='always' allowfullscreen='true' width='{0}' height='{1}'>" +
                                    "</embed>" +
                                    "</object>" +
                                  "</body>" +
                                "</html>";
    private WebBrowser player;
    private TextBox tb = new TextBox();

    private string strData;

    private NotifyIcon notifyIcon1 = new NotifyIcon();
    private string strTmp;
    private ListBox lbx;
    private string[] strUrl = new string[10];

    [STAThread]
    public static void Main()
    {
        Application.Run(new YouTube1());
    }
    public YouTube1()
    {
        this.Text = "MusicTube";
        this.Width = 600;
        this.Height = 170;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.ClientSizeChanged += new EventHandler(form_Size_Changed);
        this.MaximizeBox = false;
        notifyIcon1.DoubleClick += new EventHandler(notifyIcon1_DoubleClick);
        notifyIcon1.Text = "MusicTube";


        Icon icon = YouTube.Properties.Resources.icon1;
        this.Icon = icon;

        tb.Dock = DockStyle.Top;
        tb.Parent = this;
        //query.Start

        player = new WebBrowser();
        player.Dock = DockStyle.Top;


        //player.AllowNavigation = false;
        player.Dock = System.Windows.Forms.DockStyle.Fill;
        player.Location = new System.Drawing.Point(0, 0);
        player.MinimumSize = new System.Drawing.Size(0, 0);
        player.Name = "MusicTube";
        player.ScrollBarsEnabled = false;
        player.Size = new System.Drawing.Size(0, 0);
        player.TabIndex = 0;

        player.AllowWebBrowserDrop = false;
        player.IsWebBrowserContextMenuEnabled = false;
        player.WebBrowserShortcutsEnabled = false;

        lbx = new ListBox();
        lbx.Width = tb.Width;
        lbx.Height = 130;
        lbx.Top = tb.Bottom;
        
        lbx.Parent = this;
        player.Parent = this;

        tb.KeyDown += new KeyEventHandler(tb_KeyDown);
        lbx.SelectedIndexChanged += new EventHandler(lbx_SelectedIndexChanged);
    }
    public void tb_KeyDown(Object sender, KeyEventArgs e){
        TextBox tmp = (TextBox)sender;

        if(e.KeyCode == Keys.Enter){

            if (tmp.Text == strTmp)
            {
                strTmp = "";
                player.DocumentText = "";
                return;
            }
            strTmp = tmp.Text;


            YouTubeService service = new YouTubeService("MusicTube");
            YouTubeQuery query = new YouTubeQuery();
            query.StartIndex = 0;
            query.NumberToRetrieve = 10;

            query.Uri = new Uri(String.Format("http://gdata.youtube.com/feeds/api/videos?vq={0}",
                 HttpUtility.UrlEncode(tmp.Text))
                 );
            YouTubeFeed feed = service.Query(query);



            int i = 0;
            lbx.Items.Clear();
            foreach (YouTubeEntry entry in feed.Entries)
            {
                MediaGroup group = entry.Media;

                //タイトルと説明
                //Console.WriteLine("Title / {0}", group.Title.Value);

                //コンテンツ数分ループ
                foreach (MediaContent content in group.Contents)
                {
                    SortedList attributes = content.Attributes;

                    //属性数分ループ
                    foreach (DictionaryEntry attribute in attributes)
                    {
                        //Console.WriteLine("Thumbnail key / {0}", attribute.Value + "\n");
                        if (attribute.Key.Equals("url"))
                        {
                            //サムネイルのURL
                            //Console.WriteLine("Thumbnail key / {0}", attribute.Value + "\n");
                            strData = (string)attribute.Value;
                            //Console.WriteLine("test:{0}\n", strData.IndexOf("?"));
                            //Console.WriteLine("aaa:\n" + strData);
                            if (strData.IndexOf("?") < 0)
                            {
                                i--;
                                break;
                            }
                            lbx.Items.Add(group.Title.Value);
                            strData = strData.Substring(0, strData.IndexOf("?"));
                            strUrl[i] = strData;
                            //Console.WriteLine("test2:{0}\n", strData);
                            break;
                        }
                    }
                    break;
                }
                if (i >= 9) break;
                i++;
            }
        }
    }
    private void form_Size_Changed(Object sender, EventArgs e)
    {
        notifyIcon1.Icon = YouTube.Properties.Resources.icon1;
        if (this.WindowState == FormWindowState.Minimized)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = false;
            this.Visible = false;
            notifyIcon1.Visible = true;
        }
    }

    private void notifyIcon1_DoubleClick(Object sender, EventArgs e)
    {
        this.Visible = true;
        this.ShowInTaskbar = true;
        notifyIcon1.Icon = null;
    }

    public void lbx_SelectedIndexChanged(Object sender, EventArgs e)
    {
        ListBox tmp = (ListBox)sender;
        player.DocumentText = string.Format(Html, 0, 0, strUrl[lbx.SelectedIndex]);
    }
}
