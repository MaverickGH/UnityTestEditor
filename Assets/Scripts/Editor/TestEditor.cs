using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


[CustomEditor(typeof(TestScript))]
public class TestEditor : Editor  {

	static CityViewSetSerialized _cityView;

	static XmlDocument _document;

	static string _xmlName = "city_view.xml";
	static string _fileName = "city_view";

	private string selectedBackgroundItemAlias;
	private string selectedConstructionItemAlias;
	private string selectedConstructionId;
	private int selectedBackgroundItem;
	private int selectedConstruction;
	private int selectedConstructionItem;


	private string infoString = "Info box";

	public void OnEnable()
	{

	}

	private void ScreenWordPoint(TestScript myTarget)
	{
		if (myTarget.isScreenPoint != true) {

			EditorGUILayout.LabelField ("Offset X Y:", myTarget.screenPos.x + " " + myTarget.screenPos.y);

			myTarget.screenPos.x = (int)Camera.main.WorldToScreenPoint (myTarget.transform.position).x;
			myTarget.screenPos.y = (int)Camera.main.WorldToScreenPoint (myTarget.transform.position).y;

		} else {
			myTarget.screenPos = EditorGUILayout.Vector2Field ("Offset X Y:", myTarget.screenPos);

			Vector3 thisPosition = new Vector3 (Camera.main.ScreenToWorldPoint (myTarget.screenPos).x,
				Camera.main.ScreenToWorldPoint (myTarget.screenPos).y, 0);

			myTarget.transform.position = thisPosition;
		}
	}

	private void BackgroundLoadSave(TestScript myTarget)
	{
		selectedBackgroundItemAlias = myTarget.name;
		LoadFile ();
		ScreenWordPoint (myTarget);

		GUILayout.BeginHorizontal();        

		if(GUILayout.Button("Save XML file"))
		{
			_cityView.Background [0].items [selectedBackgroundItem].stingItemOffset = string.Format ((int)myTarget.screenPos.x +
				" " + (int)myTarget.screenPos.y);

			SaveFile();
		}

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();        

		if(GUILayout.Button("Load XML file"))
		{

			int index = _cityView.Background [0].items [selectedBackgroundItem].stingItemOffset.IndexOf (" ");
			myTarget.screenPos.x = int.Parse (_cityView.Background [0].items [selectedBackgroundItem].stingItemOffset.Substring (0, index));
			myTarget.screenPos.y = int.Parse (_cityView.Background [0].items [selectedBackgroundItem].stingItemOffset.Substring (index));

			Vector3 loadPosition = new Vector3 (Camera.main.ScreenToWorldPoint(myTarget.screenPos).x,
				Camera.main.ScreenToWorldPoint(myTarget.screenPos).y, 0);

			myTarget.transform.position =  loadPosition;
		}

		GUILayout.EndHorizontal();

		myTarget.isScreenPoint = EditorGUILayout.Toggle ("is Screen Point:", myTarget.isScreenPoint);
	}

	private void ConstructionLoadSave(TestScript myTarget)
	{
		selectedConstructionId = myTarget.name;
		LoadFile ();
		ScreenWordPoint (myTarget);

		GUILayout.BeginHorizontal();        

		if(GUILayout.Button("Save XML file"))
		{
			_cityView.Construction [selectedConstruction].left = (int)myTarget.screenPos.x;
			_cityView.Construction [selectedConstruction].top = (int)myTarget.screenPos.y;

			SaveFile();
		}

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();        

		if(GUILayout.Button("Load XML file"))
		{
			myTarget.screenPos.x = _cityView.Construction [selectedConstruction].left;
			myTarget.screenPos.y = _cityView.Construction [selectedConstruction].top;

			Vector3 loadPosition = new Vector3 (Camera.main.ScreenToWorldPoint(myTarget.screenPos).x,
				Camera.main.ScreenToWorldPoint(myTarget.screenPos).y, 0);

			myTarget.transform.position =  loadPosition;
		}

		GUILayout.EndHorizontal();

		myTarget.isScreenPoint = EditorGUILayout.Toggle ("is Screen Point:", myTarget.isScreenPoint);
	}

	private void ItemLoadSave(TestScript myTarget)
	{
		selectedConstructionItemAlias = myTarget.name;
		selectedConstructionId = myTarget.gameObject.transform.parent.gameObject.name;
		LoadFile ();

		//EditorGUILayout.LabelField ("Xml Offset X:", myTarget.screenPos.x.ToString ());
		//EditorGUILayout.LabelField ("Xml Offset Y:", myTarget.screenPos.y.ToString ());

		GUILayout.BeginHorizontal();        

		if(GUILayout.Button("Save XML file"))
		{
			_cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].stingItemOffset = 
				string.Format ((int)myTarget.transform.localPosition.x + " " + (int)myTarget.transform.localPosition.y);

			SaveFile();
		}

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();        

		if(GUILayout.Button("Load XML file"))
		{
			int index = _cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].stingItemOffset.IndexOf (" ");
			myTarget.screenPos.x = int.Parse (_cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].stingItemOffset.Substring (0, index));
			myTarget.screenPos.y = int.Parse (_cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].stingItemOffset.Substring (index));

			Vector3 loadPosition = new Vector3 (myTarget.screenPos.x,
				myTarget.screenPos.y, 0);

			myTarget.transform.localPosition =  loadPosition;
		}

		GUILayout.EndHorizontal();

	//	myTarget.isScreenPoint = EditorGUILayout.Toggle ("is Screen Point:", myTarget.isScreenPoint);
	}

	public override void OnInspectorGUI()
	{
		TestScript myTarget = (TestScript)target;

		if (myTarget.tag == "Background" && myTarget.enabled == true) {
			BackgroundLoadSave (myTarget);
		} 
		else if (myTarget.tag == "Construction" && myTarget.enabled == true) {
			ConstructionLoadSave(myTarget);
		}
		else if(myTarget.tag == "Item" && myTarget.enabled == true)
		{
			ItemLoadSave (myTarget);
		}
	}

	private void SaveFile()
	{

		XmlSerializer serializer = new XmlSerializer(typeof(CityViewSetSerialized));

		FileStream stream = new FileStream(Application.dataPath + @"/Resources/city_view.xml", FileMode.Create);

		serializer.Serialize(stream, _cityView);


		stream.Close();


		UnityEditor.AssetDatabase.Refresh ();

		infoString = "File saved";

	}

	private void LoadFile()
	{
		_document = new XmlDocument();

		TextAsset text = new TextAsset();
		text = (TextAsset)Resources.Load(_fileName, typeof(TextAsset));

		_document.LoadXml(text.text);

		XmlSerializer serializer = new XmlSerializer(typeof(CityViewSetSerialized));
		XmlReader reader = new XmlNodeReader(_document.ChildNodes[1]);


		_cityView = (CityViewSetSerialized)serializer.Deserialize(reader);

		selectedConstruction = _cityView.GenerateConstructionList(selectedConstructionId);
		selectedBackgroundItem = _cityView.GenerateBackgroundItemList (0,selectedBackgroundItemAlias);
		//selectedConstructionItem = _cityView.GenerateConstructionListItem (selectedConstructionId,selectedBackgroundItemAlias);
		selectedConstructionItem = _cityView.GenerateConstructionListItem (selectedConstruction,selectedConstructionItemAlias);

		infoString = "File loaded";
	}
}
