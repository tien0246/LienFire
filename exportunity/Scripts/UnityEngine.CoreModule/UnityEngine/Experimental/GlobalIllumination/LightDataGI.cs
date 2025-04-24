using UnityEngine.Scripting;

namespace UnityEngine.Experimental.GlobalIllumination;

[UsedByNativeCode]
public struct LightDataGI
{
	public int instanceID;

	public int cookieID;

	public float cookieScale;

	public LinearColor color;

	public LinearColor indirectColor;

	public Quaternion orientation;

	public Vector3 position;

	public float range;

	public float coneAngle;

	public float innerConeAngle;

	public float shape0;

	public float shape1;

	public LightType type;

	public LightMode mode;

	public byte shadow;

	public FalloffType falloff;

	public void Init(ref DirectionalLight light, ref Cookie cookie)
	{
		instanceID = light.instanceID;
		cookieID = cookie.instanceID;
		cookieScale = cookie.scale;
		color = light.color;
		indirectColor = light.indirectColor;
		orientation = light.orientation;
		position = light.position;
		range = 0f;
		coneAngle = cookie.sizes.x;
		innerConeAngle = cookie.sizes.y;
		shape0 = light.penumbraWidthRadian;
		shape1 = 0f;
		type = LightType.Directional;
		mode = light.mode;
		shadow = (byte)(light.shadow ? 1u : 0u);
		falloff = FalloffType.Undefined;
	}

	public void Init(ref PointLight light, ref Cookie cookie)
	{
		instanceID = light.instanceID;
		cookieID = cookie.instanceID;
		cookieScale = cookie.scale;
		color = light.color;
		indirectColor = light.indirectColor;
		orientation = light.orientation;
		position = light.position;
		range = light.range;
		coneAngle = 0f;
		innerConeAngle = 0f;
		shape0 = light.sphereRadius;
		shape1 = 0f;
		type = LightType.Point;
		mode = light.mode;
		shadow = (byte)(light.shadow ? 1u : 0u);
		falloff = light.falloff;
	}

	public void Init(ref SpotLight light, ref Cookie cookie)
	{
		instanceID = light.instanceID;
		cookieID = cookie.instanceID;
		cookieScale = cookie.scale;
		color = light.color;
		indirectColor = light.indirectColor;
		orientation = light.orientation;
		position = light.position;
		range = light.range;
		coneAngle = light.coneAngle;
		innerConeAngle = light.innerConeAngle;
		shape0 = light.sphereRadius;
		shape1 = (int)light.angularFalloff;
		type = LightType.Spot;
		mode = light.mode;
		shadow = (byte)(light.shadow ? 1u : 0u);
		falloff = light.falloff;
	}

	public void Init(ref RectangleLight light, ref Cookie cookie)
	{
		instanceID = light.instanceID;
		cookieID = cookie.instanceID;
		cookieScale = cookie.scale;
		color = light.color;
		indirectColor = light.indirectColor;
		orientation = light.orientation;
		position = light.position;
		range = light.range;
		coneAngle = 0f;
		innerConeAngle = 0f;
		shape0 = light.width;
		shape1 = light.height;
		type = LightType.Rectangle;
		mode = light.mode;
		shadow = (byte)(light.shadow ? 1u : 0u);
		falloff = light.falloff;
	}

	public void Init(ref DiscLight light, ref Cookie cookie)
	{
		instanceID = light.instanceID;
		cookieID = cookie.instanceID;
		cookieScale = cookie.scale;
		color = light.color;
		indirectColor = light.indirectColor;
		orientation = light.orientation;
		position = light.position;
		range = light.range;
		coneAngle = 0f;
		innerConeAngle = 0f;
		shape0 = light.radius;
		shape1 = 0f;
		type = LightType.Disc;
		mode = light.mode;
		shadow = (byte)(light.shadow ? 1u : 0u);
		falloff = light.falloff;
	}

	public void Init(ref SpotLightBoxShape light, ref Cookie cookie)
	{
		instanceID = light.instanceID;
		cookieID = cookie.instanceID;
		cookieScale = cookie.scale;
		color = light.color;
		indirectColor = light.indirectColor;
		orientation = light.orientation;
		position = light.position;
		range = light.range;
		coneAngle = 0f;
		innerConeAngle = 0f;
		shape0 = light.width;
		shape1 = light.height;
		type = LightType.SpotBoxShape;
		mode = light.mode;
		shadow = (byte)(light.shadow ? 1u : 0u);
		falloff = FalloffType.Undefined;
	}

	public void Init(ref SpotLightPyramidShape light, ref Cookie cookie)
	{
		instanceID = light.instanceID;
		cookieID = cookie.instanceID;
		cookieScale = cookie.scale;
		color = light.color;
		indirectColor = light.indirectColor;
		orientation = light.orientation;
		position = light.position;
		range = light.range;
		coneAngle = light.angle;
		innerConeAngle = 0f;
		shape0 = light.aspectRatio;
		shape1 = 0f;
		type = LightType.SpotPyramidShape;
		mode = light.mode;
		shadow = (byte)(light.shadow ? 1u : 0u);
		falloff = light.falloff;
	}

	public void Init(ref DirectionalLight light)
	{
		Cookie cookie = Cookie.Defaults();
		Init(ref light, ref cookie);
	}

	public void Init(ref PointLight light)
	{
		Cookie cookie = Cookie.Defaults();
		Init(ref light, ref cookie);
	}

	public void Init(ref SpotLight light)
	{
		Cookie cookie = Cookie.Defaults();
		Init(ref light, ref cookie);
	}

	public void Init(ref RectangleLight light)
	{
		Cookie cookie = Cookie.Defaults();
		Init(ref light, ref cookie);
	}

	public void Init(ref DiscLight light)
	{
		Cookie cookie = Cookie.Defaults();
		Init(ref light, ref cookie);
	}

	public void Init(ref SpotLightBoxShape light)
	{
		Cookie cookie = Cookie.Defaults();
		Init(ref light, ref cookie);
	}

	public void Init(ref SpotLightPyramidShape light)
	{
		Cookie cookie = Cookie.Defaults();
		Init(ref light, ref cookie);
	}

	public void InitNoBake(int lightInstanceID)
	{
		instanceID = lightInstanceID;
		mode = LightMode.Unknown;
	}
}
