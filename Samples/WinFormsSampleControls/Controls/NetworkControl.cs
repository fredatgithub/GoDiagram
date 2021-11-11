﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Northwoods.Go;
using Northwoods.Go.Layouts;
using Northwoods.Go.Models;
using Northwoods.Go.Tools;
using Northwoods.Go.WinForms;

namespace WinFormsSampleControls.Network {
  [ToolboxItem(false)]
  public partial class NetworkControl : System.Windows.Forms.UserControl {
    private Diagram myDiagram;
    private Palette myPalette;
    private Overview myOverview;

    private Dictionary<string, Part> sharedNodeTemplateMap;

    public NetworkControl() {
      InitializeComponent();

      diagramControl1.AfterRender = Setup;
      paletteControl1.AfterRender = SetupPalette;
      overviewControl1.AfterRender = SetupOverview;

      saveLoadModel1.SaveClick += (e, obj) => SaveModel();
      saveLoadModel1.LoadClick += (e, obj) => LoadModel();

      saveLoadModel1.ModelJson = @"
    {
      ""NodeDataSource"": [
        {""Key"":0, ""Text"":""Gen1"", ""Category"":""Generator"", ""Loc"":""300 0""},
        {""Key"":1, ""Text"":""Bar1"", ""Category"":""HBar"", ""Loc"":""100 100"", ""Size"":""500 4"", ""Fill"":""green""},
        { ""Key"":3, ""Text"":""Cons1"", ""Category"":""Consumer"", ""Loc"":""53 234""},
        { ""Key"":2, ""Text"":""Bar2"", ""Category"":""HBar"", ""Loc"":""0 300"", ""Size"":""600 4"", ""Fill"":""orange""},
        { ""Key"":4, ""Text"":""Conn1"", ""Category"":""Connector"", ""Loc"":""232.5 207.75""},
        { ""Key"":5, ""Text"":""Cons3"", ""Category"":""Consumer"", ""Loc"":""357.5 230.75""},
        { ""Key"":6, ""Text"":""Cons2"", ""Category"":""Consumer"", ""Loc"":""484.5 164.75""}
     ],
      ""LinkDataSource"": [
        {""From"":0, ""To"":1},
        { ""From"":0, ""To"":2},
        { ""From"":3, ""To"":2},
        { ""From"":4, ""To"":1},
        { ""From"":4, ""To"":2},
        { ""From"":5, ""To"":2},
        { ""From"":6, ""To"":1}
     ]}
    ";

    }

