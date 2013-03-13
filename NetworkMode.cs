/***************************************************************************
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) version 3.                                           *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   along with this program; if not, write to the                         *
 *   Free Software Foundation, Inc.,                                       *
 *   51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.         *
 ***************************************************************************/
// Mode

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    [Serializable]
    public class NetworkMode
    {
        /// <summary>
        /// Raw mode
        /// </summary>
        public List<string> _Mode = new List<string>();
        /// <summary>
        /// Optional parameters for each mode
        /// </summary>
        public string parameter = null;
        /// <summary>
        /// Network associated with mode
        /// </summary>
        public Network network = null;
        /// <summary>
        /// Type
        /// </summary>
        public ModeType _ModeType = ModeType.Network;

        public enum ModeType
        { 
            Channel,
            User,
            Network
        }

        public override string ToString()
        {
            string _val = "";
            int curr = 0;
            lock (_Mode)
            {
                while (curr < _Mode.Count)
                {
                    _val += _Mode[curr];
                    curr++;
                }
            }
            return "+" + _val;
        }

        public NetworkMode(ModeType MT, Network _Network)
        {
            _ModeType = MT;
            network = _Network;
        }

        public NetworkMode(string DefaultMode)
        {
            mode(DefaultMode);
        }

        public NetworkMode()
        {
            network = null;
        }

        /// <summary>
        /// Change mode
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool mode(string text)
        {
            char prefix = ' ';
            foreach (char _x in text)
            {
                if (network != null && _ModeType != ModeType.Network)
                {
                    switch (_ModeType)
                    {
                        case ModeType.User:
                            if (network.CModes.Contains(_x))
                            {
                                continue;
                            }
                            break;
                        case ModeType.Channel:
                            if (network.CUModes.Contains(_x) || network.PModes.Contains(_x))
                            {
                                continue;
                            }
                            break;
                    }
                }
                if (_x == ' ')
                {
                    return true;
                }
                if (_x == '-')
                {
                    prefix = _x;
                    continue;
                }
                if (_x == '+')
                {
                    prefix = _x;
                    continue;
                }
                switch (prefix)
                {
                    case '+':
                        if (!_Mode.Contains(_x.ToString()))
                        {
                            this._Mode.Add(_x.ToString());
                        }
                        continue;
                    case '-':
                        if (_Mode.Contains(_x.ToString()))
                        {
                            this._Mode.Remove(_x.ToString());
                        }
                        continue;
                } continue;
            }
            return false;
        }
    }
}
