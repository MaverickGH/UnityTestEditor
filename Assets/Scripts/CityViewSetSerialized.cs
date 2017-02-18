using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

using System.IO;


[XmlRoot(ElementName="CityView")]
public class CityViewSetSerialized {

	private string _cityViewId = "EGYPT";

	[XmlElement("Background")]
	public List<BackgroundSerialized> Background;


	[XmlArray("Constructions"), XmlArrayItem("Construction",typeof(ConstructionSerialized))]
	public List<ConstructionSerialized> Construction;


	[XmlIgnore]
	public string[] ConstructionValue;

	[XmlIgnore]
	public string[] ItemBackground;

	[XmlIgnore]
	public int ItemBackgroundAlias;

	[XmlIgnore]
	public string[] ItemConstruction;

	public CityViewSetSerialized()
	{
		Construction = new List<ConstructionSerialized>();
		Background = new List<BackgroundSerialized> ();
	}
		
	[XmlAttribute]
	public string id{
		get {return _cityViewId; }
		set{ _cityViewId = value;}
	}

	public void GenerateBackgroundItemList(int selected)
	{
		List<string> temp = new List<string> ();

		foreach (Items item in Background[selected].items) {
			temp.Add (item.itemAlias);
		}
		ItemBackground = temp.ToArray ();
	}

	public int GenerateBackgroundItemList(int selected,string alias)
	{
		int i=0;
		foreach (Items item in Background[selected].items) {
			
			if (item.itemAlias == alias) {
				return i;
			}
			i++;
		}
		return 0;
	}

	public void GenerateConstructionListItem(int selected)
	{
		List<string> temp = new List<string> ();

		foreach (Items item in Construction[selected].Layer[0].items) {
			temp.Add (item.itemAlias);
		}
		ItemConstruction = temp.ToArray ();
	}

	public int GenerateConstructionListItem(int selected,string alias)
	{
		//int i=0;
		int j = 0;

	/*	foreach (ConstructionSerialized item in Construction) {
			if (item.id == selected) {
				break;
			}
			i++;
		}*/
			
		foreach (Items item in Construction[selected].Layer[0].items) {
			if (item.itemAlias == alias) {
				return j;
			}
			j++;
		}

		return 0;
	}

	public void GenerateConstructionList()
	{
		List<string> temp = new List<string> ();

		foreach (ConstructionSerialized item in Construction) {
			temp.Add (item.id);
		}
		ConstructionValue = temp.ToArray ();
	}
	public int GenerateConstructionList(string id)
	{
		int i=0;

		foreach (ConstructionSerialized item in Construction) {
			if (item.id == id) {
				return i;
			}
			i++;
		}
		return 0;
	}
}	

public class BackgroundSerialized
{
	private string _id; 

	//[XmlElement("Layer")]
	[XmlArray("Layer"), XmlArrayItem("Item",typeof(Items))]
	public List<Items> items;

	public BackgroundSerialized()
	{
		items = new List<Items> ();
	}

	[XmlAttribute]
	public string id{
		get {return _id; }
		set{ _id = value;}
	}
}

public class ConstructionSerialized
{
	private string _id; 
	private string _type; 
	private string _openSound;
	private int _left; 
	private int _top; 

	[XmlArray("Layers"), XmlArrayItem("Layer",typeof(ItemSerialized2))]
	public List<ItemSerialized2> Layer;

	public ConstructionSerialized()
	{
		Layer = new List<ItemSerialized2> ();
	}
		
	[XmlAttribute]
	public string id{
		get {return _id; }
		set{ _id = value;}
	}
		
	[XmlAttribute]
	public string type{
		get {return _type; }
		set{ _type = value;}
	}

	[XmlAttribute]
	public string open_sound{
		get {return _openSound; }
		set{ _openSound = value;}
	}

	[XmlAttribute]
	public int left{
		get {return _left; }
		set{ _left = value;}
	}

	[XmlAttribute]
	public int top{
		get {return _top; }
		set{ _top = value;}
	}
}
	
/*public class BackgroundItemSerialized
{
	[XmlElement("Item")]
	public List<Items> items;

	public BackgroundItemSerialized()
	{
		items = new List<Items> ();
	}
}*/
public class ItemSerialized2
{
	private int _upgrade = 1; 

	[XmlElement("Item")]
	public List<Items> items;

	[XmlIgnore]
	public ItemOffset itemOffset;

	[XmlAttribute("offset")]
	public string stingItemOffset;

	[XmlAttribute]
	public int upgrade{
		get {return _upgrade; }
		set{ _upgrade = value;}
	}
	public ItemSerialized2()
	{
		items = new List<Items> ();
	}

	public ItemSerialized2(ItemOffset itemOffset)
	{
		items = new List<Items> ();
		stingItemOffset = itemOffset.ToString();
		this.itemOffset = itemOffset;
	}


}

public class Items
{
	[XmlAttribute("type")]
	public string itemType;

	[XmlAttribute("alias")]
	public string itemAlias;


	[XmlIgnore]
	public ItemOffset itemOffset;

	[XmlAttribute("offset")]
	public string stingItemOffset;

	public Items(){
	}

	public Items(string itemType,string itemAlias,ItemOffset itemOffset)
	{
		this.itemType = itemType;
		this.itemAlias = itemAlias;
		stingItemOffset = itemOffset.ToString();
		this.itemOffset = itemOffset;
	}
}

public struct ItemOffset
{
	
	public int x;

	public int y;

	public ItemOffset(int x,int y){
		this.x = x;
		this.y = y;
	}

	public override string ToString ()
	{
		return string.Format (x + " " + y);
	}
}