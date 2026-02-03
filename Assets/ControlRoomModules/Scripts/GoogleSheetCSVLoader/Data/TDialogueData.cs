using ControlRoom;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TDialogueData : DataForm
{
	public TableType<int> DialogueID;
	public TableType<string> CharacterName;
	public TableType<int> TalkObjectID;
	public TableType<string> DialogueText;
	public TableType<bool> EndDialoue;

	public TDialogueData()
	{
		this.DialogueID = new TableType<int>("DialogueID", this);
		this.TalkObjectID = new TableType<int>("TalkObjectID", this);
		this.CharacterName = new TableType<string>("CharacterName", this);
		this.DialogueText = new TableType<string>("DialogueText", this);
		this.EndDialoue = new TableType<bool>("EndDialogue", this);
	}
}

public class DialogueData : ITableData , IKeyProvider<int>
{
	public int DialogueID;
	public string CharacterName;
	public int talkObjectID;
	public string DialogueText;
	public bool EndDialogue;

	public int GetKey()
	{
		return DialogueID;
	}

	public void SetValue(DataForm dataForm)
	{
		var tData = dataForm as TDialogueData;
		
		if (tData != null)
		{
			this.DialogueID = tData.DialogueID.Value;
			this.CharacterName = tData.CharacterName.Value;
			this.DialogueText = tData.DialogueText.Value;
			this.EndDialogue = tData.EndDialoue.Value;
			this.talkObjectID = tData.TalkObjectID.Value;
		}
		
	}
}

public class DialogueDataManager : TableBaseDataManager<DialogueData,TDialogueData,int>
{
	protected override TableManager.GoogleDocsID currentTableId => TableManager.GoogleDocsID.DIALOGUE;
	
	public DialogueData GetDialogueData(int dialogueID)
	{
		if (dicDatas.TryGetValue(dialogueID, out var dialogueData))
			return dialogueData;

		return null;
	}

}
