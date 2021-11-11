﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Northwoods.Go;
using Northwoods.Go.Layouts;
using Northwoods.Go.Models;
using Northwoods.Go.Tools;

namespace WinFormsSampleControls.KanbanBoard {
  [ToolboxItem(false)]
  public partial class KanbanBoardControl : System.Windows.Forms.UserControl {
    private Diagram myDiagram;
    
    public KanbanBoardControl() {
      InitializeComponent();

      diagramControl1.AfterRender = Setup;

      saveLoadModel1.SaveClick += (e, obj) => SaveModel();
      saveLoadModel1.LoadClick += (e, obj) => LoadModel();

      goWebBrowser1.Html = @"

   <p>A Kanban board is a work and workflow visualization used to communicate the status and progress of work items. Click on the color of a note to cycle through colors.</p>
  <p>
    This design and implementation were adapted from the <a href=""swimLanesVertical.html"">Swim Lanes (vertical)</a> sample.
    Unlike that sample:
    <ul>
      <li> there are no Links </li>
   
         <li> lanes cannot be nested into ""pools"" </li>
      
            <li> lanes cannot be resized </li>
         
               <li> the user cannot drop tasks into the diagram's background</li>
                  <li> all tasks are ordered within a single column; the user can rearrange the order</li>
               
                     <li> tasks can freely be moved into other lanes </li>
                  
                        <li> lanes are not movable or copyable or deletable </li>
                     
                         </ul>
                     
                       </p>
";

      saveLoadModel1.ModelJson = @"{
  ""NodeDataSource"": [
    { ""Key"":""Problems"", ""Text"":""Problems"", ""IsGroup"":true, ""Loc"":""0 23.52284749830794"" },
    { ""Key"":""Reproduced"", ""Text"":""Reproduced"", ""IsGroup"":true, ""Color"":0, ""Loc"":""109 23.52284749830794"" },
    { ""Key"":""Identified"", ""Text"":""Identified"", ""IsGroup"":true, ""Color"":0, ""Loc"":""235 23.52284749830794"" },
    { ""Key"":""Fixing"", ""Text"":""Fixing"", ""IsGroup"":true, ""Color"":0, ""Loc"":""343 23.52284749830794"" },
    { ""Key"":""Reviewing"", ""Text"":""Reviewing"", ""IsGroup"":true, ""Color"":0, ""Loc"":""451 23.52284749830794"" },
    { ""Key"":""Testing"", ""Text"":""Testing"", ""IsGroup"":true, ""Color"":0, ""Loc"":""562 23.52284749830794"" },
    { ""Key"":""Customer"", ""Text"":""Customer"", ""IsGroup"":true, ""Color"":0, ""Loc"":""671 23.52284749830794"" },
    { ""Key"":""-1"", ""Group"":""Problems"", ""Category"":""NewButton"",  ""Loc"":""12 35.52284749830794"" },
    { ""Key"":""1"", ""Text"":""text for oneA"", ""Group"":""Problems"", ""Color"":0, ""Loc"":""12 35.52284749830794"" },
    { ""Key"":""2"", ""Text"":""text for oneB"", ""Group"":""Problems"", ""Color"":1, ""Loc"":""12 65.52284749830794"" },
    { ""Key"":""3"", ""Text"":""text for oneC"", ""Group"":""Problems"", ""Color"":0, ""Loc"":""12 95.52284749830794"" },
    { ""Key"":""4"", ""Text"":""text for oneD"", ""Group"":""Problems"", ""Color"":1, ""Loc"":""12 125.52284749830794"" },
    { ""Key"":""5"", ""Text"":""text for twoA"", ""Group"":""Reproduced"", ""Color"":1, ""Loc"":""121 35.52284749830794"" },
    { ""Key"":""6"", ""Text"":""text for twoB"", ""Group"":""Reproduced"", ""Color"":1, ""Loc"":""121 65.52284749830794"" },
    { ""Key"":""7"", ""Text"":""text for twoC"", ""Group"":""Identified"", ""Color"":0, ""Loc"":""247 35.52284749830794"" },
    { ""Key"":""8"", ""Text"":""text for twoD"", ""Group"":""Fixing"", ""Color"":0, ""Loc"":""355 35.52284749830794"" },
    { ""Key"":""9"", ""Text"":""text for twoE"", ""Group"":""Reviewing"", ""Color"":0, ""Loc"":""463 35.52284749830794"" },
    { ""Key"":""10"", ""Text"":""text for twoF"", ""Group"":""Reviewing"", ""Color"":1, ""Loc"":""463 65.52284749830794"" },
    { ""Key"":""11"", ""Text"":""text for twoG"", ""Group"":""Testing"", ""Color"":0, ""Loc"":""574 35.52284749830794"" },
    { ""Key"":""12"", ""Text"":""text for fourA"", ""Group"":""Customer"", ""Color"":1, ""Loc"":""683 35.52284749830794"" },
    { ""Key"":""13"", ""Text"":""text for fourB"", ""Group"":""Customer"", ""Color"":1, ""Loc"":""683 65.52284749830794"" },
    { ""Key"":""14"", ""Text"":""text for fourC"", ""Group"":""Customer"", ""Color"":1, ""Loc"":""683 95.52284749830794"" },
    { ""Key"":""15"", ""Text"":""text for fourD"", ""Group"":""Customer"", ""Color"":0, ""Loc"":""683 125.52284749830794"" },
    { ""Key"":""16"", ""Text"":""text for fiveA"", ""Group"":""Customer"", ""Color"":0, ""Loc"":""683 155.52284749830795"" }
  ],
  ""LinkDataSource"": []
}";

    }

