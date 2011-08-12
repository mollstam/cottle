﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Cottle.Functions;
using Cottle.Values;

namespace   Cottle.Commons
{
    public static class CommonFunctions
    {
        #region Constants

        public static readonly IFunction    FunctionAbsolute = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return Math.Abs (values[0].AsNumber);
        }, 1);

        public static readonly IFunction    FunctionAdd = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber + values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionCat = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            List<KeyValuePair<Value, Value>>    array;
            StringBuilder                       builder;

            if (values[0].Type == Value.DataType.ARRAY)
            {
                array = new List<KeyValuePair<Value, Value>> (values[0].Fields.Count * 2 + 1);

                foreach (Value value in values)
                    array.AddRange (value.Fields);

                return array;
            }
            else
            {
                builder = new StringBuilder ();

                foreach (Value value in values)
                    builder.Append (value.AsString);

                return builder.ToString ();
            }
        }, 1, -1);

        public static readonly IFunction    FunctionChar = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            try
            {
                return char.ConvertFromUtf32 ((int)values[0].AsNumber);
            }
            catch
            {
                return '?';
            }
        }, 1);

        public static readonly IFunction    FunctionCompare = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].CompareTo (values[1]);
        }, 2);

        public static readonly IFunction    FunctionCount = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].Fields.Count;
        }, 1);

        public static readonly IFunction    FunctionDiv = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            decimal denominator = values[1].AsNumber;

            if (denominator == 0)
                return UndefinedValue.Instance;

            return values[0].AsNumber / denominator;
        }, 2);

        public static readonly IFunction    FunctionEqual = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            decimal compare = values[0].AsNumber;
            int     i;

            for (i = 1; i < values.Count; ++i)
                if (values[i].AsNumber != compare)
                    return false;

            return true;
        }, 1, -1);

        public static readonly IFunction    FunctionFind = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            int     index = values.Count > 2 ? (int)values[2].AsNumber : 0;
            Value   token = values[1];
            Value   value = values[0];

            if (value.Type == Value.DataType.ARRAY)
                return value.Fields.FindIndex (index, p => p.Value.CompareTo (token) == 0);
            else
                return value.AsString.IndexOf (token.AsString, index);
        }, 2, 3);

        public static readonly IFunction    FunctionGreater = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber > values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionGreaterEqual = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber >= values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionHas = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            Value   value = values[0];
            int     i;

            for (i = 1; i < values.Count; ++i)
                if (!value.Has (values[i]))
                    return false;

            return true;
        }, 1, -1);

        public static readonly IFunction    FunctionLength = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsString.Length;
        }, 1);

        public static readonly IFunction    FunctionLower = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber < values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionLowerCase = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsString.ToLowerInvariant ();
        }, 1);

        public static readonly IFunction    FunctionLowerEqual = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber <= values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionMap = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            KeyValuePair<Value, Value>[]    array = new KeyValuePair<Value, Value>[values[0].Fields.Count];
            IFunction                       callback = values[1].AsFunction;
            int                             i = 0;

            if (callback == null)
                return UndefinedValue.Instance;

            foreach (KeyValuePair<Value, Value> pair in values[0].Fields)
                array[i++] = new KeyValuePair<Value, Value> (pair.Key, callback.Execute (new Value[] {pair.Value}, scope, output));

            return array;
        }, 2);

        public static readonly IFunction    FunctionMatch = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            try
            {
                return Regex.IsMatch (values[0].AsString, values[1].AsString);
            }
            catch
            {
                return UndefinedValue.Instance;
            }
        }, 2);

        public static readonly IFunction    FunctionMaximum = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            Value   array;
            decimal max;
            int     i;

            if (values[0].Type == Value.DataType.ARRAY)
            {
                array = values[0];

                if (array.Fields.Count <= 0)
                    return UndefinedValue.Instance;

                max = array.Fields[0].Value.AsNumber;

                for (i = 1; i < array.Fields.Count; ++i)
                    max = Math.Max (max, array.Fields[i].Value.AsNumber);
            }
            else
            {
                max = values[0].AsNumber;

                for (i = 1; i < values.Count; ++i)
                    max = Math.Max (max, values[i].AsNumber);
            }

            return max;
        }, 1, -1);

        public static readonly IFunction    FunctionMinimum = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            Value   array;
            decimal min;
            int     i;

            if (values[0].Type == Value.DataType.ARRAY)
            {
                array = values[0];

                if (array.Fields.Count <= 0)
                    return UndefinedValue.Instance;

                min = array.Fields[0].Value.AsNumber;

                for (i = 1; i < array.Fields.Count; ++i)
                    min = Math.Min (min, array.Fields[i].Value.AsNumber);
            }
            else
            {
                min = values[0].AsNumber;

                for (i = 1; i < values.Count; ++i)
                    min = Math.Min (min, values[i].AsNumber);
            }

            return min;
        }, 1, -1);

        public static readonly IFunction    FunctionMod = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            decimal denominator = values[1].AsNumber;

            if (denominator == 0)
                return UndefinedValue.Instance;

            return values[0].AsNumber % denominator;
        }, 2);

        public static readonly IFunction    FunctionMul = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber * values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionOrd = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            string  str = values[0].AsString;

            return str.Length > 0 ? char.ConvertToUtf32 (str, 0) : 0;
        }, 1);

        public static readonly IFunction    FunctionRandom = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            if (CommonFunctions.random == null)
                CommonFunctions.random = new Random ();

            switch (values.Count)
            {
                case 0:
                    return CommonFunctions.random.Next ();

                case 1:
                    return CommonFunctions.random.Next ((int)values[0].AsNumber);

                default:
                    return CommonFunctions.random.Next ((int)values[0].AsNumber, (int)values[1].AsNumber);
            }
        }, 0, 2);

        public static readonly IFunction    FunctionSlice = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            int     count;
            int     limit;
            int     index;
            Value   value = values[0];

            limit = value.Type == Value.DataType.ARRAY ? value.Fields.Count : value.AsString.Length;
            index = Math.Min ((int)values[1].AsNumber, limit);
            count = values.Count > 2 ? Math.Min ((int)values[2].AsNumber, limit - index) : limit - index;

            if (value.Type == Value.DataType.ARRAY)
                return value.Fields.GetRange (index, count);
            else
                return value.AsString.Substring (index, count);
        }, 2, 3);

        public static readonly IFunction    FunctionSort = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            IFunction                           callback = values.Count > 1 ? values[1].AsFunction : null;
            List<KeyValuePair<Value, Value>>    source = values[0].Fields;
            List<KeyValuePair<Value, Value>>    sorted = new List<KeyValuePair<Value, Value>> (source);

            if (callback != null)
                sorted.Sort ((a, b) => (int)callback.Execute (new Value[] {a.Value, b.Value}, scope, output).AsNumber);
            else
                sorted.Sort ((a, b) => a.Value.CompareTo (b.Value));

            return sorted;
        }, 1, 2);

        public static readonly IFunction    FunctionSub = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsNumber - values[1].AsNumber;
        }, 2);

        public static readonly IFunction    FunctionUpperCase = new CallbackFunction (delegate (IList<Value> values, Scope scope, TextWriter output)
        {
            return values[0].AsString.ToUpperInvariant ();
        }, 1);

        #endregion

        #region Attributes

        [ThreadStatic]
        private static Random   random = null;

        #endregion

        #region Methods

        public static void  Assign (Document document)
        {
            document.Values["abs"] = new FunctionValue (CommonFunctions.FunctionAbsolute);
            document.Values["add"] = new FunctionValue (CommonFunctions.FunctionAdd);
            document.Values["cat"] = new FunctionValue (CommonFunctions.FunctionCat);
            document.Values["char"] = new FunctionValue (CommonFunctions.FunctionChar);
            document.Values["cmp"] = new FunctionValue (CommonFunctions.FunctionCompare);
            document.Values["count"] = new FunctionValue (CommonFunctions.FunctionCount);
            document.Values["div"] = new FunctionValue (CommonFunctions.FunctionDiv);
            document.Values["eq"] = new FunctionValue (CommonFunctions.FunctionEqual);
            document.Values["find"] = new FunctionValue (CommonFunctions.FunctionFind);
            document.Values["ge"] = new FunctionValue (CommonFunctions.FunctionGreaterEqual);
            document.Values["gt"] = new FunctionValue (CommonFunctions.FunctionGreater);
            document.Values["has"] = new FunctionValue (CommonFunctions.FunctionHas);
            document.Values["lcase"] = new FunctionValue (CommonFunctions.FunctionLowerCase);
            document.Values["le"] = new FunctionValue (CommonFunctions.FunctionLowerEqual);
            document.Values["len"] = new FunctionValue (CommonFunctions.FunctionLength);
            document.Values["lt"] = new FunctionValue (CommonFunctions.FunctionLower);
            document.Values["map"] = new FunctionValue (CommonFunctions.FunctionMap);
            document.Values["match"] = new FunctionValue (CommonFunctions.FunctionMatch);
            document.Values["max"] = new FunctionValue (CommonFunctions.FunctionMaximum);
            document.Values["min"] = new FunctionValue (CommonFunctions.FunctionMinimum);
            document.Values["mod"] = new FunctionValue (CommonFunctions.FunctionMod);
            document.Values["mul"] = new FunctionValue (CommonFunctions.FunctionMul);
            document.Values["ord"] = new FunctionValue (CommonFunctions.FunctionOrd);
            document.Values["rand"] = new FunctionValue (CommonFunctions.FunctionRandom);
            document.Values["slice"] = new FunctionValue (CommonFunctions.FunctionSlice);
            document.Values["sort"] = new FunctionValue (CommonFunctions.FunctionSort);
            document.Values["sub"] = new FunctionValue (CommonFunctions.FunctionSub);
            document.Values["ucase"] = new FunctionValue (CommonFunctions.FunctionUpperCase);
        }

        #endregion
    }
}
