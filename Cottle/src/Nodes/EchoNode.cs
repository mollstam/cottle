﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cottle.Nodes.Generics;

namespace   Cottle.Nodes
{
    sealed class    EchoNode : Node
    {
        #region Attributes

        private IExpression expression;

        #endregion

        #region Constructors

        public  EchoNode (IExpression expression)
        {
            this.expression = expression;
        }

        #endregion

        #region Methods

        public override IValue  Apply (Scope scope, TextWriter output)
        {
            output.Write (this.expression.Evaluate (scope, output).AsString);

            return null;
        }

        public override void    Debug (TextWriter output)
        {
            output.Write ("{echo ");
            output.Write (this.expression);
            output.Write ('}');
        }

        #endregion
    }
}