    private void Setup() {
      myDiagram = diagramControl1.Diagram;

      // make sure the top-left corner of the viewport is occupied
      myDiagram.ContentAlignment = Spot.TopLeft;
      // use a simple layout to stack the top-level Groups next to each other
      myDiagram.Layout = new PoolLayout();
      // Disallow nodes to be dragged to the diagram's background
      myDiagram.MouseDrop = (e) => {
        e.Diagram.CurrentTool.DoCancel();
      };
      // a clipboard copied node is pasted into the original node's lane
      myDiagram.CommandHandler.CopiesGroupKey = true;
      // automatically relayout the swim lanes after dragging the selection
      myDiagram.SelectionMoved += relayoutDiagram; // this DiagramEvent listener is
      myDiagram.SelectionCopied += relayoutDiagram; // defined above
      myDiagram.UndoManager.IsEnabled = true;
      // allow TextEditingTool to start without selecting first
      myDiagram.ToolManager.TextEditingTool.Starting = TextEditingStarting.SingleClick;

      // Customize the dragging tool:
      // When dragging a node set its opacity to 0.6 and move it to be in front of other nodes
      myDiagram.ToolManager.DraggingTool = new CustomDraggingTool();

      var noteColors = new[] { "#009CCC", "#CC293D", "#FFD700" };
      object getNoteColor(object num, object _) {
        return noteColors[Math.Min((int)num, noteColors.Length - 1)];
      }

      myDiagram.NodeTemplate =
        new Node("Horizontal")
          .Add(
            new Shape("Rectangle") {
              Fill = "#009CCC", StrokeWidth = 1, Stroke = "#009CCC",
              Width = 6, Stretch = Stretch.Vertical, Alignment = Spot.Left,
              // if a user clicks the colored portion of a node, cycle through colors
              Click = (e, obj) => {
                myDiagram.StartTransaction("Update node color");
                var newColor = 1 + (obj.Part.Data as NodeData).Color;
                if (newColor > noteColors.Length - 1) newColor = 0;
                myDiagram.Model.Set(obj.Part.Data, "Color", newColor);
                myDiagram.CommitTransaction("Update node color");
              }
            }
              .Bind("Fill", "Color", getNoteColor)
              .Bind("Stroke", "Color", getNoteColor),
            new Panel("Auto")
              .Add(
                new Shape("Rectangle") { Fill = "white", Stroke = "#CCCCCC" },
                new Panel("Table") {
                  Width = 130, MinSize = new Size(double.NaN, 50)
                }
                  .Add(
                    new TextBlock {
                      Name = "TEXT",
                      Margin = 6, Font = "Segoe UI, 11px", Editable = true,
                      Stroke = "#000", MaxSize = new Size(130, double.NaN),
                      Alignment = Spot.TopLeft
                    }
                      .Bind(new Binding("Text").MakeTwoWay())
                  )
              )
          );

      // unmovable node that acts as a button
      myDiagram.NodeTemplateMap.Add("NewButton",
        new Node("Horizontal") {
          Selectable = false,
          Click = (e, node) => {
            myDiagram.StartTransaction("add node");
            var newdata = new NodeData { Group = "Problems", Loc = "0 50", Text = "New item " + (node as Node).ContainingGroup.MemberParts.Count, Color = 0 };
            myDiagram.Model.AddNodeData(newdata);
            myDiagram.CommitTransaction("add node");
            var newnode = myDiagram.FindNodeForData(newdata);
            myDiagram.Select(newnode);
            myDiagram.CommandHandler.EditTextBlock();
          },
          Background = "white"
        }
          .Bind("Location", "Loc", Point.Parse, Point.Stringify)
          .Add(
            new Panel("Auto")
              .Add(
                new Shape("Rectangle") { StrokeWidth = 0, Stroke = null, Fill = "#6FB583" },
                new Shape("PlusLine") { Margin = 6, StrokeWidth = 2, Width = 12, Height = 12, Stroke = "white", Background = "#6FB583" }
              ),
            new TextBlock("New item") { Font = "Segoe UI, 10px", Margin = 6 }
          )
      );

      // While dragging, highlight the dragged-over group
      void highlightGroup(object grp, object show) {
        if ((bool)show) {
          var part = myDiagram.ToolManager.DraggingTool.CurrentPart;
          if (part.ContainingGroup != grp) {
            (grp as Group).IsHighlighted = true;
            return;
          }
        }
        (grp as Group).IsHighlighted = false;
      }

      myDiagram.GroupTemplate =
        new Group("Vertical") {
          Selectable = false,
          SelectionElementName = "SHAPE",  // even though its not selectable, this is used in the layout
          LayerName = "Background",  // all lanes are always behind all nodes and links
          Layout = new GridLayout {  // automatically lay out the lane's subgraph
            WrappingColumn = 1,
            CellSize = new Size(1, 1),
            Spacing = new Size(5, 5),
            Alignment = GridAlignment.Position,
            Comparer = Comparer<Part>.Create(
                (a, b) => {  // can re-order tasks within a lane
                  var ay = a.Location.Y;
                  var by = b.Location.Y;
                  if (double.IsNaN(ay) || double.IsNaN(by)) return 0;
                  if (ay < by) return -1;
                  if (ay > by) return 1;
                  return 0;
                }
              )
          },
          Click = (e, grp) => {  // allow simple click on group to clear selection
            if (!e.Shift && !e.Control && !e.Meta) e.Diagram.ClearSelection();
          },
          ComputesBoundsAfterDrag = true,  // needed to prevent recomputing Group.placeholder bounds too soon
          HandlesDragDropForMembers = true,  // don't need to define handlers on member nodes and links
          MouseDragEnter = (e, grp, prev) => highlightGroup(grp, true),
          MouseDragLeave = (e, grp, next) => highlightGroup(grp, false),
          MouseDrop = (e, grp) => {  // dropping a copy of some Nodes and links onto theis Group adds them to this Group
                                     // don't allow drag-and-dropping a mix of regular Nodes and Groups
            if (e.Diagram.Selection.All(n => !(n is Group))) {
              var ok = (grp as Group).AddMembers(grp.Diagram.Selection, true);
              if (!ok) grp.Diagram.CurrentTool.DoCancel();
            }
          },
          SubGraphExpandedChanged = (grp) => {
            var shp = grp.SelectionElement;
            if (grp.Diagram.UndoManager.IsUndoingRedoing) return;
            if (grp.IsSubGraphExpanded) {
              shp.Width = (double)grp["_SavedBreadth"];
            } else {   // remember the original width
              grp["_SavedBreadth"] = shp.Width;
              shp.Width = double.NaN;
            }
          }
        }
          .Bind(
            new Binding("Location", "Loc", Point.Parse, Point.Stringify),
            new Binding("IsSubGraphExpanded", "Expanded").MakeTwoWay()
          )
          .Add(
            // the lane header consisting of a TextBlock and an expander button
            new Panel("Horizontal") {
              Name = "HEADER", Alignment = Spot.Left
            }
              .Add(
                Builder.Make<Panel>("SubGraphExpanderButton").Set(new { Margin = 5 }),  // this remains always visible
                new TextBlock { // the lane label
                  Font = "Segoe UI, 15px", Editable = true, Margin = new Margin(2, 0, 0, 0)
                }.Bind(
                    // this is hidden when the swimlane is collapsed
                    new Binding("Visible", "IsSubGraphExpanded").OfElement(),
                    new Binding("Text").MakeTwoWay()
                  )
              ), // end Horizontal Panel
            new Panel("Auto")  // the lane consisting of a background Shape and a Placeholder representing the subgraph
              .Add(
                new Shape("Rectangle") {  // this is the resized object
                  Name = "SHAPE", Fill = "#F1F1F1", Stroke = null, StrokeWidth = 4 // strokeWidth controls spacing
                }
                  .Bind(
                    new Binding("Fill", "IsHighlighted", (h, _) => (bool)h ? "#D6D6D6" : "#F1F1F1").OfElement(),
                    new Binding("DesiredSize", "Size", Northwoods.Go.Size.Parse, Northwoods.Go.Size.Stringify)
                  ),
                new Placeholder { Padding = 12, Alignment = Spot.TopLeft },
                new TextBlock {  // this TextBlock is only seen when the swimlane is collapsed
                  Name = "LABEL", Font = "Segoe UI, 15px, style=bold", Editable = true,
                  Angle = 90, Alignment = Spot.TopLeft, Margin = new Margin(4, 0, 0, 2)
                }.Bind(
                    new Binding("Visible", "IsSubGraphExpanded", (e, _) => !(bool)e).OfElement(),
                    new Binding("Text").MakeTwoWay()
                  )
              ) // end Auto Panel
          ); // end Group

      // Set up an unmodeled Part as a legend, and place it directly on the diagram.
      myDiagram.Add(
        new Part("Table") {
          Position = new Point(10, 10), Selectable = false, Name = "LEGEND"
        }
          .Add(
            new TextBlock("Key") {
              Row = 0, Font = "Segoe UI, 14px, style=bold"
            },  // end Row 0
            new Panel("Horizontal") {
              Row = 1, Alignment = Spot.Left
            }
              .Add(
                new Shape("Rectangle") {
                  DesiredSize = new Size(10, 10), Fill = "#CC293D", Margin = 5
                },
                new TextBlock("Halted") {
                  Font = "Segoe UI, 13px, style=bold"
                }
              ), // end row 1
            new Panel("Horizontal") {
              Row = 2, Alignment = Spot.Left
            }
              .Add(
                new Shape("Rectangle") {
                  DesiredSize = new Size(10, 10), Fill = "#FFD700", Margin = 5
                },
                new TextBlock("In Progress") {
                  Font = "Segoe UI, 13px, style=bold"
                }
              ), // end row 2
            new Panel("Horizontal") {
              Row = 3, Alignment = Spot.Left
            }
              .Add(
                new Shape("Rectangle") {
                  DesiredSize = new Size(10, 10), Fill = "#009CCC", Margin = 5
                },
                new TextBlock("Completed") {
                  Font = "Segoe UI, 13px, style=bold"
                }
              ) // end row 3
          )
        );

      LoadModel();
    }



