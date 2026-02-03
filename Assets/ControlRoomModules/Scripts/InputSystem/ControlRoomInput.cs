using UnityEngine;

namespace ControlRoom
{
    public static class ControlRoomInput
    {
        public static IInputHelper InputHelper;

        public static bool DisableInput
        {
            get
            {
                if (InputHelper != null)
                {
                    return InputHelper.DisableInput;
                }
                return false;
            }
            set
            {
                if (InputHelper != null)
                {
                    InputHelper.DisableInput = value;
                }
            }
        }
    }
}
