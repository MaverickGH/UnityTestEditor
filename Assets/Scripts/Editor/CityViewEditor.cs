using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Timers;


public class CityViewEditor : EditorWindow {

	static CityViewSetSerialized _cityView;

	static XmlDocument _document;

	static string _fileName = "city_view";


	private ChangingOffset[] citys;
	private Vector3 myTarget;

	private int selectedConstruction;
	private int selectedBackgroundItem;
	private int selectedConstructionItem;
	private string selectedBackgroundItemAlias="";
	private string selectedConstructionId="";
	private string selectedConstructionItemAlias ="";

	private string infoString = "Info box";
	private string updateString = "Update scenes!";
	private string backgroundTagString = "Background";
	private string constructionTagString = "Construction";
	private string itemTagString = "Item";

	private static GUIContent cityViewGUIContent = new GUIContent(LocalString("CityView Editor")
		,LocalString("CityView Editor XML position."));

	static CityViewEditor window;


	[MenuItem("Custom Editor/City View Editor")]
	static void Init(){
		window = (CityViewEditor)EditorWindow.GetWindow (typeof(CityViewEditor));
		window.titleContent = cityViewGUIContent;
		window.autoRepaintOnSceneChange = true;

	}
		

	private void OnEnable(){
		LoadFile ();
	}

	private static string LocalString(string text) {
		return LocalizationDatabase.GetLocalizedString(text);
	}
		
	private void OnGUI(){

		GUILayout.BeginHorizontal();

		if (GUILayout.Button ("Save XML position")) {
			LoadFile ();
			foreach (ChangingOffset city in citys) {

				if (city.gameObject.tag == backgroundTagString) {
					
					selectedBackground (city);

					_cityView.Background [0].items [selectedBackgroundItem].stingItemOffset = 
						string.Format ((int)city.screenPos.x + " " + (int)city.screenPos.y);
					
				} 
				else if (city.gameObject.tag == constructionTagString) {
					
					selectedConstructions (city);

					_cityView.Construction [selectedConstruction].left = (int)city.screenPos.x;
					_cityView.Construction [selectedConstruction].top = (int)city.screenPos.y;

				}
				else if (city.gameObject.tag == itemTagString) {
					
					selectedItem (city);

					_cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].stingItemOffset = 
						string.Format ((int)city.gameObject.transform.localPosition.x + 
							" " + (int)city.gameObject.transform.localPosition.y);
				}
			}
			SaveFile ();
		}

		if(GUILayout.Button("Update CityView position"))
		{
			UnityEditor.AssetDatabase.Refresh ();
			LoadFile ();
			foreach (ChangingOffset city in citys) {
				
				if (city.gameObject.tag == backgroundTagString) {
					
					selectedBackground (city);

					int index = _cityView.Background [0].items [selectedBackgroundItem].stingItemOffset.IndexOf (" ");
				
					myTarget.x = int.Parse (_cityView.Background [0].items [selectedBackgroundItem].stingItemOffset.Substring (0, index));
					myTarget.y = int.Parse (_cityView.Background [0].items [selectedBackgroundItem].stingItemOffset.Substring (index));

					city.gameObject.GetComponent<ChangingOffset> ().screenPos = myTarget;
			
					city.ConvertScreenToWorldPoint (myTarget);

				} else if (city.gameObject.tag == constructionTagString) {
					
					selectedConstructions (city);

					myTarget.x = _cityView.Construction [selectedConstruction].left;
					myTarget.y = _cityView.Construction [selectedConstruction].top;

					city.gameObject.GetComponent<ChangingOffset> ().screenPos = myTarget;

					city.ConvertScreenToWorldPoint (myTarget);
				}
				else if(city.gameObject.tag == itemTagString)
				{
					selectedItem (city);

					int index = _cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].stingItemOffset.IndexOf (" ");
					myTarget.x = int.Parse (_cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].stingItemOffset.Substring (0, index));
					myTarget.y = int.Parse (_cityView.Construction[selectedConstruction].Layer[0].items[selectedConstructionItem].stingItemOffset.Substring (index));

					Vector3 tempPosition = new Vector3 (myTarget.x,
						myTarget.y, 0);

					city.gameObject.transform.localPosition =  tempPosition;
				}
			}
			infoString = updateString;
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

		infoString = "File loaded";

		citys = FindObjectsOfType(typeof(ChangingOffset)) as ChangingOffset[];
		myTarget = Vector3.zero;
	}

	private void selectedBackground(ChangingOffset city)
	{
		selectedBackgroundItemAlias = city.gameObject.name;
		selectedBackgroundItem = _cityView.GenerateBackgroundItemList (0, selectedBackgroundItemAlias);
	}

	private void selectedConstructions(ChangingOffset city)
	{
		selectedConstructionId = city.gameObject.name;
		selectedConstruction = _cityView.GenerateConstructionList(selectedConstructionId);
	}

	private void selectedItem(ChangingOffset city)
	{
		selectedConstructionItemAlias = city.gameObject.name;
		selectedConstructionId = city.gameObject.transform.parent.gameObject.name;

		selectedConstruction = _cityView.GenerateConstructionList(selectedConstructionId);
		selectedConstructionItem = _cityView.GenerateConstructionListItem (selectedConstruction,selectedConstructionItemAlias);
	}
}
