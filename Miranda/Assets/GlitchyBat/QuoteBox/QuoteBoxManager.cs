using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuoteBoxManager : Singleton<QuoteBoxManager> {
	QuoteBox defaultQuoteBox = null;
	QuoteBox currentQuoteBox = null;

	[Header("Player buttons")]
	[SerializeField] string buttonConfirm = "Jump";
	[SerializeField] string buttonDeny = "Fire1";

	#region Properties
	public QuoteBox DefaultQuoteBox {
		get{
			if (defaultQuoteBox)
				return defaultQuoteBox;
			defaultQuoteBox = LinkDefaultQuoteBox();
			return defaultQuoteBox;
		}
	}

	public string ButtonConfirm {get{return buttonConfirm;}}
	public string ButtonDeny {get{return buttonDeny;}}
	#endregion

	#region MonoBehaviour

	#endregion

	public void ShowText(QuotePayload mPayload) {
		ShowText(DefaultQuoteBox,mPayload);
	}

	public void ShowText(QuoteBox mTargetBox, QuotePayload mPayload) {
		currentQuoteBox = mTargetBox;
		currentQuoteBox.ShowText(mPayload);
	}

	QuoteBox LinkDefaultQuoteBox() {
		QuoteBox tQuoteBox = FindObjectOfType<QuoteBox>();
		if (!tQuoteBox) {
			Debug.LogError("No QuoteBox available in scene!", this);
			return null;
		}
		return tQuoteBox;
	}

	#region Misc
	#endregion
}
