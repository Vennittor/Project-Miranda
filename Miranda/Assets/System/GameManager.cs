using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {
	bool isPaused = false;

	#region Properties
	public bool IsPaused {get{return isPaused;}}
	#endregion

	#region Events
	public delegate void OnPauseSet(bool mPaused);
	public event OnPauseSet EvOnPauseSet;
	#endregion

	#region MonoBehaviour
	void Start() {
		QuoteBoxManager.Instance.DefaultQuoteBox.EvOnOpen += Pause;
		QuoteBoxManager.Instance.DefaultQuoteBox.EvOnClose += Resume;
	}

	void OnDestroy() {
		EvOnPauseSet = null;
	}
	#endregion

	void Pause() {
		isPaused = true;
		if (EvOnPauseSet != null)
			EvOnPauseSet(isPaused);
	}

	void Resume() {
		isPaused = false;
		if (EvOnPauseSet != null)
			EvOnPauseSet(isPaused);
	}
}
