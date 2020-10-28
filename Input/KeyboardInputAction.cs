using System;
using Microsoft.Xna.Framework.Input;

namespace Zen.Input
{
    public readonly struct KeyboardInputAction
    {
        private object Sender { get; }
        public Keys Key { get; }
        public KeyboardInputActionType InputActionType { get; }
        private Action<object, KeyboardEventArgs> ActionEvent { get; }

        public KeyboardInputAction(object sender, Keys key, KeyboardInputActionType inputActionType, Action<object, KeyboardEventArgs> action)
        {
            Sender = sender;
            Key = key;
            InputActionType = inputActionType;
            ActionEvent = action;
        }

        public void Invoke(KeyboardHandler keyboard, object state, float deltaTime)
        {
            ActionEvent(Sender, new KeyboardEventArgs(keyboard, Key, state, deltaTime));
        }
    }
}