/*
    Copyright (c) 2007 - 2010, Carlos Guzmán Álvarez

    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, 
    are permitted provided that the following conditions are met:

        * Redistributions of source code must retain the above copyright notice, 
          this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, 
          this list of conditions and the following disclaimer in the documentation and/or 
          other materials provided with the distribution.
        * Neither the name of the author nor the names of its contributors may be used to endorse or 
          promote products derived from this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
    CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
    EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
    PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace BabelIm.Net.Xmpp.InstantMessaging
{
    /// <summary>
    /// Property Extension Methods
    /// </summary>
    /// <remarks>
    /// http://reyntjes.blogspot.com/2009/04/master-detail-viewmodel_24.html
    /// http://blogs.ugidotnet.org/bmatte/archive/2008/11/28/pattern-model-view-viewmodel-inotifypropertychanged-static-reflection-e-extension-methods.aspx
    /// </remarks>
    public static class PropertyExtensions
    {
        #region · Extension Methods ·

        /// <summary>
        /// Creates a <see cref="PropertyChangedEventArgs" /> instance for a given property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static PropertyChangedEventArgs CreateChangeEventArgs<T>(this Expression<Func<T>> property)
        {
            var expression = property.Body as MemberExpression;
            var member = expression.Member;

            return new PropertyChangedEventArgs(member.Name);
        }

        /// <summary>
        /// Returns property name from expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetPropertyName<T>(this Expression<Func<T>> property)
        {
            var expression = property.Body as MemberExpression;
            
            return expression.Member.Name;
        }

        /// <summary>
        /// Return property name from expression.
        /// </summary>
        /// <example>
        /// <![CDATA[
        ///     Expression<Func<Item, object>> expression = i => i.Name;
        ///     var propertyName = expression.GetPropertyName(); // propertyName = "Name"
        /// ]]>
        /// </example>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPropertyName<T, TValue>(this Expression<Func<T, TValue>> expression)
        {
            var lambda = expression as LambdaExpression;

            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }

            return null;
        }

        #endregion
    }
}
