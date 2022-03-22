﻿using System.Collections.Generic;
using System.ComponentModel;
using Northwoods.Go;
using Northwoods.Go.Models;
using Northwoods.Go.Tools;
using Northwoods.Go.Tools.Extensions;

namespace WinFormsExtensionControls.NodeLabelDragging {
  [ToolboxItem(false)]
  public partial class NodeLabelDraggingControl : System.Windows.Forms.UserControl {
    private Diagram myDiagram;
    
    public NodeLabelDraggingControl() {
      InitializeComponent();

      saveLoadModel1.SaveClick += (e, obj) => SaveModel();
      saveLoadModel1.LoadClick += (e, obj) => LoadModel();

      goWebBrowser1.Html = @"
        <p>
          This sample is a modification of the <a href=""StateChart"">State Chart</a> sample
          that makes use of the NodeLabelDraggingTool that is defined in its own file,
          as <a href=""https://github.com/NorthwoodsSoftware/GoDiagram/blob/main/Extensions/Tools/NodeLabelDragging/NodeLabelDraggingTool.cs"">NodeLabelDraggingTool.cs</a>.
        </p>
 
        <p>
          Note that after dragging a node label you can move that node and the label maintains the same position relative to the node.
          That relative position is specified by the <a>GraphObject.Alignment</a> property, used by the ""Spot"" <a>Panel</a>.
          This sample also saves any changes to that property by means of a TwoWay <a>Binding</a>.
        </p>
";

      saveLoadModel1.ModelJson = @"{
  ""NodeKeyProperty"": ""Id"",
  ""NodeDataSource"": [
    { ""Id"": ""0"", ""Loc"": ""120 120"", ""Text"": ""Initial"" },
    { ""Id"": ""1"", ""Loc"": ""330 120"", ""Text"": ""First down"" },
    { ""Id"": ""2"", ""Loc"": ""226 376"", ""Text"": ""First up"" },
    { ""Id"": ""3"", ""Loc"": ""60 276"", ""Text"": ""Second down"" },
    { ""Id"": ""4"", ""Loc"": ""226 226"", ""Text"": ""Wait"" }
  ],
  ""LinkDataSource"": [
    { ""From"": ""0"", ""To"": ""0"", ""Text"": ""up or timer"", ""Curviness"": -20 },
    { ""From"": ""0"", ""To"": ""1"", ""Text"": ""down"", ""Curviness"": 20 },
    { ""From"": ""1"", ""To"": ""1"", ""Text"": ""down"", ""Curviness"": -20 },
    { ""From"": ""1"", ""To"": ""2"", ""Text"": ""up (no move)"" },
    { ""From"": ""1"", ""To"": ""4"", ""Text"": ""timer"" },
    { ""From"": ""2"", ""To"": ""0"", ""Text"": ""timer\nPOST"" },
    { ""From"": ""2"", ""To"": ""3"", ""Text"": ""down"" },
    { ""From"": ""3"", ""To"": ""0"", ""Text"": ""up\nPOST\n(dblclick\nif no move)"" },
    { ""From"": ""3"", ""To"": ""3"", ""Text"": ""down or timer"", ""Curviness"": 20 },
    { ""From"": ""4"", ""To"": ""0"", ""Text"": ""up\nPOST"" },
    { ""From"": ""4"", ""To"": ""4"", ""Text"": ""down"" }
  ]
}";

      Setup();
    }

