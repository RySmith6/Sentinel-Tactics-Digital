using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TokenLoader : MonoBehaviour {

	public static Dictionary<string, Sprite> CharacterTokens;
	public static Dictionary<string, Texture> DeployableTokens;
	// Use this for initialization
	void Awake () {
		CharacterTokens = new Dictionary<string, Sprite> ();
		DeployableTokens = new Dictionary<string, Texture> ();
		Sprite[] tokenSprites = Resources.LoadAll<Sprite> ("CharacterSpriteSheet");
		List<string> tokenNames = new List<string> (System.Enum.GetNames (typeof(Characters)));
		foreach (Sprite s in tokenSprites) {
			
			if (tokenNames.Contains (s.name)) {
				CharacterTokens.Add (s.name, s);
			}
		}
//		pathString = "DeployableTokens";
//		Texture [] resourceDeployables = Resources.LoadAll<Texture> (pathString);
//		foreach (Texture t in resourceDeployables) {
//			DeployableTokens.Add(t.name, t);
//		}
	}
}
