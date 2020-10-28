using System;

namespace Zen.Input
{
    public class MouseEventArgs : EventArgs
    {
        public MouseHandler Mouse { get; }
        public object State { get; }
        public float DeltaTime { get; }

        public MouseEventArgs(MouseHandler mouse, object state, float deltaTime)
        {
            Mouse = mouse;
            State = state;
            DeltaTime = deltaTime;
        }
    }
}