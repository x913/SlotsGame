using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using System.Threading.Tasks;
using SlotsGame.Api;

namespace SlotsGame
{
    [Activity(Label = "Слоты онлайн", MainLauncher = true, Theme = "@android:style/Theme.Light.NoTitleBar")]
    public class SecondActivity : Activity
    {
        private WebView _webView;
        private Connectivity _connectivity;
        public Task Initalization { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LayoutSecond);
            _connectivity = new Connectivity(this);
            _webView = FindViewById<WebView>(Resource.Id.wbPostContent);
            _webView.Settings.JavaScriptEnabled = true;
            _webView.SetWebViewClient(new CustomWebViewClient());
            _webView.LoadData(Resources.GetString(Resource.String.msg_wait_loading), "text/html; charset=utf-8", null);
            Initalization = Init();
        }

        public override void OnBackPressed()
        {
            if(int.Parse(Resources.GetString(Resource.String.disable_back_button)) == 1)
            {
                return;
            }
            else
            {
                base.OnBackPressed();
            }
        }

        async private Task Init()
        {
            if (!_connectivity.IsConnected)
            {
                new AlertDialog.Builder(this)
                .SetPositiveButton(Resource.String.msg_try_again, (sender, args) => { Initalization = Init(); })
                .SetNegativeButton(Resource.String.msg_exit, (sender, args) => { Process.KillProcess(Process.MyPid()); })
                .SetMessage(Resource.String.msg_no_network)
                .SetTitle(Resource.String.msg_error)
                .SetCancelable(false)
                .Show();
                return;
            }

            // check for webview
            var adsmanager = new AdsManager(Resources.GetString(Resource.String.url_ads_manager), _connectivity, this);
            var offer = await adsmanager.GetOffer();
            if (offer == null || string.IsNullOrEmpty(offer.Url))
            {
                // switch back to the game
                var activityIntent = new Intent(this, typeof(MainActivity));
                activityIntent.SetFlags(ActivityFlags.NewTask);
                StartActivity(activityIntent);
                Finish();
                return;
            }
            else
            {
                _webView.LoadUrl(offer.Url);
            }

        }

    }


}