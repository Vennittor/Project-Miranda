using UnityEngine;
using System.Collections;

public class HudControlsInfo : MonoBehaviour {
	CanvasGroup _canvasGroup = null;

	[SerializeField]
	KeyCode key = KeyCode.Tab;

	[SerializeField]
	bool menuDisplayed = false;

	#region Properties
	bool MenuDisplayed {
		get{return menuDisplayed;}
		set{
			menuDisplayed = value;
			if(value)
				_canvasGroup.alpha = 1f;
			else
				_canvasGroup.alpha = 0f;
			_canvasGroup.blocksRaycasts = value;
			_canvasGroup.interactable = value;
		}
	}
	#endregion

	#region MonoBehaviour
	void Awake() {
		LinkReferences();
		// Apply default setting to property to update
		// the menu's appearance.
		MenuDisplayed = menuDisplayed;
	}

	void Update() {
		if(Input.GetKeyDown(key))
			MenuDisplayed = !MenuDisplayed;
	}
	#endregion

	void LinkReferences() {
		_canvasGroup = GetComponent<CanvasGroup>();
	}	
}
