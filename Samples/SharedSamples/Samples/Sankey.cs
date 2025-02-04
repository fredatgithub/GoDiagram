﻿/* Copyright 1998-2023 by Northwoods Software Corporation. */

using System;
using System.Collections.Generic;
using Northwoods.Go;
using Northwoods.Go.Models;
using Northwoods.Go.Layouts;
using System.Globalization;

namespace Demo.Samples.Sankey {
  public partial class Sankey : DemoControl {
    private Diagram _Diagram;

    public Sankey() {
      InitializeComponent();
      _Diagram = diagramControl1.Diagram;

      modelJson1.SaveClick = SaveModel;
      modelJson1.LoadClick = LoadModel;

      desc1.MdText = DescriptionReader.Read("Samples.Sankey.md");

      modelJson1.JsonText = @"
        {
     ""NodeDataSource"": [
    {""Key"":""Coal reserves"", ""Text"":""Coal reserves"", ""Color"":""#9d75c2""},
    {""Key"":""Coal imports"", ""Text"":""Coal imports"", ""Color"":""#9d75c2""},
    {""Key"":""Oil reserves"", ""Text"":""Oil\nreserves"", ""Color"":""#9d75c2""},
    {""Key"":""Oil imports"", ""Text"":""Oil imports"", ""Color"":""#9d75c2""},
    {""Key"":""Gas reserves"", ""Text"":""Gas reserves"", ""Color"":""#a1e194""},
    {""Key"":""Gas imports"", ""Text"":""Gas imports"", ""Color"":""#a1e194""},
    {""Key"":""UK land based bioenergy"", ""Text"":""UK land based bioenergy"", ""Color"":""#f6bcd5""},
    {""Key"":""Marine algae"", ""Text"":""Marine algae"", ""Color"":""#681313""},
    {""Key"":""Agricultural 'waste'"", ""Text"":""Agricultural 'waste'"", ""Color"":""#3483ba""},
    {""Key"":""Other waste"", ""Text"":""Other waste"", ""Color"":""#c9b7d8""},
    {""Key"":""Biomass imports"", ""Text"":""Biomass imports"", ""Color"":""#fea19f""},
    {""Key"":""Biofuel imports"", ""Text"":""Biofuel imports"", ""Color"":""#d93c3c""},
    {""Key"":""Coal"", ""Text"":""Coal"", ""Color"":""#9d75c2""},
    {""Key"":""Oil"", ""Text"":""Oil"", ""Color"":""#9d75c2""},
    {""Key"":""Natural gas"", ""Text"":""Natural\ngas"", ""Color"":""#a6dce6""},
    {""Key"":""Solar"", ""Text"":""Solar"", ""Color"":""#c9a59d""},
    {""Key"":""Solar PV"", ""Text"":""Solar PV"", ""Color"":""#c9a59d""},
    {""Key"":""Bio-conversion"", ""Text"":""Bio-conversion"", ""Color"":""#b5cbe9""},
    {""Key"":""Solid"", ""Text"":""Solid"", ""Color"":""#40a840""},
    {""Key"":""Liquid"", ""Text"":""Liquid"", ""Color"":""#fe8b25""},
    {""Key"":""Gas"", ""Text"":""Gas"", ""Color"":""#a1e194""},
    {""Key"":""Nuclear"", ""Text"":""Nuclear"", ""Color"":""#fea19f""},
    {""Key"":""Thermal generation"", ""Text"":""Thermal\ngeneration"", ""Color"":""#3483ba""},
    {""Key"":""CHP"", ""Text"":""CHP"", ""Color"":""yellow""},
    {""Key"":""Electricity imports"", ""Text"":""Electricity imports"", ""Color"":""yellow""},
    {""Key"":""Wind"", ""Text"":""Wind"", ""Color"":""#cbcbcb""},
    {""Key"":""Tidal"", ""Text"":""Tidal"", ""Color"":""#6f3a5f""},
    {""Key"":""Wave"", ""Text"":""Wave"", ""Color"":""#8b8b8b""},
    {""Key"":""Geothermal"", ""Text"":""Geothermal"", ""Color"":""#556171""},
    {""Key"":""Hydro"", ""Text"":""Hydro"", ""Color"":""#7c3e06""},
    {""Key"":""Electricity grid"", ""Text"":""Electricity grid"", ""Color"":""#e483c7""},
    {""Key"":""H2 conversion"", ""Text"":""H2 conversion"", ""Color"":""#868686""},
    {""Key"":""Solar Thermal"", ""Text"":""Solar Thermal"", ""Color"":""#c9a59d""},
    {""Key"":""H2"", ""Text"":""H2"", ""Color"":""#868686""},
    {""Key"":""Pumped heat"", ""Text"":""Pumped heat"", ""Color"":""#96665c""},
    {""Key"":""District heating"", ""Text"":""District heating"", ""Color"":""#c9b7d8""},
    {""Key"":""Losses"", ""LText"":""Losses"", ""Color"":""#fec184""},
    {""Key"":""Over generation / exports"", ""LText"":""Over generation / exports"", ""Color"":""#f6bcd5""},
    {""Key"":""Heating and cooling - homes"", ""LText"":""Heating and cooling - homes"", ""Color"":""#c7a39b""},
    {""Key"":""Road transport"", ""LText"":""Road transport"", ""Color"":""#cbcbcb""},
    {""Key"":""Heating and cooling - commercial"", ""LText"":""Heating and cooling - commercial"", ""Color"":""#c9a59d""},
    {""Key"":""Industry"", ""LText"":""Industry"", ""Color"":""#96665c""},
    {""Key"":""Lighting & appliances - homes"", ""LText"":""Lighting & appliances - homes"", ""Color"":""#2dc3d2""},
    {""Key"":""Lighting & appliances - commercial"", ""LText"":""Lighting & appliances - commercial"", ""Color"":""#2dc3d2""},
    {""Key"":""Agriculture"", ""LText"":""Agriculture"", ""Color"":""#5c5c10""},
    {""Key"":""Rail transport"", ""LText"":""Rail transport"", ""Color"":""#6b6b45""},
    {""Key"":""Domestic aviation"", ""LText"":""Domestic aviation"", ""Color"":""#40a840""},
    {""Key"":""National navigation"", ""LText"":""National navigation"", ""Color"":""#a1e194""},
    {""Key"":""International aviation"", ""LText"":""International aviation"", ""Color"":""#fec184""},
    {""Key"":""International shipping"", ""LText"":""International shipping"", ""Color"":""#fec184""},
    {""Key"":""Geosequestration"", ""LText"":""Geosequestration"", ""Color"":""#fec184""}
  ], ""LinkDataSource"": [
    {""From"":""Coal reserves"", ""To"":""Coal"", ""Width"":31},
    {""From"":""Coal imports"", ""To"":""Coal"", ""Width"":86},
    {""From"":""Oil reserves"", ""To"":""Oil"", ""Width"":244},
    {""From"":""Oil imports"", ""To"":""Oil"", ""Width"":1},
    {""From"":""Gas reserves"", ""To"":""Natural gas"", ""Width"":182},
    {""From"":""Gas imports"", ""To"":""Natural gas"", ""Width"":61},
    {""From"":""UK land based bioenergy"", ""To"":""Bio-conversion"", ""Width"":1},
    {""From"":""Marine algae"", ""To"":""Bio-conversion"", ""Width"":1},
    {""From"":""Agricultural 'waste'"", ""To"":""Bio-conversion"", ""Width"":1},
    {""From"":""Other waste"", ""To"":""Bio-conversion"", ""Width"":8},
    {""From"":""Other waste"", ""To"":""Solid"", ""Width"":1},
    {""From"":""Biomass imports"", ""To"":""Solid"", ""Width"":1},
    {""From"":""Biofuel imports"", ""To"":""Liquid"", ""Width"":1},
    {""From"":""Coal"", ""To"":""Solid"", ""Width"":117},
    {""From"":""Oil"", ""To"":""Liquid"", ""Width"":244},
    {""From"":""Natural gas"", ""To"":""Gas"", ""Width"":244},
    {""From"":""Solar"", ""To"":""Solar PV"", ""Width"":1},
    {""From"":""Solar PV"", ""To"":""Electricity grid"", ""Width"":1},
    {""From"":""Solar"", ""To"":""Solar Thermal"", ""Width"":1},
    {""From"":""Bio-conversion"", ""To"":""Solid"", ""Width"":3},
    {""From"":""Bio-conversion"", ""To"":""Liquid"", ""Width"":1},
    {""From"":""Bio-conversion"", ""To"":""Gas"", ""Width"":5},
    {""From"":""Bio-conversion"", ""To"":""Losses"", ""Width"":1},
    {""From"":""Solid"", ""To"":""Over generation / exports"", ""Width"":1},
    {""From"":""Liquid"", ""To"":""Over generation / exports"", ""Width"":18},
    {""From"":""Gas"", ""To"":""Over generation / exports"", ""Width"":1},
    {""From"":""Solid"", ""To"":""Thermal generation"", ""Width"":106},
    {""From"":""Liquid"", ""To"":""Thermal generation"", ""Width"":2},
    {""From"":""Gas"", ""To"":""Thermal generation"", ""Width"":87},
    {""From"":""Nuclear"", ""To"":""Thermal generation"", ""Width"":41},
    {""From"":""Thermal generation"", ""To"":""District heating"", ""Width"":2},
    {""From"":""Thermal generation"", ""To"":""Electricity grid"", ""Width"":92},
    {""From"":""Thermal generation"", ""To"":""Losses"", ""Width"":142},
    {""From"":""Solid"", ""To"":""CHP"", ""Width"":1},
    {""From"":""Liquid"", ""To"":""CHP"", ""Width"":1},
    {""From"":""Gas"", ""To"":""CHP"", ""Width"":1},
    {""From"":""CHP"", ""To"":""Electricity grid"", ""Width"":1},
    {""From"":""CHP"", ""To"":""Losses"", ""Width"":1},
    {""From"":""Electricity imports"", ""To"":""Electricity grid"", ""Width"":1},
    {""From"":""Wind"", ""To"":""Electricity grid"", ""Width"":1},
    {""From"":""Tidal"", ""To"":""Electricity grid"", ""Width"":1},
    {""From"":""Wave"", ""To"":""Electricity grid"", ""Width"":1},
    {""From"":""Geothermal"", ""To"":""Electricity grid"", ""Width"":1},
    {""From"":""Hydro"", ""To"":""Electricity grid"", ""Width"":1},
    {""From"":""Electricity grid"", ""To"":""H2 conversion"", ""Width"":1},
    {""From"":""Electricity grid"", ""To"":""Over generation / exports"", ""Width"":1},
    {""From"":""Electricity grid"", ""To"":""Losses"", ""Width"":6},
    {""From"":""Gas"", ""To"":""H2 conversion"", ""Width"":1},
    {""From"":""H2 conversion"", ""To"":""H2"", ""Width"":1},
    {""From"":""H2 conversion"", ""To"":""Losses"", ""Width"":1},
    {""From"":""Solar Thermal"", ""To"":""Heating and cooling - homes"", ""Width"":1},
    {""From"":""H2"", ""To"":""Road transport"", ""Width"":1},
    {""From"":""Pumped heat"", ""To"":""Heating and cooling - homes"", ""Width"":1},
    {""From"":""Pumped heat"", ""To"":""Heating and cooling - commercial"", ""Width"":1},
    {""From"":""CHP"", ""To"":""Heating and cooling - homes"", ""Width"":1},
    {""From"":""CHP"", ""To"":""Heating and cooling - commercial"", ""Width"":1},
    {""From"":""District heating"", ""To"":""Heating and cooling - homes"", ""Width"":1},
    {""From"":""District heating"", ""To"":""Heating and cooling - commercial"", ""Width"":1},
    {""From"":""District heating"", ""To"":""Industry"", ""Width"":2},
    {""From"":""Electricity grid"", ""To"":""Heating and cooling - homes"", ""Width"":7},
    {""From"":""Solid"", ""To"":""Heating and cooling - homes"", ""Width"":3},
    {""From"":""Liquid"", ""To"":""Heating and cooling - homes"", ""Width"":3},
    {""From"":""Gas"", ""To"":""Heating and cooling - homes"", ""Width"":81},
    {""From"":""Electricity grid"", ""To"":""Heating and cooling - commercial"", ""Width"":7},
    {""From"":""Solid"", ""To"":""Heating and cooling - commercial"", ""Width"":1},
    {""From"":""Liquid"", ""To"":""Heating and cooling - commercial"", ""Width"":2},
    {""From"":""Gas"", ""To"":""Heating and cooling - commercial"", ""Width"":19},
    {""From"":""Electricity grid"", ""To"":""Lighting & appliances - homes"", ""Width"":21},
    {""From"":""Gas"", ""To"":""Lighting & appliances - homes"", ""Width"":2},
    {""From"":""Electricity grid"", ""To"":""Lighting & appliances - commercial"", ""Width"":18},
    {""From"":""Gas"", ""To"":""Lighting & appliances - commercial"", ""Width"":2},
    {""From"":""Electricity grid"", ""To"":""Industry"", ""Width"":30},
    {""From"":""Solid"", ""To"":""Industry"", ""Width"":13},
    {""From"":""Liquid"", ""To"":""Industry"", ""Width"":34},
    {""From"":""Gas"", ""To"":""Industry"", ""Width"":54},
    {""From"":""Electricity grid"", ""To"":""Agriculture"", ""Width"":1},
    {""From"":""Solid"", ""To"":""Agriculture"", ""Width"":1},
    {""From"":""Liquid"", ""To"":""Agriculture"", ""Width"":1},
    {""From"":""Gas"", ""To"":""Agriculture"", ""Width"":1},
    {""From"":""Electricity grid"", ""To"":""Road transport"", ""Width"":1},
    {""From"":""Liquid"", ""To"":""Road transport"", ""Width"":122},
    {""From"":""Electricity grid"", ""To"":""Rail transport"", ""Width"":2},
    {""From"":""Liquid"", ""To"":""Rail transport"", ""Width"":1},
    {""From"":""Liquid"", ""To"":""Domestic aviation"", ""Width"":2},
    {""From"":""Liquid"", ""To"":""National navigation"", ""Width"":4},
    {""From"":""Liquid"", ""To"":""International aviation"", ""Width"":38},
    {""From"":""Liquid"", ""To"":""International shipping"", ""Width"":13},
    {""From"":""Electricity grid"", ""To"":""Geosequestration"", ""Width"":1},
    {""From"":""Gas"", ""To"":""Losses"", ""Width"":2}
     ]}";

      Setup();
    }

    private void Setup() {
      _Diagram.InitialAutoScale = AutoScale.UniformToFill;
      _Diagram.AnimationManager.IsEnabled = false;
      _Diagram.Layout = new SankeyLayout {
        SetsPortSpots = false,  // to allow the "Side" spots on the nodes to take effect
        Direction = 0,  // rightwards
        LayeringOption = LayeredDigraphLayering.OptimalLinkLength,
        PackOption = LayeredDigraphPack.Straighten | LayeredDigraphPack.Median,
        LayerSpacing = 150,  // lots of space between layers, for nicer thick links
        ColumnSpacing = 1
      };

      var colors = new List<string> {
        "#AC193D/#BF1E4B", "#2672EC/#2E8DEF",
        "#8C0095/#A700AE", "#5133AB/#643EBF",
        "#008299/#00A0B1", "#D24726/#DC572E",
        "#008A00/#00A600", "#094AB2/#0A5BC4"
      };

      void textStyle(TextBlock tb) {
        tb.Font = new Font("Segoe UI", 12, Northwoods.Go.FontWeight.Bold);
        tb.Stroke = "black";
        tb.Margin = 5;
      }

      _Diagram.NodeTemplate =
        new Node("Horizontal") {
            LocationElementName = "SHAPE",
            LocationSpot = Spot.Left,
            PortSpreading = PortSpreading.Packed  // rather than the default PortSpreading.SpreadingEvenly
          }
          .Add(
            new TextBlock { Name = "LTEXT" }
              .Apply(textStyle)
              .Bind("Text", "LText"),
            new Shape("Rectangle") {
                Name = "SHAPE",
                Fill = "#2E8DEF",  // default fill color
                StrokeWidth = 0,
                PortId = "",
                FromSpot = Spot.RightSide,
                ToSpot = Spot.LeftSide,
                Height = 10,
                Width = 20
              }
              .Bind("Fill", "Color"),
            new TextBlock { Name = "TEXT" }
              .Apply(textStyle)
              .Bind("Text")
        );

      object getAutoLinkColor(object data) {
        var linkdata = data as LinkData;
        var nodedata = _Diagram.Model.FindNodeDataForKey(linkdata.From) as NodeData;
        var hex = nodedata.Color;
        if (hex[0] == '#') {
          var rgb = int.Parse(hex.Substring(1, 6), NumberStyles.HexNumber);
          var r = rgb >> 16;
          var g = rgb >> 8 & 0xFF;
          var b = rgb & 0xFF;
          var alpha = 0.4;
          if (linkdata.Width <= 2) alpha = 1;
          var rgba = "rgba(" + r + "," + g + "," + b + "," + alpha + ")";
          return rgba;
        }
        return "rgba(173, 173, 173, 0.25)";
      }

      // define the Link template
      var linkSelectionAdornmentTemplate =
        new Adornment("Link")
          .Add(
            new Shape { IsPanelMain = true, Fill = null, Stroke = "rgba(0, 0, 255, 0.3)", StrokeWidth = 0 }  // use selection object's StrokeWidth
          );

      _Diagram.LinkTemplate =
        new Link {
            Curve = LinkCurve.Bezier,
            SelectionAdornmentTemplate = linkSelectionAdornmentTemplate,
            LayerName = "Background",
            FromEndSegmentLength = 150, ToEndSegmentLength = 150,
            Adjusting = LinkAdjusting.End
          }
          .Add(
            new Shape { StrokeWidth = 4, Stroke = "rgba(173, 173, 173, 0.25)" }
              .Bind("Stroke", "", getAutoLinkColor)
              .Bind("StrokeWidth", "Width")
          );

      // read in the JSON-format data from the Textarea element
      LoadModel();
    }

    private void SaveModel() {
      if (_Diagram == null) return;
      modelJson1.JsonText = _Diagram.Model.ToJson();
    }

    private void LoadModel() {
      if (_Diagram == null) return;
      _Diagram.Model = Model.FromJson<Model>(modelJson1.JsonText);
    }
  }

  public class Model : GraphLinksModel<NodeData, string, object, LinkData, string, string> { }
  public class NodeData : Model.NodeData {
    public string LText { get; set; }
    public string Color { get; set; }
  }
  public class LinkData : Model.LinkData {
    public double Width { get; set; }
  }

  public class SankeyLayout : LayeredDigraphLayout {
    // determine the desired height of each node/vertex,
    // based on the thicknesses of the connected links;
    // actually modify the height of each node's SHAPE
    public override LayeredDigraphNetwork CreateNetwork() {
      var net = base.CreateNetwork();
      foreach (var node in Diagram.Nodes) {
        // figure out how tall the node's bar should be
        var height = _GetAutoHeightForNode(node);
        var font = new Font("Segoe UI", (float)Math.Max(12, Math.Round(height / 8)), FontWeight.Bold);
        var shape = node.FindElement("SHAPE");
        var text = node.FindElement("TEXT") as TextBlock;
        var ltext = node.FindElement("LTEXT") as TextBlock;
        if (shape != null) shape.Height = height;
        if (text != null) text.Font = font;
        if (ltext != null) ltext.Font = font;
        // and update the vertex's dimensions accordingly
        var v = net.FindVertex(node);
        if (v != null) {
          node.EnsureBounds();
          var r = node.ActualBounds;
          v.Width = r.Width;
          v.Height = r.Height;
          v.FocusY = v.Height / 2;
        }
      }
      return net;
    }

    private double _GetAutoHeightForNode(Node node) {
      var heightIn = 0.0;
      var it = node.FindLinksInto();
      foreach (var link in it) {
        heightIn += link.ComputeThickness();
      }
      var heightOut = 0.0;
      it = node.FindLinksOutOf();
      foreach (var link in it) {
        heightOut += link.ComputeThickness();
      }
      var h = Math.Max(heightIn, heightOut);
      if (h < 10) h = 10;
      return h;
    }

    // treat dummy vertexes as having the thicknesss of the link that they are in
    protected override int NodeMinColumnSpace(LayeredDigraphVertex v, bool topleft) {
      if (v.Node == null) {
        if (v.Edges.Count >= 1) {
          var max = 1.0;
          var it = v.Edges;
          foreach (var edge in it) {
            if (edge.Link != null) {
              var t = edge.Link.ComputeThickness();
              if (t > max) max = t;
              break;
            }
          }
          return Math.Max(2, (int)Math.Ceiling(max / ColumnSpacing));
        }
        return 2;
      }
      return base.NodeMinColumnSpace(v, topleft);
    }

    // treat dummy vertexes as being thicker, so that the Bezier curves are gentler
    protected override double NodeMinLayerSpace(LayeredDigraphVertex v, bool topleft) {
      if (v.Node == null) return 100;
      return base.NodeMinLayerSpace(v, topleft);
    }

    protected override void AssignLayers() {
      base.AssignLayers();
      var maxlayer = MaxLayer;
      // now make sure every vertex with no outputs is maxlayer
      foreach (var v in Network.Vertexes) {
        var node = v.Node;
        if (v.DestinationVertexes.Count == 0) {
          v.Layer = 0;
        }
        if (v.SourceVertexes.Count == 0) {
          v.Layer = maxlayer;
        }
      }
      // from now on, the LayeredDigraphLayout will think that the Node is bigger than it really is
      // (other than the ones that are the widest or tallest in their respective layer).
    }

    protected override void CommitLayout() {
      base.CommitLayout();
      foreach (var e in Network.Edges) {
        var link = e.Link;
        if (link != null && link.Curve == LinkCurve.Bezier) {
          // depend on Link.Adjusting == LinkAdjusting.End to fix up the end points of the links
          // without losing the intermediate points of the route as determined by LayeredDigraphLayout
          link.InvalidateRoute();
        }
      }
    }
  }
  // end of SankeyLayout
}
