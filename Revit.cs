using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Net;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Mechanical;
using System.Xml;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.DB.Events;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.ApplicationServices;






namespace Wxx_Revit
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Revit : IExternalCommand
    {

       

        static AddInId appId = new AddInId(new Guid("B6FBC0C1-F3AE-4ffa-AB46-B4CF94304827"));
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //创建事务t1
                Transaction t1 = new Transaction(doc, "创建");//创建事务
           t1.Start();
                        Material oldmat = WxxRTU.GetMaterial("GFRC", uidoc);
          //  var oldmat = doc.Settings.Categories.OfType<Material>().FirstOrDefault(m => m.Name.Equals("Concrete"));

            Material newmat = oldmat.Duplicate("myMaterial"); 
            double x = 1000;
            double y = 1000;
                     WxxRTU.ChangeRenderingTexturePath(newmat, doc, @"G:\C#\Wxx_Revit0617\timg.jpeg", x,y);
                        t1.Commit();
                     

                   
            WxxRTU.CreateCylinder(doc,oldmat);
            return Result.Succeeded;

            #region 墙
            /* //创建一道结构墙
             Wall wall = Wall.Create(doc, Line.CreateBound(new XYZ(), new XYZ(0, 10, 0)), Level.Create(doc, 0).Id,false);//false表示创建结构墙

             TaskDialog.Show("Tip", "结构墙已创建完成!");

             WxxRTU.CreateCSforWall(wall, uidoc, mat);
             Autodesk.Revit.ApplicationServices.Application revitApp =commandData.Application.Application;
             // WxxRTU.loadAppearance(revitApp,doc,mat);*/
            #endregion
            //拉伸 可以用
            #region 
            /*  //创建事务:拉伸
              Transaction trans = new Transaction(doc, "LS");
              trans.Start();

              //创建拉伸的闭合曲线
              Curve c1 = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 10, 0));
              Curve c2 = Line.CreateBound(new XYZ(0, 10, 0), new XYZ(10, 10, 0));
              Curve c3 = Line.CreateBound(new XYZ(10, 10, 0), new XYZ(10, 0, 0));
              Curve c4 = Line.CreateBound(new XYZ(10, 0, 0), new XYZ(0, 0, 0));
              CurveArray curveArray = new CurveArray();
              curveArray.Append(c1);
              curveArray.Append(c2);
              curveArray.Append(c3);
              curveArray.Append(c4);
              CurveArrArray curveArr = new CurveArrArray();
              curveArr.Append(curveArray);

              //创建拉伸
              Plane p1 = Plane.CreateByNormalAndOrigin(new XYZ(0, 0, 1), XYZ.Zero);
              Extrusion solid = doc.FamilyCreate.NewExtrusion(true, curveArr, SketchPlane.Create(doc, p1), 10);
              doc.FamilyManager.NewType("我创建的类");
              Selection sel = uidoc.Selection;
              Reference ref1 = sel.PickObject(ObjectType.Element, "选择一个族实例");
              Element elem = doc.GetElement(ref1);

              ElementId lalala=solid.Id;
              Material mat2 = WxxRTU.CreateSampleBrickMaterial(uidoc);   //从elementId中获得材料


             ICollection<ElementId> matId = elem.GetMaterialIds(true);

              foreach (var item in matId)
              {

                  TaskDialog.Show("REVIT", item.ToString());

                  GeometryElement geometryElement = solid.get_Geometry(new Options());
                  foreach (GeometryObject geometryObject in geometryElement)
                  {
                      if (geometryObject is Solid)
                      {
                          Solid solid1 = geometryObject as Solid;
                          foreach (Face face in solid1.Faces)
                          {
                              if (doc.IsPainted(solid.Id, face) == false)
                              {
                                  doc.Paint(solid.Id, face, mat2.Id);
                              }
                          }
                      }
                  }
              }



              trans.Commit();
              return Result.Succeeded;

              */
            #endregion
            //绘制钢筋用不了
            #region 
            /* UIApplication uiApp = commandData.Application;

             Selection sel = uiApp.ActiveUIDocument.Selection;

             Application app = uiApp.Application;

             Document doc = uiApp.ActiveUIDocument.Document;



             RebarBarType barType = null;

             RebarHookType hookType = null;



             Transaction transaction = new Transaction(doc, "CreateRebar Tool");

             try

             {

                 transaction.Start();



                 IList<Curve> curves1 = new List<Curve>();
                 ICollection<ElementId> selectedIds = sel.GetElementIds();

                 Curve c1 = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 10, 0));
                 Curve c2 = Line.CreateBound(new XYZ(0, 10, 0), new XYZ(10, 10, 0));
                 Curve c3 = Line.CreateBound(new XYZ(10, 10, 0), new XYZ(10, 0, 0));
                 Curve c4 = Line.CreateBound(new XYZ(10, 0, 0), new XYZ(0, 0, 0));
                 curves1.Add(c1);
                 curves1.Add(c2);
                 curves1.Add(c3);
                 curves1.Add(c4);*/
            /* foreach (ElementId id in selectedIds)
             {
                 Element elem = doc.GetElement(id);
                 if ( elem is ModelLine)

                 {

                     Line l = (elem as ModelLine).GeometryCurve as Line;

                     Line line = Line.CreateBound(l.GetEndPoint(0), l.GetEndPoint(1));

                     curves1.Add(line);

                 }
             }
             */

            /*



          
                foreach (RebarBarType bt in elements)

                {

                    barType = bt;

                    break;

                }

                foreach (RebarHookType ht in elements)

                {

                    hookType = ht;

                }


                //RebarBarType bartype = new FilteredElementCollector(doc).OfClass(typeof(RebarBarType)).FirstOrDefault(t => t.Name == SystemInfoConfig.SteelTypeName) as RebarBarType;
                {



                    Reference hasPickOne = sel.PickObject(ObjectType.Element);

                    FamilyInstance hostFi = doc.GetElement(hasPickOne.ElementId) as FamilyInstance;

                    Rebar rebar = CreateRebar1(uidoc, curves1, hostFi, barType, hookType);

                }

            }

            catch (Exception ex)

            {

                message += ex.Message;

                return Autodesk.Revit.UI.Result.Failed;

            }

            finally

            {

                transaction.Commit();

            }



            return Result.Succeeded;*/
            #endregion

        }

         
        /*暂时没用
        private BRepBuilder CreateCubeImpl()
        {
            // create a BRepBuilder; add faces to build a cube

            BRepBuilder brepBuilder = new BRepBuilder(BRepType.Solid);

            // a cube 100x100x100, from (0,0,0) to (100, 100, 100)

            // 1. Planes.
            // naming convention for faces and planes:
            // We are looking at this cube in an isometric view. X is down and to the left of us, Y is horizontal and points to the right, Z is up.
            // front and back faces are along the X axis, left and right are along the Y axis, top and bottom are along the Z axis.
            Plane bottom = Plane.CreateByOriginAndBasis(new XYZ(50, 50, 0), new XYZ(1, 0, 0), new XYZ(0, 1, 0)); // bottom. XY plane, Z = 0, normal pointing inside the cube.
            Plane top = Plane.CreateByOriginAndBasis(new XYZ(50, 50, 100), new XYZ(1, 0, 0), new XYZ(0, 1, 0)); // top. XY plane, Z = 100, normal pointing outside the cube.
            Plane front = Plane.CreateByOriginAndBasis(new XYZ(100, 50, 50), new XYZ(0, 0, 1), new XYZ(0, 1, 0)); // front side. ZY plane, X = 0, normal pointing inside the cube.
            Plane back = Plane.CreateByOriginAndBasis(new XYZ(0, 50, 50), new XYZ(0, 0, 1), new XYZ(0, 1, 0)); // back side. ZY plane, X = 0, normal pointing outside the cube.
            Plane left = Plane.CreateByOriginAndBasis(new XYZ(50, 0, 50), new XYZ(0, 0, 1), new XYZ(1, 0, 0)); // left side. ZX plane, Y = 0, normal pointing inside the cube
            Plane right = Plane.CreateByOriginAndBasis(new XYZ(50, 100, 50), new XYZ(0, 0, 1), new XYZ(1, 0, 0)); // right side. ZX plane, Y = 100, normal pointing outside the cube
                                                                                                                  //Note that the alternating of "inside/outside" matches the alternating of "true/false" in the next block that defines faces. 
                                                                                                                  //There must be a correspondence to ensure that all faces are correctly oriented to point out of the solid.
                                                                                                                  // 2. Faces.
            BRepBuilderGeometryId faceId_Bottom = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(bottom, null), true);
            BRepBuilderGeometryId faceId_Top = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(top, null), false);
            BRepBuilderGeometryId faceId_Front = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(front, null), true);
            BRepBuilderGeometryId faceId_Back = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(back, null), false);
            BRepBuilderGeometryId faceId_Left = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(left, null), true);
            BRepBuilderGeometryId faceId_Right = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(right, null), false);

            // 3. Edges.

            // 3.a (define edge geometry)
            // walk around bottom face
            BRepBuilderEdgeGeometry edgeBottomFront = BRepBuilderEdgeGeometry.Create(new XYZ(100, 0, 0), new XYZ(100, 100, 0));
            BRepBuilderEdgeGeometry edgeBottomRight = BRepBuilderEdgeGeometry.Create(new XYZ(100, 100, 0), new XYZ(0, 100, 0));
            BRepBuilderEdgeGeometry edgeBottomBack = BRepBuilderEdgeGeometry.Create(new XYZ(0, 100, 0), new XYZ(0, 0, 0));
            BRepBuilderEdgeGeometry edgeBottomLeft = BRepBuilderEdgeGeometry.Create(new XYZ(0, 0, 0), new XYZ(100, 0, 0));

            // now walk around top face
            BRepBuilderEdgeGeometry edgeTopFront = BRepBuilderEdgeGeometry.Create(new XYZ(100, 0, 100), new XYZ(100, 100, 100));
            BRepBuilderEdgeGeometry edgeTopRight = BRepBuilderEdgeGeometry.Create(new XYZ(100, 100, 100), new XYZ(0, 100, 100));
            BRepBuilderEdgeGeometry edgeTopBack = BRepBuilderEdgeGeometry.Create(new XYZ(0, 100, 100), new XYZ(0, 0, 100));
            BRepBuilderEdgeGeometry edgeTopLeft = BRepBuilderEdgeGeometry.Create(new XYZ(0, 0, 100), new XYZ(100, 0, 100));

            // sides
            BRepBuilderEdgeGeometry edgeFrontRight = BRepBuilderEdgeGeometry.Create(new XYZ(100, 100, 0), new XYZ(100, 100, 100));
            BRepBuilderEdgeGeometry edgeRightBack = BRepBuilderEdgeGeometry.Create(new XYZ(0, 100, 0), new XYZ(0, 100, 100));
            BRepBuilderEdgeGeometry edgeBackLeft = BRepBuilderEdgeGeometry.Create(new XYZ(0, 0, 0), new XYZ(0, 0, 100));
            BRepBuilderEdgeGeometry edgeLeftFront = BRepBuilderEdgeGeometry.Create(new XYZ(100, 0, 0), new XYZ(100, 0, 100));

            // 3.b (define the edges themselves)
            BRepBuilderGeometryId edgeId_BottomFront = brepBuilder.AddEdge(edgeBottomFront);
            BRepBuilderGeometryId edgeId_BottomRight = brepBuilder.AddEdge(edgeBottomRight);
            BRepBuilderGeometryId edgeId_BottomBack = brepBuilder.AddEdge(edgeBottomBack);
            BRepBuilderGeometryId edgeId_BottomLeft = brepBuilder.AddEdge(edgeBottomLeft);
            BRepBuilderGeometryId edgeId_TopFront = brepBuilder.AddEdge(edgeTopFront);
            BRepBuilderGeometryId edgeId_TopRight = brepBuilder.AddEdge(edgeTopRight);
            BRepBuilderGeometryId edgeId_TopBack = brepBuilder.AddEdge(edgeTopBack);
            BRepBuilderGeometryId edgeId_TopLeft = brepBuilder.AddEdge(edgeTopLeft);
            BRepBuilderGeometryId edgeId_FrontRight = brepBuilder.AddEdge(edgeFrontRight);
            BRepBuilderGeometryId edgeId_RightBack = brepBuilder.AddEdge(edgeRightBack);
            BRepBuilderGeometryId edgeId_BackLeft = brepBuilder.AddEdge(edgeBackLeft);
            BRepBuilderGeometryId edgeId_LeftFront = brepBuilder.AddEdge(edgeLeftFront);

            // 4. Loops.
            BRepBuilderGeometryId loopId_Bottom = brepBuilder.AddLoop(faceId_Bottom);
            BRepBuilderGeometryId loopId_Top = brepBuilder.AddLoop(faceId_Top);
            BRepBuilderGeometryId loopId_Front = brepBuilder.AddLoop(faceId_Front);
            BRepBuilderGeometryId loopId_Back = brepBuilder.AddLoop(faceId_Back);
            BRepBuilderGeometryId loopId_Right = brepBuilder.AddLoop(faceId_Right);
            BRepBuilderGeometryId loopId_Left = brepBuilder.AddLoop(faceId_Left);

            // 5. Co-edges. 
            // Bottom face. All edges reversed
            brepBuilder.AddCoEdge(loopId_Bottom, edgeId_BottomFront, true); // other direction in front loop
            brepBuilder.AddCoEdge(loopId_Bottom, edgeId_BottomLeft, true);  // other direction in left loop
            brepBuilder.AddCoEdge(loopId_Bottom, edgeId_BottomBack, true);  // other direction in back loop
            brepBuilder.AddCoEdge(loopId_Bottom, edgeId_BottomRight, true); // other direction in right loop
            brepBuilder.FinishLoop(loopId_Bottom);
            brepBuilder.FinishFace(faceId_Bottom);

            // Top face. All edges NOT reversed.
            brepBuilder.AddCoEdge(loopId_Top, edgeId_TopFront, false);  // other direction in front loop.
            brepBuilder.AddCoEdge(loopId_Top, edgeId_TopRight, false);  // other direction in right loop
            brepBuilder.AddCoEdge(loopId_Top, edgeId_TopBack, false);   // other direction in back loop
            brepBuilder.AddCoEdge(loopId_Top, edgeId_TopLeft, false);   // other direction in left loop
            brepBuilder.FinishLoop(loopId_Top);
            brepBuilder.FinishFace(faceId_Top);

            // Front face.
            brepBuilder.AddCoEdge(loopId_Front, edgeId_BottomFront, false); // other direction in bottom loop
            brepBuilder.AddCoEdge(loopId_Front, edgeId_FrontRight, false);  // other direction in right loop
            brepBuilder.AddCoEdge(loopId_Front, edgeId_TopFront, true); // other direction in top loop.
            brepBuilder.AddCoEdge(loopId_Front, edgeId_LeftFront, true); // other direction in left loop.
            brepBuilder.FinishLoop(loopId_Front);
            brepBuilder.FinishFace(faceId_Front);

            // Back face
            brepBuilder.AddCoEdge(loopId_Back, edgeId_BottomBack, false); // other direction in bottom loop
            brepBuilder.AddCoEdge(loopId_Back, edgeId_BackLeft, false);   // other direction in left loop.
            brepBuilder.AddCoEdge(loopId_Back, edgeId_TopBack, true); // other direction in top loop
            brepBuilder.AddCoEdge(loopId_Back, edgeId_RightBack, true); // other direction in right loop.
            brepBuilder.FinishLoop(loopId_Back);
            brepBuilder.FinishFace(faceId_Back);

            // Right face
            brepBuilder.AddCoEdge(loopId_Right, edgeId_BottomRight, false); // other direction in bottom loop
            brepBuilder.AddCoEdge(loopId_Right, edgeId_RightBack, false);  // other direction in back loop
            brepBuilder.AddCoEdge(loopId_Right, edgeId_TopRight, true);   // other direction in top loop
            brepBuilder.AddCoEdge(loopId_Right, edgeId_FrontRight, true); // other direction in front loop
            brepBuilder.FinishLoop(loopId_Right);
            brepBuilder.FinishFace(faceId_Right);

            // Left face
            brepBuilder.AddCoEdge(loopId_Left, edgeId_BottomLeft, false); // other direction in bottom loop
            brepBuilder.AddCoEdge(loopId_Left, edgeId_LeftFront, false); // other direction in front loop
            brepBuilder.AddCoEdge(loopId_Left, edgeId_TopLeft, true);   // other direction in top loop
            brepBuilder.AddCoEdge(loopId_Left, edgeId_BackLeft, true);  // other direction in back loop
            brepBuilder.FinishLoop(loopId_Left);
            brepBuilder.FinishFace(faceId_Left);

            brepBuilder.Finish();
            return brepBuilder;
        }*/
     
        #region
        /* Rebar CreateRebar1(
   Autodesk.Revit.UI.UIDocument uidoc,    IList<Curve> curves, FamilyInstance column, RebarBarType barType,  RebarHookType hookType)

         {



             LocationPoint location = column.Location as LocationPoint;

             XYZ origin = location.Point;



             Rebar rebar = Rebar.CreateFromCurves(uidoc.Document,

               RebarStyle.StirrupTie, barType, hookType, hookType,

               column, new XYZ(0, 0, 1), curves,

               RebarHookOrientation.Left, RebarHookOrientation.Left, true, true);



             return rebar; ;
         }*/
        #endregion
    }
}



