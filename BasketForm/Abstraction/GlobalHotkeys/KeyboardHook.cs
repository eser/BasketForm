// -----------------------------------------------------------------------
// <copyright file="KeyboardHook.cs" company="-">
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

//// Original code taken from Pooyan Fekrati's MSDN answers
//// http://social.msdn.microsoft.com/Forums/vstudio/en-US/c061954b-19bf-463b-a57d-b09c98a3fe7d/assign-global-hotkey-to-a-system-tray-application-in-c?forum=csharpgeneral

namespace BasketForm.Abstraction.GlobalHotkeys
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class KeyboardHook : IDisposable
    {
        // Registers a hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        public KeyboardHook()
        {
            this.Window = new Window();
            this.Window.KeyPressed += this.Window_KeyPressed;
        }

        public Window Window { get; private set; }
        public int CurrentId { get; private set; }

        /// <summary>
        /// Registers a hot key in the system.
        /// </summary>
        /// <param name="modifier">The modifiers that are associated with the hot key.</param>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public void RegisterHotKey(ModifierKeys modifier, Keys key)
        {
            // register the hot key.
            if (!KeyboardHook.RegisterHotKey(this.Window.Handle, ++this.CurrentId, (uint)modifier, (uint)key))
            {
                throw new InvalidOperationException("Couldn’t register the hot key.");
            }
        }

        public void Dispose()
        {
            // unregister all the registered hot keys.
            for (int i = this.CurrentId; i > 0; i--)
            {
                KeyboardHook.UnregisterHotKey(this.Window.Handle, i);
            }

            // dispose the inner native window.
            this.Window.Dispose();
        }

        protected void Window_KeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (this.KeyPressed != null)
            {
                this.KeyPressed(this, e);
            }
        }
    }
}
