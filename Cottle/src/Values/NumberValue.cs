
namespace Cottle.Values
{
	public sealed class NumberValue : ScalarValue<double>
	{
		#region Properties

		public override bool			AsBoolean
		{
			get
			{
				return this.value != 0;
			}
		}

		public override double			AsNumber
		{
			get
			{
				return this.value;
			}
		}

		public override string			AsString
		{
			get
			{
				return this.value.ToString ();
			}
		}

		public override ValueContent	Type
		{
			get
			{
				return ValueContent.Number;
			}
		}

		#endregion

		#region Constructors / Public

		public	NumberValue (byte value) :
			this ((double)value)
		{
		}

		public	NumberValue (double value) :
			base (value, (source) => source.AsNumber)
		{
		}

		public	NumberValue (float value) :
			this ((double)value)
		{
		}

		public	NumberValue (int value) :
			this ((double)value)
		{
		}

		public	NumberValue (long value) :
			this ((double)value)
		{
		}

		public	NumberValue (short value) :
			this ((double)value)
		{
		}

		#endregion

		#region Methods

		public override string	ToString ()
		{
			return this.value.ToString ();
		}

		#endregion
	}
}
