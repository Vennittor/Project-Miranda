using UnityEngine;
using System.Collections;

[System.Serializable]
public class QuoteChunk {
	[SerializeField]
	[Multiline(5)]
	string textChunk = string.Empty;

	[SerializeField]
	bool clearOnEnd = true;

	#region Properties
	public string Text {get{return textChunk;}}
	public bool ClearOnEnd {get{return clearOnEnd;}}
	#endregion

	#region Globals
	/// <summary>
	/// Returns empty text.
	/// </summary>
	/// <value></value>
	public static QuoteChunk Empty {
		get{
			QuoteChunk quote = new QuoteChunk();
			quote.textChunk = "Uninitialized Text";
			return quote;
		}
	}
	#endregion
}
