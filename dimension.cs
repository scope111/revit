using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Structure;

namespace dimensions
{
    #region
    /*// 标注钢筋
    [TransactionAttribute(TransactionMode.Manual)]
    public class TestRebar : IExternalCommand
    {
        UIApplication m_uiApp;
        Document m_doc;

        ElementId elementId = ElementId.InvalidElementId;

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            try
            {
                initStuff(commandData);
                if (m_doc == null)
                    return Result.Failed;

                m_uiApp = commandData.Application;
                Selection sel = m_uiApp.ActiveUIDocument.Selection;
                Reference refr = sel.PickObject(ObjectType.Element);

                Rebar rebar = m_doc.GetElement(refr.ElementId) as Rebar;

                Line rebarSeg = null;
                bool bOk = getRebarSegment(rebar, out rebarSeg);
                if (!bOk)
                    return Result.Failed;

                Options options = new Options();
                // the view in which you want to place the dimension
                options.View = m_uiApp.ActiveUIDocument.ActiveView;
                options.ComputeReferences = true; // produce references
                options.IncludeNonVisibleObjects = true;
                GeometryElement wholeRebarGeometry
                  = rebar.get_Geometry(options);
                Reference refForEndOfBar = getReferenceForEndOfBar(
                  wholeRebarGeometry, rebarSeg);

                Reference refGrid = getGridRef();

                ReferenceArray refArray = new ReferenceArray();
                refArray.Append(refForEndOfBar);
                refArray.Append(refGrid);

                double dist = 10;

                // a line parallel with our rebar segment somewhere in space
                Line dimLine = rebarSeg.CreateOffset(
                  dist, XYZ.BasisY) as Line;

                using (Transaction tr = new Transaction(m_doc))
                {
                    tr.Start("Create Dimension");
                    m_doc.Create.NewDimension(
                      m_uiApp.ActiveUIDocument.ActiveView,
                      dimLine, refArray);
                    tr.Commit();
                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("exception", e.Message);
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private Reference getGridRef()
        {
            ElementId idGrd = new ElementId(397028);
            Element elemGrid = m_doc.GetElement(idGrd);
            Options options = new Options();
            // the view in which you want to place the dimension
            options.View = m_uiApp.ActiveUIDocument.ActiveView;
            options.ComputeReferences = true; // produce references
            options.IncludeNonVisibleObjects = true;
            GeometryElement wholeGridGeometry
              = elemGrid.get_Geometry(options);
            IList<Reference> allRefs = new List<Reference>();
            foreach (GeometryObject geomObj in wholeGridGeometry)
            {
                Line refLine = geomObj as Line;
                if (refLine != null && refLine.Reference != null)
                    return refLine.Reference;
            }

            return null;
        }

        private Reference getReferenceForEndOfBar(
          GeometryElement geom,
          Line rebarSeg)
        {
            foreach (GeometryObject geomObj in geom)
            {
                Solid sld = geomObj as Solid;
                if (sld != null)
                {
                    // I'll get the references from curves;
                    continue;
                }
                else
                {
                    Line refLine = geomObj as Line;
                    if (refLine != null && refLine.Reference != null)
                    {
                        // We found one reference. 
                        // Let's see if it is the correct one. 
                        // The correct referece need to be perpendicular 
                        // to rebar segement and the end point of rebar 
                        // segment should be on the reference curve.
                        double dotProd = refLine.Direction.DotProduct(
                          rebarSeg.Direction);
                        if (Math.Abs(dotProd) != 0)
                            continue; // curves are not perpendicular.

                        XYZ endPointOfRebar = rebarSeg.GetEndPoint(1);
                        IntersectionResult ir = refLine.Project(
                          endPointOfRebar);
                        if (ir == null)
                            continue; // end point of rebar segment is not on the reference curve.

                        if (Math.Abs(ir.Distance) != 0)
                            continue; // end point of rebar segment is not on the reference curve.

                        return refLine.Reference;
                    }
                }
            }
            return null;
        }

        private bool getRebarSegment(
          Rebar rebar,
          out Line rebarSeg)
        {
            rebarSeg = null;
            IList<Curve> rebarSegments
              = rebar.GetCenterlineCurves(false, true, true,
              MultiplanarOption.IncludeOnlyPlanarCurves, 0);
            if (rebarSegments.Count != 1)
                return false;

            rebarSeg = rebarSegments[0] as Line;
            if (rebarSeg == null)
                return false;

            return true;
        }

        void initStuff(ExternalCommandData commandData)
        {
            m_uiApp = commandData.Application;
            m_doc = m_uiApp.ActiveUIDocument.Document;
        }
    }*/
    #endregion
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class dimension : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Autodesk.Revit.ApplicationServices.Application app = commandData.Application.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Autodesk.Revit.DB.View view = doc.ActiveView;
            ViewType vt = view.ViewType;
            if (vt == ViewType.FloorPlan || vt == ViewType.Elevation)
            {
                #region 
                /*
                Reference eRef = uidoc.Selection.PickObject(ObjectType.Element, "Please pick a curve based element like wall.");
                Element element = doc.GetElement(eRef);
                if (eRef != null && element != null)
                {
                    XYZ dirVec = new XYZ();
                    //视图投影位置
                    XYZ viewNormal = view.ViewDirection;
                    //元素的曲线位置
                    LocationCurve locCurve = element.Location as LocationCurve;
                    if (locCurve == null || locCurve.Curve == null)
                    {
                        TaskDialog.Show("Prompt", "Selected element isn’t curve based!");
                        //  return Result.Cancelled;
                    }
                    //曲线开始坐标减去终点坐标并标准化
                    XYZ dirCur = locCurve.Curve.GetEndPoint(0).Subtract(locCurve.Curve.GetEndPoint(1)).Normalize();
                    //点积
                    double d = dirCur.DotProduct(viewNormal);
                    //点积是否为0  即这两个向量是否垂直
                    if (d > -0.000000001 && d < 0.000000001)
                    {
                        //交叉乘积
                        dirVec = dirCur.CrossProduct(viewNormal);
                        XYZ p0 = locCurve.Curve.GetEndPoint(0);
                        XYZ p2 = locCurve.Curve.GetEndPoint(1);
                      
                        XYZ p1 = new XYZ(p0.X, p2.Y, 0); 
                        XYZ dirLine = XYZ.Zero.Add(p0);
                        double distancep2 = p2.DistanceTo(p1);
                        double distancep0 = p0.DistanceTo(p1);
                        double slope = distancep2 / distancep0;
                        dirLine = dirLine.Subtract(p2);
                        //这几行主要设置标注的位置
                        XYZ newVec = XYZ.Zero.Add(dirVec);
                         newVec = newVec.Normalize().Multiply(10);
                         p1 = p1.Add(newVec);
                            p2 = p2.Add(newVec);
                       // TaskDialog.Show("Prompt", p0.ToString()+ p2.ToString()+ p1.ToString());
                        //end
                        Line newLine = Line.CreateBound(p1, p2);
                        ReferenceArray arrRefs = new ReferenceArray();
                       
                        Options options = app.Create.NewGeometryOptions();
                        options.ComputeReferences = true;
                        options.DetailLevel = ViewDetailLevel.Fine;
                        GeometryElement gelement = element.get_Geometry(options);
                        foreach (var geoObject in gelement)
                        {
                            Solid solid = geoObject as Solid;
                            if (solid == null)
                                continue;

                            FaceArrayIterator fIt = solid.Faces.ForwardIterator();
                            while (fIt.MoveNext())
                            {
                                PlanarFace p = fIt.Current as PlanarFace;
                                if (p == null)
                                    continue;

                                p2 = p.FaceNormal.CrossProduct(dirLine);
                             if (p2.IsZeroLength())
                               {
                                    arrRefs.Append(p.Reference);
                                }
                                if (2 == arrRefs.Size)
                                {
                                    break;
                                }
                            }
                            if (2 == arrRefs.Size)
                            {
                                break;
                            }
                        }
                        if (arrRefs.Size != 2)
                        {
                            TaskDialog.Show("Prompt", "Couldn’t find enough reference for creating dimension");
                            //return Result.Cancelled;
                        }
                     
                        Transaction trans = new Transaction(doc, "create dimension");
                        trans.Start();
                        doc.Create.NewDimension(doc.ActiveView, newLine, arrRefs);
                        TextNote text = AddNewTextNote(uidoc,p0,0.2,slope.ToString());
                        trans.Commit();
                    }
                    else
                    {
                        TaskDialog.Show("Prompt", "Selected element isn’t curve based!");
                        // return Result.Cancelled;
                    }
                }
            }
            else
            {
                TaskDialog.Show("Prompt", "Only support Plan View or Elevation View");
            }*/
                #endregion
               
                XYZ pt1 = XYZ.Zero;
                XYZ pt2 = new XYZ(10, 20, 0);
                XYZ pt3 = new XYZ(30, 10, 0);
                Line line1 = Line.CreateBound(pt1, pt2);
                Line line2 = Line.CreateBound(pt2, pt3);
                Transaction trans1 = new Transaction(doc, "creaerwqte dimension");
                trans1.Start();
                //辅助线
                Line dummyLine = Line.CreateBound(XYZ.Zero, XYZ.BasisY);
                //辅助点
                XYZ pt0 = new XYZ(pt3.X,pt1.Y,0);
              
                Line newLine = Line.CreateBound(pt0, pt3);
                Line newLine2 = Line.CreateBound(pt0, pt1);

                DetailLine dummy = doc.Create.NewDetailCurve(view, dummyLine) as DetailLine;
                DetailLine detailLine1 = doc.Create.NewDetailCurve(view, line1) as DetailLine;
                  DetailLine detailLine2 = doc.Create.NewDetailCurve(view, line2) as DetailLine;

                ReferenceArray arrRefs = new ReferenceArray();
               arrRefs.Append(dummy.GeometryCurve.Reference);
              arrRefs.Append(detailLine1.GeometryCurve.GetEndPointReference(0));
               arrRefs.Append(detailLine2.GeometryCurve.GetEndPointReference(1));
              
                 Dimension newdimension= doc.Create.NewDimension(view, newLine, arrRefs);
                Dimension newdimension2 = doc.Create.NewDimension(view, newLine2, arrRefs);
                List<Dimension> linearDimensions = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Dimensions)
                        .Cast<Dimension>().Where(q => q.DimensionShape == DimensionShape.Linear).ToList();
                newdimension2 = linearDimensions[0];
                Arc arc = Arc.Create(pt1, pt3, pt2);
                IList<Reference> refe=new List<Reference>();
                refe.Add(detailLine1.GeometryCurve.GetEndPointReference(0));
                refe.Add(detailLine2.GeometryCurve.GetEndPointReference(1));
                FilteredElementCollector DimesionTypeCollector = new FilteredElementCollector(doc);
                DimesionTypeCollector.OfClass(typeof(DimensionType));

