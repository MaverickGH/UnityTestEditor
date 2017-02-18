using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class CityViewEditor : EditorWindow {

	static CityViewSetSerialized _cityView;

	static XmlDocument _document;

	static string _xmlName = "city_view.xml";
	static string _fileName = "city_view";

	private int selectedConstruction;
	private int selectedBackgroundItem;
	private int selectedConstructionItem;
	private string infoString = "Info box";

	static CityViewEditor window;

	[MenuItem("Custom Editor/City View Editor")]
	static void Init(){
		window = (CityViewEditor)EditorWindow.GetWindow (typeof(CityViewEditor));
		window.title = "CityView Editor";
		window.autoRepaintOnSceneChange = true;
	}

	private void OnEnable(){
		LoadFile ();
	}

	private void OnGUI(){

		GUILayout.BeginHorizontal ();

		selectedBackgroundItem = EditorGUILayout.Popup ("Background Items: ", selectedBackgroundItem, _cityView.ItemBackground,GUILayout.MinWidth(100), GUILayout.MaxWidth(400));

		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();

		_cityView.Background[0].items[selectedBackgroundItem].stingItemOffset = EditorGUILayout.TextField("Offset X Y:", _cityView.Background[0].items[selectedBackgroundItem].stingItemOffset);

		//_cityView.Background[0].items[selectedBackgroundItem].itemOffset.x = EditorGUILayout.IntField("Offset X:", _cityView.Background[0].items[selectedBackgroundItem].itemOffset.x);

		//_cityView.Background[0].items[selectedBackgroundItem].itemOffset.y = EditorGUILayout.IntField("Offset Y:", _cityView.Background[0].items[selectedBackgroundItem].itemOffset.y);

		//_cityView.Background [0].items [selectedBackgroundItem].stingItemOffset = string.Format (_cityView.Background [0].items [selectedBackgroundItem].itemOffset.x + " " + _cityView.Background [0].items [selectedBackgroundItem].itemOffset.y);

		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();

		selectedConstruction = EditorGUILayout.Popup ("Constructions: ", selectedConstruction, _cityView.ConstructionValue,GUILayout.MinWidth(100), GUILayout.MaxWidth(400));

		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();

		_cityView.Construction[selectedConstruction].left  = EditorGUILayout.IntField("Left:", _cityView.Construction[selectedConstruction].left);

		_cityView.Construction[selectedConstruction].top  = EditorGUILayout.IntField("Top:", _cityView.Construction[selectedConstruction].top);

		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();

	
		selectedConstructionItem = EditorGUILayout.Popup ("Construction Layer Items: ", selectedConstructionItem, _cityView.ItemConstruction,GUILayout.MinWidth(100), GUILayout.MaxWidth(400));
		_cityView.GenerateConstructionListItem (selectedConstruction);

		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ();
		try {
			_cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].stingItemOffset = EditorGUILayout.TextField("Offset X Y:", _cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].stingItemOffset);
		} catch (System.Exception ex) {
			selectedConstructionItem = 0;
		}
			
		if(_cityView.Construction[selectedConstruction].Layer[0].stingItemOffset != null)
			_cityView.Construction[selectedConstruction].Layer[0].stingItemOffset = EditorGUILayout.TextField("Layer Offset X Y:", _cityView.Construction[selectedConstruction].Layer[0].stingItemOffset);
		//_cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].itemOffset.x = EditorGUILayout.IntField("Offset X:", _cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].itemOffset.x);

		//_cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].itemOffset.y = EditorGUILayout.IntField("Offset Y:", _cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].itemOffset.y);

		//_cityView.Construction[selectedConstruction].Layer[0].items [selectedConstructionItem].stingItemOffset = string.Format (_cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].itemOffset.x + " " + _cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].itemOffset.y);

		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal();        

		if(GUILayout.Button("Reload XML file"))
		{
			LoadFile();
		}

		if(GUILayout.Button("Update XML file"))
		{
			TestScript[] citys = FindObjectsOfType(typeof(TestScript)) as TestScript[];
			Vector3 myTarget = Vector3.zero;

			foreach (TestScript city in citys) {
				
				if (city.gameObject.tag == "Background") {
					string selectedBackgroundItemAlias = city.gameObject.name;
					int selectedBItem = _cityView.GenerateBackgroundItemList (0, selectedBackgroundItemAlias);

					int index = _cityView.Background [0].items [selectedBItem].stingItemOffset.IndexOf (" ");
				
					myTarget.x = int.Parse (_cityView.Background [0].items [selectedBItem].stingItemOffset.Substring (0, index));
					myTarget.y = int.Parse (_cityView.Background [0].items [selectedBItem].stingItemOffset.Substring (index));

					city.gameObject.GetComponent<TestScript> ().screenPos = myTarget;
			
					Vector3 loadPosition = new Vector3 (Camera.main.ScreenToWorldPoint (myTarget).x,
						                       Camera.main.ScreenToWorldPoint (myTarget).y, 0);

					city.gameObject.transform.position = loadPosition;

				} else if (city.gameObject.tag == "Construction") {
					string selectedConstructionId = city.gameObject.name;
					int selectedC = _cityView.GenerateConstructionList(selectedConstructionId);

					myTarget.x = _cityView.Construction [selectedC].left;
					myTarget.y = _cityView.Construction [selectedC].top;

					city.gameObject.GetComponent<TestScript> ().screenPos = myTarget;

					Vector3 loadPosition = new Vector3 (Camera.main.ScreenToWorldPoint(myTarget).x,
						Camera.main.ScreenToWorldPoint(myTarget).y, 0);

					city.gameObject.transform.position =  loadPosition;

				}
				else if(city.gameObject.tag == "Item")
				{
					string selectedConstructionItemAlias = city.gameObject.name;
					string selectedConstructionId = city.gameObject.transform.parent.gameObject.name;

					int selectedC = _cityView.GenerateConstructionList(selectedConstructionId);
					int selectedConstructionItems = _cityView.GenerateConstructionListItem (selectedC,selectedConstructionItemAlias);

					int index = _cityView.Construction[selectedC].Layer[0].items[selectedConstructionItems].stingItemOffset.IndexOf (" ");
					myTarget.x = int.Parse (_cityView.Construction[selectedC].Layer[0].items[selectedConstructionItems].stingItemOffset.Substring (0, index));
					myTarget.y = int.Parse (_cityView.Construction[selectedC].Layer[0].items[selectedConstructionItems].stingItemOffset.Substring (index));

					Vector3 loadPosition = new Vector3 (myTarget.x,
						myTarget.y, 0);

					city.gameObject.transform.localPosition =  loadPosition;
				}
			}
		}

		GUILayout.EndHorizontal();      

		GUILayout.BeginHorizontal();        

		if(GUILayout.Button("Save XML file"))
		{
			SaveFile();
			_cityView.GenerateConstructionList();
			selectedBackgroundItem = 0;
			_cityView.GenerateBackgroundItemList (selectedBackgroundItem);
			_cityView.GenerateConstructionListItem (selectedConstruction);
		}

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();        

		EditorGUILayout.HelpBox(infoString,MessageType.Info);

		GUILayout.EndHorizontal();
	
	}

	private void SaveFile()
	{
		
		XmlSerializer serializer = new XmlSerializer(typeof(CityViewSetSerialized));

		FileStream stream = new FileStream(Application.dataPath + @"/Resources/city_view.xml", FileMode.Create);

		serializer.Serialize(stream, _cityView);

		stream.Close();
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
		_cityView.GenerateConstructionList();
		_cityView.GenerateBackgroundItemList (selectedBackgroundItem);
		_cityView.GenerateConstructionListItem (selectedConstruction);
		infoString = "File loaded";
	}
}
