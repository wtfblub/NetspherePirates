using System;

namespace Netsphere.Server.Game
{
    public class EquipValidatorHookEventArgs : EventArgs
    {
        public Player Player => Character.CharacterManager.Player;
        public Character Character { get; }
        public Room Room => Player.Room;
        public bool? Result { get; set; }

        public EquipValidatorHookEventArgs(Character character)
        {
            Character = character;
        }
    }
}
