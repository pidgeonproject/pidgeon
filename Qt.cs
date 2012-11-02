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

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public static class Qt
    {
        public class QMetaType 
        {
            public enum Type
            { 
                Void = 0,
                Bool = 1,
                Int = 2,
                UInt = 3,
                LongLong = 4,
                Double = 6,
                QChar = 7,
                QVariantMap = 8,
                QVariantList = 9,
                QString = 10,
                QStringList = 11,
                QByteArray = 12,
                QBitArray = 13,
                QDate = 14,
                QTime = 15,
                QDateTime = 16,
                QUrl = 17,
                QLocale = 18,
                QRect = 19,
                QRectF = 20,
                QSize = 21,
                QLine = 23,
                QPoint = 25,
                QRegExp = 27,
                QVariantHash = 28,
                QEasingCurve = 29,
                LastCoreType = 29,
                FirstGuiType = 63,
                QColorGroup = 63,
                QFont = 64,
                QPixmap = 65,
                QBrush = 66,
                QColor = 67,
            }

            int id;
            string name;

        }

        /// <summary>
        /// QT variant for compatibility with quassel
        /// </summary>
        public class QVariant
        {
            private string data;
            private QVariantType type = QVariantType.Invalid;
            private string userTypeName = null;

            public QVariant(string VariantData, string userType)
            {
                data = VariantData;
                userTypeName = userType;
                type = QVariantType.UserType;
            }

            public QVariant(string VariantData, QVariantType VariantType)
            {
                type = VariantType;
                data = VariantData;
            }

            public QVariantType getType()
            {
                return type;
            }

            public string getData()
            {
                return data;
            }

            public bool isValid()
            {
                if (type == QVariantType.Invalid || data == null)
                {
                    return false;
                }
                return true;
            }

            public static class QVariantSerializer
            { 
                
            }

            public string getUserTypeName()
            {
                return userTypeName;
            }

            public override string ToString()
            {
                switch (type)
                { 
                    case QVariantType.Bool:
                    case QVariantType.ByteArray:
                    case QVariantType.String:
                    case QVariantType.CString:
                        return data;
                    case QVariantType.UserType:
                        return userTypeName + data;
                }
                return "/" + type.ToString() + " [ " + data + " ]/";
            }
        }

        public enum QVariantType
        {
            ByteArray,
            String,
            CString,
            UInt,
            Int,
            Bool,
            Map,
            List,
            UserType,
            Invalid,
        }
    }
}
