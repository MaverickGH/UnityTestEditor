using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class ItemCreator : MonoBehaviour {

	private XmlDocument _document;
	private string _xmlName = "ItemData2.xml";
	private string _fileName = "ItemData2";

	private CityViewSetSerialized _cityViewSet;
	//private BackgroundSerialized _backgroundSerialized;


	// Use this for initialization
	void Start () {

		_cityViewSet = new CityViewSetSerialized ();
		//_backgroundSerialized = new BackgroundSerialized ();
	

	
		BackgroundSerialized _backgroundSerialized = new BackgroundSerialized ();
		_backgroundSerialized.id="CITY_BACKGROUND";

		_backgroundSerialized.items.Add(new Items ("sprites", "city3", new ItemOffset (0, 0)));
		_backgroundSerialized.items.Add(new Items ("sprites", "city4", new ItemOffset (0, 0)));

		ConstructionSerialized _constructionSerialized = new ConstructionSerialized ();
		_constructionSerialized.id = "city_field";
		_constructionSerialized.type = "resource";
		_constructionSerialized.left = 10;
		_constructionSerialized.top = 10;
		_constructionSerialized.open_sound = "int_btn_down";

		ConstructionSerialized _constructionSerialized2 = new ConstructionSerialized ();

		_constructionSerialized2.id = "city_field2";
		_constructionSerialized2.type = "resource";
		_constructionSerialized2.left = 10;
		_constructionSerialized2.top = 10;
		_constructionSerialized2.open_sound = "int_btn_down";

		ItemSerialized2 itemOne = new ItemSerialized2 ();

		itemOne.items.Add(new Items ("sprites", "city", new ItemOffset (0, 0)));
		itemOne.items.Add(new Items ("sprites", "city2", new ItemOffset (0, 0)));

		ItemSerialized2 itemTwo = new ItemSerialized2 (new ItemOffset (10, 0));

		itemTwo.items.Add(new Items ("sprites", "city", new ItemOffset (0, 0)));
		itemTwo.items.Add(new Items ("sprites", "city2", new ItemOffset (0, 0)));

		_constructionSerialized.Layer.Add (itemOne);
		_constructionSerialized2.Layer.Add (itemTwo);

		_cityViewSet.Background.Add (_backgroundSerialized);
		_cityViewSet.Construction.Add (_constructionSerialized);
		_cityViewSet.Construction.Add (_constructionSerialized2);


		SaveFile ();
	}

	// Update is called once per frame
	void Update () {

	}

	private void SaveFile(){

		XmlSerializer serializer = new XmlSerializer (typeof(CityViewSetSerialized));
	
		XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces ();
		namespaces.Add (string.Empty, string.Empty);


		FileStream stream = new FileStream(Application.dataPath + @"/Resources/ItemData2.xml", FileMode.Create);

		serializer.Serialize (stream, _cityViewSet,namespaces);
	
		stream.Close ();
	}

	private void LoadXmlData()
	{
		_document = new XmlDocument();

		TextAsset text = new TextAsset();
		text = (TextAsset)Resources.Load(_fileName, typeof(TextAsset));

		_document.LoadXml(text.text);

		XmlSerializer serializer = new XmlSerializer(typeof(CityViewSetSerialized));
		XmlReader reader = new XmlNodeReader(_document.ChildNodes[1]);

		_cityViewSet = (CityViewSetSerialized)serializer.Deserialize(reader);
	}


}
