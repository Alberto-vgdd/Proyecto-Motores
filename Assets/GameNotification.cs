using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameNotification {

	public string infoText;
	public Color infoColor;
	public int infoSize;

	public GameNotification(string text, Color color, int size)
	{
		this.infoText = text;
		this.infoColor = color;
		this.infoSize = size;
	}
}
