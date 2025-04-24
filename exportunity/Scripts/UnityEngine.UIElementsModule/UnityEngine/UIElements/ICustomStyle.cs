namespace UnityEngine.UIElements;

public interface ICustomStyle
{
	bool TryGetValue(CustomStyleProperty<float> property, out float value);

	bool TryGetValue(CustomStyleProperty<int> property, out int value);

	bool TryGetValue(CustomStyleProperty<bool> property, out bool value);

	bool TryGetValue(CustomStyleProperty<Color> property, out Color value);

	bool TryGetValue(CustomStyleProperty<Texture2D> property, out Texture2D value);

	bool TryGetValue(CustomStyleProperty<Sprite> property, out Sprite value);

	bool TryGetValue(CustomStyleProperty<VectorImage> property, out VectorImage value);

	bool TryGetValue<T>(CustomStyleProperty<T> property, out T value) where T : Object;

	bool TryGetValue(CustomStyleProperty<string> property, out string value);
}
