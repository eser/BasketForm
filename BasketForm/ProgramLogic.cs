// -----------------------------------------------------------------------
// <copyright file="ProgramLogic.cs" company="-">
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

namespace BasketForm
{
    using System;
    using System.IO;
    using BasketForm.Abstraction;
    using BasketForm.Abstraction.Config;

    public class ProgramLogic
    {
        public const string ConfigFilename = "config.json";

        public ProgramLogic()
        {
            ProgramLogic.Instance = this;

            // config file
            string configFile = Path.Combine(Environment.CurrentDirectory, ProgramLogic.ConfigFilename);

            if (File.Exists(configFile))
            {
                Stream fileStream = File.OpenRead(configFile);
                this.Config = ConfigSerializer.Load<ProgramConfig>(fileStream);
            }
            else
            {
                this.Config = new ProgramConfig();
                ConfigSerializer.Reset(this.Config);
                ConfigSerializer.Save(File.OpenWrite(configFile), this.Config);
            }

            this.MainForm = new frmMain();
        }

        public static ProgramLogic Instance { get; private set; }
        public frmMain MainForm { get; private set; }
        public ProgramConfig Config { get; private set; }
    }
}