    // Must use sharedNodeTemplate because don't know if palette or diagram will be initialized first
    private void DefineNodeTemplate() {
      if (sharedNodeTemplateMap != null) return;  // already defined

      var KAPPA = 4 * ((Math.Sqrt(2) - 1) / 3);
      Shape.DefineFigureGenerator("ACvoltageSource", (shape, w, h) => {
        var geo = new Geometry();
        var cpOffset = KAPPA * .5;
        var radius = .5;
        var centerx = .5;
        var centery = .5;
        var fig = new PathFigure((centerx - radius) * w, centery * h, false);
        geo.Add(fig);

        fig.Add(new PathSegment(SegmentType.Bezier, centerx * w, (centery - radius) * h, (centerx - radius) * w, (centery - cpOffset) * h,
          (centerx - cpOffset) * w, (centery - radius) * h));
        fig.Add(new PathSegment(SegmentType.Bezier, (centerx + radius) * w, centery * h, (centerx + cpOffset) * w, (centery - radius) * h,
          (centerx + radius) * w, (centery - cpOffset) * h));
        fig.Add(new PathSegment(SegmentType.Bezier, centerx * w, (centery + radius) * h, (centerx + radius) * w, (centery + cpOffset) * h,
          (centerx + cpOffset) * w, (centery + radius) * h));
        fig.Add(new PathSegment(SegmentType.Bezier, (centerx - radius) * w, centery * h, (centerx - cpOffset) * w, (centery + radius) * h,
          (centerx - radius) * w, (centery + cpOffset) * h));
        fig.Add(new PathSegment(SegmentType.Move, (centerx - radius + .1) * w, centery * h));
        fig.Add(new PathSegment(SegmentType.Bezier, (centerx + radius - .1) * w, centery * h, centerx * w, (centery - radius) * h,
          centerx * w, (centery + radius) * h));
        return geo;
      });

      var generatorTemplate = new Node(PanelLayoutSpot.Instance) {
          LocationSpot = Spot.Center,
          SelectionElementName = "BODY"
        }.Bind(
          new Binding("Location", "Loc", Point.Parse, Point.Stringify)
        ).Add(
          new Shape {
            Figure = "ACVoltageSource",
            Name = "BODY", Stroke = "white", StrokeWidth = 3, Fill = "transparent", Background = "darkblue",
            Width = 40, Height = 40, Margin = 5,
            PortId = "", FromLinkable = true, Cursor = "pointer", FromSpot = Spot.TopBottomSides, ToSpot = Spot.TopBottomSides
          },
          new TextBlock {
            Alignment = Spot.Right,
            AlignmentFocus = Spot.Left,
            Editable = true
          }.Bind(new Binding("Text").MakeTwoWay())
        
      );

      var connectorTemplate = new Node(PanelLayoutSpot.Instance) {
          LocationSpot = Spot.Center,
          SelectionElementName = "BODY"
        }.Bind(
          new Binding("Location", "Loc", Point.Parse, Point.Stringify)
        ).Add(
          new Shape {
            Figure = "Circle",
            Name = "BODY",
            Stroke = null,
            Fill = new Brush(new LinearGradientPaint(new Dictionary<float, string> {
              { 0, "lightgray" }, { 1, "gray" }
            }, Spot.Left, Spot.Right)),
            Background = "gray", Width = 20, Height = 20, Margin = 2,
            PortId = "", FromLinkable = true, Cursor = "pointer", FromSpot = Spot.TopBottomSides, ToSpot = Spot.TopBottomSides
          },
          new TextBlock {
            Alignment = Spot.Right,
            AlignmentFocus = Spot.Left,
            Editable = true
          }.Bind(new Binding("Text").MakeTwoWay())
      );

      var consumerTemplate = new Node(PanelLayoutSpot.Instance) {
          LocationSpot = Spot.Center, LocationElementName = "BODY",
          SelectionElementName = "BODY"
        }.Bind(
          new Binding("Location", "Loc", Point.Parse, Point.Stringify)
        ).Add(
          new Picture {
            Source = "https://img.icons8.com/ios/50/000000/my-computer.png",
            Name = "BODY", Width = 50, Height = 40, Margin = 2,
            PortId = "", FromLinkable = true, Cursor = "pointer", FromSpot = Spot.TopBottomSides, ToSpot = Spot.TopBottomSides
          },
          new TextBlock {
            Alignment = Spot.Right, AlignmentFocus = Spot.Left, Editable = true
          }.Bind(new Binding("Text").MakeTwoWay())
      );

      var hBarTemplate = new Node(PanelLayoutSpot.Instance) {
          LayerName = "Background",
          // special resizing, just at the ends
          Resizable = true, ResizeElementName = "SHAPE",
          ResizeAdornmentTemplate = new Adornment(PanelLayoutSpot.Instance).Add(
            new Placeholder(),
            new Shape { // left resize handle
              Alignment = Spot.Left, Cursor = "col-resize",
              DesiredSize = new Size(6, 6), Fill = "lightblue", Stroke = "dodgerblue"
            },
            new Shape { // right resize handle
              Alignment = Spot.Right, Cursor = "col-resize",
              DesiredSize = new Size(6, 6), Fill = "lightblue", Stroke = "dodgerblue"
            }
          )
        }.Bind(
          new Binding("Location", "Loc", Point.Parse, Point.Stringify)
        ).Add(
          new Shape {
            Figure = "Rectangle",
            Name = "SHAPE",
            Fill = "black", Stroke = null, StrokeWidth = 0,
            Width = 1000, Height = 4,
            MinSize = new Size(100, 4),
            MaxSize = new Size(double.PositiveInfinity, 4),
            PortId = "",
            ToLinkable = true
          }.Bind(
            new Binding("DesiredSize", "Size", Northwoods.Go.Size.Parse, Northwoods.Go.Size.Stringify),
            new Binding("Fill")
          ),
          new TextBlock {
            Alignment = Spot.Right,
            AlignmentFocus = Spot.Left,
            Editable = true
          }.Bind(new Binding("Text").MakeTwoWay())
      );

      sharedNodeTemplateMap = new Dictionary<string, Part> {
        { "Generator", generatorTemplate },
        { "Connector", connectorTemplate },
        { "Consumer", consumerTemplate },
        { "HBar", hBarTemplate },
      };

    }

