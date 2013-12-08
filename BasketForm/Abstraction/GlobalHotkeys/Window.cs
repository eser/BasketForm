// -----------------------------------------------------------------------
// <copyright file="Window.cs" company="-">
// Copyright (c) 2013 larukedi (eser@sent.com). All rights reserved.
// </copyright>
// <author>larukedi (http://github.com/larukedi/)</author>
// -----------------------------------------------------------------------

//// This program is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 3 of the License, or
//// (at your option) any later version.
//// 
//// This program is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//// GNU General Public License for more details.
////
//// You should have received a copy of the GNU General Public License
//// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace BasketForm.Abstraction.GlobalHotkeys
{
    using System;
    using System.Windows.Forms;

    public class Window : NativeWindow, IDisposable
    {
        private const int WM_HOTKEY = 0x0312;

        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        public Window()
        {
            // create the handle for the window.
            this.CreateHandle(new CreateParams());
        }

        public void Dispose()
        {
            this.DestroyHandle();
        }

        /// <summary>
        /// Overridden to get the notifications.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // check if we got a hot key pressed.
            if (m.Msg == WM_HOTKEY)
            {
                // get the keys.
                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                // invoke the event to notify the parent.
                if (this.KeyPressed != null)
                {
                    this.KeyPressed(this, new KeyPressedEventArgs(modifier, key));
                }
            }
        }
    }
}
