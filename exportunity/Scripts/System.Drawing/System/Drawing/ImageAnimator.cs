using System.Collections;
using System.Drawing.Imaging;
using System.Threading;

namespace System.Drawing;

public sealed class ImageAnimator
{
	private static Hashtable ht = Hashtable.Synchronized(new Hashtable());

	private ImageAnimator()
	{
	}

	public static void Animate(Image image, EventHandler onFrameChangedHandler)
	{
		if (CanAnimate(image) && !ht.ContainsKey(image))
		{
			byte[] value = image.GetPropertyItem(20736).Value;
			int[] array = new int[value.Length >> 2];
			int num = 0;
			int num2 = 0;
			while (num < value.Length)
			{
				int num3 = BitConverter.ToInt32(value, num) * 10;
				array[num2] = ((num3 < 100) ? 100 : num3);
				num += 4;
				num2++;
			}
			AnimateEventArgs e = new AnimateEventArgs(image);
			Thread thread = new Thread(new WorkerThread(onFrameChangedHandler, e, array).LoopHandler);
			thread.IsBackground = true;
			e.RunThread = thread;
			ht.Add(image, e);
			thread.Start();
		}
	}

	public static bool CanAnimate(Image image)
	{
		if (image == null)
		{
			return false;
		}
		int num = image.FrameDimensionsList.Length;
		if (num < 1)
		{
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			if (image.FrameDimensionsList[i].Equals(FrameDimension.Time.Guid))
			{
				return image.GetFrameCount(FrameDimension.Time) > 1;
			}
		}
		return false;
	}

	public static void StopAnimate(Image image, EventHandler onFrameChangedHandler)
	{
		if (image != null && ht.ContainsKey(image))
		{
			((AnimateEventArgs)ht[image]).RunThread.Abort();
			ht.Remove(image);
		}
	}

	public static void UpdateFrames()
	{
		foreach (Image key in ht.Keys)
		{
			UpdateImageFrame(key);
		}
	}

	public static void UpdateFrames(Image image)
	{
		if (image != null && ht.ContainsKey(image))
		{
			UpdateImageFrame(image);
		}
	}

	private static void UpdateImageFrame(Image image)
	{
		AnimateEventArgs e = (AnimateEventArgs)ht[image];
		image.SelectActiveFrame(FrameDimension.Time, e.GetNextFrame());
	}
}
