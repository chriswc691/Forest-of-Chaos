#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
[AttributeUsage(AttributeTargets.Class)]
public class vFSMHelpboxAttribute : PropertyAttribute {
    public MessageType messageType;
    public string text;
	public vFSMHelpboxAttribute(string text,MessageType messageType)
    {
        this.text = text;
        this.messageType = messageType;
    }
}
#endif