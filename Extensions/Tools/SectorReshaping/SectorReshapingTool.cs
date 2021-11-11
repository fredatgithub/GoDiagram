﻿/*
*  Copyright (C) 1998-2020 by Northwoods Software Corporation. All Rights Reserved.
*/

/*
* This is an extension and not part of the main GoJS library.
* Note that the API for this class may change with any version, even point releases.
* If you intend to use an extension in production, you should copy the code to your own source directory.
* Extensions can be found in the GoJS kit under the extensions or extensionsTS folders.
* See the Extensions intro page (https://gojs.Net/latest/intro/extensions.Html) for more information.
*/

using System;
using Northwoods.Go.Models;

namespace Northwoods.Go.Tools.Extensions {

  /// <summary>
  /// The SectorReshapingTool class lets the user interactively modify the angles of a "pie"-shaped sector of a circle.
  /// When a node is selected, this shows two handles for changing the angles of the sides of the sector and one handle for changing the radius.
  ///
  /// This depends on there being three data properties, "angle", "sweep", and "radius",
  /// that hold the needed information to be able to reproduce the sector.
  ///
  /// If you want to experiment with this extension, try the <a href="../../extensionsTS/SectorReshaping.Html">Sector Reshaping</a> sample.
  /// </summary>
  /// @category Tool Extension
  public class SectorReshapingTool : Tool {
    private GraphObject _Handle;
    private double _OriginalRadius = 0;
    private double _OriginalAngle = 0;
    private double _OriginalSweep = 0;

    private string _RadiusProperty = "Radius";
    private string _AngleProperty = "Angle";
    private string _SweepProperty = "Sweep";

    /// <summary>
    /// Constructs a SectorReshapingTool and sets the name for the tool.
    /// </summary>
    public SectorReshapingTool() : base() {
      Name = "SectorReshaping";
    }

    /// <summary>
    /// Gets or sets the name of the data property for the sector radius.
    ///
    /// The default value is "radius".
    /// </summary>
    public string RadiusProperty {
      get {
        return _RadiusProperty;
      }
      set {
        _RadiusProperty = value;
      }
    }

    /// <summary>
    /// Gets or sets the name of the data property for the sector start angle.
    ///
    /// The default value is "angle".
    /// </summary>
    public string AngleProperty {
      get {
        return _AngleProperty;
      }
      set {
        _AngleProperty = value;
      }
    }

    /// <summary>
    /// Gets or sets the name of the data property for the sector sweep angle.
    ///
    /// The default value is "sweep".
    /// </summary>
    public string SweepProperty {
      get {
        return _SweepProperty;
      }
      set {
        _SweepProperty = value;
      }
    }

    /// <summary>
    /// This tool can only start if Diagram.AllowReshape is true and the mouse-down event
    /// is at a tool handle created by this tool.
    /// </summary>
    public override bool CanStart() {
      if (!IsEnabled) return false;
      var diagram = Diagram;
      if (diagram.IsReadOnly) return false;
      if (!diagram.AllowReshape) return false;
      var h = FindToolHandleAt(diagram.FirstInput.DocumentPoint, Name);
      return (h != null);
    }

    /// <summary>
    /// If the Part is selected, show two angle-changing tool handles and one radius-changing tool handle.
    /// </summary>
    public override void UpdateAdornments(Part part) {
      var data = part.Data;
      if (part.IsSelected && data != null && !Diagram.IsReadOnly) {
        var ad = part.FindAdornment(Name);
        if (ad == null) {
          ad =
              new Adornment(PanelLayoutSpot.Instance).Add(
                  new Placeholder(),
                  new Shape {
                    Figure = "Diamond",
                    Name = "RADIUS",
                    Fill = "lime",
                    Width = 10,
                    Height = 10,
                    Cursor = "move"
                  }.Bind(
                    new Binding("Alignment", "", (d, obj) => {
                      var angle = SectorReshapingTool.GetAngle(d);
                      var sweep = SectorReshapingTool.GetSweep(d);
                      var p = new Point(0.5, 0).Rotate(angle + sweep / 2);
                      return new Spot(0.5 + p.X, 0.5 + p.Y);
                    })
                  ),
                  new Shape {
                    Figure = "Circle",
                    Name = "ANGLE",
                    Fill = "lime",
                    Width = 8,
                    Height = 8,
                    Cursor = "move"
                  }.Bind(
                    new Binding("Alignment", "", (d, obj) => {
                      var angle = SectorReshapingTool.GetAngle(d);
                      var sweep = SectorReshapingTool.GetSweep(d);
                      var p = new Point(0.5, 0).Rotate(angle);
                      return new Spot(0.5 + p.X, 0.5 + p.Y);
                    })
                  ),
                  new Shape {
                    Figure = "Circle",
                    Name = "SWEEP",
                    Fill = "lime",
                    Width = 8,
                    Height = 8,
                    Cursor = "move"
                  }.Bind(
                    new Binding("Alignment", "", (d, obj) => {
                      var angle = SectorReshapingTool.GetAngle(d);
                      var sweep = SectorReshapingTool.GetSweep(d);
                      var p = new Point(0.5, 0).Rotate(angle + sweep);
                      return new Spot(0.5 + p.X, 0.5 + p.Y);
                    })
                  )
                );
          ad.AdornedElement = part.LocationElement;
          part.AddAdornment(Name, ad);
        } else {
          ad.Location = part.Position;
          var ns = part.NaturalBounds;
          if (ad.Placeholder != null) ad.Placeholder.DesiredSize = new Size((ns.Width) * part.Scale, (ns.Height) * part.Scale);
          ad.UpdateTargetBindings();
        }
      } else {
        part.RemoveAdornment(Name);
      }
    }

