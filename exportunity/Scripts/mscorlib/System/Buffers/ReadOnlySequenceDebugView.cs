using System.Diagnostics;

namespace System.Buffers;

internal sealed class ReadOnlySequenceDebugView<T>
{
	[DebuggerDisplay("Count: {Segments.Length}", Name = "Segments")]
	public struct ReadOnlySequenceDebugViewSegments
	{
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public ReadOnlyMemory<T>[] Segments { get; set; }
	}

	private readonly T[] _array;

	private readonly ReadOnlySequenceDebugViewSegments _segments;

	public ReadOnlySequenceDebugViewSegments BufferSegments => _segments;

	[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
	public T[] Items => _array;

	public ReadOnlySequenceDebugView(ReadOnlySequence<T> sequence)
	{
		_array = BuffersExtensions.ToArray(in sequence);
		int num = 0;
		ReadOnlySequence<T>.Enumerator enumerator = sequence.GetEnumerator();
		while (enumerator.MoveNext())
		{
			_ = enumerator.Current;
			num++;
		}
		ReadOnlyMemory<T>[] array = new ReadOnlyMemory<T>[num];
		int num2 = 0;
		enumerator = sequence.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ReadOnlyMemory<T> current = enumerator.Current;
			array[num2] = current;
			num2++;
		}
		_segments = new ReadOnlySequenceDebugViewSegments
		{
			Segments = array
		};
	}
}
