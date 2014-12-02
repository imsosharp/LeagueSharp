using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;

namespace Ultimate_Carry_Prevolution
{
	class Program
	{

		

		private static Sprite _xSprite;
		private static Texture _xTexture;
		private static bool _donateclicked;

		// ReSharper disable once UnusedParameter.Local
		static void Main(string[] args)
		{
			Events.Game.OnGameStart += OnGameStart;

			_xSprite = new Sprite(Drawing.Direct3DDevice);
			_xTexture = Texture.FromMemory(
		   Drawing.Direct3DDevice,
		   (byte[])new ImageConverter().ConvertTo(GetImageFromUrl("http://goo.gl/ElubcM"), typeof(byte[])), 266, 38, 0,
		   Usage.None, Format.A1, Pool.Managed, Filter.Default, Filter.Default, 0);
			
			Game.OnWndProc += Game_OnWndProc;
			Drawing.OnPreReset += DrawOnPreReset;
			Drawing.OnPostReset += DrawOnPostReset;
			AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
			AppDomain.CurrentDomain.ProcessExit += OnDomainUnload;
			
		}

		private static void Game_OnWndProc(WndEventArgs args)
		{
			if((args.Msg == (uint)WindowsMessages.WM_KEYUP || args.Msg == (uint)WindowsMessages.WM_KEYDOWN) && args.WParam == 32 && !_donateclicked)
			{
				_donateclicked = true;
				Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=SHPHSQ2LNX8BE");
			}
		}
		private static Image GetImageFromUrl(string url)
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.111 Safari/537.36";
			httpWebRequest.Headers[HttpRequestHeader.AcceptEncoding] = "*/*";
			httpWebRequest.Headers[HttpRequestHeader.AcceptLanguage] = "de-de,de;q=0.8,en-us;q=0.5,en;q=0.3";
			httpWebRequest.Headers[HttpRequestHeader.AcceptCharset] = "ISO-8859-1,utf-8;q=0.7,*;q=0.7";
			var httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
			var stream = httpWebReponse.GetResponseStream();
			// ReSharper disable once AssignNullToNotNullAttribute
			return Image.FromStream(stream);
		}
		private static void OnDomainUnload(object sender, EventArgs e)
		{
			_xSprite.Dispose();
		}

		private static void DrawOnPostReset(EventArgs args)
		{
			_xSprite.OnResetDevice();
		}

		private static void DrawOnPreReset(EventArgs args)
		{
			_xSprite.OnLostDevice();
		}

		private static void OnGameStart(EventArgs args)
		{
			try
			{
				Game.OnWndProc -= Game_OnWndProc;
				Drawing.OnPreReset -= DrawOnPreReset;
				Drawing.OnPostReset -= DrawOnPostReset;
				AppDomain.CurrentDomain.DomainUnload -= OnDomainUnload;
				AppDomain.CurrentDomain.ProcessExit -= OnDomainUnload;
				_xSprite.Dispose();
			}
			catch(Exception)
			{

			}
		}
	}
}
