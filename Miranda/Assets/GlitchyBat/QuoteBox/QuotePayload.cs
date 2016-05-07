using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class QuotePayload {
	[SerializeField]
	List<QuoteChunk> fullText = new List<QuoteChunk>();

	#region Properties
	public List<QuoteChunk> Text {get{return fullText;}}
	#endregion

	#region Globals
	/// <summary>
	/// Returns a dummy payload.
	/// </summary>
	/// <value></value>
	public static QuotePayload Uninitialized {
		get{
			QuotePayload empty = new QuotePayload();
			empty.fullText.Add(QuoteChunk.Empty);
			return empty;
		}
	}
	#endregion
}
