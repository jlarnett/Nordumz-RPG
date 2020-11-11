using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Cinemachine.Utility;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue SelectedDialogue = null;               //NonSerialized is added to fix issue where root note is being added everytime

        [NonSerialized]
        private GUIStyle nodeStyle;

        [NonSerialized]
        private GUIStyle playerNodeStyle;

        [NonSerialized]
        private DialogueNode draggingNode = null;

        [NonSerialized]
        private Vector2 dragginOffset;

        [NonSerialized]
        private DialogueNode creatingNode = null;

        [NonSerialized]
        private DialogueNode DeletingNode = null;

        [NonSerialized]
        private DialogueNode linkingParentNode = null;

        private Vector2 scrollPosition;

        [NonSerialized] private bool draggingCanvas = false;
        [NonSerialized] private Vector2 draggingCanvasOffset;


        private const float canvasSize = 4000;
        private const float backgroundSize = 50;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;

            if (dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            nodeStyle = new GUIStyle();         //Creates the note styling
            nodeStyle.normal.background = EditorGUIUtility.Load("Node0") as Texture2D;
            nodeStyle.normal.textColor = Color.black;
            nodeStyle.padding = new RectOffset(10, 10, 10, 10);
            nodeStyle.border = new RectOffset(10, 10, 10, 10);

            playerNodeStyle = new GUIStyle();         //Creates the note styling
            playerNodeStyle.normal.background = EditorGUIUtility.Load("Node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.black;
            playerNodeStyle.padding = new RectOffset(10, 10, 10, 10);
            playerNodeStyle.border = new RectOffset(10, 10, 10, 10);
        }

        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;      //We getg the editor selections activeobject and cast it as a new Dialoguew object

            if (newDialogue != null)            //If the value is not null assign dialogue to selected dialogue class variable
            {
                SelectedDialogue = newDialogue;         //Assign selected dialogue to be drawONGUI
                Repaint();      //CALLS ONGUI AUTOMATICALLY TO UPDATE SELECTION
            }
        }

        private void OnGUI()
        {
            //Checks for the selected dialogue in dialogue scriptable object
            if (SelectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");        //If its null
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);           //Assign scrollposition = beginScrollView and we pass in scroll position too it

                Rect canvas = GUILayoutUtility.GetRect(canvasSize, canvasSize);         //Creates a rect call canvas
                Texture2D backgroundTex = Resources.Load("background") as Texture2D;        //Loads our background texture from resources
                Rect texCoords = new Rect(0, 0, canvasSize / backgroundSize, canvasSize / backgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);                                   //Draws the background texture to the canvas rect.

                foreach (DialogueNode node in SelectedDialogue.GetAllNodes())       //Foreach node in the selectetDialogues getallnode method only called by editor
                {
                    DrawConnections(node);  //Calls the DrawConnection function and passes in the node in our loops
                }

                foreach (DialogueNode node in SelectedDialogue.GetAllNodes())       //Foreach node in the selectetDialogues getallnode method only called by editor
                {
                    DrawNode(node);        // Calls the DrawNode method and passes in the node in our loop
                }

                EditorGUILayout.EndScrollView();                                //End scroll View

                if (creatingNode != null)
                {

                    SelectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if (DeletingNode != null)
                {
                    SelectedDialogue.DeleteNote(DeletingNode);
                    DeletingNode = null;
                }
            }
        }



        private void ProcessEvents()            //Handles clicking events and updating dragging bool
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);

                if (draggingNode != null)
                {
                    dragginOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    //Record dragOffset and dragging
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;

                    Selection.activeObject = SelectedDialogue;
                }

            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                draggingNode.SetPosition(Event.current.mousePosition + dragginOffset);             //Sets the node position = mouse position
                GUI.changed = true;                                                                     //Tells the gui something changed and forces a repaint
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                //Update scrollPosition
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)           //if mouse is released
            {
                draggingNode = null;
            }
            else if(Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private void DrawNode(DialogueNode node)
        {

            GUIStyle style = nodeStyle;     

            if (node.IsPlayerSpeaking())            //If this nodes player is speaking change style of node to playerNodeStyle
            {
                style = playerNodeStyle;
            }


            GUILayout.BeginArea(node.GetRect(), style);            //Gets the Rect from the node.position classfield
            node.SetText(EditorGUILayout.TextField(node.GetText()));         //Gets the value from the editorguitextfield and assigns it into a string
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Delete"))     //Creates a button and checks it it was pressed on this call of OnGui
            {
                DeletingNode = node;        //Assigns the node we should create to our dialogueEditor variable. 
            }

            DrawLinkButtons(node);

            if (GUILayout.Button("+"))     //Creates a button and checks it it was pressed on this call of OnGui
            {
                creatingNode = node;        //Assigns the node we should create to our dialogueEditor variable. 
            }


            GUILayout.EndHorizontal();

            GUILayout.EndArea();                //Ends the above BeginArea call
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingParentNode = node;
                }
            }
            else if(linkingParentNode == node)      //If the linking node is = to our current node display cancel button instead of child or link
            {
                if (GUILayout.Button("Cancel"))    
                {
                    linkingParentNode = null;       //Cancels linking?
                }
            }
            else if (linkingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("Unlink"))     //Creates a button and checks it it was pressed on this call of OnGui
                {
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;        //Assigns the node we should create to our dialogueEditor variable. 
                }
            }
            else
            {
                if (GUILayout.Button("Child"))     //Creates a button and checks it it was pressed on this call of OnGui
                {
                    Undo.RecordObject(SelectedDialogue, "Add dialogue Link");
                    linkingParentNode.AddChild(node.name);
                    linkingParentNode = null;        //Assigns the node we should create to our dialogueEditor variable. 
                }
            }

        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);            //Tells the line to be on the right side of parent node and center of y

            foreach (DialogueNode childNode in SelectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);    //Tells the line to be on the left side of child node and center of y

                Vector3 controlPointOffset = endPosition - startPosition;                                        //Create a vector3 to create an offset to create line curvurture
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;

                Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffset, endPosition - controlPointOffset, Color.blue, null, 4f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;

            foreach (DialogueNode node in SelectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(point))
                {
                    foundNode = node;
                }
            }

            return foundNode;
        }
    }
}
