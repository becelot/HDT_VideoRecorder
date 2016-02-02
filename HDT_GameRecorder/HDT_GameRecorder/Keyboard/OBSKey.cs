using Keyboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyboard
{
    public class OBSKey : Key
    {
        public enum KeyFlag
        {
            KF_ALT = 0x0400,
            KF_SHIFT = 0x0100,
            KF_CMD = 0x0200
        }
        private const uint keyIdentifier = 0xFF;

        public OBSKey() : base()
        {
            
        }

        public OBSKey(int c)
        {
            _buttonCounter = 0;
            Vk = (Messaging.VKeys)(c & keyIdentifier);
            ShiftKey = Messaging.VKeys.NULL;

            bool Alt = (c & (int)KeyFlag.KF_ALT) > 0;
            bool Cmd = (c & (int)KeyFlag.KF_CMD) > 0;
            bool Shift = (c & (int)KeyFlag.KF_SHIFT) > 0;

            if (Alt)
            {
                if (Cmd)
                {
                    if (Shift)
                    {
                        ShiftType = Messaging.ShiftType.ALT_CTRL_SHIFT;
                    }
                    else
                    {
                        ShiftType = Messaging.ShiftType.ALT_CTRL;
                    }
                }
                else
                {
                    if (Shift)
                    {
                        ShiftType = Messaging.ShiftType.ALT_SHIFT;
                    }
                    else
                    {
                        ShiftType = Messaging.ShiftType.ALT;
                    }
                }
            }
            else
            {
                if (Cmd)
                {
                    if (Shift)
                    {
                        ShiftType = Messaging.ShiftType.CTRL_SHIFT;
                    }
                    else
                    {
                        ShiftType = Messaging.ShiftType.CTRL;
                    }
                }
                else
                {
                    if (Shift)
                    {
                        ShiftType = Messaging.ShiftType.SHIFT;
                    }
                    else
                    {
                        ShiftType = Messaging.ShiftType.NONE;
                    }
                }
            }
        }
    }
}
