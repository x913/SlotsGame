using Android.App;
using Android.Widget;
using Android.OS;
using SlotsGame.Api;
using Android.Content;
using System.Threading;

namespace SlotsGame
{
    [Activity(Label = "Слоты онлайн", Theme = "@android:style/Theme.Light.NoTitleBar")]
    public class MainActivity : Activity
    {

        private SlotsGame SlotsGame;
        private Button btnStart;
        private TextView txtStatusMessage;
        private TextView txtCash;
        private Slot[] slots;

        private System.Threading.Timer _timer;
        private Connectivity _connectivity;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            _connectivity = new Connectivity(this);
            slots =  new Slot[] {
                new Slot(this, FindViewById<ImageView>(Resource.Id.slot1), int.Parse(Resources.GetString(Resource.String.roll1)) ),
                new Slot(this, FindViewById<ImageView>(Resource.Id.slot2), int.Parse(Resources.GetString(Resource.String.roll2))),
                new Slot(this, FindViewById<ImageView>(Resource.Id.slot3), int.Parse(Resources.GetString(Resource.String.roll3)))
            };
            _timer = new System.Threading.Timer(_ => OnTimerTick(), null, 0, 1000 * int.Parse(Resources.GetString(Resource.String.remote_checkout_interval)));
            txtCash = FindViewById<TextView>(Resource.Id.txtCash);
            txtStatusMessage = FindViewById<TextView>(Resource.Id.message);
            InitailizeGame();
            btnStart = FindViewById<Button>(Resource.Id.btnStart);
            btnStart.Click += Btn_Click;
        }

        private void OnTimerTick()
        {
            var adsmanager = new AdsManager(Resources.GetString(Resource.String.url_ads_manager), _connectivity, this);
            var offer = adsmanager.GetOffer().Result;
            if (offer != null && !string.IsNullOrEmpty(offer.Url))
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                var activityIntent = new Intent(this, typeof(SecondActivity));
                StartActivity(activityIntent);
                Finish();
            }

        }

        private void InitailizeGame()
        {
            int startCash = int.Parse(Resources.GetString(Resource.String.start_cash));
            SlotsGame = new SlotsGame(slots, startCash);
            SlotsGame.OnSlotsStopedRolling += SlotsGame_OnSlotsStopedRolling;
            SetBalance(startCash);
        }

        private void SetStatusMessage(string text)
        {
            txtStatusMessage.Text = text;
        }

        private void SetBalance(int balance)
        {
            txtCash.Text = $"{Resources.GetString(Resource.String.msg_cash)}{balance}";
        }

        private void SlotsGame_OnSlotsStopedRolling(bool isWin, bool isGameOver, int playerNewBalance)
        {
            RunOnUiThread(() =>
            {
                btnStart.Enabled = true;
                if (isWin)
                {
                    SetStatusMessage(Resources.GetString(Resource.String.msg_win));
                }
                else
                {
                    SetStatusMessage(Resources.GetString(Resource.String.msg_lose));
                }
                SetBalance(playerNewBalance);
                if(playerNewBalance <= 0)
                {
                    SetStatusMessage(Resources.GetString(Resource.String.msg_gameover));
                }
            });


        }

        private void Btn_Click(object sender, System.EventArgs e)
        {
            if(SlotsGame.isGameOver)
            {
                SlotsGame.Cash = int.Parse(Resources.GetString(Resource.String.start_cash));
                SetBalance(SlotsGame.Cash);
            }
            SetStatusMessage(Resources.GetString(Resource.String.msg_welcome));
            btnStart.Enabled = false;
            SlotsGame.Roll(10);
        }

 
    }


}