                DimensionType dimesionType = DimesionTypeCollector.Cast<DimensionType>().ToList().FirstOrDefault();
             //   DimensionType newdimesionType = dimesionType.Duplicate("new dimesionType") as DimensionType;
              //  newdimesionType.LookupParameter("Color").Set(125);
              AngularDimension dd = AngularDimension.Create(doc,view,arc,refe,dimesionType);
                //删除辅助线
                doc.Delete(dummy.Id);
                trans1.Commit();

               
            }
            return Result.Succeeded;
        }
        #region Autodesk.Revit.DB.TextElement.GetMinimumAllowedWidth(Autodesk.Revit.DB.Document, Autodesk.Revit.DB.ElementId)
        public TextNote AddNewTextNote(UIDocument uiDoc,XYZ textLoc, double noteWidth ,string text)
        {
            Document doc = uiDoc.Document;
            ElementId defaultTextTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
            

            // make sure note width works for the text type
            double minWidth = TextNote.GetMinimumAllowedWidth(doc, defaultTextTypeId);
            double maxWidth = TextNote.GetMaximumAllowedWidth(doc, defaultTextTypeId);
            if (noteWidth < minWidth)
            {
                noteWidth = minWidth;
            }
            else if (noteWidth > maxWidth)
            {
                noteWidth = maxWidth;
            }

            TextNoteOptions opts = new TextNoteOptions(defaultTextTypeId);
            opts.HorizontalAlignment = HorizontalTextAlignment.Left;
            opts.Rotation = 0;
            
            TextNote textNote = TextNote.Create(doc, doc.ActiveView.Id, textLoc, noteWidth, text, opts);

            return textNote;
        }
        #endregion



    }
}

   

