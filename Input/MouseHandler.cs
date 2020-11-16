using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Zen.Input
{
    public class MouseHandler
    {
        #region State
        private MouseState _currentState;
        private MouseState _previousState;
        private readonly Dictionary<MouseInputActionType, Func<bool>> _switch;
        #endregion End State

        internal MouseHandler()
        {
            _currentState = Mouse.GetState();

            _switch = new Dictionary<MouseInputActionType, Func<bool>>
            {
                { MouseInputActionType.LeftButtonDown, IsLeftButtonDown },
                { MouseInputActionType.LeftButtonPressed, IsLeftButtonPressed },
                { MouseInputActionType.LeftButtonReleased, IsLeftButtonReleased },
                { MouseInputActionType.MiddleButtonDown, IsMiddleButtonDown },
                { MouseInputActionType.MiddleButtonPressed, IsMiddleButtonPressed },
                { MouseInputActionType.MiddleButtonReleased, IsMiddleButtonReleased },
                { MouseInputActionType.RightButtonDown, IsRightButtonDown },
                { MouseInputActionType.RightButtonPressed, IsRightButtonPressed },
                { MouseInputActionType.RightButtonReleased, IsRightButtonReleased },
                { MouseInputActionType.WheelUp, MouseWheelUp },
                { MouseInputActionType.WheelDown, MouseWheelDown },
                { MouseInputActionType.LeftButtonDrag, () => IsLeftButtonDown() && HasMouseMoved() },
                { MouseInputActionType.MiddleButtonDrag, () => IsMiddleButtonDown() && HasMouseMoved() },
                { MouseInputActionType.RightButtonDrag, () => IsRightButtonDown() && HasMouseMoved() },
                { MouseInputActionType.Moved, HasMouseMoved },
                { MouseInputActionType.AtTopOfScreen, IsMouseIsAtTopOfScreen },
                { MouseInputActionType.AtBottomOfScreen, MouseIsAtBottomOfScreen },
                { MouseInputActionType.AtLeftOfScreen, MouseIsAtLeftOfScreen },
                { MouseInputActionType.AtRightOfScreen, MouseIsAtRightOfScreen },
                { MouseInputActionType.CheckForHoverOver, () => true},
            };
        }

        public Point Location => _currentState.Position;
        public Point Movement => _currentState.Position - _previousState.Position;

        internal void Update(Dictionary<string, Dictionary<string, MouseInputAction>> mouseEventHandlers, object state, float deltaTime)
        {
            _previousState = _currentState;
            _currentState = Mouse.GetState();

            HandleMouse(mouseEventHandlers, state, deltaTime);
        }

        // for testing
        public void SetMousePosition(Point pos)
        {
            Mouse.SetPosition(pos.X, pos.Y);
        }

        internal bool IsLeftButtonDown()
        {
            return _currentState.LeftButton == ButtonState.Pressed;
        }

        private bool IsLeftButtonPressed()
        {
            return _previousState.LeftButton == ButtonState.Released && _currentState.LeftButton == ButtonState.Pressed;
        }

        internal bool IsLeftButtonReleased()
        {
            return _previousState.LeftButton == ButtonState.Pressed && _currentState.LeftButton == ButtonState.Released;
        }

        private bool IsMiddleButtonDown()
        {
            return _currentState.MiddleButton == ButtonState.Pressed;
        }

        private bool IsMiddleButtonPressed()
        {
            return _previousState.MiddleButton == ButtonState.Released && _currentState.MiddleButton == ButtonState.Pressed;
        }

        private bool IsMiddleButtonReleased()
        {
            return _previousState.MiddleButton == ButtonState.Pressed && _currentState.MiddleButton == ButtonState.Released;
        }

        private bool IsRightButtonDown()
        {
            return _currentState.RightButton == ButtonState.Pressed;
        }

        private bool IsRightButtonPressed()
        {
            return _previousState.RightButton == ButtonState.Released && _currentState.RightButton == ButtonState.Pressed;
        }

        private bool IsRightButtonReleased()
        {
            return _previousState.RightButton == ButtonState.Pressed && _currentState.RightButton == ButtonState.Released;
        }

        private bool MouseWheelUp()
        {
            return _currentState.ScrollWheelValue > _previousState.ScrollWheelValue;
        }

        private bool MouseWheelDown()
        {
            return _currentState.ScrollWheelValue < _previousState.ScrollWheelValue;
        }

        internal bool HasMouseMoved()
        {
            return _previousState.Position != _currentState.Position;
        }

        public bool IsMouseIsAtTopOfScreen()
        {
            return Location.Y <= 30.0f && Location.Y >= 0.0f && Location.X >= 0.0f && Location.X <= 1680.0f;
        }

        public bool MouseIsAtBottomOfScreen()
        {
            return Location.Y >= 1080 - 30.0f && Location.Y <= 1080.0f && Location.X >= 0.0f && Location.X <= 1680.0f;
        }

        public bool MouseIsAtLeftOfScreen()
        {
            return Location.X < 30.0f && Location.X >= 0.0f && Location.Y >= 0.0f && Location.Y <= 1080.0f;
        }

        public bool MouseIsAtRightOfScreen()
        {
            return Location.X > 1680.0f - 30.0f && Location.X <= 1680.0f && Location.Y >= 0.0f && Location.Y <= 1080.0f;
        }

        private void HandleMouse(Dictionary<string, Dictionary<string, MouseInputAction>> mouseEventHandlers, object state, float deltaTime)
        {
            foreach (var item in mouseEventHandlers.Values)
            {
                foreach (var thenAction in item.Values)
                {
                    var ifFunc = _switch[thenAction.InputActionType];
                    var ifConditionSatisfied = ifFunc();

                    if (ifConditionSatisfied)
                    {
                        thenAction.Invoke(this, state, deltaTime);
                    }
                }
            }
        }
    }
}