    // define a custom grid layout that makes sure the length of each lane is the same
    // and that each lane is broad enough to hold its subgraph
    public class PoolLayout : GridLayout {
      public int MINLENGTH { get; set; }
      public int MINBREADTH { get; set; }

      public PoolLayout() : base() {
        MINLENGTH = 200; // this controls the minimum length of any swim lane
        MINBREADTH = 100; // this controls the minimum breadth of any swim lane
        CellSize = new Size(1, 1);
        WrappingColumn = int.MaxValue;
        WrappingWidth = int.MaxValue;
        Spacing = new Size(0, 0);
        Alignment = GridAlignment.Position;

        Comparer = Comparer<Part>.Create((pa, pb) => {
          if (pa.Name == "LEGEND") return -1;
          if (pb.Name == "LEGEND") return 1;
          return 0;
        });
      }


      public override void DoLayout(IEnumerable<Part> coll = null) {
        var diagram = Diagram;
        if (Diagram == null) return;
        Diagram.StartTransaction("PoolLayout");
        // make sure all of the group shapes are big enough
        var minlen = _ComputeMinPoolLength();
        var it = Diagram.FindTopLevelGroups();
        while (it.MoveNext()) {
          var lane = it.Current;
          var shape = lane.SelectionElement;
          if (shape != null) { // change the desired size to be big enough in both direction
            var sz = _ComputeLaneSize(lane);
            shape.Width = (!double.IsNaN(shape.Width)) ? Math.Max(shape.Width, sz.Width) : sz.Width;
            // if you want the height of all the lanes to shrink as the maximum needed height decreases:
            shape.Height = minlen;
            // if you want the height of all the lanes to remain at the maximum height ever needed:
            // shape.Height = (isNaN(shape.Height)) ? minlen : Math.Max(shape.Height, minlen);
            var cell = lane.ResizeCellSize;
            if (!double.IsNaN(shape.Width) && !double.IsNaN(cell.Width) && cell.Width > 0) shape.Width = Math.Ceiling(shape.Width / cell.Width) * cell.Width;
            if (!double.IsNaN(shape.Height) && !double.IsNaN(cell.Height) && cell.Height > 0) shape.Height = Math.Ceiling(shape.Height / cell.Height) * cell.Height;
          }
        }
        // now do all the usual stuff, according to whatever properties have been set on this GridLayout
        base.DoLayout(coll);
        Diagram.CommitTransaction("PoolLayout");
      }

