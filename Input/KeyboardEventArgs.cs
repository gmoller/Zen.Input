using System;
using Microsoft.Xna.Framework.Input;

namespace Zen.Input
{
    public class KeyboardEventArgs : EventArgs
    {
        public KeyboardHandler Keyboard { get; }
        public object State { get; }
        public Keys Key { get; }
        public float DeltaTime { get; }

        public KeyboardEventArgs(KeyboardHandler keyboard, Keys key, object state, float deltaTime)
        {
            Keyboard = keyboard;
            Key = key;
            State = state;
            DeltaTime = deltaTime;
        }
    }
}