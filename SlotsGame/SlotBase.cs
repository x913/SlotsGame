using Android.App;
using Android.Widget;
using System;
using System.Threading;

namespace SlotsGame
{
    abstract class SlotBase
    {
        public delegate void SlotStops(int rolledItemIndex);
        public event SlotStops OnSlotStops;

        public int[] SlotImages = new int[] {
            Resource.Drawable.img1,
            Resource.Drawable.img2,
            Resource.Drawable.img3,
            Resource.Drawable.img4,
            Resource.Drawable.img5,
            Resource.Drawable.img6,
        };
        public bool Enabled = true;
        private ImageView _imageView;
        private Timer _timer;
        private int _timesToRoll = 0;
        private int _rollCounter = 0;
        private Activity _activity;
        private Random _random = new Random((int)DateTime.Now.Ticks);
        private int _valueToRoll = -1;
        public int LastItem { get; private set; }

        public SlotBase(Activity activity, ImageView imageView, int timesToRoll)
        {
            _imageView = imageView;
            _activity = activity;
            _timesToRoll = timesToRoll;
        }

        private void OnTimerTick()
        {
            if(Enabled)
            {
                LastItem = _random.Next(0, SlotImages.Length);
                if(_rollCounter >= _timesToRoll)
                {
                    if (_valueToRoll != -1)
                        LastItem = _valueToRoll;

                    Enabled = false;
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    _rollCounter = 0;
                    if (OnSlotStops != null)
                        OnSlotStops(LastItem);
                }
                _activity.RunOnUiThread(() => _imageView.SetImageResource(SlotImages[LastItem]));
            }
            _rollCounter++;
        }

        public void Roll(int valueToRoll = -1)
        {
            Enabled = true;
            _rollCounter = 0;
            _valueToRoll = valueToRoll;
            _timer = new Timer(_ => OnTimerTick(), null, 0, 100);
        }

    }


}

