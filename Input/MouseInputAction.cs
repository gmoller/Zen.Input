using System;

namespace Zen.Input
{
    public readonly struct MouseInputAction
    {
        private object Sender { get; }
        public MouseInputActionType InputActionType { get; }
        private Action<object, MouseEventArgs> ActionEvent { get; }

        public MouseInputAction(object sender, MouseInputActionType inputActionType, Action<object, MouseEventArgs> action)
        {
            Sender = sender;
            InputActionType = inputActionType;
            ActionEvent = action;
        }

        public void Invoke(MouseHandler mouse, object state, float deltaTime)
        {
            ActionEvent(Sender, new MouseEventArgs(mouse, state, deltaTime));
        }
    }
}