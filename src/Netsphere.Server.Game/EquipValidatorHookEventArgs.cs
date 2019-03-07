using System;

namespace Netsphere.Server.Game
{
    public class EquipValidatorHookEventArgs : EventArgs
    {
        public Character Character { get; }
        public bool? Result { get; set; }

        public EquipValidatorHookEventArgs(Character character)
        {
            Character = character;
        }
    }
}
