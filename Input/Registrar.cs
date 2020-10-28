using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Zen.Input
{
    internal class Registrar
    {
        #region State
        private bool RegistrationBegun { get; set; }
        private List<string> GameStatuses { get; set; }
        private string Owner { get; set; }
        private InputHandler Input { get; }

        private Dictionary<string, (int id, object sender, Keys keys, KeyboardInputActionType inputActionType, Action<object, KeyboardEventArgs> action)> KeyboardEventHandlersRepository { get; }
        private Dictionary<string, (int id, object sender, MouseInputActionType inputActionType, Action<object, MouseEventArgs> action)> MouseEventHandlersRepository { get; }
        #endregion End State

        internal Registrar(InputHandler input)
        {
            GameStatuses = new List<string>();
            Owner = string.Empty;
            KeyboardEventHandlersRepository = new Dictionary<string, (int id, object sender, Keys keys, KeyboardInputActionType inputActionType, Action<object, KeyboardEventArgs> action)>();
            MouseEventHandlersRepository = new Dictionary<string, (int id, object sender, MouseInputActionType inputActionType, Action<object, MouseEventArgs> action)>();

            Input = input;
        }

        internal void BeginRegistration(List<string> gameStatuses, string owner)
        {
            if (RegistrationBegun) throw new Exception("Registration already begun.");

            RegistrationBegun = true;
            GameStatuses = gameStatuses;
            Owner = owner;
        }

        internal void Register(int id, object sender, Keys keys, KeyboardInputActionType inputActionType, Action<object, KeyboardEventArgs> action)
        {
            foreach (var gameStatus in GameStatuses)
            {
                var key = $"{gameStatus}.{Owner}.{id}";
                KeyboardEventHandlersRepository.Add(key, (id, sender, keys, inputActionType, action));
            }
        }

        internal void Register(int id, object sender, MouseInputActionType inputActionType, Action<object, MouseEventArgs> action)
        {
            foreach (var gameStatus in GameStatuses)
            {
                var key = $"{gameStatus}.{Owner}.{id}";
                MouseEventHandlersRepository.Add(key, (id, sender, inputActionType, action));
            }
        }

        internal void EndRegistration()
        {
            if (!RegistrationBegun) throw new Exception("Registration has not begun.");

            RegistrationBegun = false;
            GameStatuses.Clear();
            Owner = string.Empty;
        }

        internal void Subscribe(string gameStatus, string owner)
        {
            foreach (var item in KeyboardEventHandlersRepository)
            {
                if (item.Key.StartsWith($"{gameStatus}.{owner}."))
                {
                    Input.SubscribeToEventHandler(owner, item.Value.id, item.Value.sender, item.Value.keys, item.Value.inputActionType, item.Value.action);
                }
            }

            foreach (var item in MouseEventHandlersRepository)
            {
                if (item.Key.StartsWith($"{gameStatus}.{owner}."))
                {
                    Input.SubscribeToEventHandler(owner, item.Value.id, item.Value.sender, item.Value.inputActionType, item.Value.action);
                }
            }
        }

        internal void Unsubscribe(string gameStatus, string owner)
        {
            foreach (var item in KeyboardEventHandlersRepository)
            {
                if (item.Key.StartsWith($"{gameStatus}.{owner}."))
                {
                    Input.UnsubscribeFromEventHandler(owner, item.Value.id, item.Value.inputActionType);
                }
            }

            foreach (var item in MouseEventHandlersRepository)
            {
                if (item.Key.StartsWith($"{gameStatus}.{owner}."))
                {
                    Input.UnsubscribeFromEventHandler(owner, item.Value.id, item.Value.inputActionType);
                }
            }
        }
    }
}