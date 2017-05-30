using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ManageRandomTokens : MonoBehaviour {
	public static ManageRandomTokens randomTokenManager;
	public Map gameMap;
	public delegate void PlayerChange(Token t);
	public static event PlayerChange OnPlayerChange;
	// Use this for initialization
	public GameObject flatToken;
	public Texture[] characterTextures;
	public Token token;
	Renderer tokenRenderer;
	List<Token> totalTokens;
	bool done;
	void Awake()
	{
		if (randomTokenManager == null) {
			DontDestroyOnLoad(gameObject);
			randomTokenManager=this;
		} else if(randomTokenManager!=this) {
			Destroy(gameObject);
		}
	}

	void Start () {
		gameMap = Map.map;
		totalTokens = new List<Token> ();
		//SpawnTokens ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void RespawnTokens()
	{
		WipeTokens ();
		SpawnTokens ();

	}
	void WipeTokens()
	{
		foreach (Token t in totalTokens) {
			GameObject.DestroyObject(t.gameObject);
		}
		totalTokens.Clear ();
	}
	public void SpawnTokens()
	{
		int numberOfTokens = UnityEngine.Random.Range (1, 7);
		List<int> indexes = new List<int> (){0,1,2,3,4,5,6,7};
		
		for (int i =0; i<numberOfTokens; i++) {
			
			AxialCoord tempPos = gameMap.RandomHex();
			//Vector3 tokenPos= AxialCoord.WorldPositionFromAxial(tempPos);
			//tokenPos.z = GameController.gameControl.TraversalHeight(tempPos);
			
			Token obj = Instantiate(token) as Token;
			//Characters c = (Characters)	Enum.GetValues(typeof(Characters)).GetValue(UnityEngine.Random.Range(0, (int)Characters.TheWraith));
			int characterIndex = indexes[ UnityEngine.Random.Range(0, indexes.Count)];
			obj.SetCharacterName((Characters)	Enum.GetValues(typeof(Characters)).GetValue(characterIndex));
			indexes.Remove(characterIndex);
			//SetMaterialOnToken(obj);
			obj.SetPosition(tempPos);
			obj.turnOrder = i;
			//obj.transform.position = tokenPos;
			if(i==0)
			{
				obj.gameObject.tag = "Player";


			}
			totalTokens.Add(obj);
			OnPlayerChange(totalTokens[0]);
		}
		done = true;
	}

	void SetMaterialOnToken(GameObject g)
	{
		tokenRenderer = g.GetComponent<Renderer> ();
		if (tokenRenderer != null) {
			tokenRenderer.material.mainTexture = characterTextures[(int) UnityEngine.Random.Range(0,characterTextures.Length)];
			//Material[] rendererMats = tokenRenderer.materials
		}
	}
	public void NextTurn()
	{
		int currentTurn = totalTokens.FindIndex(t => t.tag == "Player");
		totalTokens [currentTurn].tag = "Untagged";
		totalTokens [(currentTurn + 1) % totalTokens.Count].tag = "Player";
		OnPlayerChange(totalTokens [(currentTurn + 1) % totalTokens.Count]);
	}
	public List<Token> ShowTokens()
	{
		//List<Token> resultList = new List<Token> ();
		return totalTokens;
		//return resultList;
	}
}
