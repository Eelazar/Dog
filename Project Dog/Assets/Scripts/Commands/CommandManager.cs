using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class CommandContext
{
    public BaseObject sender;

    public BaseObject receiver;

    public string[] command;

    public object[] parameters;
}

public class CommandFeedback
{
    public bool valid;

    public string feedback;
}
