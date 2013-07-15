//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or   
//  (at your option) version 3.                                         

//  This program is distributed in the hope that it will be useful,     
//  but WITHOUT ANY WARRANTY; without even the implied warranty of      
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       
//  GNU General Public License for more details.                        

//  You should have received a copy of the GNU General Public License   
//  along with this program; if not, write to the                       
//  Free Software Foundation, Inc.,                                     
//  51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    /// <summary>
    /// Qt
    /// </summary>
    public static class Qt
    {
        /// <summary>
        /// qmeta type
        /// </summary>
        public class QMetaType 
        {
            /// <summary>
            /// Type
            /// </summary>
            public enum Type
            { 
                /// <summary>
                /// Constant void.
                /// </summary>
                Void = 0,
                /// <summary>
                /// Constant bool.
                /// </summary>
                Bool = 1,
                /// <summary>
                /// Constant int.
                /// </summary>
                Int = 2,
                /// <summary>
                /// Constant U int.
                /// </summary>
                UInt = 3,
                /// <summary>
                /// Constant long long.
                /// </summary>
                LongLong = 4,
                /// <summary>
                /// Constant double.
                /// </summary>
                Double = 6,
                /// <summary>
                /// Constant Q char.
                /// </summary>
                QChar = 7,
                /// <summary>
                /// Constant Q variant map.
                /// </summary>
                QVariantMap = 8,
                /// <summary>
                /// Constant Q variant list.
                /// </summary>
                QVariantList = 9,
                /// <summary>
                /// Constant Q string.
                /// </summary>
                QString = 10,
                /// <summary>
                /// Constant Q string list.
                /// </summary>
                QStringList = 11,
                /// <summary>
                /// Constant Q byte array.
                /// </summary>
                QByteArray = 12,
                /// <summary>
                /// Constant Q bit array.
                /// </summary>
                QBitArray = 13,
                /// <summary>
                /// Constant Q date.
                /// </summary>
                QDate = 14,
                /// <summary>
                /// Constant Q time.
                /// </summary>
                QTime = 15,
                /// <summary>
                /// Constant Q date time.
                /// </summary>
                QDateTime = 16,
                /// <summary>
                /// Constant Q URL.
                /// </summary>
                QUrl = 17,
                /// <summary>
                /// Constant Q locale.
                /// </summary>
                QLocale = 18,
                /// <summary>
                /// Constant Q rect.
                /// </summary>
                QRect = 19,
                /// <summary>
                /// Constant Q rect f.
                /// </summary>
                QRectF = 20,
                /// <summary>
                /// Constant Q size.
                /// </summary>
                QSize = 21,
                /// <summary>
                /// Constant Q line.
                /// </summary>
                QLine = 23,
                /// <summary>
                /// Constant Q point.
                /// </summary>
                QPoint = 25,
                /// <summary>
                /// Constant Q reg exp.
                /// </summary>
                QRegExp = 27,
                /// <summary>
                /// Constant Q variant hash.
                /// </summary>
                QVariantHash = 28,
                /// <summary>
                /// Constant Q easing curve.
                /// </summary>
                QEasingCurve = 29,
                /// <summary>
                /// Constant last core type.
                /// </summary>
                LastCoreType = 29,
                /// <summary>
                /// Constant first GUI type.
                /// </summary>
                FirstGuiType = 63,
                /// <summary>
                /// Constant Q color group.
                /// </summary>
                QColorGroup = 63,
                /// <summary>
                /// Constant Q font.
                /// </summary>
                QFont = 64,
                /// <summary>
                /// Constant Q pixmap.
                /// </summary>
                QPixmap = 65,
                /// <summary>
                /// Constant Q brush.
                /// </summary>
                QBrush = 66,
                /// <summary>
                /// Constant Q color.
                /// </summary>
                QColor = 67,
            }
            
        }

        /// <summary>
        /// QT variant for compatibility with quassel
        /// </summary>
        public class QVariant
        {
            private string data;
            private QVariantType type = QVariantType.Invalid;
            private string userTypeName = null;
            
            /// <summary>
            /// Initializes a new instance of the <see cref="Client.Qt.QVariant"/> class.
            /// </summary>
            /// <param name='VariantData'>
            /// Variant data.
            /// </param>
            /// <param name='userType'>
            /// User type.
            /// </param>
            public QVariant(string VariantData, string userType)
            {
                data = VariantData;
                userTypeName = userType;
                type = QVariantType.UserType;
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="Client.Qt.QVariant"/> class.
            /// </summary>
            /// <param name='VariantData'>
            /// Variant data.
            /// </param>
            /// <param name='VariantType'>
            /// Variant type.
            /// </param>
            public QVariant(string VariantData, QVariantType VariantType)
            {
                type = VariantType;
                data = VariantData;
            }
            
            /// <summary>
            /// Gets the type.
            /// </summary>
            /// <returns>
            /// The type.
            /// </returns>
            public QVariantType getType()
            {
                return type;
            }
            
            /// <summary>
            /// Gets the data.
            /// </summary>
            /// <returns>
            /// The data.
            /// </returns>
            public string getData()
            {
                return data;
            }
            
            /// <summary>
            /// Ises the valid.
            /// </summary>
            /// <returns>
            /// The valid.
            /// </returns>
            public bool isValid()
            {
                if (type == QVariantType.Invalid || data == null)
                {
                    return false;
                }
                return true;
            }
            
            /// <summary>
            /// Q variant serializer.
            /// </summary>
            public static class QVariantSerializer
            { 
                
            }
            
            /// <summary>
            /// Gets the name of the user type.
            /// </summary>
            /// <returns>
            /// The user type name.
            /// </returns>
            public string getUserTypeName()
            {
                return userTypeName;
            }
            
            /// <summary>
            /// Returns a <see cref="System.String"/> that represents the current <see cref="Client.Qt.QVariant"/>.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String"/> that represents the current <see cref="Client.Qt.QVariant"/>.
            /// </returns>
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
        
        /// <summary>
        /// Q variant type.
        /// </summary>
        public enum QVariantType
        {
            /// <summary>
            /// Constant byte array.
            /// </summary>
            ByteArray,
            /// <summary>
            /// Constant string.
            /// </summary>
            String,
            /// <summary>
            /// Constant C string.
            /// </summary>
            CString,
            /// <summary>
            /// Constant U int.
            /// </summary>
            UInt,
            /// <summary>
            /// Constant int.
            /// </summary>
            Int,
            /// <summary>
            /// Constant bool.
            /// </summary>
            Bool,
            /// <summary>
            /// Constant map.
            /// </summary>
            Map,
            /// <summary>
            /// Constant list.
            /// </summary>
            List,
            /// <summary>
            /// Constant user type.
            /// </summary>
            UserType,
            /// <summary>
            /// Constant invalid.
            /// </summary>
            Invalid,
        }
    }
}
