﻿/*
*  Copyright (C) 1998-2023 by Northwoods Software Corporation. All Rights Reserved.
*/

/*
* This is an extension and not part of the main GoDiagram library.
* Note that the API for this class may change with any version, even point releases.
* If you intend to use an extension in production, you should copy the code to your own source directory.
* Extensions can be found in the GoDiagram repository (https://github.com/NorthwoodsSoftware/GoDiagram/tree/main/Extensions).
* See the Extensions intro page (https://godiagram.com/intro/extensions.html) for more information.
*/

using System;
using Northwoods.Go.PanelLayouts;

namespace Northwoods.Go.Tools.Extensions {
  /// <summary>
  /// The RowResizingTool class lets the user resize each row of a named Table Panel in a selected Part.
  /// </summary>
  /// @category Tool Extension
  public class RowResizingTool : Tool {
    private GraphObject _HandleArchetype;
    private string _TableName = "TABLE";

    // internal state
    private GraphObject _Handle;
    private Panel _AdornedTable;

    /// <summary>
    /// Constructs a RowResizingTool and sets the handle and name of the tool.
    /// </summary>
    public RowResizingTool() : base() {
      var h = new Shape {
        GeometryString = "M0 0 H14 M0 2 H14",
        DesiredSize = new Size(14, 2),
        Cursor = "row-resize",
        GeometryStretch = GeometryStretch.None,
        Background = "rgba(255,255,255,0.5)",
        Stroke = "rgba(30,144,255,0.5)"
      };
      _HandleArchetype = h;
      Name = "RowResizing";
    }

    /// <summary>
    /// Gets or sets small GraphObject that is copied as a resize handle for each row.
    /// </summary>
    /// <remarks>
    /// This tool expects that this object's <see cref="GraphObject.DesiredSize"/> (width and height) has been set to real numbers.
    ///
    /// The default value is a <see cref="Shape"/> that is a narrow rectangle.
    /// </remarks>
    public GraphObject HandleArchetype {
      get {
        return _HandleArchetype;
      }
      set {
        _HandleArchetype = value;
      }
    }

    /// <summary>
    /// Gets or sets the name of the Table Panel to be resized.
    /// </summary>
    /// <remarks>
    /// The default value is the name "TABLE".
    /// </remarks>
    public string TableName {
      get {
        return _TableName;
      }
      set {
        _TableName = value;
      }
    }

    /// <summary>
    /// This read-only property returns the <see cref="GraphObject"/> that is the tool handle being dragged by the user.
    /// </summary>
    /// <remarks>
    /// This will be contained by an <see cref="Adornment"/> whose category is "RowResizing".
    /// Its <see cref="Adornment.AdornedElement"/> is the same as the <see cref="AdornedTable"/>.
    /// </remarks>
    public GraphObject Handle {
      get {
        return _Handle;
      }
    }

    /// <summary>
    /// This read-only property returns the <see cref="Panel"/> of type <see cref="PanelLayoutTable"/> whose rows are being resized.
    /// </summary>
    /// <remarks>
    /// This must be contained within the selected <see cref="Part"/>.
    /// </remarks>
    public Panel AdornedTable {
      get {
        return _AdornedTable;
      }
    }

    /// <summary>
    /// Show an <see cref="Adornment"/> with a resize handle at each row.
    /// </summary>
    /// <remarks>
    /// Don't show anything if <see cref="TableName"/> doesn't identify a <see cref="Panel"/>
    /// that has a <see cref="Panel.Type"/> of type <see cref="PanelLayoutTable"/>.
    /// </remarks>
    public override void UpdateAdornments(Part part) {
      if ((part == null) || (part is Link)) return;  // this tool never applies to Links
      if (part.IsSelected && !Diagram.IsReadOnly) {
        var selelt = part.FindElement(TableName);
        if (selelt is Panel panel && selelt.ActualBounds.IsReal() && selelt.IsVisibleElement() &&
            part.ActualBounds.IsReal() && part.IsVisible() &&
            panel.Type == PanelLayoutTable.Instance) {
          var table = selelt as Panel;
          var adornment = part.FindAdornment(Name);
          if (adornment == null) {
            adornment = MakeAdornment(table);
            part.AddAdornment(Name, adornment);
          }
          if (adornment != null) {
            var pad = (Margin)(table.Padding);
            var numrows = table.RowCount;
            // update the position/alignment of each handle
            foreach (var h in adornment.Elements) {
              if (!h.Pickable) continue;
              var rowdef = table.GetRowDefinition(h.Row);
              var hgt = rowdef.ActualHeight;
              if (hgt > 0) hgt = rowdef.TotalHeight;
              var sep = 0d;
              // find next non-zero-height row's SeparatorStrokeWidth
              var idx = h.Row + 1;
              while (idx < numrows && table.GetRowDefinition(idx).ActualHeight == 0) idx++;
              if (idx < numrows) {
                sep = table.GetRowDefinition(idx).SeparatorStrokeWidth;
                if (double.IsNaN(sep)) sep = table.DefaultRowSeparatorStrokeWidth;
              }
              h.Alignment = new Spot(0, 0, pad.Left + h.Width / 2, pad.Top + rowdef.ActualY + hgt + sep / 2);
            }
            adornment.LocationElement.DesiredSize = table.ActualBounds.Size;
            adornment.Location = table.GetDocumentPoint(adornment.LocationSpot);
            adornment.Angle = table.GetDocumentAngle();
            return;
          }
        }
      }
      part.RemoveAdornment(Name);
    }

