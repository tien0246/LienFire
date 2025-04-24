namespace System.Configuration;

public class IntegerValidator : ConfigurationValidatorBase
{
	private bool rangeIsExclusive;

	private int minValue;

	private int maxValue = int.MaxValue;

	private int resolution;

	public IntegerValidator(int minValue, int maxValue, bool rangeIsExclusive, int resolution)
	{
		if (minValue != 0)
		{
			this.minValue = minValue;
		}
		if (maxValue != 0)
		{
			this.maxValue = maxValue;
		}
		this.rangeIsExclusive = rangeIsExclusive;
		this.resolution = resolution;
	}

	public IntegerValidator(int minValue, int maxValue, bool rangeIsExclusive)
		: this(minValue, maxValue, rangeIsExclusive, 0)
	{
	}

	public IntegerValidator(int minValue, int maxValue)
		: this(minValue, maxValue, rangeIsExclusive: false, 0)
	{
	}

	public override bool CanValidate(Type type)
	{
		return type == typeof(int);
	}

	public override void Validate(object value)
	{
		int num = (int)value;
		if (!rangeIsExclusive)
		{
			if (num < minValue || num > maxValue)
			{
				throw new ArgumentException("The value must be in the range " + minValue + " - " + maxValue);
			}
		}
		else if (num >= minValue && num <= maxValue)
		{
			throw new ArgumentException("The value must not be in the range " + minValue + " - " + maxValue);
		}
		if (resolution != 0 && num % resolution != 0)
		{
			throw new ArgumentException("The value must have a resolution of " + resolution);
		}
	}
}