    /// <summary>
    /// Remember the original angles and radius and start a transaction.
    /// </summary>
    public override void DoActivate() {
      var diagram = Diagram;
      _Handle = FindToolHandleAt(diagram.FirstInput.DocumentPoint, Name);
      if (_Handle == null) return;
      var part = (_Handle.Part as Adornment).AdornedPart;
      if (part == null || part.Data == null) return;

      var data = part.Data;
      _OriginalRadius = SectorReshapingTool.GetRadius(data);
      _OriginalAngle = SectorReshapingTool.GetAngle(data);
      _OriginalSweep = SectorReshapingTool.GetSweep(data);

      StartTransaction(Name);
      IsActive = true;
    }

    /// <summary>
    /// Stop the transaction.
    /// </summary>
    public override void DoDeactivate() {
      StopTransaction();

      _Handle = null;
      IsActive = false;
    }

    /// <summary>
    /// Restore the original angles and radius and then stop this tool.
    /// </summary>
    public override void DoCancel() {
      if (_Handle != null) {
        var part = (_Handle.Part as Adornment).AdornedPart;
        if (part != null) {
          var model = Diagram.Model;
          model.Set(part.Data, _RadiusProperty, _OriginalRadius);
          model.Set(part.Data, _AngleProperty, _OriginalAngle);
          model.Set(part.Data, _SweepProperty, _OriginalSweep);
        }
      }
      StopTool();
    }

    /// <summary>
    /// Depending on the current handle being dragged, update the "radius", the "angle", or the "sweep"
    /// properties on the model data.
    /// Those property names are currently parameterized as static members of SectorReshapingTool.
    /// </summary>
    public override void DoMouseMove() {
      var diagram = Diagram;
      var h = _Handle;
      if (IsActive && h != null) {
        var adorned = (h.Part as Adornment).AdornedElement;
        if (adorned == null) return;
        var center = adorned.GetDocumentPoint(Spot.Center);
        var node = adorned.Part;
        if (node == null || node.Data == null) return;
        var mouse = diagram.LastInput.DocumentPoint;
        if (h.Name == "RADIUS") {
          var dst = Math.Sqrt(center.DistanceSquared(mouse));
          diagram.Model.Set(node.Data, _RadiusProperty, dst);
        } else if (h.Name == "ANGLE") {
          var dir = center.Direction(mouse);
          diagram.Model.Set(node.Data, _AngleProperty, dir);
        } else if (h.Name == "SWEEP") {
          var dir = center.Direction(mouse);
          var ang = SectorReshapingTool.GetAngle(node.Data);
          var swp = (dir - ang + 360) % 360;
          if (swp > 359) swp = 360;  // make it easier to get a full circle
          diagram.Model.Set(node.Data, _SweepProperty, swp);
        }
      }
    }

    /// <summary>
    /// Finish the transaction and stop the tool.
    /// </summary>
    public override void DoMouseUp() {
      var diagram = Diagram;
      if (IsActive) {
        TransactionResult = Name;  // successful finish
      }
      StopTool();
    }

    // static functions for getting data
    /** @hidden @internal */
    public static double GetRadius(object data) {
      // null-coalesce operator to handle null
      var radius = data.GetType().GetProperty("Radius").GetValue(data) as double? ?? double.NaN;
      if (double.IsNaN(radius) || radius <= 0) radius = 50;
      return radius;
    }

    /** @hidden @internal */
    public static double GetAngle(object data) {
      // null-coalesce operator to handle null
      var angle = data.GetType().GetProperty("Angle").GetValue(data) as double? ?? double.NaN;
      if (double.IsNaN(angle)) angle = 0;
      else angle %= 360;
      return angle;
    }

    /** @hidden @internal */
    public static double GetSweep(object data) {
      // null-coalesce operator to handle null
      var sweep = data.GetType().GetProperty("Sweep").GetValue(data) as double? ?? double.NaN;
      if (double.IsNaN(sweep)) sweep = 360;
      return sweep;
    }
  }
}
