using UnityEngine;

public class Line : MonoBehaviour
{
	public float tg;

	public float tdThuStart;

	public float tdThuEnd;

	private Vector3 vt1;

	private Vector3 vt2;

	private LineRenderer line;

	private void Start()
	{
		line = GetComponent<LineRenderer>();
		DrawLine();
	}

	private void Update()
	{
		if (tg > 0f)
		{
			tg -= Time.deltaTime;
			if (line.startWidth > 0f)
			{
				line.startWidth -= tdThuStart / 1000f;
			}
			else
			{
				line.startWidth = 0f;
			}
			if (line.endWidth > 0f)
			{
				line.endWidth -= tdThuEnd / 1000f;
			}
			else
			{
				line.endWidth = 0f;
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void SetLine(Vector3 v1, Vector3 v2)
	{
		vt1 = v1;
		vt2 = v2;
	}

	private void DrawLine()
	{
		line.SetPosition(0, vt1);
		line.SetPosition(1, vt2);
	}
}
