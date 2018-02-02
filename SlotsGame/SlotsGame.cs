using System;
using System.Collections.Generic;

namespace SlotsGame
{
    class SlotsGame
    {

        public delegate void SlotsStopedRolling(bool isWin, bool isGameOver, int playerBalance);
        public event SlotsStopedRolling OnSlotsStopedRolling;

        //public bool IsPlayerWin { get; private set; }
        private Random _rnd = new Random((int)DateTime.Now.Ticks);
        private int slotsStoppedCounter;
        private int lastRolledIndex;
        private int matchedSlots;
        private Slot[] _slots;
        public bool isGameOver { get; private set; }

        public int Cash { get; set; }
        public int CurrentBet { get; private set; }

        public SlotsGame(Slot[] slots, int cash)
        {
            _slots = slots;
            foreach (var slot in _slots)
                slot.OnSlotStops += OnSlotStopped;
            Cash = cash;
        }

        private void OnSlotStopped(int rolledItemIndex)
        {
            if (lastRolledIndex == -1)
            {
                lastRolledIndex = rolledItemIndex;
            }
            else
            {
                if (lastRolledIndex == rolledItemIndex)
                    matchedSlots++;

                lastRolledIndex = rolledItemIndex;
            }

            slotsStoppedCounter++;
            if(slotsStoppedCounter >= _slots.Length)
            {
                var isWin = matchedSlots == _slots.Length - 1;
                if (!isWin)
                    CurrentBet = CurrentBet * -1;

                Cash += CurrentBet;
                isGameOver = Cash <= 0;

                if (OnSlotsStopedRolling != null)
                    OnSlotsStopedRolling(isWin, isGameOver, Cash);

                //System.Diagnostics.Debug.WriteLine($"ALL DONE {matchedSlots} / {_slots.Length - 1}");
            }
            
        }

        public void Roll(int bet)
        {
            CurrentBet = bet;
            lastRolledIndex = -1;
            slotsStoppedCounter = 0;
            matchedSlots = 0;

            var values = new Stack<int>();
            var tmp = _rnd.Next(0, 100);
            var value = _rnd.Next(0, _slots.Length);
            if (tmp % 2 == 0)
            {
                // first and last values are matches
                values.Push(value);
                values.Push(_rnd.Next(0, _slots.Length));
                values.Push(value);
            }
            else if (tmp % 3 == 0)
            {
                // last two matches
                values.Push(_rnd.Next(0, _slots.Length));
                values.Push(value);
                values.Push(value);
            }
            else if (tmp % 4 == 0)
            {
                // first two matches
                values.Push(value);
                values.Push(value);
                values.Push(_rnd.Next(0, _slots.Length));
            }
            else
            {
                values.Push(_rnd.Next(0, _slots.Length));
                values.Push(_rnd.Next(0, _slots.Length));
                values.Push(_rnd.Next(0, _slots.Length));
            }

            foreach (var slot in _slots)
            {
                slot.Roll(values.Pop());
            }
        }

 
    }


}