    private void Setup() {

      /*
      var KAPPA = 4 * ((Math.Sqrt(2) - 1) / 3);
      Shape.DefineFigureGenerator("ACvoltageSource", (shape, w, h) => {
        var geo = new Geometry();
        var cpOffset = KAPPA * .5;
        var radius = .5;
        var centerx = .5;
        var centery = .5;
        var fig = new PathFigure((centerx - radius) * w, centery * h, false);
        geo.Add(fig);

        fig.Add(new PathSegment(SegmentType.Bezier, centerx * w, (centery - radius) * h, (centerx - radius) * w, (centery - cpOffset) * h,
          (centerx - cpOffset) * w, (centery - radius) * h));
        fig.Add(new PathSegment(SegmentType.Bezier, (centerx + radius) * w, centery * h, (centerx + cpOffset) * w, (centery - radius) * h,
          (centerx + radius) * w, (centery - cpOffset) * h));
        fig.Add(new PathSegment(SegmentType.Bezier, centerx * w, (centery + radius) * h, (centerx + radius) * w, (centery + cpOffset) * h,
          (centerx + cpOffset) * w, (centery + radius) * h));
        fig.Add(new PathSegment(SegmentType.Bezier, (centerx - radius) * w, centery * h, (centerx - cpOffset) * w, (centery + radius) * h,
          (centerx - radius) * w, (centery + cpOffset) * h));
        fig.Add(new PathSegment(SegmentType.Move, (centerx - radius + .1) * w, centery * h));
        fig.Add(new PathSegment(SegmentType.Bezier, (centerx + radius - .1) * w, centery * h, centerx * w, (centery - radius) * h,
          centerx * w, (centery + radius) * h));
        return geo;
      });
      */

      myDiagram = diagramControl1.Diagram;

      myDiagram.ToolManager.LinkingTool.Direction = LinkingDirection.ForwardsOnly;
      myDiagram.UndoManager.IsEnabled = true;

      DefineNodeTemplate();
      myDiagram.NodeTemplateMap = sharedNodeTemplateMap;

      myDiagram.LinkTemplate = new NetworkBarLink { // subclass defined below
        Routing = LinkRouting.Orthogonal,
        RelinkableFrom = true, RelinkableTo = true,
        ToPortChanged = (link, oldport, newport) => {
          if (newport is Shape newshape) link.Path.Stroke = newshape.Fill;
        }
      }.Add(
        new Shape { StrokeWidth = 2 }
      );

      // start off with a simple diagram
      LoadModel();
    }

    private void SetupPalette() {
      // initialize Palette
      myPalette = paletteControl1.Diagram as Palette;
      DefineNodeTemplate();
      myPalette.NodeTemplateMap = sharedNodeTemplateMap;
      myPalette.Layout = new GridLayout {
        CellSize = new Size(2, 2),
        IsViewportSized = true
      };

      myPalette.Model = new Model {
        NodeDataSource = new List<NodeData> {
          new NodeData { Text = "Generator", Category = "Generator" },
          new NodeData { Text = "Consumer", Category = "Consumer" },
          new NodeData { Text = "Connector", Category = "Connector" },
          new NodeData { Text = "Bar", Category = "HBar", Size = "100 4" }
        }
      };

      // remove cursors on all ports in the Palette
      // make TextBlocks invisible, to reduce size of Nodes
      foreach (var node in myPalette.Nodes) {
        foreach (var port in node.Ports) {
          port.Cursor = "";
        }
        foreach (var tb in node.Elements) {
          if (tb is TextBlock) tb.Visible = false;
        }
      }
    }

    private void SetupOverview() {
      // initialize Overview
      myOverview = overviewControl1.Diagram as Overview;
      myOverview.ObservedControl = diagramControl1;
      myOverview.ContentAlignment = Spot.Center;
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

  public class Model : GraphLinksModel<NodeData, int, object, LinkData, int, string> { }

  public class NodeData : Model.NodeData {
    public string Size { get; set; }
    public string Loc { get; set; }
  }

  public class LinkData : Model.LinkData { }

  public class NetworkBarLink : Link {
    public override Point GetLinkPoint(Node node, GraphObject port, Spot spot, bool from, bool ortho, Node othernode, GraphObject otherport) {
      if (node.Category == "HBar") {
        var op = base.GetLinkPoint(othernode, otherport, ComputeSpot(!from), !from, ortho, node, port);
        var r = port.GetDocumentBounds();
        var y = (op.Y > r.CenterY) ? r.Bottom : r.Top;
        if (op.X < r.Left) return new Point(r.Left, y);
        if (op.X > r.Right) return new Point(r.Right, y);
        return new Point(op.X, y);
      } else {
        return base.GetLinkPoint(node, port, spot, from, ortho, othernode, otherport);
      }
    }

    public override int GetLinkDirection(Node node, GraphObject port, Point linkpoint, Spot spot, bool from, bool ortho, Node othernode, GraphObject otherport) {
      var p = port.GetDocumentPoint(Spot.Center);
      var op = otherport.GetDocumentPoint(Spot.Center);
      var below = op.Y > p.Y;
      return below ? 90 : 270;
    }
  }

}

