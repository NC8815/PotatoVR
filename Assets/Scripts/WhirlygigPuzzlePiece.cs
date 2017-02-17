using UnityEngine;
using System.Collections;

public class WhirlygigPuzzlePiece : PuzzlePiece{

	public override void ProgressPuzzle ()
	{	
		Instantiate (Resources.Load<GameObject> ("Prefabs/DemoCube"));
		Destroy (transform.parent.gameObject);
		base.ProgressPuzzle ();
	}
}
