﻿using Android.Webkit;

namespace SlotsGame
{
    public class CustomWebViewClient : WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            view.LoadUrl(url);
            return false;
        }
    }


}