      // compute the minimum length of the whole diagram needed to hold all of the lane Groups
      private double _ComputeMinPoolLength() {
        double len = MINLENGTH;
        var it = Diagram.FindTopLevelGroups();
        while (it.MoveNext()) {
          var lane = it.Current;
          var holder = lane.Placeholder;
          if (holder != null) {
            var sz = holder.ActualBounds;
            len = Math.Max(len, sz.Height);
          }
        }
        return len;
      }

      // compute the minimum size for a particular lane group
      private Size _ComputeLaneSize(Group lane) {
        var sz = new Size(lane.IsSubGraphExpanded ? MINBREADTH : 1, MINLENGTH);
        if (lane.IsSubGraphExpanded) {
          var holder = lane.Placeholder;
          if (holder != null) {
            var hsz = holder.ActualBounds;
            sz.Width = Math.Max(sz.Width, hsz.Width);
          }
        }
        // minimum breadth needs to be big enough to hold the header
        var hdr = lane.FindElement("HEADER");
        if (hdr != null) sz.Width = Math.Max(sz.Width, hdr.ActualBounds.Width);
        return sz;
      }
    }

    // Customize the dragging tool:
    // When dragging a node set its opacity to 0.6 and move it to be in front of other nodes
    public class CustomDraggingTool : DraggingTool {
      public override void DoActivate() {
        base.DoActivate();
        CurrentPart.Opacity = 0.6;
        CurrentPart.LayerName = "Foreground";
      }

      public override void DoDeactivate() {
        CurrentPart.Opacity = 1;
        CurrentPart.LayerName = "";
        base.DoDeactivate();
      }
    }

    // this is called after nodes have been moved
    private void relayoutDiagram(object obj, DiagramEvent e) {
      foreach (var n in myDiagram.Selection) n.InvalidateLayout();
      myDiagram.LayoutDiagram();
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

  public class Model : GraphLinksModel<NodeData, string, object, LinkData, string, string> { }

  public class NodeData : Model.NodeData {
    public int Color { get; set; }
    public string Loc { get; set; }
    public bool Expanded { get; set; } = true;
  }

  public class LinkData : Model.LinkData { }

  

}
