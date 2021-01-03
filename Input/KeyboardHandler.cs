using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Zen.Input
{
    public class KeyboardHandler
    {
        #region State
        private KeyboardState _currentState;
        private KeyboardState _previousState;
        private readonly Dictionary<KeyboardInputActionType, Func<Keys, bool>> _switch;
        #endregion End State

        /// <summary>
        /// Returns an array of values holding keys that are currently being pressed.
        /// </summary>
        public Keys[] Keys => _currentState.GetPressedKeys();

        internal KeyboardHandler()
        {
            _currentState = Keyboard.GetState();

            _switch = new Dictionary<KeyboardInputActionType, Func<Keys, bool>>
            {
                { KeyboardInputActionType.Up, IsKeyUp },
                { KeyboardInputActionType.Down, IsKeyDown },
                { KeyboardInputActionType.Pressed, IsKeyPressed },
                { KeyboardInputActionType.Released, IsKeyReleased }
            };
        }

        internal void Update(Dictionary<string, Dictionary<string, KeyboardInputAction>> keyboardEventHandlers, object state, float deltaTime)
        {
            _previousState = _currentState;
            _currentState = Keyboard.GetState();

            HandleKeyboard(keyboardEventHandlers, state, deltaTime);
        }

        /// <summary>
        /// Is the key currently not being pressed?
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True if key is currently not being pressed.</returns>
        private bool IsKeyUp(Keys key)
        {
            return _currentState.IsKeyUp(key);
        }

        /// <summary>
        /// Is the key currently being pressed?
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True if key is currently being pressed.</returns>
        public bool IsKeyDown(Keys key)
        {
            return _currentState.IsKeyDown(key);
        }

        /// <summary>
        /// Has the key just been pressed?
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True if key has just been pressed (between this frame and last frame).</returns>
        private bool IsKeyPressed(Keys key)
        {
            return _previousState.IsKeyUp(key) && _currentState.IsKeyDown(key);
        }

        /// <summary>
        /// Has the key just been released?
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True if key has just been released (between this frame and last frame).</returns>
        internal bool IsKeyReleased(Keys key)
        {
            return _previousState.IsKeyDown(key) && _currentState.IsKeyUp(key);
        }

        private void HandleKeyboard(Dictionary<string, Dictionary<string, KeyboardInputAction>> keyboardEventHandlers, object state, float deltaTime)
        {
            foreach (var item in keyboardEventHandlers.Values)
            {
                foreach (var thenAction in item.Values)
                {
                    var ifFunc = _switch[thenAction.InputActionType];
                    var ifConditionSatisfied = ifFunc(thenAction.Key);

                    if (ifConditionSatisfied)
                    {
                        thenAction.Invoke(this, state, deltaTime);
                    }
                }
            }
        }
    }
}