// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic.Attributes
{
    public class SimplePropertyInfo : PropertyInfo
    {
        private readonly string _name;
        private readonly Type _propertyType;

        public SimplePropertyInfo(string name, Type propertyType)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (propertyType == null)
                throw new ArgumentNullException("propertyType");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name");

            _name = name;
            _propertyType = propertyType;
        }

        public override string Name
        {
            get
            {
                return _name;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return _propertyType;
            }
        }

        public override PropertyAttributes Attributes
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanRead
        {
            get { throw new NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get { throw new NotImplementedException(); }
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override Type DeclaringType
        {
            get { throw new NotImplementedException(); }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override Type ReflectedType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
