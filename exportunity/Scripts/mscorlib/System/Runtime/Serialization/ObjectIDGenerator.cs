using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization;

[Serializable]
[ComVisible(true)]
public class ObjectIDGenerator
{
	private const int numbins = 4;

	internal int m_currentCount;

	internal int m_currentSize;

	internal long[] m_ids;

	internal object[] m_objs;

	private static readonly int[] sizes = new int[21]
	{
		5, 11, 29, 47, 97, 197, 397, 797, 1597, 3203,
		6421, 12853, 25717, 51437, 102877, 205759, 411527, 823117, 1646237, 3292489,
		6584983
	};

	public ObjectIDGenerator()
	{
		m_currentCount = 1;
		m_currentSize = sizes[0];
		m_ids = new long[m_currentSize * 4];
		m_objs = new object[m_currentSize * 4];
	}

	private int FindElement(object obj, out bool found)
	{
		int num = RuntimeHelpers.GetHashCode(obj);
		int num2 = 1 + (num & 0x7FFFFFFF) % (m_currentSize - 2);
		while (true)
		{
			int num3 = (num & 0x7FFFFFFF) % m_currentSize * 4;
			for (int i = num3; i < num3 + 4; i++)
			{
				if (m_objs[i] == null)
				{
					found = false;
					return i;
				}
				if (m_objs[i] == obj)
				{
					found = true;
					return i;
				}
			}
			num += num2;
		}
	}

	public virtual long GetId(object obj, out bool firstTime)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj", Environment.GetResourceString("Object cannot be null."));
		}
		bool found;
		int num = FindElement(obj, out found);
		long result;
		if (!found)
		{
			m_objs[num] = obj;
			m_ids[num] = m_currentCount++;
			result = m_ids[num];
			if (m_currentCount > m_currentSize * 4 / 2)
			{
				Rehash();
			}
		}
		else
		{
			result = m_ids[num];
		}
		firstTime = !found;
		return result;
	}

	public virtual long HasId(object obj, out bool firstTime)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj", Environment.GetResourceString("Object cannot be null."));
		}
		bool found;
		int num = FindElement(obj, out found);
		if (found)
		{
			firstTime = false;
			return m_ids[num];
		}
		firstTime = true;
		return 0L;
	}

	private void Rehash()
	{
		int i = 0;
		for (int currentSize = m_currentSize; i < sizes.Length && sizes[i] <= currentSize; i++)
		{
		}
		if (i == sizes.Length)
		{
			throw new SerializationException(Environment.GetResourceString("The internal array cannot expand to greater than Int32.MaxValue elements."));
		}
		m_currentSize = sizes[i];
		long[] ids = new long[m_currentSize * 4];
		object[] objs = new object[m_currentSize * 4];
		long[] ids2 = m_ids;
		object[] objs2 = m_objs;
		m_ids = ids;
		m_objs = objs;
		for (int j = 0; j < objs2.Length; j++)
		{
			if (objs2[j] != null)
			{
				bool found;
				int num = FindElement(objs2[j], out found);
				m_objs[num] = objs2[j];
				m_ids[num] = ids2[j];
			}
		}
	}
}
