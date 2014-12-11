using System;
using System.Linq;
using System.Collections.Generic;
using Android.Views;
using pdsharp.utils;

namespace pdsharp.input
{
    public class Keys
    {

        public const Keycode Back = Keycode.Back;
        public const Keycode Menu = Keycode.Menu;
        public const Keycode VolumeUp = Keycode.VolumeUp;
        public const Keycode VolumeDown = Keycode.VolumeDown;

        public static Signal<Key> Event = new Signal<Key>(true);

        public static void ProcessTouchEvents(List<KeyEvent> events)
        {
            foreach (var e in events.ToList())
            {
                switch (e.Action)
                {
                    case KeyEventActions.Down:
                        Event.Dispatch(new Key(e.KeyCode, true));
                        break;
                    case KeyEventActions.Multiple:
                        break;
                    case KeyEventActions.Up:
                        Event.Dispatch(new Key(e.KeyCode, false));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    public class Key
    {
        public Keycode Code { get; private set; }
        public bool Pressed { get; private set; }

        public Key(Keycode code, bool pressed)
        {
            Code = code;
            Pressed = pressed;
        }
    }
}