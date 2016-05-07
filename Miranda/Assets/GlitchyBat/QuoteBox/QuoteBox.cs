using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum QuoteBoxSpeed : byte {
	INSTANT = 0,
	FAST,
	NORMAL,
	SLOW,
	AWFUL
}

/// <summary>
/// Main quote box class.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class QuoteBox : MonoBehaviour {
	bool isLocked = false;

	[Header("References")]
	CanvasGroup canvasGroup = null;
	Image panel = null;
	Text textField = null;

	string displayedText = string.Empty;

	[Header("Options")]
	[SerializeField]
	QuoteBoxSpeed defaultTextSpeed = QuoteBoxSpeed.NORMAL;
	[SerializeField]
	bool isPanelVisible = true;

	#region Events
	/// <summary>
	/// Called when the dialogue box is first activated.
	/// </summary>
	public delegate void OnOpen();
	public event OnOpen EvOnOpen;
	/// <summary>
	/// Called when a chunk of text is finished typing.
	/// </summary>
	public delegate void OnLineFinished();
	public event OnLineFinished EvOnLineFinished;
	/// <summary>
	/// Called when the dialogue box is closed
	/// </summary>
	public delegate void OnClose();
	public event OnClose EvOnClose;
	#endregion

	#region Properties
	/// <summary>
	/// Whether or not the text box is displayed.
	/// </summary>
	/// <value><c>true</c> if this instance is displayed; otherwise, <c>false</c>.</value>
	public bool IsDisplayed {
		get{return canvasGroup.blocksRaycasts;}
		private set{
			if (value) {
				canvasGroup.alpha = 1f;
				canvasGroup.blocksRaycasts = true;
				canvasGroup.interactable = true;
			} else {
				canvasGroup.alpha = 0f;
				canvasGroup.blocksRaycasts = false;
				canvasGroup.interactable = false;
			}
			// visibility of panel background
			if (isPanelVisible) {
				panel.color = new Color(panel.color.r,panel.color.g,panel.color.b,255);
			} else {
				panel.color = new Color(panel.color.r,panel.color.g,panel.color.b,0);
			}
		}
	}

	/// <summary>
	/// Text speed as an enumerator.
	/// </summary>
	/// <value>The text speed.</value>
	public QuoteBoxSpeed DefaultTextSpeed {
		get{return defaultTextSpeed;}
		set{defaultTextSpeed = value;}
	}

	/// <summary>
	/// Shows or hides the panel the text is on.
	/// </summary>
	/// <value><c>true</c> to show panel, <c>false</c> to show only text.</value>
	public bool IsPanelVisible {
		get{return isPanelVisible;}
		set{
			isPanelVisible = false;
		}
	}
	#endregion

	#region MonoBehaviour
	void Awake() {
		LinkReferences();
		IsDisplayed = false;
	}
	#endregion

	#region Open and close
	void Open() {
		isLocked = true;

		// TODO if no quotebox open, instantiate new one
		IsDisplayed = true;

		if (EvOnOpen != null)
			EvOnOpen();
	}

	void Close() {
		Clear();

		IsDisplayed = false;

		if (EvOnClose != null)
			EvOnClose();

		isLocked = false;
	}

	// Forces the quotebox closed
	public void ForceClose() {
		// TODO safely force close
		Close();
	}
	#endregion

	#region ShowText
	public void ShowText(QuotePayload mDialogue) {
		if (!isLocked) {
			Open();
			StartCoroutine(IterateChunk(mDialogue.Text));
		}
	}

	IEnumerator IterateChunk(List<QuoteChunk> mChunks) {
		bool isWaitingForResume = false;
		foreach (QuoteChunk c in mChunks) {
			yield return StartCoroutine(TypeText(c.Text));
			yield return null; // skip a frame so button presses from TypeText() don't bleed here
			// LAZY loop until player input says to resume to the next chunk
			isWaitingForResume = true;
			while(isWaitingForResume) {
				if (CustomInput.GetButtonDown(QuoteBoxManager.Instance.ButtonConfirm)
					|| CustomInput.GetButtonDown(QuoteBoxManager.Instance.ButtonDeny))
					isWaitingForResume = false;
				yield return null;
			}
			if (c.ClearOnEnd)
				Clear();
			else PutChar(char.Parse("\n"));
		}
		Close();
	}

	IEnumerator TypeText(string mText) {
		Queue<char> sourceText = QueueText(mText);

		QuoteBoxSpeed tTextSpeed = DefaultTextSpeed;

		foreach(char c in sourceText) {
			if (CustomInput.GetButtonDown(QuoteBoxManager.Instance.ButtonConfirm)
				&& tTextSpeed != QuoteBoxSpeed.INSTANT)
				tTextSpeed = QuoteBoxSpeed.FAST;
			if (CustomInput.GetButtonDown(QuoteBoxManager.Instance.ButtonDeny))
				tTextSpeed = QuoteBoxSpeed.INSTANT;

			// Skip typewriting
			if (tTextSpeed == QuoteBoxSpeed.INSTANT) {
				displayedText = mText;
				RefreshQuoteText();
				break;
			}

			PutChar(c);
			RefreshQuoteText();
			yield return new WaitForSeconds(TextSpeedAsTime(tTextSpeed));
		}

		if (EvOnLineFinished != null)
			EvOnLineFinished();
	}
	#endregion

	#region Inner text handling
	Queue<char> QueueText(string mText) {
		Queue<char> newText = new Queue<char>();

		foreach(char c in mText.ToCharArray())
			newText.Enqueue(c);

		return newText;
	}

	void PutChar(char mChar) {
		// play sound effect if applicable
		displayedText = displayedText + mChar;
	}

	void RefreshQuoteText() {
		textField.text = displayedText;
	}

	void Clear() {
		displayedText = string.Empty;
		RefreshQuoteText();
	}
	#endregion

	#region Text shortcut functions
	#endregion

	#region Misc
	float TextSpeedAsTime(QuoteBoxSpeed mQuoteBoxSpeed) {
		// all times are in seconds.
		float t = 0;
		switch(mQuoteBoxSpeed) {
		case QuoteBoxSpeed.AWFUL:
			t = 0.25f;
			break;
		case QuoteBoxSpeed.SLOW:
			t = 0.15f;
			break;
		case QuoteBoxSpeed.NORMAL:
			t = 0.05f;
			break;
		case QuoteBoxSpeed.FAST:
			t = 0.01f;
			break;
		case QuoteBoxSpeed.INSTANT:
			t = 0.0f;
			break;
		}
		return t;
	}

	void LinkReferences() {
		canvasGroup = GetComponent<CanvasGroup>();
		panel = GetComponent<Image>();
		textField = GetComponentInChildren<Text>();
	}
	#endregion
}
