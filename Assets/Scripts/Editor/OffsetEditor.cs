using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[CustomEditor(typeof(ChangingOffset))]
public class OffsetEditor : Editor {

	private const float FIELD_WIDTH = 212.0f;
	private const bool WIDE_MODE = true;

	//Ограничение экранных координат 
	private const int POSITION_HEIGHT_MAX = 1261;
	private const int POSITION_WIDTH_MAX = 2048;
	private const int POSITION_MIN= 0;

	private static GUIContent positionGUIContent = new GUIContent(LocalString("Screen position")
		,LocalString("The screen position of this Game Object."));
	private static GUIContent isScreenGUIContent = new GUIContent(LocalString("is Screen Point")
		,LocalString("The bool."));

	private static string positionWarningText = LocalString("Due to int-point precision limitations, it is recommended to bring the screen coordinates of the GameObject within a [0,2048], [0,1261] range or saving the XML document.");

	private SerializedProperty positionProperty;
	private SerializedProperty isScreenProperty;

	private ChangingOffset myTarget;

	bool layoutDone = false;

	private static string LocalString(string text) {
		return LocalizationDatabase.GetLocalizedString(text);
	}
		

	public void OnEnable () {
		this.positionProperty = this.serializedObject.FindProperty("screenPos");
		this.isScreenProperty = this.serializedObject.FindProperty("isScreenPoint");
		myTarget = (ChangingOffset)target;
	}

	public override void OnInspectorGUI() {
		EditorGUIUtility.wideMode = OffsetEditor.WIDE_MODE;
		EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth - OffsetEditor.FIELD_WIDTH; 

		this.serializedObject.Update();

		if (myTarget.tag != "Item") 
			EditorGUILayout.PropertyField(this.isScreenProperty, isScreenGUIContent);

		if (!this.isScreenProperty.boolValue) {
			EditorGUILayout.LabelField ("Screen position",this.positionProperty.vector2Value.ToString());

			myTarget.ConvertWorldToScreenPoint ();

		} else {
			EditorGUILayout.PropertyField(this.positionProperty, positionGUIContent);

			myTarget.ConvertScreenToWorldPoint (this.positionProperty.vector2Value);
		}

		if (!ValidateScenePosition((this.positionProperty.vector2Value))) {
			if ((Event.current.type == EventType.Repaint && layoutDone == true) ||
			    Event.current.type == EventType.Layout) {
				EditorGUILayout.HelpBox (positionWarningText, MessageType.Warning);
			}
			if (Event.current.type == EventType.Layout)
			{        
				layoutDone = true;
			}
		}

		this.serializedObject.ApplyModifiedProperties();
	}

	private bool ValidateScenePosition(Vector2 position) {
		if (position.x > OffsetEditor.POSITION_WIDTH_MAX) {
			myTarget.screenPos.x = POSITION_WIDTH_MAX;
			myTarget.ConvertScreenToWorldPoint (myTarget.screenPos);
			return false;
		}
		if (position.y > OffsetEditor.POSITION_HEIGHT_MAX) {
			myTarget.screenPos.y = POSITION_HEIGHT_MAX;
			myTarget.ConvertScreenToWorldPoint (myTarget.screenPos);
			return false;
		}
		if (position.x < OffsetEditor.POSITION_MIN) {
			myTarget.screenPos.x = POSITION_MIN;
			myTarget.ConvertScreenToWorldPoint (myTarget.screenPos);
			return false;
		}
		if (position.y < OffsetEditor.POSITION_MIN) {
			myTarget.screenPos.y = POSITION_MIN;
			myTarget.ConvertScreenToWorldPoint (myTarget.screenPos);
			return false;
		}

		if (position.x == OffsetEditor.POSITION_MIN || position.x == OffsetEditor.POSITION_WIDTH_MAX
			|| position.y == OffsetEditor.POSITION_MIN || position.y == OffsetEditor.POSITION_HEIGHT_MAX) {
			return false;
		}

		return true;
	}


}