    private void Setup() {

      myDiagram = diagramControl1.Diagram;

      // diagram properties
      myDiagram.ToolManager.MouseWheelBehavior = WheelMode.Zoom;
      myDiagram.ToolManager.ClickCreatingTool.ArchetypeNodeData =
        new NodeData { Text = "new node" };
      myDiagram.UndoManager.IsEnabled = true;

      // install the NodeLabelDraggingTool as a mouse-move tool
      myDiagram.ToolManager.MouseMoveTools.Insert(0, new NodeLabelDraggingTool());

      // when the document is modified, add a "*" to the title and enable the "Save" button
      // TODO

      // make a textblock with ad-hoc property
      var textblock =
        new TextBlock {
          Font = new Font("Arial", 11, FontWeight.Bold),
          Editable = true, // editing the text automatically updates the model data
          Cursor = "move" // visual hint that the user can do something with this node label
        }.Bind(
          new Binding("Text", "Text").MakeTwoWay(),
          // the GraphObject.Alignment property is what the NodelabelDraggingTool modifies
          // This TwoWay binding saves any changes to the same named property on the node data
          new Binding("Alignment", "Alignment", Spot.Parse).MakeTwoWay(Spot.Stringify)
        );
      textblock["_IsNodeLabel"] = true;


      // define the node template
      myDiagram.NodeTemplate =
        new Node(PanelLayoutSpot.Instance) {
          LocationElementName = "ICON",
          LocationSpot = Spot.Center,
          SelectionElementName = "ICON"
        }
        .Bind(new Binding("Location", "Loc", Point.Parse).MakeTwoWay(Point.Stringify))
        .Add(new Shape {
          Figure = "RoundedRectangle",
          Name = "ICON",
          Parameter1 = 10, // corner has a medium radius
          DesiredSize = new Size(40, 40),
          // TODO: make gradient when gradient brushes are working
          Fill = "orange",
          Stroke = "black",
          PortId = "",
          FromLinkable = true,
          FromLinkableSelfNode = true,
          FromLinkableDuplicates = true,
          ToLinkable = true,
          ToLinkableDuplicates = true,
          ToLinkableSelfNode = true,
          Cursor = "pointer"
        },
          new Shape { // provide interior area where the user can grab the node
            Fill = "transparent",
            Stroke = (Brush)null,
            DesiredSize = new Size(30, 30),
          },
          textblock
        );

      void AddNodeAndLink(InputEvent e, GraphObject obj) {
        var adorn = obj.Part as Adornment;
        var fromNode = adorn.AdornedPart;
        if (fromNode == null) return;

        e.Handled = true;
        var diagram = e.Diagram;
        diagram.StartTransaction("Add State");

        // get the node data for which the user clicked the button
        var fromData = fromNode.Data as NodeData;
        Point p = fromNode.Location.Offset(200, 0);
        // create a new "state" data object, positioned to the right of the adorned node
        var toData = new NodeData { Text = "new", Loc = Point.Stringify(p) };
        var model = diagram.Model as Model;
        model.AddNodeData(toData);

        // create a link data from the old node data to the new node data
        var linkdata = new LinkData {
          From = fromData.Id,
          To = toData.Id,
          Text = "transition"
        };
        // add the link data to model
        model.AddLinkData(linkdata);

        // select new node
        var newnode = diagram.FindNodeForData(toData);
        diagram.Select(newnode);

        diagram.CommitTransaction("Add State");

        // if new node is off-screen, scroll to show new node
        if (newnode != null) diagram.ScrollToRect(newnode.ActualBounds);
      }

      // make button for selection adornment template
      var button = Builder.Make<Panel>("Button").Add(
        new Shape {
          Figure = "PlusLine",
          DesiredSize = new Size(6, 6)
        }
      );
      button.Click = AddNodeAndLink;
      button.Alignment = Spot.TopRight;

      // unlike the normal selection adornment, this one includes a Button
      myDiagram.NodeTemplate.SelectionAdornmentTemplate =
        new Adornment(PanelLayoutSpot.Instance).Add(
          new Panel(PanelLayoutAuto.Instance).Add(
            new Shape {
              Fill = (Brush)null,
              Stroke = "blue",
              StrokeWidth = 2
            },
            new Placeholder() // this represents the selected node
          ),
          // the button to create a "next" node, at the top-right corner
          button
        );

      var colorStops = new Dictionary<float, string> {
        { 0, "rgb(240, 240, 240)" },
        { 0.3f, "rgb(240, 240, 240)" },
        { 1, "rgba(0, 240, 240, 0)" }
      };
      var paint = new RadialGradientPaint(colorStops);

      // link template
      myDiagram.LinkTemplate =
        new Link {
          Curve = LinkCurve.Bezier,
          Adjusting = LinkAdjusting.Stretch,
          Reshapable = true
        }
        .Bind(new Binding("Points").MakeTwoWay(),
          new Binding("Curviness"))
        .Add(new Shape { // the link shape
          StrokeWidth = 1.5
        },
          new Shape { // the arrowhead
            ToArrow = "standard",
            Stroke = (Brush)null
          },
          new Panel(PanelLayoutAuto.Instance)
            .Add(
              new Shape {
                Fill = new Brush(paint),
                Stroke = (Brush)null
              },
              new TextBlock() {
                Text = "transition",
                TextAlign = TextAlign.Center,
                Font = new Font("Arial", 10, FontWeight.Bold),
                Stroke = "black",
                Margin = 4,
                Editable = true
              }.Bind(new Binding("Text", "Text").MakeTwoWay())
            )
        );

      LoadModel();
    }

    private void SaveModel() {
      if (myDiagram == null) return;
      saveLoadModel1.ModelJson = myDiagram.Model.ToJson();
    }

    private void LoadModel() {
      if (myDiagram == null) return;
      myDiagram.Model = Model.FromJson<Model>(saveLoadModel1.ModelJson);
      myDiagram.Model.UndoManager.IsEnabled = true;
    }

  }

  // model data
  public class Model : GraphLinksModel<NodeData, string, object, LinkData, string, string> { }
  public class NodeData : Model.NodeData {
    public string Id { get; set; }
    public string Loc { get; set; }
    public string Alignment { get; set; }
  }
  public class LinkData : Model.LinkData {
    public List<Point> Points { get; set; }
    public double? Curviness { get; set; }
  }

}
