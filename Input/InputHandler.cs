using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Zen.Input
{
    public class InputHandler
    {
        #region State
        public KeyboardHandler Keyboard { get; }
        public MouseHandler Mouse { get; }
        private Dictionary<string, Dictionary<string, KeyboardInputAction>> KeyboardEventHandlers { get; }
        private Dictionary<string, Dictionary<string, MouseInputAction>> MouseEventHandlers { get; }
        private Registrar Registrar { get; }
        #endregion End State

        public InputHandler()
        {
            Keyboard = new KeyboardHandler();
            Mouse = new MouseHandler();

            KeyboardEventHandlers = new Dictionary<string, Dictionary<string, KeyboardInputAction>>();
            MouseEventHandlers = new Dictionary<string, Dictionary<string, MouseInputAction>>();

            Registrar = new Registrar(this);
        }

        /// <summary>
        /// Is the left mouse button currently down on this frame?
        /// </summary>
        public bool IsLeftMouseButtonDown => Mouse.IsLeftButtonDown();

        /// <summary>
        /// Has the left mouse button changed from up to down between this frame and the previous one?
        /// </summary>
        public bool IsLeftMouseButtonPressed => Mouse.IsLeftButtonPressed();

        /// <summary>
        /// Has the left mouse button changed from down to up between this frame and the previous one?
        /// </summary>
        public bool IsLeftMouseButtonReleased => Mouse.IsLeftButtonReleased();

        /// <summary>
        /// Is the middle mouse button currently down on this frame?
        /// </summary>
        public bool IsMiddleMouseButtonDown => Mouse.IsMiddleButtonDown();

        /// <summary>
        /// Has the middle mouse button changed from up to down between this frame and the previous one?
        /// </summary>
        public bool IsMiddleMouseButtonPressed => Mouse.IsMiddleButtonPressed();

        /// <summary>
        /// Has the middle mouse button changed from down to up between this frame and the previous one?
        /// </summary>
        public bool IsMiddleMouseButtonReleased => Mouse.IsMiddleButtonReleased();

        /// <summary>
        /// Is the right mouse button currently down on this frame?
        /// </summary>
        public bool IsRightMouseButtonDown => Mouse.IsRightButtonDown();

        /// <summary>
        /// Has the right mouse button changed from up to down between this frame and the previous one?
        /// </summary>
        public bool IsRightMouseButtonPressed => Mouse.IsRightButtonPressed();

        /// <summary>
        /// Has the right mouse button changed from down to up between this frame and the previous one?
        /// </summary>
        public bool IsRightMouseButtonReleased => Mouse.IsRightButtonReleased();

        /// <summary>
        /// Position of mouse on this frame.
        /// </summary>
        public Point MousePosition => Mouse.Location;

        /// <summary>
        /// Position of mouse on previous frame.
        /// </summary>
        public Point PreviousMousePosition => Mouse.PreviousLocation;

        /// <summary>
        /// Movement of mouse between this frame and the previous one.
        /// </summary>
        public Point MouseMovement => Mouse.Movement;

        /// <summary>
        /// Has the mouse moved between this frame and the previous one?
        /// </summary>
        public bool MouseHasMoved => Mouse.HasMouseMoved();

        /// <summary>
        /// Has any key been pressed on this frame?
        /// </summary>
        public bool KeyPressed => Keyboard.Keys.Length > 0;

        public void Update(object state, float deltaTime)
        {
            Keyboard.Update(KeyboardEventHandlers, state, deltaTime);
            Mouse.Update(MouseEventHandlers, state, deltaTime);
        }

        // for testing
        public void SetMousePosition(Point pos)
        {
            Mouse.SetMousePosition(pos);
        }

        public void BeginRegistration(string gameStatus, string owner)
        {
            Registrar.BeginRegistration(new List<string> { gameStatus }, owner);
        }

        public void BeginRegistration(List<string> gameStatuses, string owner)
        {
            Registrar.BeginRegistration(gameStatuses, owner);
        }


        public void Register(int id, object sender, Keys keys, KeyboardInputActionType inputActionType, Action<object, KeyboardEventArgs> action)
        {
            Registrar.Register(id, sender, keys, inputActionType, action);
        }

        public void Register(int id, object sender, MouseInputActionType inputActionType, Action<object, MouseEventArgs> action)
        {
            Registrar.Register(id, sender, inputActionType, action);
        }

        public void EndRegistration()
        {
            Registrar.EndRegistration();
        }

        public void Subscribe(string gameStatus, string owner)
        {
            Registrar.Subscribe(gameStatus, owner);
        }

        public void Unsubscribe(string gameStatus, string owner)
        {
            Registrar.Unsubscribe(gameStatus, owner);
        }

        public void UnsubscribeAllFromEventHandler(string owner)
        {
            foreach (var keyboardEventHandler in KeyboardEventHandlers)
            {
                //var eventHandlers = _keyboardEventHandlers[keyboardEventHandler.Key];
                foreach (var key in keyboardEventHandler.Value.Keys)
                {
                    if (key.StartsWith($"{owner}"))
                    {
                        keyboardEventHandler.Value.Remove(key);
                    }
                }
            }

            foreach (var mouseEventHandler in MouseEventHandlers)
            {
                //var eventHandlers = _mouseEventHandlers[mouseEventHandler.Key];
                foreach (var key in mouseEventHandler.Value.Keys)
                {
                    if (key.StartsWith($"{owner}"))
                    {
                        mouseEventHandler.Value.Remove(key);
                    }
                }
            }
        }

        internal void SubscribeToEventHandler(string owner, int id, object sender, Keys key, KeyboardInputActionType inputActionType, Action<object, KeyboardEventArgs> action)
        {
            var keyboardInputAction = new KeyboardInputAction(sender, key, inputActionType, action);

            var firstKey = BuildKeyOne(id, inputActionType);
            var secondKey = BuildKeyTwo(owner, id);
            if (!KeyboardEventHandlers.ContainsKey(firstKey))
            {
                KeyboardEventHandlers.Add(firstKey, new Dictionary<string, KeyboardInputAction>());
            }

            KeyboardEventHandlers[firstKey].Add(secondKey, keyboardInputAction);
        }

        internal void SubscribeToEventHandler(string owner, int id, object sender, MouseInputActionType inputActionType, Action<object, MouseEventArgs> action)
        {
            var mouseInputAction = new MouseInputAction(sender, inputActionType, action);

            var firstKey = BuildKeyOne(id, inputActionType);
            var secondKey = BuildKeyTwo(owner, id);
            if (!MouseEventHandlers.ContainsKey(firstKey))
            {
                MouseEventHandlers.Add(firstKey, new Dictionary<string, MouseInputAction>());
            }

            MouseEventHandlers[firstKey].Add(secondKey, mouseInputAction);
        }

        internal void UnsubscribeFromEventHandler(string owner, int id, KeyboardInputActionType inputActionType)
        {
            var firstKey = BuildKeyOne(id, inputActionType);
            var secondKey = BuildKeyTwo(owner, id);
            var eventHandlers = KeyboardEventHandlers[firstKey];
            eventHandlers.Remove(secondKey);
        }

        internal void UnsubscribeFromEventHandler(string owner, int id, MouseInputActionType inputActionType)
        {
            var firstKey = BuildKeyOne(id, inputActionType);
            var secondKey = BuildKeyTwo(owner, id);
            var eventHandlers = MouseEventHandlers[firstKey];
            eventHandlers.Remove(secondKey);
        }

        private string BuildKeyOne(int id, KeyboardInputActionType inputActionType)
        {
            var key = $"Keyboard.{id}.{inputActionType}";

            return key;
        }

        private string BuildKeyOne(int id, MouseInputActionType inputActionType)
        {
            var key = $"Mouse.{id}.{inputActionType}";

            return key;
        }

        private string BuildKeyTwo(string owner, int id)
        {
            var key = $"{owner}.{id}";

            return key;
        }
    }
}