﻿/* Copyright 1998-2023 by Northwoods Software Corporation. */

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Northwoods.Go;
using Northwoods.Go.Layouts;
using Northwoods.Go.Layouts.Extensions;
using Northwoods.Go.Models;

namespace Demo.Extensions.Arranging {
  public partial class Arranging : DemoControl {
    private Diagram _Diagram;

    public Arranging() {
      InitializeComponent();
      _Diagram = diagramControl1.Diagram;

      Setup();
      desc1.MdText = DescriptionReader.Read("Extensions.Arranging.md");
    }

    private void Setup() {
      _Diagram.InitialAutoScale = AutoScale.Uniform;
      _Diagram.Layout = new CustomArrangingLayout() {
        // create a circular arrangement of circular layouts
        PrimaryLayout = new CircularLayout(), // must specify the primaryLayout
        ArrangeLayout = new CircularLayout() {
          NodeDiameterFormula = CircularNodeDiameterFormula.Circular,
          Spacing = 30

        },

        // Uncommenting this filter will force all of the nodes and links to go into the main subset and thus
        // will cause all those nodes to be arranged by this.arrangingLayout, here a CircularLayout,
        // rather than by the this.sideLayout, which by default is a GridLayout.
        // Filter = (p) => true,
      };

      _Diagram.NodeTemplate =
        new Node(PanelType.Auto) { }
          .Add(new Shape {
            Name = "SHAPE",
            Figure = "RoundedRectangle",
            Fill = "lightgray"
          }.Bind("Fill", "Color"),
          new TextBlock {
            Margin = 2,
            TextAlign = TextAlign.Center,
          }.Bind("Text", "Key", (s) => {
            // insert newlines between lowercase followed by uppercase characters
            var str = s as string;
            for (var i = 1; i < str.Length - 1; i++) {
              var a = str[i - 1];
              var b = str[i];
              if (char.IsLower(a) && char.IsUpper(b)) {
                str = str.Insert(i, "\n");
                i += 2;
              }
            }
            return str;
          }));

      _Diagram.LinkTemplate =
        new Link() {
          LayerName = "Background"
        }.Add(new Shape { });

      // Collect all of the data for the model of the class hierarchy
      var nodeDataSource = new List<NodeData>();

      static bool includeType(Type t) {
        return t.Namespace != null &&
          t.Namespace.Contains("Northwoods.Go") &&
          t.IsVisible &&
          !t.IsEnum &&
          !Attribute.IsDefined(t, typeof(UndocumentedAttribute)) &&
          !t.Name.Contains("<>");
      };

      // iterate over all the classes in Go namespace, including layouts
      var asm = Assembly.GetAssembly(typeof(Diagram));
      var classlist = asm.GetTypes().Where(includeType).ToList();
      asm = Assembly.GetAssembly(typeof(CircularLayout));
      classlist.AddRange(asm.GetTypes().Where(includeType));
      asm = Assembly.GetAssembly(typeof(ForceDirectedLayout));
      classlist.AddRange(asm.GetTypes().Where(includeType));
      asm = Assembly.GetAssembly(typeof(LayeredDigraphLayout));
      classlist.AddRange(asm.GetTypes().Where(includeType));
      asm = Assembly.GetAssembly(typeof(TreeLayout));
      classlist.AddRange(asm.GetTypes().Where(includeType));

      foreach(var c in classlist) {
        if (c.BaseType?.Name == c.Name) continue;  // don't repeat derived types that share name with parent
        // find base class constructor
        var parent = c.BaseType;
        if (parent == null || parent.Name == null ||
            parent.FullName == "System.Object" ||
            parent.FullName == "System.ValueType" ||
            parent.FullName == "System.MulticastDelegate" ||
            parent.FullName == "System.Windows.Forms.Control") {  // "root" node?
          nodeDataSource.Add(new NodeData { Key = c.Name });
        } else {
          nodeDataSource.Add(new NodeData { Key = c.Name, Parent = parent.Name });
        }
      }

      // Create the model for the hierarchy diagram
      _Diagram.Model = new Model {
        NodeDataSource = nodeDataSource
      };
    }
  }

  public class Model : TreeModel<NodeData, string, string> { }
  public class NodeData : Model.NodeData {}

  public class CustomArrangingLayout : ArrangingLayout {
    // additional custom properties for use by PreparePrimaryLayout
    private readonly List<string> _Colors= new() { "red", "orange", "yellow", "lime", "cyan" };  // possible node colors
    private int _ColorIndex;  // cycle through the given colors

    public CustomArrangingLayout() : base() { }

    // called for each separate connected subgraph
    public override void PreparePrimaryLayout(Layout primaryLayout, IEnumerable<Part> coll) {
      Part root = null; // find the root node in this subgraph
      foreach (var node in coll) {
        if (node is Node nd && !nd.FindLinksInto().Any()) root = node;
      }
      var color = "white"; // determine the color for the nodes in this subgraph
      if (root != null) {
        // root.key will be the name of the class that this node represents
        // Special case: "Network", "Vertex", and "Edge" classes are "violet"
        if (((root as Node).Key as string).StartsWith("Network") || (root as Node).Key as string == "Vertex" || (root as Node).Key as string == "Edge") {
          color = "violet";
        } else { // otherwise cycle through the Array of colors
          color = _Colors[_ColorIndex++ % _Colors.Count];
        }
      }
      foreach (var node in coll) { // assign the fill color for all of the nodes in the subgraph
        if (node is Node nd) {
          if (nd.FindElement("SHAPE") is Shape shape) shape.Fill = color;
        }
      }
    }

    public override void PrepareSideLayout(Layout lay, IEnumerable<Part> coll, Rect b) {
      if (lay is not GridLayout glay) return;
      // adjust how wide the GridLayout lays out
      glay.WrappingWidth = Math.Max(b.Width, Diagram.ViewportBounds.Width);
    }
  }
}
