using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

internal interface IStylePropertyAnimationSystem
{
	bool StartTransition(VisualElement owner, StylePropertyId prop, float startValue, float endValue, int durationMs, int delayMs, [NotNull] Func<float, float> easingCurve);

	bool StartTransition(VisualElement owner, StylePropertyId prop, int startValue, int endValue, int durationMs, int delayMs, [NotNull] Func<float, float> easingCurve);

	bool StartTransition(VisualElement owner, StylePropertyId prop, Length startValue, Length endValue, int durationMs, int delayMs, [NotNull] Func<float, float> easingCurve);

	bool StartTransition(VisualElement owner, StylePropertyId prop, Color startValue, Color endValue, int durationMs, int delayMs, [NotNull] Func<float, float> easingCurve);

	bool StartAnimationEnum(VisualElement owner, StylePropertyId prop, int startValue, int endValue, int durationMs, int delayMs, [NotNull] Func<float, float> easingCurve);

	bool StartTransition(VisualElement owner, StylePropertyId prop, Background startValue, Background endValue, int durationMs, int delayMs, [NotNull] Func<float, float> easingCurve);

	bool StartTransition(VisualElement owner, StylePropertyId prop, FontDefinition startValue, FontDefinition endValue, int durationMs, int delayMs, [NotNull] Func<float, float> easingCurve);

	bool StartTransition(VisualElement owner, StylePropertyId prop, Font startValue, Font endValue, int durationMs, int delayMs, [NotNull] Func<float, float> easingCurve);

	bool StartTransition(VisualElement owner, StylePropertyId prop, TextShadow startValue, TextShadow endValue, int durationMs, int delayMs, [NotNull] Func<float, float> easingCurve);

	bool StartTransition(VisualElement owner, StylePropertyId prop, Scale startValue, Scale endValue, int durationMs, int delayMs, [NotNull] Func<float, float> easingCurve);

	bool StartTransition(VisualElement owner, StylePropertyId prop, TransformOrigin startValue, TransformOrigin endValue, int durationMs, int delayMs, [NotNull] Func<float, float> easingCurve);

	bool StartTransition(VisualElement owner, StylePropertyId prop, Translate startValue, Translate endValue, int durationMs, int delayMs, [NotNull] Func<float, float> easingCurve);

	bool StartTransition(VisualElement owner, StylePropertyId prop, Rotate startValue, Rotate endValue, int durationMs, int delayMs, [NotNull] Func<float, float> easingCurve);

	void CancelAllAnimations();

	void CancelAllAnimations(VisualElement owner);

	void CancelAnimation(VisualElement owner, StylePropertyId id);

	bool HasRunningAnimation(VisualElement owner, StylePropertyId id);

	void UpdateAnimation(VisualElement owner, StylePropertyId id);

	void GetAllAnimations(VisualElement owner, List<StylePropertyId> propertyIds);

	void Update();
}
