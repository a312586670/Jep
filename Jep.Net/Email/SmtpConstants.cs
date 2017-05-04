//
// SmtPop	SMTP and POP library
//
// Copyright (C) 2004-2005 sillycoder	sillycoder@users.sourceforge.net 
//
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
// 

namespace Jep.Net.Email 
{

    internal class SMTPConstants
    {
        public static readonly string SYSTEM_READY = "220";
        public static readonly string QUIT = "221";
        public static readonly string OK = "250";
        public static readonly string START_DATA = "354";
        public static readonly string AUTH = "334";
        public static readonly string AUTH_SUCCESSFUL = "235";
    }
}