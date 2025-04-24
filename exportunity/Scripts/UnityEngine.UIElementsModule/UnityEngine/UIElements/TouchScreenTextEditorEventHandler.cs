namespace UnityEngine.UIElements;

internal class TouchScreenTextEditorEventHandler : TextEditorEventHandler
{
	private IVisualElementScheduledItem m_TouchKeyboardPoller = null;

	private VisualElement m_LastPointerDownTarget;

	private static TouchScreenKeyboard s_KeyboardOnScreen;

	public TouchScreenTextEditorEventHandler(TextEditorEngine editorEngine, ITextInputField textInputField)
		: base(editorEngine, textInputField)
	{
	}

	private void PollTouchScreenKeyboard()
	{
		if (TouchScreenKeyboard.isSupported && !TouchScreenKeyboard.isInPlaceEditingAllowed)
		{
			if (m_TouchKeyboardPoller == null)
			{
				m_TouchKeyboardPoller = (base.textInputField as VisualElement)?.schedule.Execute(DoPollTouchScreenKeyboard).Every(100L);
			}
			else
			{
				m_TouchKeyboardPoller.Resume();
			}
		}
	}

	private void DoPollTouchScreenKeyboard()
	{
		if (TouchScreenKeyboard.isSupported && !TouchScreenKeyboard.isInPlaceEditingAllowed)
		{
			if (base.textInputField.editorEngine.keyboardOnScreen == null)
			{
				return;
			}
			if (s_KeyboardOnScreen != base.textInputField.editorEngine.keyboardOnScreen)
			{
				base.textInputField.editorEngine.keyboardOnScreen = null;
				m_TouchKeyboardPoller.Pause();
				return;
			}
			base.textInputField.UpdateText(base.textInputField.CullString(base.textInputField.editorEngine.keyboardOnScreen.text));
			if (!base.textInputField.isDelayed)
			{
				base.textInputField.UpdateValueFromText();
			}
			if (base.textInputField.editorEngine.keyboardOnScreen.status != TouchScreenKeyboard.Status.Visible)
			{
				base.textInputField.editorEngine.keyboardOnScreen = null;
				m_TouchKeyboardPoller.Pause();
				if (base.textInputField.isDelayed)
				{
					base.textInputField.UpdateValueFromText();
				}
			}
		}
		else
		{
			base.textInputField.editorEngine.keyboardOnScreen.active = false;
			base.textInputField.editorEngine.keyboardOnScreen = null;
			m_TouchKeyboardPoller.Pause();
		}
	}

	public override void ExecuteDefaultActionAtTarget(EventBase evt)
	{
		base.ExecuteDefaultActionAtTarget(evt);
		if (base.editorEngine.keyboardOnScreen != null)
		{
			return;
		}
		if (!base.textInputField.isReadOnly && evt.eventTypeId == EventBase<PointerDownEvent>.TypeId())
		{
			base.textInputField.CaptureMouse();
			m_LastPointerDownTarget = evt.target as VisualElement;
		}
		else
		{
			if (base.textInputField.isReadOnly || evt.eventTypeId != EventBase<PointerUpEvent>.TypeId())
			{
				return;
			}
			base.textInputField.ReleaseMouse();
			if (m_LastPointerDownTarget != null && m_LastPointerDownTarget.worldBound.Contains(((PointerUpEvent)evt).position))
			{
				m_LastPointerDownTarget = null;
				base.textInputField.SyncTextEngine();
				base.textInputField.UpdateText(base.editorEngine.text);
				base.editorEngine.keyboardOnScreen = TouchScreenKeyboard.Open(base.textInputField.text, TouchScreenKeyboardType.Default, autocorrection: true, base.editorEngine.multiline, base.textInputField.isPasswordField);
				s_KeyboardOnScreen = base.editorEngine.keyboardOnScreen;
				if (base.editorEngine.keyboardOnScreen != null)
				{
					PollTouchScreenKeyboard();
				}
				base.editorEngine.UpdateScrollOffset();
				evt.StopPropagation();
			}
		}
	}
}
