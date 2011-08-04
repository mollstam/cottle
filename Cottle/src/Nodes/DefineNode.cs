﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Expressions;
using Cottle.Nodes.Generics;
using Cottle.Values;

namespace   Cottle.Nodes
{
    sealed class    DefineNode : Node
    {
        #region Attributes

        private List<NameExpression>    arguments;

        private INode                   body;

        private NameExpression          name;

        #endregion

        #region Constructors

        public  DefineNode (NameExpression name, IEnumerable<NameExpression> arguments, INode body)
        {
            this.arguments = new List<NameExpression> (arguments);
            this.body = body;
            this.name = name;
        }

        #endregion

        #region Methods

        public override IValue  Apply (Scope scope, TextWriter output)
        {
            this.name.Set (scope, new FunctionValue (new Function (this.arguments, this.body)), Scope.SetMode.ANYWHERE);

            return null;
        }

        public override void    Debug (TextWriter output)
        {
            bool    comma = false;

            output.Write ("{def ");
            output.Write (this.name);
            output.Write ('(');

            foreach (NameExpression argument in this.arguments)
            {
                if (comma)
                    output.Write (", ");
                else
                    comma = true;

                output.Write (argument);
            }

            output.Write ("): ");

            this.body.Debug (output);

            output.Write ('}');
        }

        #endregion
    }
}