    /// <summary>
    /// Undocumented.
    /// </summary>
    [Undocumented]
    public virtual Adornment MakeAdornment(Panel table) {
      // the Adornment is a Spot Panel holding resize handles
      var adornment = new Adornment("Spot") {
        Category = Name,
        AdornedElement = table,
        LocationElementName = "BLOCK"
      };
      // create the "main" element of the Spot Panel
      var block = new TextBlock {
        Name = "BLOCK",
        Pickable = false  // it's transparent and not pickable
      };  // doesn't matter much what this is

      adornment.Add(block);
      // now add resize handles for each row
      for (var i = 0; i < table.RowCount; i++) {
        var rowdef = table.GetRowDefinition(i);
        var h = MakeHandle(table, rowdef);
        if (h != null) adornment.Add(h);
      }
      return adornment;
    }

    /// <summary>
    /// Undocumented.
    /// </summary>
    [Undocumented]
    public virtual GraphObject MakeHandle(Panel table, RowDefinition rowdef) {
      var h = HandleArchetype;
      if (h == null) return null;
      var c = h.Copy();
      c.Row = rowdef.Row;
      return c;
    }


    /// <summary>
    /// This tool may run when there is a mouse-down event on a "RowResizing" handle,
    /// the diagram is not read-only, the left mouse button is being used,
    /// and this tool's adornment's resize handle is at the current mouse point.
    /// </summary>
    public override bool CanStart() {
      if (!IsEnabled) return false;

      var diagram = Diagram;
      if (diagram.IsReadOnly) return false;
      if (!diagram.LastInput.Left) return false;
      var h = FindToolHandleAt(diagram.FirstInput.DocumentPoint, Name);
      return (h != null);
    }

    /// <summary>
    /// Find the <see cref="Handle"/>, ensure type <see cref="PanelLayoutTable"/>, capture the mouse, and start a transaction.
    /// </summary>
    /// <remarks>
    /// If the call to <see cref="Tool.FindToolHandleAt"/> finds no "RowResizing" tool handle, this method returns without activating this tool.
    /// </remarks>
    public override void DoActivate() {
      var diagram = Diagram;
      _Handle = FindToolHandleAt(diagram.FirstInput.DocumentPoint, Name);
      if (Handle == null) return;
      if ((Handle.Part as Adornment).AdornedElement is not Panel panel || panel.Type != PanelLayoutTable.Instance) return;
      _AdornedTable = panel;
      diagram.IsMouseCaptured = true;
      StartTransaction(Name);
      IsActive = true;
    }

    /// <summary>
    /// Stop the current transaction and release the mouse.
    /// </summary>
    public override void DoDeactivate() {
      StopTransaction();
      _Handle = null;
      _AdornedTable = null;
      var diagram = Diagram;
      diagram.IsMouseCaptured = false;
      IsActive = false;
    }

    /// <summary>
    /// Call <see cref="Resize"/> with a new size determined by the current mouse point.
    /// </summary>
    /// <remarks>
    /// This determines the new bounds by calling <see cref="ComputeResize"/>.
    /// </remarks>
    public override void DoMouseMove() {
      var diagram = Diagram;
      if (IsActive) {
        var newpt = ComputeResize(diagram.LastInput.DocumentPoint);
        Resize(newpt);
      }
    }

    /// <summary>
    /// Call <see cref="Resize"/> with the final bounds based on the most recent mouse point, and commit the transaction.
    /// </summary>
    /// <remarks>
    /// This determines the new bounds by calling <see cref="ComputeResize"/>.
    /// </remarks>
    public override void DoMouseUp() {
      var diagram = Diagram;
      if (IsActive) {
        var newpt = ComputeResize(diagram.LastInput.DocumentPoint);
        Resize(newpt);
        TransactionResult = Name;  // success
      }
      StopTool();
    }

    /// <summary>
    /// Change the <see cref="RowDefinition.Height"/> of the row being resized
    /// to a value corresponding to the given mouse point.
    /// </summary>
    /// <param name="newPoint">the value returned by the call to <see cref="ComputeResize"/></param>
    public void Resize(Point newPoint) {
      var table = AdornedTable;
      if (table == null) return;
      var h = Handle;
      if (h == null) return;
      var pad = (Margin)(table.Padding);
      var numrows = table.RowCount;
      var locpt = table.GetLocalPoint(newPoint);
      var rowdef = table.GetRowDefinition(h.Row);
      var sep = 0d;
      var idx = h.Row + 1;
      while (idx < numrows && table.GetRowDefinition(idx).ActualHeight == 0) idx++;
      if (idx < numrows) {
        sep = table.GetRowDefinition(idx).SeparatorStrokeWidth;
        if (double.IsNaN(sep)) sep = table.DefaultRowSeparatorStrokeWidth;
      }
      rowdef.Height = Math.Max(0, locpt.Y - pad.Top - rowdef.ActualY - (rowdef.TotalHeight - rowdef.ActualHeight) - sep / 2);
    }


    /// <summary>
    /// This can be overridden in order to customize the resizing process.
    /// </summary>
    /// <param name="p">the point where the handle is being dragged.</param>
    public virtual Point ComputeResize(Point p) {
      return p;
    }

    /// <summary>
    /// Pressing the Delete key removes any column width setting and stops this tool.
    /// </summary>
    public override void DoKeyDown() {
      if (!IsActive) return;
      var diagram = Diagram;
      var e = diagram.LastInput;
      if (e.Key == "DELETE") {  // remove height setting
        if (AdornedTable != null && Handle != null) {
          var rowdef = AdornedTable.GetRowDefinition(Handle.Row);
          rowdef.Height = double.NaN;
          TransactionResult = Name;  // success
          StopTool();
        }
      } else {
        base.DoKeyDown();
      }
    }

  }
}

